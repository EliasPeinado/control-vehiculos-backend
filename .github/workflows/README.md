# 🤖 GitHub Actions Workflows

Este directorio contiene los workflows de CI/CD que se ejecutan automáticamente en GitHub.

## 📋 Workflows Disponibles

### 1. `main-ci.yml` - Pipeline Principal (Recomendado)

**Trigger:**
- ✅ Push a `main` o `develop`
- ✅ Pull Requests a `main` o `develop`
- ✅ Manual (workflow_dispatch)

**Jobs:**
1. **Build and Test** 🔨
   - Compila el proyecto
   - Ejecuta tests unitarios (xUnit)
   - Genera reportes de tests

2. **Security Scan** 🔒
   - Inicia la API
   - Ejecuta OWASP ZAP baseline scan
   - Genera reportes de seguridad

3. **Docker Build** 🐳
   - Construye imagen Docker
   - Valida Dockerfile multi-stage

**Duración estimada:** 5-8 minutos

---

### 2. `ci-cd-complete.yml` - Pipeline Completo

**Trigger:**
- ✅ Push a `main` o `develop`
- ✅ Pull Requests a `main` o `develop`
- ✅ Manual (workflow_dispatch)

**Jobs:**
1. **Build & Compile** 🔨
2. **Unit Tests** 🧪 (con cobertura de código)
3. **Static Analysis** 🔍
4. **OWASP ZAP Security Scan** 🔒
5. **Docker Build** 🐳 (con Trivy scan)
6. **Pipeline Summary** 📊

**Duración estimada:** 10-15 minutos

---

### 3. `security-scan.yml` - Escaneo de Seguridad

**Trigger:**
- ✅ Push a `main` o `develop`
- ✅ Pull Requests a `main`
- ✅ Semanalmente (lunes 2 AM)
- ✅ Manual (workflow_dispatch)

**Jobs:**
- Escaneo completo de seguridad con OWASP ZAP
- Crea issues automáticamente si encuentra vulnerabilidades altas

**Duración estimada:** 5-10 minutos

---

## 🚀 Cómo Funcionan

### Flujo Automático

```
┌─────────────────┐
│   Git Push      │
│   to GitHub     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  GitHub Actions │
│   Triggered     │
└────────┬────────┘
         │
         ├─────────────────────────────────┐
         │                                 │
         ▼                                 ▼
┌─────────────────┐              ┌─────────────────┐
│  Build & Test   │              │ Security Scan   │
│                 │              │                 │
│  ✓ Compile      │              │  ✓ OWASP ZAP    │
│  ✓ Unit Tests   │              │  ✓ Trivy        │
│  ✓ Coverage     │              │  ✓ CodeQL       │
└────────┬────────┘              └────────┬────────┘
         │                                 │
         └─────────────┬───────────────────┘
                       │
                       ▼
              ┌─────────────────┐
              │  Docker Build   │
              │                 │
              │  ✓ Multi-stage  │
              │  ✓ Optimized    │
              └────────┬────────┘
                       │
                       ▼
              ┌─────────────────┐
              │   ✅ Success     │
              │   or            │
              │   ❌ Failure     │
              └─────────────────┘
```

---

## 📊 Ver Resultados

### En GitHub

1. Ve a la pestaña **"Actions"** en tu repositorio
2. Selecciona el workflow que quieres ver
3. Haz clic en un run específico para ver detalles

### Artifacts Generados

Después de cada ejecución, puedes descargar:

- 📄 **test-results** - Resultados de tests unitarios
- 📄 **zap-report** - Reportes de seguridad OWASP ZAP
- 📄 **coverage-report** - Cobertura de código
- 📄 **trivy-results** - Escaneo de vulnerabilidades Docker

**Retención:** 30 días

---

## 🔔 Notificaciones

### Issues Automáticos

Si el escaneo de seguridad encuentra vulnerabilidades **ALTAS**, se crea automáticamente un issue con:

- 🏷️ Labels: `security`, `high-priority`, `automated`
- 📝 Descripción del problema
- 🔗 Link al workflow run
- 📊 Resumen de vulnerabilidades

### Status Checks

Todos los workflows aparecen como **status checks** en Pull Requests:

```
✅ Build and Test — passed
✅ Security Scan — passed
✅ Docker Build — passed
```

---

## ⚙️ Configuración

### Variables de Entorno

Configuradas en los workflows:

```yaml
env:
  DOTNET_VERSION: 8.0.x
  ASPNETCORE_ENVIRONMENT: Development
```

### Secrets Requeridos

**Ninguno** - Los workflows actuales no requieren secrets.

Para deployment futuro, agregar en **Settings > Secrets**:
- `DOCKER_USERNAME`
- `DOCKER_PASSWORD`
- `DEPLOY_KEY`

---

## 🛠️ Personalización

### Ejecutar Workflow Manualmente

1. Ve a **Actions** > Selecciona el workflow
2. Haz clic en **"Run workflow"**
3. Selecciona la branch
4. Haz clic en **"Run workflow"**

### Modificar Triggers

Edita el archivo `.yml` y cambia la sección `on:`:

```yaml
on:
  push:
    branches: [ main, develop, feature/* ]  # Agregar más branches
  schedule:
    - cron: '0 2 * * *'  # Diario a las 2 AM
```

### Agregar Más Jobs

```yaml
jobs:
  my-custom-job:
    name: My Custom Job
    runs-on: ubuntu-latest
    needs: build-and-test  # Depende de otro job
    
    steps:
      - name: Do something
        run: echo "Hello World"
```

---

## 🐛 Troubleshooting

### Workflow Falla en Build

**Problema:** Error de compilación

**Solución:**
```bash
# Verificar localmente
dotnet build --configuration Release
```

### Workflow Falla en Tests

**Problema:** Tests no pasan

**Solución:**
```bash
# Ejecutar tests localmente
dotnet test --logger "console;verbosity=detailed"
```

### Security Scan Timeout

**Problema:** OWASP ZAP tarda mucho

**Solución:**
- Reducir alcance del escaneo
- Aumentar timeout en el workflow
- Usar `fail_action: false` para no bloquear

### Docker Build Falla

**Problema:** Error en Dockerfile

**Solución:**
```bash
# Probar build localmente
docker build -f ControlVehiculos/Dockerfile .
```

---

## 📈 Métricas y Badges

### Agregar Badges al README

```markdown
![CI/CD](https://github.com/USUARIO/REPO/workflows/Main%20CI%20Pipeline/badge.svg)
![Security](https://github.com/USUARIO/REPO/workflows/Security%20Scan/badge.svg)
```

### Ver Estadísticas

- **Success Rate:** % de workflows exitosos
- **Average Duration:** Tiempo promedio de ejecución
- **Failure Trends:** Tendencias de fallos

---

## 🔒 Seguridad

### Permisos de Workflows

Los workflows tienen permisos limitados por defecto:
- ✅ Leer código
- ✅ Crear artifacts
- ✅ Crear issues (solo security-scan)
- ❌ Push a branches protegidas
- ❌ Modificar settings

### Secrets

**Nunca** incluyas secrets en el código:
- ❌ Contraseñas
- ❌ API Keys
- ❌ Tokens

Usa **GitHub Secrets** en su lugar.

---

## 📚 Recursos

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Workflow Syntax](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions)
- [OWASP ZAP Action](https://github.com/zaproxy/action-baseline)
- [.NET Actions](https://github.com/actions/setup-dotnet)

---

## 🤝 Contribuir

Para mejorar los workflows:

1. Crea una branch: `feature/improve-ci`
2. Modifica el workflow
3. Prueba localmente con [act](https://github.com/nektos/act)
4. Crea un Pull Request
5. Verifica que los checks pasen

---

**Última actualización:** 11 de Noviembre, 2025
**Mantenido por:** Equipo de DevOps
