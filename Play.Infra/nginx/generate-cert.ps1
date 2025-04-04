# Create directory for SSL certificates if it doesn't exist
New-Item -ItemType Directory -Force -Path .\ssl

# Generate private key and certificate
$cert = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "Cert:\LocalMachine\My" -NotAfter (Get-Date).AddYears(1)

# Export certificate
$pwd = ConvertTo-SecureString -String "password" -Force -AsPlainText
$certPath = "Cert:\LocalMachine\My\$($cert.Thumbprint)"

# Export PFX
$pfxPath = ".\ssl\localhost.pfx"
Export-PfxCertificate -Cert $certPath -FilePath $pfxPath -Password $pwd

# Export CRT
$crtPath = ".\ssl\localhost.crt"
Export-Certificate -Cert $certPath -FilePath $crtPath -Type CERT

# Export private key
$keyPath = ".\ssl\localhost.key"
openssl pkcs12 -in $pfxPath -nocerts -nodes -out $keyPath -password pass:password

# Clean up the certificate from the store
Remove-Item $certPath

Write-Host "SSL certificate and private key have been generated in the ./ssl directory"
Write-Host "Note: Since this is a self-signed certificate, you will need to trust it in your browser"