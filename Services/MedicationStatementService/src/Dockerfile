FROM mcr.microsoft.com/dotnet/aspnet:10.0
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
RUN apt-get update -y && apt-get install -y curl telnet
RUN dpkg -r --force-all apt apt-get && dpkg -r --force-all debconf dpkg
WORKDIR /app
COPY out/ .
RUN id app &>/dev/null || useradd --user-group --system --create-home --no-log-init app
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet","MedicationStatementService.dll"]
