package main

import (
	"fmt"
	"html/template"
	"io"
	"log"
	"net/http"

	"github.com/caarlos0/env/v11"
	gomail "gopkg.in/mail.v2"
)

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

// Route de test
func homePage(writer http.ResponseWriter, request *http.Request) {
	tmpl := template.Must(template.ParseFiles("./templates/home.html"))

	tmpl.Execute(writer, nil)

	writer.WriteHeader(http.StatusOK)
}

// Page de contact
func contactPage(writer http.ResponseWriter, request *http.Request) {
	tmpl := template.Must(template.ParseFiles("./templates/contact.html"))
	tmpl.Execute(writer, nil)

	writer.WriteHeader(http.StatusOK)
}

// Envoyer le formulaire
func sendContactForm(writer http.ResponseWriter, request *http.Request) {
	tmpl := template.Must(template.ParseFiles("./templates/contact.html"))

	// Variables d'environnement
	cfg := Config{}
	if err := env.Parse(&cfg); err != nil {
		log.Printf("Erreur lors du parsing des variables d'environnement: %v", err)
		http.Error(writer, "Erreur de configuration", http.StatusInternalServerError)
		return
	}

	// Limite de taille de 100 MB (pour limiter les abus)
	const maxUploadSize = 100 << 20 // 100 MB
	request.Body = http.MaxBytesReader(writer, request.Body, maxUploadSize)

	if err := request.ParseMultipartForm(10 << 20); err != nil {
		log.Printf("Erreur lors du parsing du formulaire: %v", err)
		http.Error(writer, "Failed to parse multipart form: "+err.Error(), http.StatusBadRequest)
		return
	}

	object := request.FormValue("object")
	email := request.FormValue("email")
	message := request.FormValue("message")

	// Validation des champs requis
	if object == "" || email == "" || message == "" {
		http.Error(writer, "Tous les champs sont requis", http.StatusBadRequest)
		return
	}

	// Nouveau message
	m := gomail.NewMessage()

	// Headers de l'email
	m.SetHeader("From", email)
	m.SetHeader("To", cfg.SmtpUsername)
	m.SetHeader("Subject", object)
	// Set email body
	m.SetBody("text/html", message)

	// Récupérer le fichier depuis les données du formulaire (optionnel)
	file, fileHeader, err := request.FormFile("attachment")
	if err == nil {
		defer file.Close()

		fmt.Printf("Uploaded File: %+v\n", fileHeader.Filename)
		fmt.Printf("File Size: %+v\n", fileHeader.Size)
		fmt.Printf("MIME Header: %+v\n", fileHeader.Header)

		// Lire le contenu du fichier
		fileContent, err := io.ReadAll(file)
		if err != nil {
			log.Printf("Erreur lors de la lecture du fichier: %v", err)
			http.Error(writer, "Erreur lors de la lecture du fichier", http.StatusInternalServerError)
			return
		}

		// Attacher le fichier à l'email
		m.Attach(fileHeader.Filename, gomail.SetCopyFunc(func(w io.Writer) error {
			_, err := w.Write(fileContent)
			return err
		}))
	} else {
		log.Printf("Aucun fichier joint ou erreur: %v", err)
	}

	dialer := gomail.NewDialer(cfg.SmtpHost, 587, cfg.SmtpUsername, cfg.SmtpPassword)

	// Send the email
	if err := dialer.DialAndSend(m); err != nil {
		log.Printf("Erreur lors de l'envoi de l'email: %v", err)
		http.Error(writer, "Erreur lors de l'envoi de l'email", http.StatusInternalServerError)
		return
	}

	// Afficher la page de succès
	tmpl.Execute(writer, struct{ Success bool }{true})

	fmt.Println("Email envoyé avec succès !")

	// Message de succès
	writer.WriteHeader(http.StatusOK)
	if fileHeader != nil {
		fmt.Fprintf(writer, "Email envoyé avec succès avec le fichier: %s\n", fileHeader.Filename)
	} else {
		fmt.Fprintf(writer, "Email envoyé avec succès\n")
	}
}

func main() {
	cfg := Config{}
	if err := env.Parse(&cfg); err != nil {
		log.Fatalf("Erreur lors du parsing des variables d'environnement: %+v", err)
	}

	// Assurer que le port commence par ":"
	if cfg.Port[0] != ':' {
		cfg.Port = ":" + cfg.Port
	}

	router := http.NewServeMux()

	router.HandleFunc("GET /", homePage)
	router.HandleFunc("GET /contact", contactPage)
	router.HandleFunc("POST /contact", sendContactForm)

	fmt.Printf("Projet lançé sur le PORT %s\n", cfg.Port)
	log.Fatal(http.ListenAndServe(cfg.Port, router))
}
