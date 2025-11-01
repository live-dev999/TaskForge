# Client App Docker Configuration

This directory contains the React client application with Docker support.

## Docker Configuration

### Environment Variables

The client app uses the following environment variables:

- `REACT_APP_API_URL` - API base URL
  - For Docker: `/api` (nginx will proxy to backend)
  - For local development: `http://localhost:5009/api`
  - Can be set via docker-compose environment variables

### Docker Build

The Dockerfile uses a multi-stage build:
1. **Build stage**: Uses Node.js to build the React app
2. **Production stage**: Uses Nginx to serve static files

### Nginx Configuration

The `nginx.conf` file includes:
- Proxy configuration for `/api` requests (forwarded to `taskforge.api:80`)
- Static file serving with proper caching
- Gzip compression
- Security headers
- Health check endpoint at `/health`

### Usage

#### Running with Docker Compose

The client is included in the main `docker-compose.yml`. To run all services:

```bash
docker-compose up
```

The client will be available at `http://localhost:3000` (configurable via `CLIENT_PORT` environment variable).

#### Local Development

For local development without Docker:

```bash
cd src/client-app
npm install
npm start
```

Set `REACT_APP_API_URL=http://localhost:5009/api` in your `.env` file if needed.

### Configuration

#### Custom API URL

To use a custom API URL in Docker:

```bash
export REACT_APP_API_URL=http://your-api-url/api
docker-compose up
```

Or set it in `docker-compose.override.yml`:

```yaml
taskforge.client:
  environment:
    - REACT_APP_API_URL=http://custom-api-url/api
```

#### Custom Port

To change the client port:

```bash
export CLIENT_PORT=8080
docker-compose up
```

Or in `docker-compose.override.yml`:

```yaml
taskforge.client:
  ports:
    - "8080:80"
```

