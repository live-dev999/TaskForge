#!/usr/bin/env python3
"""
pgAdmin setup script to preconfigure server connections
This script runs inside the pgAdmin container after initialization
"""

import json
import os
import time
from pathlib import Path

# Get email from environment (default pgAdmin email)
EMAIL = os.environ.get('PGADMIN_DEFAULT_EMAIL', 'admin@pgadmin.org')
# Convert email to directory format: admin@pgadmin.org -> admin_pgadmin_org
EMAIL_DIR = EMAIL.replace('@', '_').replace('.', '_')

STORAGE_DIR = Path(f'/var/lib/pgadmin/storage/{EMAIL_DIR}')
SERVERS_JSON = STORAGE_DIR / 'servers.json'
INIT_SERVERS_JSON = Path('/pgadmin-init/servers.json')

def wait_for_pgadmin_init(max_wait=120):
    """Wait for pgAdmin to initialize storage directory"""
    wait_count = 0
    while not STORAGE_DIR.exists() and wait_count < max_wait:
        time.sleep(2)
        wait_count += 2
        if wait_count % 10 == 0:
            print(f"Waiting for pgAdmin initialization... ({wait_count}/{max_wait}s)")
    
    # Don't create directory manually - pgAdmin will create it with correct permissions
    # Just wait for it to exist
    return STORAGE_DIR.exists()

def copy_servers_config():
    """Copy servers.json configuration"""
    if not INIT_SERVERS_JSON.exists():
        print(f"⚠️  Warning: {INIT_SERVERS_JSON} not found")
        return False
    
    # Wait for storage to be ready
    if not wait_for_pgadmin_init():
        print("❌ Failed to initialize storage directory")
        return False
    
    # Copy configuration
    try:
        with open(INIT_SERVERS_JSON, 'r') as f:
            config = json.load(f)
        
        with open(SERVERS_JSON, 'w') as f:
            json.dump(config, f, indent=2)
        
        # Set permissions (readable/writable by owner)
        # Note: chown might fail if not running as root, but chmod should work
        try:
            os.chmod(SERVERS_JSON, 0o600)
        except Exception as e:
            print(f"⚠️  Warning: Could not set permissions on {SERVERS_JSON}: {e}")
        
        # Try to set owner, but don't fail if it doesn't work
        # pgAdmin will fix permissions when it accesses the file
        try:
            os.chown(SERVERS_JSON, 5050, 5050)  # pgadmin user/group
        except (PermissionError, OSError):
            # Not running as root - pgAdmin will fix ownership when accessing
            pass
        
        print(f"✅ pgAdmin server configuration initialized")
        print(f"   File: {SERVERS_JSON}")
        print(f"   Server: TaskForge DB")
        return True
    except Exception as e:
        print(f"❌ Error copying servers.json: {e}")
        return False

if __name__ == '__main__':
    print(f"🚀 Initializing pgAdmin configuration for user: {EMAIL}")
    copy_servers_config()

