FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /api
COPY ./*.csproj ./
RUN dotnet restore
COPY . .
ENTRYPOINT [ "dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000" ]
