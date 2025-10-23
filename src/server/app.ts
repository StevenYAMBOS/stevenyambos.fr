import express, { Request, Response } from "express";
import path from "path";
import multer from "multer";
import nodemailer from "nodemailer";
import {
  CONTACT_PAGE_PATH,
  HOME_PAGE_PATH,
  CONTACT_FORM_PATH,
  SMTP_HOST,
  SMTP_PORT,
  SMTP_USERNAME,
  SMTP_PASSWORD,
} from "../configs/envVariables";

export const app = express();
app.use(express.static("public"));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Configuration multer pour l'upload de fichiers
const upload = multer({
  dest: "uploads/", // Dossier temporaire pour les fichiers
  limits: {
    fileSize: 100 * 1024 * 1024, // 100MB max
  },
  fileFilter: (req, file, cb) => {
    // Types de fichiers acceptés
    const allowedTypes = /pdf|doc|docx|txt|jpg|jpeg|png/;
    const extname = allowedTypes.test(
      path.extname(file.originalname).toLowerCase()
    );
    const mimetype = allowedTypes.test(file.mimetype);

    if (mimetype && extname) {
      return cb(null, true);
    } else {
      cb(new Error("Type de fichier non autorisé"));
    }
  },
});

// Configuration du transporteur email Gmail
const transporter = nodemailer.createTransport({
  host: SMTP_HOST,
  port: parseInt(SMTP_PORT || "587"),
  secure: false, // true pour 465, false pour autres ports
  auth: {
    user: SMTP_USERNAME,
    pass: SMTP_PASSWORD,
  },
});

// Page d'accueil
const HomePage = async (req: Request, res: Response) => {
  res.sendFile(path.join(__dirname, "/public/index.html"));
};

// Page de contact
const ContactPage = async (req: Request, res: Response) => {
  res.sendFile(path.join(__dirname, "public", "contact.html"));
};

// Formulaire de contact
const ContactFormSubmit = async (req: Request, res: Response) => {
  try {
    const { object, email, message } = req.body;

    // Validation basique
    if (!object || !email || !message) {
      return res.status(400).json({
        error: "Tous les champs obligatoires doivent être remplis",
      });
    }

    // Validation des variables d'environnement SMTP
    if (!SMTP_HOST || !SMTP_PORT || !SMTP_USERNAME || !SMTP_PASSWORD) {
      console.error("Variables d'environnement SMTP manquantes");
      return res.status(500).json({
        error: "Configuration email manquante",
      });
    }

    // Configuration de l'email
    const mailOptions: any = {
      from: `"Portfolio Contact" <${SMTP_USERNAME}>`,
      to: SMTP_USERNAME,
      replyTo: email,
      subject: `[Portfolio] ${object}`,
      html: `
        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
          <h2 style="color: #00d4aa;">Nouveau message de contact</h2>
          <div style="background: #f8f9fa; padding: 20px; border-radius: 10px; margin: 20px 0;">
            <p><strong>De:</strong> ${email}</p>
            <p><strong>Objet:</strong> ${object}</p>
            <p><strong>Date:</strong> ${new Date().toLocaleString("fr-FR")}</p>
          </div>
          <div style="background: white; padding: 20px; border-left: 4px solid #00d4aa;">
            <h3>Message:</h3>
            <p style="white-space: pre-wrap; line-height: 1.6;">${message}</p>
          </div>
          ${
            req.file
              ? `<p><strong>Fichier joint:</strong> ${req.file.originalname}</p>`
              : ""
          }
        </div>
      `,
      text: `
        Nouveau message de contact
        De: ${email}
        Objet: ${object}
        Date: ${new Date().toLocaleString("fr-FR")}
        
        Message:
        ${message}
        
        ${req.file ? `Fichier joint: ${req.file.originalname}` : ""}
      `,
    };

    // Ajout des pièces jointes si un fichier est présent
    if (req.file) {
      const fs = require("fs");
      const path = require("path");

      try {
        // Lire le fichier uploadé
        const fileContent = fs.readFileSync(req.file.path);

        // Ajouter la pièce jointe
        mailOptions.attachments = [
          {
            filename: req.file.originalname,
            content: fileContent,
            contentType: req.file.mimetype,
          },
        ];

        console.log("Pièce jointe ajoutée:", req.file.originalname);
      } catch (fileError) {
        console.error("Erreur lors de la lecture du fichier:", fileError);
        // Continuer sans la pièce jointe si erreur
      }
    }

    // Envoi de l'email
    const info = await transporter.sendMail(mailOptions);
    console.log("Email envoyé avec succès:", info.messageId);
    console.log("Détails du message:");
    console.log("- De:", email);
    console.log("- Objet:", object);
    console.log("- Message ID:", info.messageId);

    // Log des informations sur les pièces jointes
    if (req.file) {
      console.log("- Fichier joint:", req.file.originalname);
      console.log("- Taille:", req.file.size, "bytes");
      console.log("- Type MIME:", req.file.mimetype);

      // Nettoyer le fichier temporaire après envoi
      try {
        const fs = require("fs");
        fs.unlinkSync(req.file.path);
        console.log("- Fichier temporaire supprimé:", req.file.path);
      } catch (cleanupError) {
        console.error(
          "Erreur lors de la suppression du fichier temporaire:",
          cleanupError
        );
      }
    }

    // Réponse de succès
    res.status(200).json({
      success: true,
      message: "Message envoyé avec succès",
    });
  } catch (error) {
    console.error("Erreur lors de l'envoi de l'email:", error);
    res.status(500).json({
      error: "Erreur lors de l'envoi du message",
    });
  }
};

app.get(HOME_PAGE_PATH, HomePage);
app.get(CONTACT_PAGE_PATH, ContactPage);
app.post(CONTACT_FORM_PATH, upload.single("attachment"), ContactFormSubmit);
