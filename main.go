package main

import (
	"fmt"
	"log"
	"net/http"

	"github.com/caarlos0/env/v11"
	gomail "gopkg.in/mail.v2"
)

// Variables d'environnement
type Config struct {
	Port         string `env:"PORT"`
	SmtpUsername string `env:"SMTP_USERNAME"`
	SmtpHost     string `env:"SMTP_HOST"`
	SmtpPort     string `env:"SMTP_PORT"`
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
func healthCheck(writer http.ResponseWriter, request *http.Request) {
	if request.Method != "GET" {
		writer.Header().Set("Allow", "GET")
		http.Error(
			writer,
			"Cette méthode n'est pas autorisée !",
			http.StatusMethodNotAllowed,
		)
		return
	}

	fmt.Println("La route de test fonctionne")
	log.Print("Route fonctionnelle")

	// Répondre au client
	writer.WriteHeader(http.StatusOK)
	writer.Write([]byte("Service en bonne santé"))
}

// Envoyer le formulaire
func sendContactForm(writer http.ResponseWriter, request *http.Request) {
	if request.Method != "POST" {
		writer.Header().Set("Allow", "POST")
		http.Error(
			writer,
			"Cette méthode n'est pas autorisée !",
			http.StatusMethodNotAllowed,
		)

		return
	}

	// Variables d'environnement
	cfg := Config{}
	if err := env.Parse(&cfg); err != nil {
		log.Fatalf("%+v", err)
	}

	// Limit the size of the incoming request body to prevent abuse (e.g., 100 MB)
	const maxUploadSize = 100 << 20 // 100 MB
	request.Body = http.MaxBytesReader(writer, request.Body, maxUploadSize)

	// Parse the multipart form
	if err := request.ParseMultipartForm(10 << 20); err != nil {
		http.Error(writer, "Failed to parse multipart form: "+err.Error(), http.StatusBadRequest)
		return
	}

	object := request.FormValue("object")
	email := request.FormValue("email")
	message := request.FormValue("message")

	// Retrieve the file from the form data
	file, fileHeader, err := request.FormFile("attachment")
	if err != nil {
		http.Error(writer, "Unable to retrieve file from form: "+err.Error(), http.StatusBadRequest)
		return
	}
	defer file.Close()

	fmt.Printf("Uploaded File: %+v\n", fileHeader.Filename)
	fmt.Printf("File Size: %+v\n", fileHeader.Size)
	fmt.Printf("MIME Header: %+v\n", fileHeader.Header)

	// Create a new message
	m := gomail.NewMessage()

	// Set email headers
	m.SetHeader("From", email)
	m.SetHeader("To", cfg.SmtpUsername)
	m.SetHeader("Subject", object)
	// Set email body
	m.SetBody("text/html", message)
	// Add attachments
	m.Attach(fileHeader.Filename)

	// Set up the SMTP dialer
	dialer := gomail.NewDialer(cfg.SmtpHost, 587, cfg.SmtpUsername, cfg.SmtpPassword)

	// Send the email
	if err := dialer.DialAndSend(m); err != nil {
		fmt.Println("Error:", err)
		panic(err)
	} else {
		fmt.Println("Email sent successfully with attachments!")
	}
	// Respond with a success message
	writer.WriteHeader(http.StatusOK)
	fmt.Fprintf(writer, "File uploaded successfully: %s\n", fileHeader.Filename)
}

func main() {
	cfg := Config{}
	if err := env.Parse(&cfg); err != nil {
		log.Fatalf("%+v", err)
	}

	router := http.NewServeMux()

	router.HandleFunc("GET /health/", healthCheck)
	router.HandleFunc("POST /contact/", sendContactForm)

	fmt.Printf("Projet lançé sur le PORT %s\n", cfg.Port)
	log.Fatal(http.ListenAndServe(cfg.Port, router))
}
