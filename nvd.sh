docker run --rm \
  -v "$PWD:/src" -v "$PWD/reports:/reports" \
  owasp/dependency-check:latest \
  --project "unity-xapi-publisher" \
  --scan /src/packages.config \
  --format SARIF \
  --out /reports \
  --nvdApiKey "$NVD_API_KEY"