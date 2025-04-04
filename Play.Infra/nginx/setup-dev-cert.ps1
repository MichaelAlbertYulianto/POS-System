# Requires running with administrator privileges

# Create directory for SSL certificates if it doesn't exist
New-Item -ItemType Directory -Force -Path .\ssl

# Generate private key and certificate
$cert = New-SelfSignedCertificate `
    -DnsName "localhost" `
    -CertStoreLocation "Cert:\LocalMachine\My" `
    -NotAfter (Get-Date).AddYears(1) `
    -KeyUsage DigitalSignature, KeyEncipherment `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1") `
    -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider"

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

# Install certificate to Trusted Root store
$rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store "Root", "LocalMachine"
$rootStore.Open("ReadWrite")
$rootStore.Add($cert)
$rootStore.Close()

# Clean up the certificate from the My store
Remove-Item $certPath

Write-Host "SSL certificate has been generated and installed as a trusted root certificate"
Write-Host "Certificate and private key are available in the ./ssl directory"