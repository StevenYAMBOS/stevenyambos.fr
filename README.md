# Steven YAMBOS — Portfolio (Go)

[![Go](https://img.shields.io/badge/Go-00ADD8?logo=go&logoColor=white)](https://go.dev)
[![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![DigitalOcean](https://img.shields.io/badge/DigitalOcean-0080FF?logo=digitalocean&logoColor=white)](https://www.digitalocean.com/)

> Portfolio personnel propulsé par Go (net/http). Pages rendues via `html/template`, formulaire de contact et déploiement conteneurisé pour DigitalOcean App Platform.

Ces badges sont référencés depuis la collection Shields.io présentée dans `md-badges` — voir la référence: [inttter/md-badges](https://github.com/inttter/md-badges).

## Présentation

Portfolio Steven YAMBOS 2025.

- Langage: Go
- Serveur: `net/http`
- Templates: `html/template`

## Fonctionnalités

- Accueil/hero avec bouton de téléchargement de CV (`docs/CV_Steven_YAMBOS.pdf`)
- Page de contact avec envoi de mail et pièce jointe (jusqu'à 100MB)

## Structure du projet

```
.
├── cmd/
│   └── main.go              # Point d'entrée du serveur
├── handlers/
│   └── handlers.go          # Routes et logique (home, contact, envoi formulaire)
├── models/
│   └── models.go            # (placeholder pour modèles)
├── templates/
│   ├── home.html            # Page d'accueil
│   └── contact.html         # Page de contact + succès
├── docs/
│   └── CV_Steven_YAMBOS.pdf # CV servi statiquement
├── tests/
│   └── handlers_test.go     # Tests handlers
├── go.mod / go.sum
└── Dockerfile
```

## Prérequis

- Go 1.22+
- Docker (optionnel pour conteneurisation)

## Démarrage rapide (local)

1) Copier/adapter les variables d'environnement (facultatif en local) dans un fichier `.env` à la racine:

```
PORT=8080
SMTP_HOST=smtp.example.com
SMTP_USERNAME=you@example.com
SMTP_PASSWORD=your_password
```

2) Lancer l'app:

```
go run ./cmd
```

L'application écoute sur `:8080` par défaut. Changer `PORT` si nécessaire.

## Variables d'environnement

- `PORT`: Port HTTP (par ex. `8080`). Le serveur accepte `8080` ou `:8080`.
- `SMTP_HOST`: Hôte SMTP (ex: `smtp.gmail.com`).
- `SMTP_USERNAME`: Identifiant/Email destinataire.
- `SMTP_PASSWORD`: Mot de passe/app password SMTP.

En production (DigitalOcean App Platform), définissez ces variables dans la configuration de l'application. Le serveur n'exige pas `.env` en production (chargement optionnel en local).

## Docker

Image multi-stage optimisée. Exemple de build et run local:

```
docker build -t stevenyambos-portfolio .

docker run --rm -p 8080:8080 \
  -e PORT=8080 \
  -e SMTP_HOST=smtp.example.com \
  -e SMTP_USERNAME=you@example.com \
  -e SMTP_PASSWORD=your_password \
  stevenyambos-portfolio
```

Le binaire est statique et l'image finale est basée sur `distroless` pour plus de sécurité.

## Déploiement — DigitalOcean App Platform

1) Poussez ce repo sur GitHub.
2) Créez une nouvelle App sur DO App Platform en pointant sur le repo.
3) Type: Container ou Go App (Container recommandé ici car Dockerfile fourni).
4) Variables d'environnement à définir dans DO:
   - `PORT=8080`
   - `SMTP_HOST`, `SMTP_USERNAME`, `SMTP_PASSWORD`
5) Exposez le service HTTP sur le port 8080.

DO injecte les variables d'environnement. N'incluez pas de `.env` dans l'image.

## Tests

```
go test ./...
```
