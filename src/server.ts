import express, { Request, Response } from "express";
import { SERVER_PORT } from "./configs/envVariables";

const app = express();
app.get("/", (req: Request, res: Response) => {
  res.send("<h1>PORTFOLIO STEVEN</h1>");
});

app.listen(SERVER_PORT, () => {
  console.log(
    `Le serveur est lançé à l'adresse : http://localhost:${SERVER_PORT}`
  );
});
