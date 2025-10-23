import { SERVER_PORT } from "./configs/envVariables";
import { app } from "./server/app";

// Lancement du serveur HTTP
app.listen(SERVER_PORT, () => {
  console.log(
    `Le serveur est lançé à l'adresse : http://localhost:${SERVER_PORT}`
  );
});
