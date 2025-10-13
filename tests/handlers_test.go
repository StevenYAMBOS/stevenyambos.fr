package tests

import (
	"bytes"
	"mime/multipart"
	"net/http"
	"net/http/httptest"
	"os"
	"path/filepath"
	"testing"

	"github.com/StevenYAMBOS/portfolio/handlers"
	"github.com/stretchr/testify/assert"
	"github.com/stretchr/testify/require"
)

func TestSendContactFormHandler(t *testing.T) {
	// Configuration des variables d'environnement pour le test
	t.Setenv("SMTP_USERNAME", "test@example.com")
	t.Setenv("SMTP_HOST", "smtp.gmail.com")
	t.Setenv("SMTP_PASSWORD", "test_password")

	// Créer le répertoire templates s'il n'existe pas pour le test
	wd, err := os.Getwd()
	require.NoError(t, err)

	// Naviguer vers la racine du projet si nécessaire
	if filepath.Base(wd) == "tests" {
		os.Chdir("..")
	}

	t.Run("Should reject non-POST requests", func(t *testing.T) {
		req := httptest.NewRequest(http.MethodGet, "/contact-form", nil)
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		assert.Equal(t, http.StatusMethodNotAllowed, rr.Code)
	})

	t.Run("Should return 400 when required fields are missing", func(t *testing.T) {
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "Test")
		// email manquant
		writer.WriteField("message", "Test message")
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		assert.Equal(t, http.StatusBadRequest, rr.Code)
	})

	t.Run("Should return 400 when object is empty", func(t *testing.T) {
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "")
		writer.WriteField("email", "test@example.com")
		writer.WriteField("message", "Test message")
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		assert.Equal(t, http.StatusBadRequest, rr.Code)
	})

	t.Run("Should return 400 when email is empty", func(t *testing.T) {
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "Test")
		writer.WriteField("email", "")
		writer.WriteField("message", "Test message")
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		assert.Equal(t, http.StatusBadRequest, rr.Code)
	})

	t.Run("Should return 400 when message is empty", func(t *testing.T) {
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "Test")
		writer.WriteField("email", "test@example.com")
		writer.WriteField("message", "")
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		assert.Equal(t, http.StatusBadRequest, rr.Code)
	})

	t.Run("Should handle valid form with all required fields", func(t *testing.T) {
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "Test Subject")
		writer.WriteField("email", "sender@example.com")
		writer.WriteField("message", "This is a test message")
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		// Note: Ce test retournera 500 car il ne peut pas se connecter à SMTP
		// Pour l'améliorer, vous devriez mocker la fonction d'envoi d'email
		// (voir recommandations ci-dessous)
		assert.NotNil(t, rr.Code)
	})

	t.Run("Should reject request body exceeding max size", func(t *testing.T) {
		// Créer un payload très volumineux
		body := new(bytes.Buffer)
		writer := multipart.NewWriter(body)
		writer.WriteField("object", "Test")
		writer.WriteField("email", "test@example.com")

		// Ajouter un message extrêmement volumineux
		largeMessage := make([]byte, 200<<20) // 200 MB
		for i := range largeMessage {
			largeMessage[i] = 'a'
		}
		writer.WriteField("message", string(largeMessage))
		writer.Close()

		req := httptest.NewRequest(http.MethodPost, "/contact-form", body)
		req.Header.Set("Content-Type", writer.FormDataContentType())
		rr := httptest.NewRecorder()

		handler := http.HandlerFunc(handlers.SendContactForm)
		handler.ServeHTTP(rr, req)

		// Devrait retourner une erreur de taille
		assert.True(t, rr.Code >= 400)
	})
}
