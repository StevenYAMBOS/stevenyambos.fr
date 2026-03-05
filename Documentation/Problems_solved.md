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


