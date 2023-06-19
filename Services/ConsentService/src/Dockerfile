FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
RUN apt-get update -y && \ 
    apt-get install -y curl telnet
WORKDIR /app
COPY . .
EXPOSE 8080
ENTRYPOINT ["dotnet","/app/bin/Debug/net6.0/ConsentService.dll"]
