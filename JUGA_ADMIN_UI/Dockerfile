# Stage 1 - install dependencies
FROM node:20-bookworm AS deps
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

# Stage 2 - build application
FROM node:20-bookworm AS builder
WORKDIR /app

ENV NODE_ENV=production

COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

# Stage 3 - production runtime
FROM node:20-bookworm
WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app ./

EXPOSE 3001

CMD ["npm", "run", "start"]

