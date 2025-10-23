import dotenv from "dotenv";
dotenv.config();

// Général
export const SERVER_PORT = process.env.SERVER_PORT;

// Config GMAIL
export const SMTP_USERNAME = process.env.SMTP_USERNAME;
export const SMTP_PORT = process.env.SMTP_PORT;
export const SMTP_HOST = process.env.SMTP_HOST;
export const SMTP_PASSWORD = process.env.SMTP_PASSWORD;

// Routes
export const HOME_PAGE_PATH: string | any = process.env.HOME_PAGE_PATH;
export const CONTACT_PAGE_PATH: string | any = process.env.CONTACT_PAGE_PATH;
export const CONTACT_FORM_PATH: string | any = process.env.CONTACT_FORM_PATH;
