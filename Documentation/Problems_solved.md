# Problèmes rencontrés

Fichier qui contient les problèmes que j'ai rencontré et les solutions trouvés.

## Comment puis-je modifier les noms de table ASP.NET Identity par défaut dans .NET CORE ?

Meilleure solution (pour moi) : [Role Based Authentication and Authorization with JWT | .Net Core
](https://medium.com/@paulojunior9395/role-based-authentication-and-authorization-with-jwt-net-core-6dfaa96ff816)

Autre solution : [How can I change default ASP.NET Identity table names in .NET CORE?
](https://stackoverflow.com/questions/41442513/how-can-i-change-default-asp-net-identity-table-names-in-net-core)

Doc officiel : [Microsoft](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-5.0#identity-and-ef-core-migrations)

## Endpoint `PATCH`

### Liens

Liens utiles :

- [Tuto YouTube](https://www.youtube.com/watch?v=RuvG5C8axw8)
- [Tuto YouTube 2](https://www.youtube.com/watch?v=W_UyPtF6q4g)
- [Doc officiel (gestion de la réponse)](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-10.0#apply-a-json-patch-document-to-an-object)
- [Blog](https://dotnettutorials.net/lesson/http-patch-method-in-asp-net-core-web-api/)

### En gros

Installer ces 2 libs : `JsonPatch` et `NewtonsoftJson` ([ici](https://www.nuget.org/packages/Microsoft.AspNetCore.JsonPatch/))

```shell
dotnet add package Microsoft.AspNetCore.JsonPatch
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
```

Dans `Program.cs` ajouter `AddNewtonsoftJson()` aux services :

```cs
builder.Services.AddControllers().AddNewtonsoftJson()
```

Ajouter `JsonPatchDocument<any_model_to_update>` (celui de `JsonPatch`) dans les paramètres de la fonction qui update, avec en type générique le modèle à update :

```cs
  public async Task<IActionResult> UpdateProfil(string id, [FromBody] JsonPatchDocument<ApplicationUser> patchDoc)
  {
    etc...
  }
```

Puis ajouter `ApplyTo()` à la requête :

```cs
patchDoc.ApplyTo(existingUser);
```

### Tester le endpoint

#### HTTP Request

```shell
PATCH
```

#### Header

```json
"Content-Type":"application/json-patch+json"
```

#### Format réponse

[Blog](https://andrewhalil.com/2020/12/13/handling-http-patch-requests-in-a-net-core-web-api/)

```json
[
  {
    "op": "replace", // type d'opération : `replace`, `add`, `remove`, `move`
    "path": "/username",
    "value": "StevenYAMBOS"
  }
]
```

## Décoder le token JWT

Pour extraire les infos du token JWT j'ai trouvé 2 solutions qui sont pas mal :

- [Blog Medium](https://jeremie-litzler.medium.com/getting-information-from-jwt-token-in-c-db04c7806115) pour la structure
- [Stack Overflow (regarder la solution de SolarBear)](https://stackoverflow.com/questions/38340078/how-to-decode-jwt-token) pour une logique plus "brute" mais fonctionnelle directement dans le contrôleur

## Port 8080 en production

ASP.NET Core 8+ a changé son comportement par défaut : sans configuration explicite, l'application écoute sur `http://[::]:8080` en production. C'est le nouveau défaut des images Docker `mcr.microsoft.com/dotnet/aspnet`.

La doc le spécifie [ici](https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port)

```yaml
# docker-compose.yml
services:
  stevenyambosfrapi:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:8080" # hôte:conteneur
    env_file:
      - ".env"
    volumes:
      - ~/.aspnet/https:/https:ro
```

On accède à l'API via `localhost:5000` → redirigé vers `8080` dans le conteneur.

---

## Clé étrangères (`foreign key`)

Étapes :

- Ajouter une clé primaire à chaque type d'entité.
- Ajouter une clé étrangère à un type d'entité.
- Associer les références entre les types d'entités à l'aide des clés primaires et étrangères afin de former une configuration de relation unique.

À savoir :

> Les propriétés de clé primaire et de clé étrangère ne doivent pas nécessairement être des propriétés visibles publiquement du type d'entité. Cependant, même lorsque ces propriétés sont masquées, il est important de garder à l'esprit qu'elles existent toujours dans le modèle EF.

Exemple :

```csharp
public class Blog
{
    [Key] // Pas obligatoire sauf si EF le mentionne lors du build
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual Uri SiteUri { get; set; }

    public ICollection<Post> Posts { get; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedOn { get; set; }
    public bool Archived { get; set; }

    [ForeignKey("post")] // Pas nécessaire
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}
```

Documentation complète :

- [ici](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#mapping-relationships-in-ef-core)
- [bonus](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete)

## Email avec pièce jointe

J'utilise `MailKit`.

Envoyer des emails [sans pièce jointe](https://dev.to/mamun_akand/sending-emails-with-gmail-using-mailkit-in-net-web-api-4lj5)

[Meilleure doc](https://www.aspsnippets.com/Articles/4222/ASPNet-Core-Send-Email-with-multiple-attachments-using-MailKit/)

Bonus :

- [Stack Overflow](https://stackoverflow.com/questions/37853903/can-i-send-files-via-email-using-mailkit)
