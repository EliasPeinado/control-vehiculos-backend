# Control de Vehículos - Backend API

Sistema de gestión y control de vehículos desarrollado con .NET 8.0 Web API.

## 🚀 Características

- **API RESTful** con .NET 8.0
- **Autenticación JWT** para seguridad
- **Entity Framework Core** con SQL Server
- **Repository Pattern** para acceso a datos
- **Logging estructurado** con Serilog
- **Observabilidad** con OpenTelemetry
- **Docker** containerization
- **CI/CD** con GitHub Actions
- **Security Scanning** con OWASP ZAP
- **Tests unitarios** con xUnit

## 📁 Estructura del Proyecto

```
ControlVehiculos/
├── src/
│   ├── ControlVehiculos/          # Proyecto principal API
│   │   ├── Controllers/           # Controladores REST
│   │   ├── Models/                # Modelos de dominio
│   │   ├── Services/              # Lógica de negocio
│   │   ├── Repositories/          # Acceso a datos
│   │   ├── Data/                  # DbContext y configuración
│   │   ├── Middleware/            # Middleware personalizado
│   │   └── Exceptions/            # Excepciones personalizadas
│   └── ControlVehiculos.Tests/    # Tests unitarios
├── .github/
│   └── workflows/                 # GitHub Actions CI/CD
├── security/                      # Configuración de seguridad
├── docs/                          # Documentación
├── Dockerfile                     # Containerización
├── docker-compose.yml             # Orquestación de servicios
└── ControlVehiculos.sln           # Solution de Visual Studio

```

## 🛠️ Tecnologías

- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0**
- **SQL Server 2022**
- **JWT Authentication**
- **Serilog** - Logging estructurado
- **OpenTelemetry** - Observabilidad
- **Docker** - Containerización
- **xUnit** - Testing
- **OWASP ZAP** - Security scanning

## 📋 Requisitos Previos

- .NET 8.0 SDK
- SQL Server 2022 (o Docker)
- Docker Desktop (opcional)
- Visual Studio 2022 / VS Code / Rider

## 🚀 Inicio Rápido

### Clonar el repositorio

```bash
git clone https://github.com/EliasPeinado/control-vehiculos-backend.git
cd control-vehiculos-backend
```

### Configurar Base de Datos

1. Actualizar connection string en `src/ControlVehiculos/appsettings.json`
2. Ejecutar migraciones:

```bash
cd src/ControlVehiculos
dotnet ef database update
```

### Ejecutar la aplicación

```bash
dotnet run --project src/ControlVehiculos/ControlVehiculos.csproj
```

La API estará disponible en: `http://localhost:5000`

### Con Docker

```bash
docker-compose up -d
```

## 🧪 Tests

Ejecutar tests unitarios:

```bash
dotnet test src/ControlVehiculos.Tests/ControlVehiculos.Tests.csproj
```

Con cobertura:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Documentación API

Una vez ejecutada la aplicación, acceder a:

- **Swagger UI**: `http://localhost:5000/swagger`
- **OpenAPI Spec**: `http://localhost:5000/swagger/v1/swagger.json`

## 🔒 Seguridad

- Autenticación JWT
- Validación de entrada
- Rate limiting
- CORS configurado
- Security headers
- OWASP ZAP scanning en CI/CD

## 🔄 CI/CD

El proyecto incluye pipelines de GitHub Actions para:

- ✅ Build y compilación
- 🧪 Tests unitarios con cobertura
- 🔍 Análisis estático de código
- 🔒 Security scanning con OWASP ZAP
- 🐳 Docker build y scan con Trivy
- 📊 Reportes y métricas

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📝 Licencia

Este proyecto es parte de un trabajo académico para la Universidad de Palermo.

## 👤 Autor

**Elias Peinado**

- GitHub: [@EliasPeinado](https://github.com/EliasPeinado)

## 🙏 Agradecimientos

- Universidad de Palermo - Técnicas Avanzadas de Programación
- Comunidad .NET
- Contribuidores de open source
