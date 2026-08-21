package demo;

import java.util.Base64;
import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.time.OffsetDateTime;
import java.time.format.DateTimeFormatter;
import java.util.UUID;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

// Review this code line-by-line before running and make the necessary updates
public class PharmaNetClaim {

    public static void main(String[] args) throws Exception {
        if (args.length < 1) {
            System.out.println("Usage: java PharmaNetClaim <ClientId>");
            System.exit(1);
        }  

        String ClientId = args[0];
        String KeycloakUrl = "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";
        String PharmaNetUrl = "https://pnet-vs1.api.gov.bc.ca/api/v1/Claim"; // different HL7 message types use
        String keycloak = "https://common-logon-test.hlth.gov.bc.ca/auth/realms/moh_applications/protocol/openid-connect/token";
                                                                             // different endpoints
        String Scope = "openid system/Claim.write system/Claim.read"; // different HL7 message types require different
        String quick = JwtService.quick(ClientId, keycloak);

        String tokenRequestBody = "grant_type=client_credentials"
                + "&client_id=" + URLEncoder.encode(ClientId, StandardCharsets.UTF_8)
                + "&audience="
                + "&scope=" + URLEncoder.encode(Scope.replaceAll("^\"|\"$", ""), StandardCharsets.UTF_8)
                + "&client_assertion_type="
                + URLEncoder.encode("urn:ietf:params:oauth:client-assertion-type:jwt-bearer", StandardCharsets.UTF_8)
                + "&client_assertion=" + quick;

        HttpRequest tokenRequest = HttpRequest.newBuilder()
                .uri(URI.create(KeycloakUrl))
                .header("Content-Type", "application/x-www-form-urlencoded")
                .POST(HttpRequest.BodyPublishers.ofString(tokenRequestBody))
                .build();

        HttpClient client = HttpClient.newHttpClient();
        HttpResponse<String> tokenResponse = client.send(tokenRequest, HttpResponse.BodyHandlers.ofString());

        ObjectMapper objectMapper = new ObjectMapper();
        JsonNode tokenJson = objectMapper.readTree(tokenResponse.body());
        String accessToken = tokenJson.path("access_token").asText();

        if (accessToken.isEmpty()) {
            System.out.println("Token has not been received from Keycloak");
            System.out.println(tokenResponse.body());
            System.exit(1);
        }
        System.out.print("Keycloak access Token: " + accessToken);

        // not a valid HL7 message, but used for testing the FHIR envelope and PharmaNet response
        String hl7 = "MSH|^~&|DESKTOP|PNET-39999999|PNP|PP||GERRYWASHERE,,WL*E5R:SS0AR|ZPN|631708|P|2.1||\n"
                + "ZCA|000001|03|00|AR|04\n"
                + "ZCB|BC00000000|260806|631708\n"
                + "ZZZ|TDR||631708|P1|07963|||";

        String hl7WithCR = hl7.replace("\n", "\r");
        String dataPayload = Base64.getEncoder().encodeToString(hl7WithCR.getBytes("UTF-8"));

        ObjectNode fhirEnvelope = objectMapper.createObjectNode();
        fhirEnvelope.put("resourceType", "DocumentReference");

        ObjectNode masterIdentifier = objectMapper.createObjectNode();
        masterIdentifier.put("system", "urn:ietf:rfc:3986");
        masterIdentifier.put("value", UUID.randomUUID().toString());
        fhirEnvelope.set("masterIdentifier", masterIdentifier);

        fhirEnvelope.put("status", "current");
        fhirEnvelope.put("date", OffsetDateTime.now().format(DateTimeFormatter.ISO_OFFSET_DATE_TIME));

        ObjectNode attachmentDetails = objectMapper.createObjectNode();
        attachmentDetails.put("contentType", "x-application/hl7-v2+er7");
        attachmentDetails.put("data", dataPayload);

        ObjectNode attachment = objectMapper.createObjectNode();
        attachment.set("attachment", attachmentDetails);

        ArrayNode content = objectMapper.createArrayNode();
        content.add(attachment);
        fhirEnvelope.set("content", content);

        HttpRequest claimRequest = HttpRequest.newBuilder()
                .uri(URI.create(PharmaNetUrl))
                .header("Content-Type", "application/fhir+json")
                .header("Authorization", "Bearer " + accessToken)
                .POST(HttpRequest.BodyPublishers.ofString(objectMapper.writeValueAsString(fhirEnvelope)))
                .build();

        HttpResponse<String> claimResponse = client.send(claimRequest, HttpResponse.BodyHandlers.ofString());

        JsonNode responseJson = objectMapper.readTree(claimResponse.body());
        String responseData = responseJson.path("content").path(0).path("attachment").path("data").asText();

        if (responseData.isEmpty()) {
            System.out.println("Unexpected response from PharmaNet, no data payload received");
            System.out.println(claimResponse.body());
            System.exit(1);
        }

        String hl7Response = new String(Base64.getDecoder().decode(responseData), "UTF-8");
        System.out.println(hl7Response.replace("\r", "\n"));
    }
}