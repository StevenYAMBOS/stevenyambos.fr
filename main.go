package main

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"os"

	gomail "gopkg.in/mail.v2"
)

type uploadedFile struct {
	Size        int64  `json:"size"`
	ContentType string `json:"content_type"`
	Filename    string `json:"filename"`
	FileContent string `json:"file_content"`
}

type Contact struct {
	Object     string       `json:"object"`
	Email      string       `json:"email"`
	Message    string       `json:"message"`
	Attachment uploadedFile `json:"attachment"`
}

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

	err := request.ParseMultipartForm(32 << 10)
	if err != nil {
		log.Fatal(err)
	}

	object := request.FormValue("object")
	email := request.FormValue("email")
	message := request.FormValue("message")

	var newFile uploadedFile

	for _, fheaders := range request.MultipartForm.File {

		for _, headers := range fheaders {
			file, err := headers.Open()
			if err != nil {
				http.Error(writer, err.Error(), http.StatusInternalServerError)
				return
			}

			defer file.Close()

			// detect contentType
			buff := make([]byte, 512)
			file.Read(buff)
			file.Seek(0, 0)
			contentType := http.DetectContentType(buff)
			newFile.ContentType = contentType

			// get file size

			var sizeBuff bytes.Buffer
			fileSize, err := sizeBuff.ReadFrom(file)

			if err != nil {
				http.Error(writer, err.Error(), http.StatusInternalServerError)
				return
			}

			file.Seek(0, 0)
			newFile.Size = fileSize
			newFile.Filename = headers.Filename
			contentBuf := bytes.NewBuffer(nil)

			if _, err := io.Copy(contentBuf, file); err != nil {
				http.Error(writer, err.Error(), http.StatusInternalServerError)
				return
			}

			newFile.FileContent = contentBuf.String()

		}

	}
	data := make(map[string]interface{})

	data["form_field_value"] = email
	data["status"] = 200
	data["file_stats"] = newFile

	if err = json.NewEncoder(writer).Encode(data); err != nil {
		http.Error(writer, err.Error(), http.StatusInternalServerError)
		return
	}

	// Create a new message
	m := gomail.NewMessage()

	// Set email headers
	m.SetHeader("From", email)
	m.SetHeader("To", os.Getenv("SMTP_USERNAME"))
	m.SetHeader("Subject", object)
	// Set email body
	m.SetBody("text/html", message)
	// Add attachments
	// m.Attach(newFile)

	// Set up the SMTP dialer
	dialer := gomail.NewDialer(os.Getenv("SMTP_HOST"), 587, os.Getenv("SMTP_USERNAME"), os.Getenv("SMTP_PASSWORD"))

	// Send the email
	if err := dialer.DialAndSend(m); err != nil {
		fmt.Println("Error:", err)
		panic(err)
	} else {
		fmt.Println("Email sent successfully with attachments!")
	}

}

func main() {
	PORT := os.Getenv("PORT")
	if PORT == "" {
		PORT = ":8080" // Port par défaut si la variable d'environnement n'est pas définie
	}

	// Assurez-vous que le port commence par ":"
	if PORT[0] != ':' {
		PORT = ":" + PORT
	}

	router := http.NewServeMux()

	router.HandleFunc("GET /health/", healthCheck)
	router.HandleFunc("POST /contact/", sendContactForm)

	fmt.Printf("Projet lançé sur le PORT %s\n", PORT)
	log.Fatal(http.ListenAndServe(PORT, router))
}
