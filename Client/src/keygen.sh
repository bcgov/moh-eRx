!/bin/bash

keytool -importkeystore -srckeystore keystore.jks \
 -destkeystore keystore.pfx -srcstoretype jks -deststoretype pkcs12

