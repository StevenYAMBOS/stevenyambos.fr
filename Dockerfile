FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -o publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
COPY --from=build /App/publish .
ENTRYPOINT ["dotnet", "BACK_END.dll"]