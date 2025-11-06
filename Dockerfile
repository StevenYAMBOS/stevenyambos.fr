# Utilise nginx pour servir les fichiers statiques
FROM nginx:alpine

# Copie les fichiers statiques dans le répertoire nginx
COPY index.html style.css script.js /usr/share/nginx/html/
COPY docs/ /usr/share/nginx/html/docs/

# Copie la configuration nginx personnalisée
COPY nginx.conf /etc/nginx/nginx.conf

# Expose le port
EXPOSE 8080

# Démarre nginx
CMD ["nginx", "-g", "daemon off;"]

