# Utilise nginx pour servir les fichiers statiques
FROM nginx:alpine

# Copie les fichiers statiques dans le répertoire nginx
COPY index.html style.css script.js /usr/share/nginx/html/
COPY docs/ /usr/share/nginx/html/docs/

# Copie la configuration nginx personnalisée (décommentez si vous voulez l'utiliser)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose le port 80
EXPOSE 80

# Démarre nginx
CMD ["nginx", "-g", "daemon off;"]

