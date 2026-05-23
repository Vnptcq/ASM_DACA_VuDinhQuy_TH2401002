FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HR_Management.sln", "./"]
COPY ["HR_Management.Domain/HR_Management.Domain.csproj", "HR_Management.Domain/"]
COPY ["HR_Management.Application/HR_Management.Application.csproj", "HR_Management.Application/"]
COPY ["HR_Management.Infrastructure/HR_Management.Infrastructure.csproj", "HR_Management.Infrastructure/"]
COPY ["HR_Management.Web/HR_Management.Web.csproj", "HR_Management.Web/"]

RUN dotnet restore "HR_Management.Web/HR_Management.Web.csproj"

COPY . .
WORKDIR "/src/HR_Management.Web"
RUN dotnet publish "HR_Management.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HR_Management.Web.dll"]
