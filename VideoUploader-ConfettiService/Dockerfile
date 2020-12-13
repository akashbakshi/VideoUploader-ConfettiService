FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["VideoUploader-ConfettiService/VideoUploader_ConfettiService.csproj", "VideoUploader-ConfettiService/"]

RUN dotnet restore "VideoUploader-ConfettiService/VideoUploader_ConfettiService.csproj"
COPY . .
WORKDIR "/src/VideoUploader-ConfettiService"
RUN dotnet build "VideoUploader_ConfettiService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VideoUploader_ConfettiService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoUploader_ConfettiService.dll"]