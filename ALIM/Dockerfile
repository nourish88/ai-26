# Build stage
FROM node:18-alpine AS builder
WORKDIR /app

# Install deps with relaxed peer checks for React 19 + vaul@0.9.9
COPY package*.json ./
RUN npm ci --legacy-peer-deps

# Build
COPY . .
RUN npm run build

# Runtime stage
FROM node:18-alpine
WORKDIR /app
ENV NODE_ENV=production

# Copy the app and preinstalled deps; no re-install to avoid ERESOLVE
COPY --from=builder /app ./
# Do NOT run: npm install --production
# Optionally prune dev deps to slim the image:
# RUN npm prune --omit=dev

EXPOSE 3000
CMD ["npm", "start"]
