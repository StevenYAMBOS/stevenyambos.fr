// package main

// import (
// 	"fmt"
// 	"log"
// 	"net/http"
// 	"os"

// 	gomail "gopkg.in/mail.v2"
// )

// func Router() http.Handler {
// 	router := http.NewServeMux()

// 	router.HandleFunc("GET /health/", healthCheck)

// 	return router
// }

// func healthCheck(writer http.ResponseWriter, request *http.Request) {
// 	if request.Method != "GET" {
// 		writer.Header().Set("Allow", "GET")
// 		http.Error(
// 			writer,
// 			"Cette méthode n'est pas autorisée !",
// 			http.StatusMethodNotAllowed,
// 		)
// 		return
// 	}

// 	fmt.Println("La route de test fonctionne")
// 	log.Print("Route fonctionnelle")

// 	// Répondre au client
// 	writer.WriteHeader(http.StatusOK)
// 	writer.Write([]byte("Service en bonne santé"))
// }

// func sendContactForm(writer http.ResponseWriter, request *http.Request) {
// 	if request.Method != "POST" {
// 		writer.Header().Set("Allow", "POST")
// 		http.Error(
// 			writer,
// 			"Cette méthode n'est pas autorisée !",
// 			http.StatusMethodNotAllowed,
// 		)

// 		return
// 	}

// 	// Create a new message
// 	message := gomail.NewMessage()

// 	// Set email headers
// 	message.SetHeader("From", os.Getenv(user))
// 	message.SetHeader("To", "abc@gmail.com")
// 	message.SetHeader("Subject", "Test Email with Attachment")

// 	// Set email body
// 	message.SetBody("text/html", `
//         <html>
//             <body>
//                 <h1>This is a Test Email</h1>
//                 <p><b>Hello!</b> Please find the attachment below.</p>
//                 <p>Thanks,<br>Mailtrap</p>
//             </body>
//         </html>
//     `)

// 	// Add attachments
// 	message.Attach("/invoice#1.pdf")

// 	// Set up the SMTP dialer
// 	dialer := gomail.NewDialer("live.smtp.mailtrap.io", 587, "api", "1a2b3c4d5e6f7g")

// 	// Send the email
// 	if err := dialer.DialAndSend(message); err != nil {
// 		fmt.Println("Error:", err)
// 		panic(err)
// 	} else {
// 		fmt.Println("Email sent successfully with attachments!")
// 	}

// }
