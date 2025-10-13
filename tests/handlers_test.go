package tests

import (
	"bytes"
	"io"
	"mime/multipart"
	"net/http"
	"net/http/httptest"
	"os"
	"testing"

	"github.com/StevenYAMBOS/portfolio/handlers"
	"github.com/stretchr/testify/require"
)

func TestSendContactFormHandler(t *testing.T) {
	t.Setenv("SMTP_USERNAME", os.Getenv("SMTP_USERNAME"))
	t.Setenv("SMTP_HOST", os.Getenv("SMTP_HOST"))
	t.Setenv("SMTP_PASSWORD", os.Getenv("SMTP_PASSWORD"))
	t.Setenv("SMTP_PORT", os.Getenv("SMTP_PORT"))

	// err := godotenv.Load()
	// if err != nil {
	// 	log.Fatal("Erreur lors du chargement du fichier .env")
	// }

	// validData := `{"object":"Test"},{"email":"stevenyambos@yopmail.com"},{"message":"Test cURL"}`

	body := new(bytes.Buffer)
	writer := multipart.NewWriter(body)
	writer.WriteField("object", "Test")
	writer.WriteField("email", "stevenyambos@yopmail.com")
	writer.WriteField("message", "Test cURL")

	require.NoError(t, writer.Close())

	// Create a request to pass to our handler. We don't have any query parameters for now, so we'll
	// pass 'nil' as the third parameter.
	req, err := http.NewRequest(http.MethodPost, "/contact-form", body)
	if err != nil {
		t.Fatal(err)
	}

	// req.Header.Set("Content-Type", "multipart/form-data")

	// We create a ResponseRecorder (which satisfies http.ResponseWriter) to record the response.
	rr := httptest.NewRecorder()
	handler := http.HandlerFunc(handlers.SendContactForm)

	require.NoError(t, err)
	req.Header.Set("Content-Type", writer.FormDataContentType())

	// handler(rr, req)
	// if rr.Code != http.StatusOK {
	// 	t.Errorf("Expected status code %v, got %v", http.StatusOK, rr.Code)
	// }

	// Our handlers satisfy http.Handler, so we can call their ServeHTTP method
	// directly and pass in our Request and ResponseRecorder.
	handler.ServeHTTP(rr, req)

	// Check the status code is what we expect.
	if req.Method != http.MethodPost {
		t.Errorf("La méthode HTTP n'est pas la bonne : nous avons %v nous voulons %v",
			req.Method, http.MethodPost)
	}

	// Limite de taille de 100 MB (pour limiter les abus)
	const maxBytesSize = 100 << 20 // 100 MB
	req.Body = http.MaxBytesReader(rr, req.Body, maxBytesSize)

	// check if request body is not too large
	data, err := io.ReadAll(req.Body)
	if err != nil {
		if len(data) >= maxBytesSize {
			http.Error(rr, "Corps de requête trop large", http.StatusBadRequest)
			return
		}

		// Check the response body is what we expect.
		// expected := template.Must(template.ParseFiles("./templates/home.html"))
		// if rr.Body.String() != expected {
		// 	t.Errorf("handler returned unexpected body: got %v want %v",
		// 		rr.Body.String(), expected)
		// }
	}
}
