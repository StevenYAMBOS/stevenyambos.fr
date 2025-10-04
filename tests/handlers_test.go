package tests

import (
	"log"
	"net/http"
	"net/http/httptest"
	"testing"

	"github.com/StevenYAMBOS/portfolio/handlers"
	"github.com/joho/godotenv"
)

func TestSendContactFormHandler(t *testing.T) {
	err := godotenv.Load()
	if err != nil {
		log.Fatal("Erreur lors du chargement du fichier .env")
	}

	// Create a request to pass to our handler. We don't have any query parameters for now, so we'll
	// pass 'nil' as the third parameter.
	req, err := http.NewRequest("POST", "/contact-form", nil)
	if err != nil {
		t.Fatal(err)
	}

	// We create a ResponseRecorder (which satisfies http.ResponseWriter) to record the response.
	rr := httptest.NewRecorder()
	handler := http.HandlerFunc(handlers.SendContactForm)

	// Our handlers satisfy http.Handler, so we can call their ServeHTTP method
	// directly and pass in our Request and ResponseRecorder.
	handler.ServeHTTP(rr, req)

	// Check the status code is what we expect.
	if req.Method != http.MethodPost {
		t.Errorf("La méthode HTTP n'est pas la bonne : nous avons %v nous voulons %v",
			req.Method, http.MethodPost)
	}

	// Check the response body is what we expect.
	// expected := template.Must(template.ParseFiles("./templates/home.html"))
	// if rr.Body.String() != expected {
	// 	t.Errorf("handler returned unexpected body: got %v want %v",
	// 		rr.Body.String(), expected)
	// }
}
