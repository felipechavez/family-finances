# ──────────────────────────────────────────────
# Stage 1: Install dependencies
# ──────────────────────────────────────────────
FROM node:22-alpine AS deps
WORKDIR /app/frontend
COPY package*.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci

# ──────────────────────────────────────────────
# Stage 2: Development (hot-reload vía compose)
# ──────────────────────────────────────────────
FROM node:22-alpine AS development
WORKDIR /app/frontend
COPY package*.json ./
RUN --mount=type=cache,target=/root/.npm \
    npm ci
EXPOSE 3000
ENV NODE_ENV=development \
    HOST=0.0.0.0 \
    PORT=3000
CMD ["npm", "run", "dev"]

# ──────────────────────────────────────────────
# Stage 3: Build production
# ──────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app/frontend
COPY --from=deps /app/frontend/node_modules ./node_modules
COPY . .
RUN npm run build

# ──────────────────────────────────────────────
# Stage 4: Runtime (producción, imagen mínima)
# ──────────────────────────────────────────────
FROM node:22-alpine AS runtime
RUN addgroup -g 1001 -S nodejs && adduser -S nuxtjs -u 1001 -G nodejs
WORKDIR /app/frontend
# .output contiene server/ (Nitro) y public/ (assets estáticos)
COPY --from=build --chown=nuxtjs:nodejs /app/frontend/.output ./.output
USER nuxtjs
EXPOSE 3000
ENV NODE_ENV=production \
    HOST=0.0.0.0 \
    PORT=3000
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD wget -qO- http://localhost:3000/ || exit 1
CMD ["node", ".output/server/index.mjs"]
