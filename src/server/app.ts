import express, { Request, Response } from "express";
import path from "path";

export const app = express();
app.use(express.static("public"));

// Page d'accueil
const HomePage = async (req: Request, res: Response) => {
  res.sendFile(path.join(__dirname + "/public/index.html"));
};

app.get("/", HomePage);
