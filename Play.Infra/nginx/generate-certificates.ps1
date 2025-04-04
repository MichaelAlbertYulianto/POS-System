# Create the ssl directory if it doesn't exist
New-Item -ItemType Directory -Force -Path .\ssl

# Generate private key and certificate
$env:OPENSSL_CONF = ""

# Generate private key
openssl genpkey -algorithm RSA -out .\ssl\localhost.key -pkeyopt rsa_keygen_bits:2048

# Generate CSR configuration
@"
subjectAltName = DNS:localhost
[req]
distinguished_name = req_distinguished_name
x509_extensions = v3_req
prompt = no
[req_distinguished_name]
CN = localhost
[v3_req]
basicConstraints = CA:FALSE
keyUsage = nonRepudiation, digitalSignature, keyEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
[alt_names]
DNS.1 = localhost
"@ | Out-File -FilePath ".\ssl\openssl.cnf" -Encoding ascii

# Generate certificate
openssl req -new -x509 -nodes -days 365 -key .\ssl\localhost.key -out .\ssl\localhost.crt -config .\ssl\openssl.cnf

Write-Host "SSL certificates generated successfully!"