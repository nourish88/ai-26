# Stage 1 - install deps
FROM node:20-bookworm AS deps
WORKDIR /app
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

# Install pnpm
RUN corepack enable && corepack prepare pnpm@9.1.3 --activate

# Install deps with relaxed peer checks for React 19 + vaul@0.9.9
COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
COPY certs/corporate-intermediate-ca.crt /usr/local/share/ca-certificates/corporate-intermediate-ca.crt
COPY certs/corporate-subca.crt /usr/local/share/ca-certificates/corporate-subca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY ALIM/package.json ALIM/pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

# Stage 2 - build
FROM node:20-bookworm AS builder
WORKDIR /app
ENV NODE_ENV=production
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

# Install pnpm
RUN corepack enable && corepack prepare pnpm@9.1.3 --activate

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
COPY certs/corporate-intermediate-ca.crt /usr/local/share/ca-certificates/corporate-intermediate-ca.crt
COPY certs/corporate-subca.crt /usr/local/share/ca-certificates/corporate-subca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=deps /app/node_modules ./node_modules
COPY ALIM/. .
RUN pnpm run build

# Stage 3 - runtime
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

# Copy the app and preinstalled deps; no re-install to avoid ERESOLVE
COPY --from=builder /app ./
# Do NOT run: npm install --production
# Optionally prune dev deps to slim the image:
# RUN npm prune --omit=dev

EXPOSE 3000
# Install pnpm for runtime (needed for npm start script)
RUN corepack enable && corepack prepare pnpm@9.1.3 --activate
CMD ["pnpm", "start"]
