package models

// Variables d'environnement
type Config struct {
	Port         string `env:"PORT" envDefault:"3000"`
	SmtpUsername string `env:"SMTP_USERNAME"`
	SmtpHost     string `env:"SMTP_HOST"`
	SmtpPort     string `env:"SMTP_PORT" envDefault:"587"`
	SmtpPassword string `env:"SMTP_PASSWORD"`
}

// Modèle du formulaire de contact
type Contact struct {
	Object     string `json:"object"`
	Email      string `json:"email"`
	Message    string `json:"message"`
	Attachment string `json:"attachment"`
}
