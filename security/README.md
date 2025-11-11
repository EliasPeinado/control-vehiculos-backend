# 🔒 OWASP ZAP Security Scanning

Configuración y scripts para ejecutar escaneos de seguridad automatizados con OWASP ZAP.

## 📋 Tabla de Contenidos

- [Requisitos](#requisitos)
- [Métodos de Ejecución](#métodos-de-ejecución)
- [Interpretación de Resultados](#interpretación-de-resultados)
- [Integración CI/CD](#integración-cicd)
- [Configuración Avanzada](#configuración-avanzada)

---

## 🔧 Requisitos

### Opción 1: Docker (Recomendado)
- Docker Desktop instalado y corriendo
- 2GB de RAM disponible

### Opción 2: OWASP ZAP Standalone
- Descargar desde: https://www.zaproxy.org/download/
- Java 11+ instalado

---

## 🚀 Métodos de Ejecución

### Método 1: Script Simplificado (Recomendado)

**Windows (PowerShell):**
```powershell
# 1. Iniciar la API en otra terminal
cd ControlVehiculos
dotnet run

# 2. En otra terminal, ejecutar el escaneo
cd security
.\run-zap-simple.ps1 -TargetUrl "http://localhost:5000"
```

**Linux/Mac:**
```bash
# 1. Iniciar la API en otra terminal
cd ControlVehiculos
dotnet run

# 2. En otra terminal, ejecutar el escaneo
cd security
chmod +x run-zap-simple.sh
./run-zap-simple.sh http://localhost:5000
```

### Método 2: Docker Compose (Completo)

Este método levanta la API y ZAP automáticamente:

```powershell
# Windows
cd security
.\run-zap-scan.ps1
```

```bash
# Linux/Mac
cd security
chmod +x run-zap-scan.sh
./run-zap-scan.sh
```

### Método 3: Docker Manual

```bash
# Iniciar API
docker run -d -p 5000:8080 --name api-test controlvehiculos-api

# Ejecutar ZAP
docker run --rm \
  -v $(pwd)/zap:/zap/wrk:rw \
  -t ghcr.io/zaproxy/zaproxy:stable \
  zap-baseline.py \
  -t http://host.docker.internal:5000 \
  -r zap-report.html \
  -J zap-report.json \
  -I

# Limpiar
docker stop api-test && docker rm api-test
```

---

## 📊 Interpretación de Resultados

### Niveles de Severidad

| Nivel | Código | Descripción | Acción Requerida |
|-------|--------|-------------|------------------|
| 🔴 **Alta** | 3 | Vulnerabilidades críticas | **Corregir inmediatamente** |
| 🟡 **Media** | 2 | Riesgos moderados | Corregir en próximo sprint |
| 🟢 **Baja** | 1 | Riesgos menores | Considerar corrección |
| ℹ️ **Info** | 0 | Informativo | Opcional |

### Reportes Generados

1. **zap-report.html** - Reporte visual completo
   - Abre en navegador para análisis detallado
   - Incluye descripción, evidencia y recomendaciones

2. **zap-report.json** - Datos estructurados
   - Para procesamiento automatizado
   - Integración con herramientas de análisis

3. **zap-report.md** - Formato Markdown
   - Para documentación
   - Fácil de incluir en PRs

### Ejemplo de Salida

```
📈 Resumen de Vulnerabilidades:
   🔴 Alta:   0
   🟡 Media:  2
   🟢 Baja:   5
   ℹ️  Info:   12

✅ No se encontraron vulnerabilidades críticas
```

---

## 🔄 Integración CI/CD

### GitHub Actions

El workflow `.github/workflows/security-scan.yml` ejecuta automáticamente:

- ✅ En cada push a `main` o `develop`
- ✅ En cada Pull Request
- ✅ Semanalmente (lunes 2 AM)
- ✅ Manualmente desde GitHub UI

**Ver resultados:**
1. Ve a la pestaña "Actions" en GitHub
2. Selecciona el workflow "Security Scan"
3. Descarga los artifacts para ver reportes

### Azure DevOps

```yaml
# azure-pipelines.yml
- task: Docker@2
  displayName: 'Run OWASP ZAP Scan'
  inputs:
    command: 'run'
    arguments: |
      --rm -v $(Build.ArtifactStagingDirectory):/zap/wrk:rw
      ghcr.io/zaproxy/zaproxy:stable
      zap-baseline.py -t $(ApiUrl) -r zap-report.html -J zap-report.json

- task: PublishBuildArtifacts@1
  displayName: 'Publish ZAP Reports'
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'zap-reports'
```

### GitLab CI

```yaml
# .gitlab-ci.yml
zap-scan:
  stage: security
  image: ghcr.io/zaproxy/zaproxy:stable
  script:
    - zap-baseline.py -t $API_URL -r zap-report.html -J zap-report.json
  artifacts:
    paths:
      - zap-report.*
    expire_in: 30 days
  allow_failure: true
```

---

## ⚙️ Configuración Avanzada

### Archivo de Configuración

Edita `zap/zap-config.conf` para:

**Ignorar falsos positivos:**
```conf
# Ignorar alertas específicas
10021 IGNORE (X-Content-Type-Options - ya implementado)
10020 IGNORE (X-Frame-Options - ya implementado)
```

**Configurar contexto de autenticación:**
```conf
# Endpoints públicos
/v1/auth/login
/v1/auth/refresh
/v1/health

# Endpoints protegidos (requieren JWT)
/v1/turnos/*
/v1/vehiculos/*
/v1/evaluaciones/*
```

### Escaneo con Autenticación

Para escanear endpoints protegidos:

```bash
# 1. Obtener token JWT
TOKEN=$(curl -X POST http://localhost:5000/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin123!"}' \
  | jq -r '.accessToken')

# 2. Ejecutar ZAP con header de autenticación
docker run --rm \
  -v $(pwd)/zap:/zap/wrk:rw \
  ghcr.io/zaproxy/zaproxy:stable \
  zap-baseline.py \
  -t http://host.docker.internal:5000 \
  -z "-config replacer.full_list(0).description=auth1 \
      -config replacer.full_list(0).enabled=true \
      -config replacer.full_list(0).matchtype=REQ_HEADER \
      -config replacer.full_list(0).matchstr=Authorization \
      -config replacer.full_list(0).replacement=Bearer $TOKEN"
```

### Escaneo Completo (Active Scan)

⚠️ **Advertencia**: El escaneo activo puede tomar horas y genera mucho tráfico.

```bash
docker run --rm \
  -v $(pwd)/zap:/zap/wrk:rw \
  ghcr.io/zaproxy/zaproxy:stable \
  zap-full-scan.py \
  -t http://host.docker.internal:5000 \
  -r zap-full-report.html
```

---

## 🛡️ Vulnerabilidades Comunes y Soluciones

### 1. Missing Security Headers

**Problema:** Headers de seguridad no configurados

**Solución:** Ya implementado en `Program.cs`:
```csharp
context.Response.Headers["X-Content-Type-Options"] = "nosniff";
context.Response.Headers["X-Frame-Options"] = "DENY";
context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
```

### 2. CORS Misconfiguration

**Problema:** CORS permite cualquier origen

**Solución:** Configurar orígenes específicos en producción:
```json
"AllowedOrigins": [
  "https://app.controlvehiculos.com"
]
```

### 3. Information Disclosure

**Problema:** Stack traces en respuestas de error

**Solución:** Usar middleware de manejo de excepciones:
```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

### 4. Weak Authentication

**Problema:** Tokens JWT débiles o mal configurados

**Solución:** 
- Usar secretos fuertes (>32 caracteres)
- Configurar expiración corta (1 hora)
- Implementar refresh tokens

---

## 📚 Recursos Adicionales

- [OWASP ZAP Documentation](https://www.zaproxy.org/docs/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ZAP Automation Framework](https://www.zaproxy.org/docs/desktop/addons/automation-framework/)
- [Security Headers Best Practices](https://owasp.org/www-project-secure-headers/)

---

## 🤝 Contribuir

Para mejorar la configuración de seguridad:

1. Identifica falsos positivos en los reportes
2. Actualiza `zap-config.conf` para ignorarlos
3. Documenta la razón en este README
4. Crea un PR con los cambios

---

## 📞 Soporte

Si encuentras problemas:

1. Revisa los logs del contenedor: `docker logs <container-id>`
2. Verifica que la API esté accesible: `curl http://localhost:5000/v1/health`
3. Consulta la documentación de ZAP
4. Abre un issue en el repositorio

---

**Última actualización:** 11 de Noviembre, 2025
**Versión ZAP:** stable (latest)
