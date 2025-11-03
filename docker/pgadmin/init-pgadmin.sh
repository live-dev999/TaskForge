#!/bin/bash

# This script runs in the background after pgAdmin starts
# It waits for pgAdmin to initialize and then copies the servers.json config

# Get email from environment variable or use default pgAdmin email
EMAIL="${PGADMIN_DEFAULT_EMAIL:-admin@pgadmin.org}"

# Convert email to directory name (replace @ and . with _)
# admin@pgadmin.org -> admin_pgadmin_org
EMAIL_DIR=$(echo "$EMAIL" | sed 's/@/_/g' | sed 's/\./_/g')

STORAGE_DIR="/var/lib/pgadmin/storage/${EMAIL_DIR}"

# Wait up to 120 seconds for pgAdmin to initialize the storage directory
MAX_WAIT=120
WAIT_COUNT=0

echo "⏳ Waiting for pgAdmin storage initialization for user: $EMAIL"

while [ ! -d "$STORAGE_DIR" ] && [ $WAIT_COUNT -lt $MAX_WAIT ]; do
  sleep 2
  WAIT_COUNT=$((WAIT_COUNT + 2))
  if [ $((WAIT_COUNT % 10)) -eq 0 ]; then
    echo "   Still waiting... ($WAIT_COUNT/$MAX_WAIT seconds)"
  fi
done

if [ ! -d "$STORAGE_DIR" ]; then
  echo "⚠️  Warning: Storage directory $STORAGE_DIR not found after $MAX_WAIT seconds"
  echo "   Creating directory manually..."
  mkdir -p "$STORAGE_DIR"
  chown -R pgadmin:pgadmin "$STORAGE_DIR"
fi

# Copy servers.json to the storage directory
if [ -f "/pgadmin-init/servers.json" ]; then
  # Always copy/update servers.json on startup (will overwrite user changes)
  # This ensures configuration is always correct from docker-compose
  cp /pgadmin-init/servers.json "$STORAGE_DIR/servers.json"
  chown pgadmin:pgadmin "$STORAGE_DIR/servers.json"
  chmod 600 "$STORAGE_DIR/servers.json"
  echo "✅ pgAdmin server configuration initialized for $EMAIL"
  echo "   Server 'TaskForge DB' should now be available in pgAdmin"
  echo "   Configuration file: $STORAGE_DIR/servers.json"
else
  echo "⚠️  Warning: /pgadmin-init/servers.json not found"
fi
