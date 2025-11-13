# Stage 1 - install deps
FROM node:20-bookworm AS deps
WORKDIR /app

# Install deps with relaxed peer checks for React 19 + vaul@0.9.9
COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY ALIM/package*.json ./
RUN npm ci --legacy-peer-deps

# Stage 2 - build
FROM node:20-bookworm AS builder
WORKDIR /app
ENV NODE_ENV=production

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
    && update-ca-certificates \
    && rm -rf /var/lib/apt/lists/*
COPY --from=deps /app/node_modules ./node_modules
COPY ALIM/. .
RUN npm run build

# Stage 3 - runtime
FROM node:20-bookworm
WORKDIR /app
ENV NODE_ENV=production

COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
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
CMD ["npm", "start"]
