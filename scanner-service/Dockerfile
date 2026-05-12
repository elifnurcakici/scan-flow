FROM golang:1.25 AS builder

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN go build -o scanner-service

FROM golang:1.25 AS nuclei-builder

RUN go install -v github.com/projectdiscovery/nuclei/v3/cmd/nuclei@latest

FROM debian:bookworm-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    git \
    && rm -rf /var/lib/apt/lists/*

RUN curl -sfL https://raw.githubusercontent.com/aquasecurity/trivy/main/contrib/install.sh | sh -s -- -b /usr/local/bin v0.70.0

COPY --from=nuclei-builder /go/bin/nuclei /usr/local/bin/nuclei
COPY --from=builder /app/scanner-service .

RUN mkdir -p /root/nuclei-templates && nuclei -ut -ud /root/nuclei-templates

ENV HOME=/root

CMD ["./scanner-service"]
