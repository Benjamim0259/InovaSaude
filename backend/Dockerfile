# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
COPY tsconfig.json ./
COPY prisma ./prisma

# Install dependencies and generate Prisma Client
RUN npm ci
RUN npm run prisma:generate

# Copy source code
COPY src ./src

# Build TypeScript
RUN npm run build

# Runtime stage
FROM node:20-alpine AS runtime
WORKDIR /app

# Install only production dependencies
COPY package*.json ./
RUN npm ci --only=production

# Copy Prisma schema and client
COPY prisma ./prisma
RUN npm run prisma:generate

# Copy built application
COPY --from=build /app/dist ./dist

# Create uploads directory
RUN mkdir -p uploads logs

EXPOSE 4000
ENV NODE_ENV=production

CMD ["npm", "start"]
