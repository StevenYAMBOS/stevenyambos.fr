# Guide de déploiement sur Digital Ocean App Platform

## Option 1 : Sans Docker (Recommandé pour un site statique)

Digital Ocean App Platform peut servir directement vos fichiers statiques sans Docker. C'est la méthode la plus simple et la plus rapide.

### Étapes

1. **Connecter votre repository GitHub** à Digital Ocean App Platform
2. **Configurer l'application** :
   - **Type** : Static Site
   - **Build Command** : (laissez vide ou `echo "No build needed"`)
   - **Output Directory** : `/` (racine)
   - **HTTP Port** : 8080 (par défaut pour les sites statiques)

3. **Fichiers nécessaires** :
   - `index.html`
   - `style.css`
   - `script.js`
   - `docs/CV_Steven_YAMBOS.pdf`

4. **Variables d'environnement** : Aucune nécessaire pour un site statique

5. **Domaine personnalisé** : Configurez `stevenyambos.fr` dans les paramètres de l'app

### Avantages

- ✅ Déploiement automatique à chaque push
- ✅ Pas besoin de gérer Docker
- ✅ Configuration minimale
- ✅ Gratuit pour les sites statiques (avec limitations)

---

## Option 2 : Avec Docker

Si vous préférez utiliser Docker pour plus de contrôle ou pour des besoins spécifiques (cache, compression, etc.).

### Étapes

1. **Utiliser le Dockerfile fourni** :

   ```bash
   docker build -t stevenyambos-portfolio .
   docker run -p 8080:80 stevenyambos-portfolio
   ```

2. **Sur Digital Ocean App Platform** :
   - **Type** : Web Service
   - **Dockerfile Path** : `Dockerfile`
   - **HTTP Port** : 80
   - **Build Command** : (laissez vide, Docker gère tout)

3. **Configuration nginx** (optionnel) :
   - Décommentez la ligne dans le Dockerfile pour utiliser `nginx.conf`
   - Cela ajoute compression gzip, cache, et sécurité

### Avantages

- ✅ Plus de contrôle sur le serveur
- ✅ Configuration nginx personnalisée
- ✅ Compression et cache optimisés
- ✅ Meilleure pour la production

### Inconvénients

- ❌ Plus complexe à configurer
- ❌ Temps de build plus long

---

## Recommandation

**Pour votre portfolio statique, je recommande l'Option 1 (sans Docker)** car :

- C'est un site statique simple (HTML/CSS/JS)
- Digital Ocean gère automatiquement le serveur
- Déploiement plus rapide
- Configuration minimale

**Utilisez Docker uniquement si** :

- Vous avez besoin de configurations nginx spécifiques
- Vous voulez un contrôle total sur le serveur
- Vous prévoyez d'ajouter des fonctionnalités backend plus tard

---

## Fichiers de configuration

- `Dockerfile` : Configuration Docker avec nginx
- `nginx.conf` : Configuration nginx optimisée (optionnel)
- `.dockerignore` : Fichiers à exclure du build Docker
