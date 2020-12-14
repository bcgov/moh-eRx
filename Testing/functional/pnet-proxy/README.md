# Fake PharmaNet Proxy server.

Use this for end-end testing. Configure your appsetting.local.json to point to this server for Pharmanet Proxy.

### appsettings.local.json

```javascript
{
    "PharmanetProxy": {
        "Endpoint": "http://localhost:8080/submit",
    }
}
```
