#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/HealthMed.Api/HealthMed.Api.csproj", "src/Presentation/HealthMed.Api/"]
COPY ["src/Contexts/Doctor/HealthMed.Doctor.IoC/HealthMed.Doctor.IoC.csproj", "src/Contexts/Doctor/HealthMed.Doctor.IoC/"]
COPY ["src/Common/HealthMed.Common/HealthMed.Common.csproj", "src/Common/HealthMed.Common/"]
COPY ["src/Contexts/Doctor/HealthMed.Doctor.Application/HealthMed.Doctor.Application.csproj", "src/Contexts/Doctor/HealthMed.Doctor.Application/"]
COPY ["src/Contexts/Doctor/HealthMed.Doctor.Domain/HealthMed.Doctor.Domain.csproj", "src/Contexts/Doctor/HealthMed.Doctor.Domain/"]
COPY ["src/Contexts/Doctor/HealthMed.Doctor.Infrastructure/HealthMed.Doctor.Infrastructure.csproj", "src/Contexts/Doctor/HealthMed.Doctor.Infrastructure/"]
COPY ["src/Contexts/Patient/HealthMed.Patient.IoC/HealthMed.Patient.IoC.csproj", "src/Contexts/Patient/HealthMed.Patient.IoC/"]
COPY ["src/Contexts/Patient/HealthMed.Patient.Application/HealthMed.Patient.Application.csproj", "src/Contexts/Patient/HealthMed.Patient.Application/"]
COPY ["src/Contexts/Patient/HealthMed.Patient.Domain/HealthMed.Patient.Domain.csproj", "src/Contexts/Patient/HealthMed.Patient.Domain/"]
COPY ["src/Contexts/Patient/HealthMed.Patient.Infrastructure/HealthMed.Patient.Infrastructure.csproj", "src/Contexts/Patient/HealthMed.Patient.Infrastructure/"]
RUN dotnet restore "./src/Presentation/HealthMed.Api/HealthMed.Api.csproj"
COPY . .
WORKDIR "/src/src/Presentation/HealthMed.Api"
RUN dotnet build "./HealthMed.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HealthMed.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HealthMed.Api.dll"]