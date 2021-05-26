FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY HelloAspNet/*.csproj ./HelloAspNet/
COPY HelloAspNet.Tests/*.csproj ./HelloAspNet.Tests/
COPY MathematicsHelper/*.csproj ./MathematicsHelper/
COPY *.sln .
RUN dotnet restore

COPY ./ ./
WORKDIR /app/HelloAspNet
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=build-env /app/HelloAspNet/out .
ENTRYPOINT ["dotnet", "HelloAspNet.dll"]
