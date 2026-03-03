# stevenyambos.fr — API

<div align="center">

[![C#](https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/.NET_10_LTS-512BD4?logo=dotnet&logoColor=fff)](#)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-512BD4?logo=dotnet&logoColor=fff)](#)
[![Postgres](https://img.shields.io/badge/NeonDB-%23316192.svg?logo=postgresql&logoColor=white)](#)
[![Cloudflare](https://img.shields.io/badge/Cloudflare-F38020?logo=Cloudflare&logoColor=white)](#)

<h3>Portfolio de Steven YAMBOS — Édition 2026</h3>

[Demo](https://stevenyambos.fr) · [Documentation](./Documentation) · [Signaler un bug](https://github.com/StevenYAMBOS/stevenyambos/issues) · [Nouvelle fonctionnalité](https://github.com/StevenYAMBOS/stevenyambos/issues)

</div>

---

## À propos

API de mon portfolio. Elle expose les fonctionnalités nécessaires à la gestion du compte utilisateur et des articles de blog : authentification, opérations CRUD sur les articles, formulaire de contact.

## Technologies

| Composant | Technologie | Version |
|---|---|---|
| Langage | C# | 13 |
| Framework | ASP.NET Core | LTS (10.0) |
| ORM | Entity Framework Core | 10.x |
| Base de données | NeonDB (PostgreSQL) | 16+ |
| Authentification | JWT (Access + Refresh Token) | — |
| Réseau / Proxy | Cloudflare | — |

## Installation

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Accès à une base NeonDB (ou PostgreSQL 16+)
- Compte Cloudflare (optionnel en développement local)

### Variables d'environnement

Créer un fichier `appsettings.Development.json` (non versionné) avec les valeurs suivantes :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "...",
    "Audience": "..."
  }
}
```

### Lancement
```bash
# Restaurer les dépendances
dotnet restore

# Appliquer les migrations
dotnet ef database update

# Lancer en développement
dotnet run
```

L'API sera accessible sur `https://localhost:{port}` (voir `Properties/launchSettings.json`).

## Endpoints principaux

| Méthode | Route | Description |
|---|---|---|
| `POST` | `/api/auth/register` | Création de compte |
| `POST` | `/api/auth/login` | Connexion (retourne Access + Refresh token) |
| `POST` | `/api/auth/refresh` | Renouvellement du token |
| `GET` | `/api/article` | Liste des articles |
| `POST` | `/api/article` | Créer un article |
| `PUT` | `/api/article/{id}` | Modifier un article |
| `DELETE` | `/api/article/{id}` | Supprimer un article |
| `POST` | `/api/contact` | Envoi du formulaire de contact |

## Sécurité

- Authentification par **JWT** avec rotation des Refresh Tokens
- Gestion des rôles via `RoleHelper`
- Variables sensibles externalisées (`appsettings.json` non versionné pour les secrets)
- Proxy Cloudflare en production (protection DDoS, HTTPS forcé)

---

<div align="center">

Développé par **[Steven YAMBOS](https://www.linkedin.com/in/steven-yambos/)**

</div>
