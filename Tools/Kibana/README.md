# Kibana

The PPM API OpenShift pods are connected to the Kibana logging service. You can access Kibana from a pod's "Logs" tab by clicking "Show in Kibana".

## Custom Dashboard

Included in this directory is a custom Kibana dashboard for viewing all the error logs of the pods. You can import it into Kibana by downloading `ppm-api-error-logs.json` and uploading it through Management > Saved Objects > Import. If you have access to OpenShift projects other than PPM API, you can choose a new index pattern to select only the PPM API namespaces.
