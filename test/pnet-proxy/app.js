const express = require("express");

const port = 8080;
const host = "0.0.0.0";
const app = express();

const meanDelayMilliseconds = 500;

const responseJson = {
    "transactionUUID": "22c263c9-1903-4cbf-8eb2-c43fa7b9dfa7",
    "hl7Message": "TVNIfF5+XCZ8MTIzNDU2N3wxMjM0NTY3fDEyMzQ1Njd8MTIzNDU2N3x8fFpQTnw0NDM3Njl8UHwyLjF8ClpDQnxCQzAwMDAwSTEwfDE0MDcxNXw0NDM3NjkKWlpafFRJRHwwfDQ0Mzc2OXxQMXxubm5ubm5ubm5ufHwwIE9wZXJhdGlvbiBzdWNjZXNzZnVsfHx8WlpaMQpaQ0N8fHx8fDE5NTAwMjE3fHx8fHwwMDBubm5ubm5ubm5ufEYKWlBBfE5TTkRKREdaVHxaTUFYQXxXfFpQQTFeXl42MDReNDU2MzE2N3xaUEEyXk1eXl5eXlhWWVRORERDT09HXl5TVVJSRVleQ0FOXlYyUjdFMF5eQkNeXl5eXl5eXl4="
};

app.post("/submit", function (req, res) {
    console.log("http://" + host + ":" + port + "/submit: POST");

    res.contentType = "application/json";

    res.setTimeout(randomExp(meanDelayMilliseconds), () => { res.send(responseJson); });
});

app.get("/", function (req, res) {
    console.log("http://" + host + ":" + port + "/: GET");

    res.contentType = "application/json";

    res.setTimeout(randomExp(meanDelayMilliseconds), () => { res.send(responseJson); });
});

// Generate a random number, exponentially distributed with the given mean.
function randomExp(mean) {
    return Math.log(1 - Math.random()) * -mean;
}

app.listen(port, host);

console.log("PharmaNet proxy now running on http://" + host + ":" + port);
