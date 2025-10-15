FROM golang:1.22-alpine AS builder

WORKDIR /src

COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 \
    go build -trimpath -ldflags="-s -w -extldflags '-static'" -o /out/portfolio ./cmd

FROM gcr.io/distroless/static-debian12

WORKDIR /app

COPY --from=builder /out/portfolio /app/portfolio

COPY templates ./templates
COPY docs ./docs

ENV PORT=8080
EXPOSE 8080

USER nonroot:nonroot

ENTRYPOINT ["/app/portfolio"]
