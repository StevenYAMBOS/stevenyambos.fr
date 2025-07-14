package handlers

import (
	"fmt"
	"html/template"
	"io"
	"log"
	"net/http"
	"os"

	"github.com/joho/godotenv"
	gomail "gopkg.in/mail.v2"
)

// Route de test
func HomePage(writer http.ResponseWriter, request *http.Request) {
	tmpl := template.Must(template.ParseFiles("./templates/home.html"))
	tmpl.Execute(writer, nil)

	writer.WriteHeader(http.StatusOK)
}

// Page de contact
func ContactPage(writer http.ResponseWriter, request *http.Request) {
	tmpl := template.Must(template.ParseFiles("./templates/contact.html"))
	tmpl.Execute(writer, struct{ Success bool }{false})

	writer.WriteHeader(http.StatusOK)
}

// Envoyer le formulaire
func SendContactForm(writer http.ResponseWriter, request *http.Request) {
	err := godotenv.Load()
	if err != nil {
		log.Fatal("Erreur lors du chargement du fichier .env")
	}

	if request.Method != http.MethodPost {
		http.Error(writer, "Méthode non autorisée", http.StatusMethodNotAllowed)
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
	m.SetHeader("To", os.Getenv("SMTP_USERNAME"))
	m.SetHeader("Subject", object)
	// Corps du mail
	/*
	 * GMAIL semble bloquer l'email de l'expéditeur, j'ai un peu tricher pour l'ajouter dans le body, ça passe donc c'est cool
	 */
	htmlBody := fmt.Sprintf(`
		<h2>Nouveau message de contact depuis le portfolio</h2>
		<p><strong>Email de l'expéditeur:</strong> %s</p>
		<p><strong>Objet:</strong> %s</p>
		<hr>
		<div style="margin-top: 20px;">
			%s
		</div>
		<hr>
		<p><em>Message reçu depuis stevenyambos.fr</em></p>
	`, email, object, message)

	m.SetBody("text/html", htmlBody)

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

	dialer := gomail.NewDialer(os.Getenv("SMTP_HOST"), 587, os.Getenv("SMTP_USERNAME"), os.Getenv("SMTP_PASSWORD"))

	// Envoyer l'email
	if err := dialer.DialAndSend(m); err != nil {
		log.Printf("Erreur lors de l'envoi de l'email: %v", err)
		http.Error(writer, "Erreur lors de l'envoi de l'email", http.StatusInternalServerError)
		return
	}

	// Afficher la page de succès
	tmpl := template.Must(template.ParseFiles("./templates/contact.html"))
	tmpl.Execute(writer, struct{ Success bool }{true})

	fmt.Println("Email envoyé avec succès !")
	fmt.Println("ENVOYER À : ", email)
}
