FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /api
COPY ./API.csproj ./
RUN dotnet restore
COPY . .
ENTRYPOINT [ "dotnet", "watch", "run", "--no-restore", "--urls", "http://0.0.0.0:5000" ]