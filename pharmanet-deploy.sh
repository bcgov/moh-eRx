
#!/bin/bash
# Manual feature build + deploy for Pharmanet API (.NET)
#
# Three modes:
#   build:  dotnet publish + podman build (local only, nothing pushed)
#   push:   Push local image to a cluster's tools namespace
#   deploy: Tag image from tools -> target namespace (fixed-tag deployment)
#
# Usage:
#   ./pharmanet-deploy.sh build <service> <image-tag>
#   ./pharmanet-deploy.sh push <cluster> <service> <full-image-tag>
#   ./pharmanet-deploy.sh deploy <environment> <full-image-tag> <service> <subtarget>
#
#   Services: locationservice (others TBD)
#   Clusters: silver, gold
#   Environments: silver-dev, silver-test, gold-prod
#   Subtargets: dev, tr1, vc2, vs1, prd (must match deployment name in OpenShift)
#   Image tag: descriptive name (e.g. dotnet10_upgrade)
#   Full image tag: <image-tag>-<SHORT_SHA> (output by build mode)
#
# Examples:
#   # 1. Build locally
#   ./pharmanet-deploy.sh build locationservice dotnet10_upgrade
#   # Output: === Registry tag: dotnet10_upgrade-a1b2c3d4e5f6 ===
#
#   # 2. Push to Silver tools (pre-stage)
# c14a22eede73
#   ./pharmanet-deploy.sh push silver locationservice dotnet10_upgrade-c14a22eede73
#
#   # 3. Deploy to Silver dev
#   ./pharmanet-deploy.sh deploy silver-dev dotnet10_upgrade-c14a22eede73 locationservice dev
#
#   # 4. Deploy to Silver test (one per subtarget)
#   ./pharmanet-deploy.sh deploy silver-test dotnet10_upgrade-c14a22eede73 locationservice tr1
#   ./pharmanet-deploy.sh deploy silver-test dotnet10_upgrade-c14a22eede73 locationservice vc2
#   ./pharmanet-deploy.sh deploy silver-test dotnet10_upgrade-c14a22eede73 locationservice vs1
#
#   # 5. Push to Gold tools (pre-stage before change window)
#   ./pharmanet-deploy.sh push gold locationservice dotnet10_upgrade-c14a22eede73
#
#   # 6. Change window — deploy to Gold prod
#   ./pharmanet-deploy.sh deploy gold-prod dotnet10_upgrade-c14a22eede73 locationservice prd
#
# Prerequisites: podman, oc, dotnet.exe, git (WSL2)
# Must be logged into the correct OpenShift cluster before running push or deploy.
set -eu

GOLD_REGISTRY="image-registry.apps.gold.devops.gov.bc.ca"
SILVER_REGISTRY="image-registry.apps.silver.devops.gov.bc.ca"
INTERNAL_REGISTRY="image-registry.openshift-image-registry.svc:5000"

GOLD_NS="d027a8"
SILVER_NS="2f77cb"

# Returns: registry tools-namespace expected-server
resolve_cluster() {
  case "$1" in
    silver) echo "${SILVER_REGISTRY} ${SILVER_NS}-tools https://api.silver.devops.gov.bc.ca:6443" ;;
    gold)   echo "${GOLD_REGISTRY} ${GOLD_NS}-tools https://api.gold.devops.gov.bc.ca:6443" ;;
    *)
      echo "Unknown cluster: $1" >&2
      echo "Valid: silver, gold" >&2
      exit 1
      ;;
  esac
}

# Returns: namespace tools-namespace env-tag expected-server
resolve_env() {
  case "$1" in
    silver-dev)  echo "${SILVER_NS}-dev ${SILVER_NS}-tools dev https://api.silver.devops.gov.bc.ca:6443" ;;
    silver-test) echo "${SILVER_NS}-test ${SILVER_NS}-tools test https://api.silver.devops.gov.bc.ca:6443" ;;
    gold-prod)   echo "${GOLD_NS}-prod ${GOLD_NS}-tools prod https://api.gold.devops.gov.bc.ca:6443" ;;
    *)
      echo "Unknown environment: $1" >&2
      echo "Valid: silver-dev, silver-test, gold-prod" >&2
      exit 1
      ;;
  esac
}

# Deployment name: <service>-<subtarget>-ppmservice (always)
get_deployment_name() {
  local svc="$1"
  local sub="$2"
  echo "${svc}-${sub}-ppmservice"
}

# Verify the user is logged into the expected cluster
verify_cluster() {
  local expected_server="$1"
  local current_server
  current_server=$(oc whoami --show-server 2>/dev/null) || {
    echo "ERROR: Not logged in to OpenShift." >&2
    echo "Run: oc login --token=<token> --server=${expected_server}" >&2
    echo "Get your token at: https://oauth-openshift.$(echo "${expected_server}" | sed 's|https://api\.||;s|:6443||')/oauth/token/request" >&2
    exit 1
  }
  if [ "${current_server}" != "${expected_server}" ]; then
    echo "ERROR: Logged into ${current_server} but expected ${expected_server}" >&2
    echo "Run: oc login --token=<token> --server=${expected_server}" >&2
    exit 1
  fi
  echo "Verified: logged into ${current_server} as $(oc whoami)"
}

MODE="${1:?Usage: $0 <build|push|deploy> ...}"

# =============================================================================
# BUILD — dotnet publish + podman build, local only
# =============================================================================
if [ "${MODE}" = "build" ]; then
  SERVICE="${2:?Usage: $0 build <service> <image-tag>}"
  IMAGE_TAG="${3:?Usage: $0 build <service> <image-tag>}"
  IMAGE_NAME="${SERVICE}"

  BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)
  SHORT_SHA=$(git rev-parse --short=12 HEAD)
  FULL_TAG="${IMAGE_TAG}-${SHORT_SHA}"

  echo "=== BUILD ==="
  echo "=== Branch: ${BRANCH_NAME} | Service: ${SERVICE} | Tag: ${IMAGE_TAG} | SHA: ${SHORT_SHA} ==="
  echo "=== Registry tag: ${FULL_TAG} ==="

  # Find the .csproj for this service (case-insensitive — .csproj uses PascalCase)
  CSPROJ=$(find . -maxdepth 4 -iname "${SERVICE}.csproj" -print -quit)
  if [ -z "${CSPROJ}" ]; then
    echo "ERROR: Could not find ${SERVICE}.csproj (case-insensitive search)" >&2
    exit 1
  fi
  CSPROJ_DIR=$(dirname "${CSPROJ}")
  echo "=== Found project: ${CSPROJ} ==="

  # dotnet publish
  dotnet.exe publish "${CSPROJ}" -c Release -o "${CSPROJ_DIR}/out"

  # Podman build (local image stays 'latest' to avoid accumulation)
  podman build -t "${IMAGE_NAME}:latest" -f "${CSPROJ_DIR}/Dockerfile" "${CSPROJ_DIR}/"

  echo ""
  echo "=== Build complete ==="
  echo "=== Local image: ${IMAGE_NAME}:latest ==="
  echo "=== Registry tag: ${FULL_TAG} ==="
  echo ""
  echo "Next steps:"
  echo "  $0 push silver ${SERVICE} ${FULL_TAG}"
  echo "  $0 push gold ${SERVICE} ${FULL_TAG}"
  exit 0
fi

# =============================================================================
# PUSH — Push local image to a cluster's tools namespace
# =============================================================================
if [ "${MODE}" = "push" ]; then
  CLUSTER="${2:?Usage: $0 push <silver|gold> <service> <full-image-tag>}"
  SERVICE="${3:?Usage: $0 push <silver|gold> <service> <full-image-tag>}"
  FULL_TAG="${4:?Usage: $0 push <silver|gold> <service> <full-image-tag>}"
  IMAGE_NAME="${SERVICE}"

  RESOLVED=$(resolve_cluster "${CLUSTER}")
  REGISTRY=$(echo "${RESOLVED}" | awk '{print $1}')
  TOOLS_NS=$(echo "${RESOLVED}" | awk '{print $2}')
  EXPECTED_SERVER=$(echo "${RESOLVED}" | awk '{print $3}')

  verify_cluster "${EXPECTED_SERVER}"

  echo "=== PUSH: ${IMAGE_NAME}:${FULL_TAG} -> ${CLUSTER} (${TOOLS_NS}) ==="

  oc whoami -t | podman login "${REGISTRY}" -u "$(oc whoami)" --password-stdin

  podman tag "${IMAGE_NAME}:latest" "${REGISTRY}/${TOOLS_NS}/${IMAGE_NAME}:${FULL_TAG}"
  podman push "${REGISTRY}/${TOOLS_NS}/${IMAGE_NAME}:${FULL_TAG}"

  echo ""
  echo "=== Push complete: ${IMAGE_NAME}:${FULL_TAG} in ${TOOLS_NS} ==="
  echo ""
  echo "Next steps:"
  case "${CLUSTER}" in
    silver)
      echo "  $0 deploy silver-dev ${FULL_TAG} ${SERVICE} dev"
      echo "  $0 deploy silver-test ${FULL_TAG} ${SERVICE} <tr1|vc2|vs1>"
      ;;
    gold)
      echo "  $0 deploy gold-prod ${FULL_TAG} ${SERVICE} prd"
      ;;
  esac
  exit 0
fi

# =============================================================================
# DEPLOY — Tag from tools -> target namespace + update deployment image
# =============================================================================
if [ "${MODE}" = "deploy" ]; then
  ENV="${2:?Usage: $0 deploy <environment> <full-image-tag> <service> <subtarget>}"
  FULL_TAG="${3:?Usage: $0 deploy <environment> <full-image-tag> <service> <subtarget>}"
  SERVICE="${4:?Usage: $0 deploy <environment> <full-image-tag> <service> <subtarget>}"
  SUBTARGET="${5:?Usage: $0 deploy <environment> <full-image-tag> <service> <subtarget>}"
  IMAGE_NAME="${SERVICE}"

  RESOLVED=$(resolve_env "${ENV}")
  TARGET_NS=$(echo "${RESOLVED}" | awk '{print $1}')
  TOOLS_NS=$(echo "${RESOLVED}" | awk '{print $2}')
  ENV_TAG=$(echo "${RESOLVED}" | awk '{print $3}')
  EXPECTED_SERVER=$(echo "${RESOLVED}" | awk '{print $4}')

  verify_cluster "${EXPECTED_SERVER}"

  DEPLOY_NAME=$(get_deployment_name "${SERVICE}" "${SUBTARGET}")
  TARGET_IS="${DEPLOY_NAME}"

  # Grant image-puller to target namespace
  oc policy add-role-to-user system:image-puller system:serviceaccount:${TARGET_NS}:default -n "${TOOLS_NS}"

  echo "=== DEPLOY: ${IMAGE_NAME}:${FULL_TAG} -> ${ENV} ==="
  echo "=== Deployment: ${DEPLOY_NAME} ==="
  echo "=== ImageStream: ${TARGET_NS}/${TARGET_IS}:${ENV_TAG} ==="

  oc tag "${TOOLS_NS}/${IMAGE_NAME}:${FULL_TAG}" "${TARGET_NS}/${TARGET_IS}:${ENV_TAG}"
  oc set image "deployment/${DEPLOY_NAME}" "${SERVICE}=${INTERNAL_REGISTRY}/${TOOLS_NS}/${IMAGE_NAME}:${FULL_TAG}" -n "${TARGET_NS}"

  echo ""
  echo "=== Deploy complete: ${IMAGE_NAME}:${FULL_TAG} -> ${TARGET_NS}/${TARGET_IS}:${ENV_TAG} ==="
  echo "=== Verify: oc -n ${TARGET_NS} rollout status deployment/${DEPLOY_NAME} ==="
  exit 0
fi

# =============================================================================
# Unknown mode
# =============================================================================
echo "Unknown mode: ${MODE}" >&2
echo "Usage:" >&2
echo "  $0 build <service> <image-tag>" >&2
echo "  $0 push <silver|gold> <service> <full-image-tag>" >&2
echo "  $0 deploy <environment> <full-image-tag> <service> <subtarget>" >&2
exit 1
