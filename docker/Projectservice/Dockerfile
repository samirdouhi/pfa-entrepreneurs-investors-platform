FROM php:8.3-fpm

# Installer les paquets nécessaires
RUN apt-get update && apt-get install -y \
    git curl libpq-dev libzip-dev unzip nginx \
    && docker-php-ext-install pdo pdo_pgsql zip \
    && rm -rf /var/lib/apt/lists/*

# Installer Composer (gestionnaire de dépendances PHP)
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# Dossier de travail dans le conteneur
WORKDIR /var/www/projectservice

# Copier le code Symfony (quand tu l'auras généré)
COPY . .

# Installer les dépendances Symfony
RUN composer install --no-interaction || true

# Copier la configuration Nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Port sur lequel Nginx va écouter
EXPOSE 8081

# Lancer Nginx + PHP-FPM
CMD service nginx start && php-fpm
