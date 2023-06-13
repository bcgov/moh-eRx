'use strict';

const express = require('express');

const port  = 8080;
const host = "0.0.0.0";
const app = express();

const responseJson = '{ "transactionUUID": "22c263c9-1903-4cbf-8eb2-c43fa7b9dfa7",' +
    '"hl7Message": "TVNIfF5+XCZ8MTIzNDU2N3wxMjM0NTY3fDEyMzQ1Njd8MTIzNDU2N3x8fFpQTnw0NDM3Njl8UHwyLjF8ClpDQnxCQzAwMDAwSTEwfDE0MDcxNXw0NDM3NjkKWlpafFRJRHwwfDQ0Mzc2OXxQMXxubm5ubm5ubm5ufHwwIE9wZXJhdGlvbiBzdWNjZXNzZnVsfHx8WlpaMQpaQ0N8fHx8fDE5NTAwMjE3fHx8fHwwMDBubm5ubm5ubm5ufEYKWlBBfE5TTkRKREdaVHxaTUFYQXxXfFpQQTFeXl42MDReNDU2MzE2N3xaUEEyXk1eXl5eXlhWWVRORERDT09HXl5TVVJSRVleQ0FOXlYyUjdFMF5eQkNeXl5eXl5eXl4="' +
    ' }';

app.post('/submit', function (req, res) {
    console.log("http://" + host + ":" + port + "/submit: " + req);
    res.contentType = 'application/json';
    res.send(responseJson);
});

app.get('/', function (req, res) {
    console.log("http://" + host + ":" + port + "/: " + req);
    res.contentType = 'application/json';
    res.send(responseJson);
});

app.listen(port, host);
console.log("Pharmanet Proxy now Running on http://" + host + ":" + port);