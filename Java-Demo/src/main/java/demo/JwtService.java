package demo;

import io.jsonwebtoken.Jwts;
import java.time.Instant;
import java.util.Date;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.security.KeyFactory;
import java.security.PrivateKey;
import java.security.spec.PKCS8EncodedKeySpec;
import java.util.Base64;
import java.util.UUID;

public class JwtService {
    public static String quick(String ClientId, String keycloak) throws Exception {
        // Read the PEM file and isolate the Base64-encoded key body.
        String pem = Files.readString(Paths.get(ClientId + ".key"));
        String base64Key = pem
                .replace("-----BEGIN PRIVATE KEY-----", "")
                .replace("-----END PRIVATE KEY-----", "")
                .replaceAll("\\s", "");
        // System.out.println("Base64 Key: " + base64Key);

        byte[] keyBytes = Base64.getDecoder().decode(base64Key);
        PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(keyBytes);
        PrivateKey privateKey = KeyFactory.getInstance("RSA").generatePrivate(keySpec);

        // Build the signed JWT
        Instant now = Instant.now();
        String signedJWT = Jwts.builder()
                .issuer(ClientId)
                .subject(ClientId)
                .audience().add(keycloak).and()
                .id(UUID.randomUUID().toString())
                .issuedAt(Date.from(now))
                .expiration(Date.from(now.plusSeconds(300)))
                .signWith(privateKey, Jwts.SIG.RS256)
                .compact();

        return signedJWT;
    }
}