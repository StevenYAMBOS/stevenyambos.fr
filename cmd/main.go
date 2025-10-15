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
	// Charge .env en dev si présent, sinon continue (en prod, DO injecte des vars)
	if err := godotenv.Load(); err != nil {
		log.Println("[main.go] .env non chargé (probablement prod), on continue…")
	}

	router := http.NewServeMux()

	router.HandleFunc("GET /", handlers.HomePage)
	router.HandleFunc("GET /contact", handlers.ContactPage)
	router.HandleFunc("POST /contact-form", handlers.SendContactForm)

	port := os.Getenv("PORT")
	if port == "" {
		port = "8080"
	}
	// Assure le format :8080 attendu par net/http
	if port[0] != ':' {
		port = ":" + port
	}
	fmt.Printf("Projet lancé sur le PORT %s\n", port)
	log.Fatal(http.ListenAndServe(port, router))
}
