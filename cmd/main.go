package main

import (
	"fmt"
	"os"

	"log"
	"net/http"

	"github.com/StevenYAMBOS/portfolio/handlers"
	"github.com/joho/godotenv"
)

func main() {
	err := godotenv.Load()
	if err != nil {
		log.Fatal("Erreur lors du chargement du fichier .env")
	}

	router := http.NewServeMux()

	router.HandleFunc("GET /", handlers.HomePage)
	router.HandleFunc("GET /contact", handlers.ContactPage)
	router.HandleFunc("POST /contact-form", handlers.SendContactForm)

	fmt.Printf("Projet lançé sur le PORT %s\n", os.Getenv("PORT"))
	log.Fatal(http.ListenAndServe(os.Getenv("PORT"), router))
}
