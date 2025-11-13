# Stage 1 - install deps
FROM node:20-bookworm AS deps
WORKDIR /app

# Install deps with relaxed peer checks for React 19 + vaul@0.9.9
COPY package*.json ./
RUN npm config set strict-ssl false
RUN npm ci --legacy-peer-deps

# Stage 2 - build
FROM node:20-bookworm AS builder
WORKDIR /app
ENV NODE_ENV=production

COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

# Stage 3 - runtime
FROM node:20-bookworm
WORKDIR /app
ENV NODE_ENV=production

# Copy the app and preinstalled deps; no re-install to avoid ERESOLVE
COPY --from=builder /app ./
# Do NOT run: npm install --production
# Optionally prune dev deps to slim the image:
# RUN npm prune --omit=dev

EXPOSE 3000
CMD ["npm", "start"]
