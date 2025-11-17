# Stage 1 - install dependencies
FROM node:20-bookworm AS deps
WORKDIR /app

COPY certs/aa.crt /usr/local/share/ca-certificates/aa.crt
COPY certs/pypi.org.crt /usr/local/share/ca-certificates/pypi.org.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY JUGA_ADMIN_UI/package.json JUGA_ADMIN_UI/package-lock.json ./
RUN npm ci

# Stage 2 - build application
FROM node:20-bookworm AS builder
WORKDIR /app

ENV NODE_ENV=production
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt
COPY certs/aa.crt /usr/local/share/ca-certificates/aa.crt
COPY certs/pypi.org.crt /usr/local/share/ca-certificates/pypi.org.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=deps /app/node_modules ./node_modules
COPY JUGA_ADMIN_UI/. .
RUN npm run build

# Stage 3 - production runtime
FROM node:20-bookworm
WORKDIR /app

ENV NODE_ENV=production

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
COPY certs/corporate-intermediate-ca.crt /usr/local/share/ca-certificates/corporate-intermediate-ca.crt
COPY certs/corporate-subca.crt /usr/local/share/ca-certificates/corporate-subca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app ./

EXPOSE 3001

CMD ["npm", "run", "start"]

