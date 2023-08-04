# Kibana

The PPM API OpenShift pods are connected to the Kibana logging service. You can access Kibana from the OpenShift web console by clicking "Show in Kibana" from a pod's "Logs" tab. You can also access Kibana at the following URLs:

- [Kibana Silver](https://kibana-openshift-logging.apps.silver.devops.gov.bc.ca/app/kibana#/home?_g=())
- [Kibana Gold](https://kibana-openshift-logging.apps.gold.devops.gov.bc.ca/app/kibana#/home?_g=())
- [Kibana GoldDR](https://kibana-openshift-logging.apps.golddr.devops.gov.bc.ca/app/kibana#/home?_g=())

## Custom Dashboard

Included in this directory are two Saved Searches for Kibana. "PPM API Error Logs" will show you only log messages with one or more of the words "warning", "error", or "exception", which will allow you to easily see if, when, and how many errors occurred. "PPM API Transaction Logs" will show you all logs except for the regular health checks, the lines stating the source of a log message, and the "Informational: WORKAROUND" logs that are printed whenever a transaction is successfully completed with PharmaNet; essentially, it only shows logs that are relevant to the PharmaNet transactions.
You can import the Saved Searches into Kibana by downloading `kibana-saved-searches.json` and uploading it in Kibana through Management > Saved Objects > Import. If you have access to OpenShift projects other than PPM API, you may need to choose a new index pattern to select only the PPM API namespaces.
