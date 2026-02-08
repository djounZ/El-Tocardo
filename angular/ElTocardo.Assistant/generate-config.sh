#!/bin/bash

# Get issuer from environment variables with fallback chain
ISSUER="${services__authorization_server__https__0:-${services__authorization_server__http__0:-http://localhost:5095}}"

# Add trailing slash if missing
[[ "$ISSUER" != */ ]] && ISSUER="${ISSUER}/"

# Get API base path from environment variables with fallback chain
API_BASE="${services__eltocardoapi__https__0:-${services__eltocardoapi__http__0:-http://localhost:40068}}"

# Remove trailing slash if present
API_BASE="${API_BASE%/}"

# Get scope from environment variable with fallback
SCOPE="${AUTH_SCOPE:-openid profile email eltocardoapiuser}"

# Get scope from environment variable with fallback
CLIENTID="${AUTH_CLIENTID:-ElTocardoAssistant}"

# Generate config.json
cat > public/config.json << EOF
{
  "auth": {
    "issuer": "$ISSUER",
    "scope": "$SCOPE",
    "clientId": "$CLIENTID"
  },
  "api": {
    "basePath": "$API_BASE"
  }
}
EOF

echo "Generated config.json with:"
echo "  issuer: $ISSUER"
echo "  scope: $SCOPE"
echo "  clientId: $CLIENTID"
echo "  api.basePath: $API_BASE"
