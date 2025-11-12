# ============================================
# Script para Inicializar Repositorio Git desde Cero
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
git commit -m "🎉 Initial commit - Control de Vehiculos API

- Estructura de proyecto limpia con src/
- .NET 8.0 Web API
- Entity Framework Core con SQL Server
- Autenticación JWT
- Repository Pattern
- Tests unitarios con xUnit
- GitHub Actions CI/CD completo
- OWASP ZAP security scanning
- Docker containerization
- Logging con Serilog
- Observabilidad con OpenTelemetry"

Write-Host "✅ Commit inicial creado" -ForegroundColor Green
Write-Host ""

# Paso 8: Instrucciones para conectar con GitHub
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "📋 PRÓXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1️⃣  Ve a GitHub y crea un nuevo repositorio vacío" -ForegroundColor White
Write-Host "   Nombre sugerido: control-vehiculos-backend" -ForegroundColor Gray
Write-Host ""
Write-Host "2️⃣  NO inicialices con README, .gitignore o licencia" -ForegroundColor White
Write-Host ""
Write-Host "3️⃣  Ejecuta estos comandos para conectar con GitHub:" -ForegroundColor White
Write-Host ""
Write-Host "   git remote add origin https://github.com/TU_USUARIO/TU_REPO.git" -ForegroundColor Yellow
Write-Host "   git push -u origin main" -ForegroundColor Yellow
Write-Host ""
Write-Host "4️⃣  Los GitHub Actions se ejecutarán automáticamente:" -ForegroundColor White
Write-Host "   ✅ Build y compilación" -ForegroundColor Green
Write-Host "   ✅ Tests unitarios" -ForegroundColor Green
Write-Host "   ✅ Análisis estático" -ForegroundColor Green
Write-Host "   ✅ Security scan con OWASP ZAP" -ForegroundColor Green
Write-Host "   ✅ Docker build y scan" -ForegroundColor Green
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "🎉 Repositorio listo para subir a GitHub!" -ForegroundColor Green
Write-Host "============================================" -ForegroundColor Cyan
