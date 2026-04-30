# ScanFlow

ScanFlow is a small ASPM-inspired demo platform for monitoring assets, scans and vulnerabilities in real time.

It demonstrates an event-driven architecture built with:

- `Frontend`: React, TypeScript, Vite, Tailwind CSS, shadcn-style UI primitives
- `Backend`: ASP.NET Core Web API, JWT auth, Redis, Kafka consumer/producer, WebSocket
- `Scanner Service`: Go, Kafka consumer/producer, PostgreSQL writes
- `Database`: PostgreSQL
- `Messaging`: Kafka
- `Cache`: Redis

## Overview

The system lets a user:

- register and log in
- create assets
- start scans for assets
- watch scan status updates live
- inspect scan history
- view generated vulnerabilities

The demo uses simulated scan execution and simulated vulnerabilities, but the architecture is designed so real scanning engines can be integrated later.

## Architecture

The current flow is:

`UI -> Backend -> Kafka -> Scanner Service -> PostgreSQL + Kafka -> Backend Consumer -> WebSocket -> UI`

### Responsibilities

`Backend`

- authentication and authorization
- asset CRUD
- initial scan creation with `Pending` status
- publishing `scan-created` events
- consuming result events only to notify the UI through WebSocket

`Scanner Service`

- consuming `scan-created` events
- updating scan status in PostgreSQL
- writing `ScanHistory`
- creating simulated vulnerabilities
- publishing `Running`, `Finished` or `Failed` result events

`Frontend`

- login/register experience
- asset creation and scan start
- scan list and scan detail view
- real-time refresh via WebSocket-triggered query invalidation

## Features

- JWT-based authentication
- refresh token flow
- Redis access token blacklist on logout
- token invalidation via `tokenVersion`
- asset management
- scan creation
- real-time scan lifecycle updates
- vulnerability listing
- Docker Compose setup for the full stack
- backend integration tests

## Tech Stack

### Backend

- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis
- Kafka
- WebSocket
- FluentValidation
- Serilog

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS


### Scanner

- Go
- Sarama (Kafka)
- PostgreSQL driver

## Project Structure

```text
scan-flow/
├── backend/            # ASP.NET Core API, auth, Kafka, WebSocket
├── backend.Tests/      # Integration tests
├── frontend/           # React + Vite client
├── scanner-service/    # Go scanner worker
└── docker-compose.yml  # Full local environment
```

## Authentication Notes

The auth flow is hybrid on purpose:

- access tokens are blacklisted in Redis on logout
- refresh tokens are persisted and rotated
- user token invalidation is supported through `tokenVersion`

This makes the demo closer to a production-friendly auth model while still staying understandable.

## Running The Project

### Option 1: Docker Compose

Run the full stack:

```bash
docker compose up -d --build
```
```bash
docker compose down
```

### Option 2: Run Services Individually

#### Backend

```bash
cd backend
dotnet restore
dotnet run
```

#### Scanner Service

```bash
cd scanner-service
go run .
```

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Environment Notes

The Docker setup already provides working defaults for:

- PostgreSQL connection
- Redis connection
- Kafka brokers and topics
- frontend API base URL

If you run services manually, check:

- [backend/appsettings.json](/Users/elifnurcakici/Desktop/scan-flow/backend/appsettings.json:1)
- [docker-compose.yml](/Users/elifnurcakici/Desktop/scan-flow/docker-compose.yml:1)

## Testing

Run backend tests:

```bash
dotnet test backend.Tests/backend.Tests.csproj
```

## License

This repository is currently intended as a demo and portfolio-style learning project.
