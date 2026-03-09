
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY ["BACK_END.csproj", "./"]
RUN dotnet restore "./BACK_END.csproj"
COPY . ./
RUN dotnet build "BACK_END.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BACK_END.csproj" -c Release -o /app/publish

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BACK_END.dll"]
