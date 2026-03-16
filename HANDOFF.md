# HANDOFF

## Project Overview
This project has completed the **Pass** phase of the AMD201 coursework.

## Repository Information
- **GitHub repository URL:** [https://github.com/phihung1357/amd201-urlshortener.git]

## Docker Hub Repositories
- `hungluphi/urlshortener-api`
- `hungluphi/urlshortener-web`

## Render Deployment
### Render URLs
- **API URL:** [https://amd201-urlshortener-api.onrender.com]
- **Web URL:** [http://localhost:3000/]

### Service Names on Render
- `amd201-urlshortener-api`
- `amd201-urlshortener-web`

## Required GitHub Secrets
The following secrets must be configured in the GitHub repository:
- `DOCKERHUB_USERNAME`
- `DOCKERHUB_TOKEN`
- `RENDER_API_DEPLOY_HOOK`
- `RENDER_WEB_DEPLOY_HOOK`

## Current Status
- **Pass phase:** Completed
- **Merit phase remaining work:** Add Redis caching

## Important Files
- `.github/workflows/ci-cd.yml`
- `UrlShortener.API/Program.cs`
- `urlshortener-web/.env.production`
