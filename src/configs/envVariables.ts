import dotenv from "dotenv";
dotenv.config();

// Général
export const SERVER_PORT = process.env.SERVER_PORT;

// Config GMAIL
export const SMTP_USERNAME = process.env.SMTP_USERNAME;
export const SMTP_PORT = process.env.SMTP_PORT;
export const SMTP_HOST = process.env.SMTP_HOST;
export const SMTP_PASSWORD = process.env.SMTP_PASSWORD;
