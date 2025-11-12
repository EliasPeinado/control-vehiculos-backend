# ============================================
# Script para Inicializar Repositorio Git desde Cero
# Conectando con repositorio EXISTENTE en GitHub
# Control de Vehiculos - Backend API
# ============================================

Write-Host "🚀 Inicializando repositorio Git limpio..." -ForegroundColor Cyan
Write-Host ""

# Paso 1: Eliminar repositorio Git existente
Write-Host "📦 Paso 1: Eliminando repositorio Git existente..." -ForegroundColor Yellow
if (Test-Path ".git") {
    Remove-Item -Path ".git" -Recurse -Force
    Write-Host "✅ Repositorio anterior eliminado" -ForegroundColor Green
} else {
    Write-Host "⚠️  No se encontró repositorio anterior" -ForegroundColor Yellow
}
Write-Host ""

# Paso 2: Limpiar carpetas .git dentro de src/
Write-Host "📦 Paso 2: Limpiando carpetas .git internas..." -ForegroundColor Yellow
if (Test-Path "src/ControlVehiculos/.git") {
    Remove-Item -Path "src/ControlVehiculos/.git" -Recurse -Force
    Write-Host "✅ .git eliminado de src/ControlVehiculos/" -ForegroundColor Green
}
if (Test-Path "src/ControlVehiculos/.github") {
    Remove-Item -Path "src/ControlVehiculos/.github" -Recurse -Force
    Write-Host "✅ .github eliminado de src/ControlVehiculos/" -ForegroundColor Green
}
Write-Host ""

# Paso 3: Inicializar nuevo repositorio Git
Write-Host "📦 Paso 3: Inicializando nuevo repositorio Git..." -ForegroundColor Yellow
git init
git branch -M main
Write-Host "✅ Repositorio Git inicializado con rama 'main'" -ForegroundColor Green
Write-Host ""

# Paso 4: Verificar archivos que se van a trackear
Write-Host "📦 Paso 4: Verificando archivos a incluir..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Archivos que serán incluidos en el commit:" -ForegroundColor Cyan
git add --dry-run -A
Write-Host ""

# Paso 5: Agregar todos los archivos
Write-Host "📦 Paso 5: Agregando archivos al staging..." -ForegroundColor Yellow
git add -A
Write-Host "✅ Archivos agregados al staging" -ForegroundColor Green
Write-Host ""

# Paso 6: Mostrar estado
Write-Host "📦 Paso 6: Estado del repositorio..." -ForegroundColor Yellow
git status
Write-Host ""

# Paso 7: Crear commit inicial
Write-Host "📦 Paso 7: Creando commit inicial..." -ForegroundColor Yellow
$commitMessage = @"
🎉 Initial commit - Control de Vehiculos API

- Estructura de proyecto limpia con src/
- .NET 8.0 Web API
- Entity Framework Core con SQL Server
- Autenticacion JWT
- Repository Pattern
- Tests unitarios con xUnit
- GitHub Actions CI/CD completo
- OWASP ZAP security scanning
- Docker containerization
- Logging con Serilog
- Observabilidad con OpenTelemetry
"@
git commit -m $commitMessage

Write-Host "✅ Commit inicial creado" -ForegroundColor Green
Write-Host ""

# Paso 8: Conectar con GitHub (repositorio existente)
Write-Host "📦 Paso 8: Conectando con GitHub..." -ForegroundColor Yellow
git remote add origin https://github.com/EliasPeinado/control-vehiculos-backend.git
Write-Host "✅ Remote 'origin' configurado" -ForegroundColor Green
Write-Host ""

# Paso 9: Confirmación antes de push
Write-Host "============================================" -ForegroundColor Red
Write-Host "⚠️  ADVERTENCIA IMPORTANTE" -ForegroundColor Red
Write-Host "============================================" -ForegroundColor Red
Write-Host ""
Write-Host "Estás a punto de SOBRESCRIBIR completamente el repositorio:" -ForegroundColor Yellow
Write-Host "https://github.com/EliasPeinado/control-vehiculos-backend" -ForegroundColor Cyan
Write-Host ""
Write-Host "Esto eliminará TODO el historial anterior." -ForegroundColor Yellow
Write-Host ""
$confirmacion = Read-Host "¿Estás seguro? Escribe 'SI' para continuar"

if ($confirmacion -eq "SI") {
    Write-Host ""
    Write-Host "📦 Paso 10: Haciendo push a GitHub (force)..." -ForegroundColor Yellow
    git push -u origin main --force
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Green
    Write-Host "✅ ¡REPOSITORIO ACTUALIZADO CON ÉXITO!" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎉 Tu repositorio está ahora en:" -ForegroundColor Cyan
    Write-Host "   https://github.com/EliasPeinado/control-vehiculos-backend" -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Los GitHub Actions se ejecutarán automáticamente:" -ForegroundColor Cyan
    Write-Host "   ✅ Build y compilación" -ForegroundColor Green
    Write-Host "   ✅ Tests unitarios" -ForegroundColor Green
    Write-Host "   ✅ Análisis estático" -ForegroundColor Green
    Write-Host "   ✅ Security scan con OWASP ZAP" -ForegroundColor Green
    Write-Host "   ✅ Docker build y scan" -ForegroundColor Green
    Write-Host ""
    Write-Host "🔗 Ver en GitHub:" -ForegroundColor Cyan
    Write-Host "   https://github.com/EliasPeinado/control-vehiculos-backend" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🔗 Ver Actions:" -ForegroundColor Cyan
    Write-Host "   https://github.com/EliasPeinado/control-vehiculos-backend/actions" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ Operación cancelada" -ForegroundColor Red
    Write-Host ""
    Write-Host "El commit local está creado pero NO se hizo push." -ForegroundColor Yellow
    Write-Host "Puedes hacer push manualmente cuando estés listo:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   git push -u origin main --force" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host "============================================" -ForegroundColor Cyan
