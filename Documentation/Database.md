# Base de données

- Mise à jour le : 08 février 2026
- Par [**Steven YAMBOS**](www.linkedin.com/in/steven-yambos)

## Tables

### Table `Articles`

Nom : `articles`

Description : Table des articles de blog.

```sql
CREATE TABLE articles (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    slug VARCHAR(250) NOT NULL UNIQUE,
    description VARCHAR(500),
    content TEXT NOT NULL,
    cover VARCHAR(500),
    author VARCHAR(100) NOT NULL,
    is_published BOOLEAN DEFAULT FALSE,
    published_at TIMESTAMP,
    categories TEXT[], 
    tags TEXT[], 
    view_count INTEGER DEFAULT 0,
    reading_time_minutes INTEGER DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Index pour améliorer les performances
CREATE INDEX idx_articles_slug ON articles(slug);
CREATE INDEX idx_articles_is_published ON articles(is_published);
CREATE INDEX idx_articles_published_at ON articles(published_at DESC);
CREATE INDEX idx_articles_categories ON articles USING GIN(categories); -- Index GIN pour recherche dans tableaux
CREATE INDEX idx_articles_tags ON articles USING GIN(tags);

-- Fonction pour mettre à jour automatiquement updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger pour updated_at
CREATE TRIGGER update_articles_updated_at
    BEFORE UPDATE ON articles
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Commentaires sur la table
COMMENT ON TABLE articles IS 'Table des articles du blog';
COMMENT ON COLUMN articles.slug IS 'URL-friendly identifier pour SEO';
COMMENT ON COLUMN articles.view_count IS 'Nombre de vues de l''article';
```
