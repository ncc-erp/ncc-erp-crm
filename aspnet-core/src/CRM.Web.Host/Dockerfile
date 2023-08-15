FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine

WORKDIR /app
COPY . .

RUN apk add tzdata
RUN cp /usr/share/zoneinfo/Asia/Ho_Chi_Minh /etc/localtime

ENTRYPOINT ["dotnet", "CRM.Web.Host.dll"]
