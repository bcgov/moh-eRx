import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Base64;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ArrayNode;
import com.fasterxml.jackson.databind.node.ObjectNode;

// Review this code line-by-line before running and make the necessary updates
public class PharmaNetClaim {

    public static void main(String[] args) throws Exception {

        String ClientId = "[your client id]";
        String ClientSecret = "[your client secret]";
        String KeycloakUrl = "[your keycloak token endpoint]";
        String PharmaNetUrl = "[pharmanet endpoint for claim submission]"; // different HL7 message types use different endpoints
        String Scope = "openid system/Claim.write system/Claim.read"; // different HL7 message types require different scopes

        HttpClient client = HttpClient.newHttpClient();

        String tokenBody = "client_id=" + ClientId
                + "&client_secret=" + ClientSecret
                + "&audience="
                + "&scope=" + Scope.replace(" ", "%20")
                + "&grant_type=client_credentials";

        HttpRequest tokenRequest = HttpRequest.newBuilder()
                .uri(URI.create(KeycloakUrl))
                .header("Content-Type", "application/x-www-form-urlencoded")
                .POST(HttpRequest.BodyPublishers.ofString(tokenBody))
                .build();

        HttpResponse<String> tokenResponse = client.send(tokenRequest, HttpResponse.BodyHandlers.ofString());

        ObjectMapper objectMapper = new ObjectMapper();
        JsonNode tokenJson = objectMapper.readTree(tokenResponse.body());
        String accessToken = tokenJson.path("access_token").asText();

        if (accessToken.isEmpty()) {
            System.out.println("Token has not been received from Keycloak");
            System.out.println(tokenResponse.body());
            System.exit(1);
        }

		// TDT (Daily Totals Inquiry)
		// Enter your Vendor IDs (looks like BC00000000), control ID, etc.
        // MSH|^~\&|sending_app|sending_fac|receiving_app|receiving_fac|timestamp||msg_type|control_id|proc_id|version
        String hl7 = "MSH|^~&|PHARMACY|BC00000000|PNP|PP||RB:69.11.119.122|ZPN|000000|D|2.1||\n"
                + "ZZZ|TDT||180736|P1|XXBSK||||\n"
                + "ZCA|000001|03|30|KC|16|\n"
                + "ZCB|BC00000000|220823|180736\n"
                + "ZCC|||||||||||\n"
                + "ZCF|220823|000000000|999999999";

        String hl7WithCR = hl7.replace("\n", "\r");
        String dataPayload = Base64.getEncoder().encodeToString(hl7WithCR.getBytes("UTF-8"));

        ObjectNode fhirEnvelope = objectMapper.createObjectNode();
        fhirEnvelope.put("resourceType", "DocumentReference");

        ObjectNode masterIdentifier = objectMapper.createObjectNode();
        masterIdentifier.put("system", "urn:ietf:rfc:3986");
        masterIdentifier.put("value", "e7e78a7a-aae8-4c4b-908a-99a6c8c3bf6a"); // Enter your UUID
        fhirEnvelope.set("masterIdentifier", masterIdentifier);

        fhirEnvelope.put("status", "current");
        fhirEnvelope.put("date", "2026-05-15T10:47:31+00:00"); // Enter your date/time in ISO 8601 format

        ArrayNode content = objectMapper.createArrayNode();
        ObjectNode attachment = objectMapper.createObjectNode();
        ObjectNode attachmentDetails = objectMapper.createObjectNode();
        attachmentDetails.put("contentType", "x-application/hl7-v2+er7");
        attachmentDetails.put("data", dataPayload);
        attachment.set("attachment", attachmentDetails);
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

        /* 
        Expected response looks like:
        MSH|^~&|PHARMACY|BC00000000|PHARMACY|BC00000000||RB:69.11.119.122|ZPN|180736|D|2.1||
        ZZZ|TDT|0|180736|P1|XXBSK|
        ZCA|1|3|80|KC|16|
        ZCB|BC00000000|220823|180736
        ZCG|220823|180736|80||R|A1|||||||||111111||||

		Notes:
		    ZZZ field 2 = 0 → success/no error code
		    ZCA field 3 = 3 (response code indicating success for a TDT inquiry)
		    ZCG → Daily Totals (date 220823, trace 180736, count 80, status R/A1)
        */
    }
}
