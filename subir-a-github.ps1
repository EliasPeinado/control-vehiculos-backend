# Script para subir todos los archivos a GitHub
# Uso: .\subir-a-github.ps1

Write-Host "🚀 Subiendo archivos a GitHub..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Verificar si estamos en un repositorio git
if (-not (Test-Path ".git")) {
    Write-Host "⚠️  No se detectó repositorio git. Inicializando..." -ForegroundColor Yellow
    
    git init
    
    $repoUrl = Read-Host "Ingresa la URL del repositorio (ej: https://github.com/EliasPeinado/control-vehiculos-backend.git)"
    
    if ([string]::IsNullOrWhiteSpace($repoUrl)) {
        $repoUrl = "https://github.com/EliasPeinado/control-vehiculos-backend.git"
        Write-Host "Usando URL por defecto: $repoUrl" -ForegroundColor Gray
    }
    
    git remote add origin $repoUrl
    Write-Host "✅ Repositorio inicializado" -ForegroundColor Green
}

# Mostrar estado actual
Write-Host "`n📊 Estado actual del repositorio:" -ForegroundColor Cyan
git status --short

# Contar archivos nuevos y modificados
$newFiles = (git status --short | Where-Object { $_ -match '^\?\?' }).Count
$modifiedFiles = (git status --short | Where-Object { $_ -match '^ M' }).Count
$totalFiles = $newFiles + $modifiedFiles

Write-Host "`n📦 Archivos a subir:" -ForegroundColor Cyan
Write-Host "   Nuevos:      $newFiles" -ForegroundColor Green
Write-Host "   Modificados: $modifiedFiles" -ForegroundColor Yellow
Write-Host "   Total:       $totalFiles" -ForegroundColor White

if ($totalFiles -eq 0) {
    Write-Host "`n✅ No hay cambios para subir" -ForegroundColor Green
    exit 0
}

# Confirmar
Write-Host "`n¿Deseas continuar? (S/N): " -ForegroundColor Yellow -NoNewline
$confirm = Read-Host

if ($confirm -ne 'S' -and $confirm -ne 's') {
    Write-Host "❌ Operación cancelada" -ForegroundColor Red
    exit 1
}

# Agregar archivos
Write-Host "`n📥 Agregando archivos..." -ForegroundColor Cyan
git add .

# Mostrar qué se va a subir
Write-Host "`n📋 Archivos agregados:" -ForegroundColor Cyan
git status --short

# Crear commit
Write-Host "`n💬 Creando commit..." -ForegroundColor Cyan
$commitMessage = @"
feat: implementar CI/CD completo con GitHub Actions y OWASP ZAP

- Agregar workflows de GitHub Actions (main-ci, ci-cd-complete, security-scan)
- Implementar OWASP ZAP security scanning con scripts automatizados
- Corregir Dockerfile para compatibilidad con GitHub Actions
- Agregar documentación completa de implementación
- Configurar reportes automáticos y alertas de seguridad
- Implementar Serilog para logging estructurado
- Configurar OpenTelemetry para observabilidad
- Crear proyecto de tests unitarios con xUnit y FluentAssertions
- Mejorar configuración de seguridad (CORS y headers)

Cumplimiento de requisitos: 80%
"@

git commit -m $commitMessage

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al crear commit" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Commit creado exitosamente" -ForegroundColor Green

# Push a GitHub
Write-Host "`n📤 Subiendo a GitHub..." -ForegroundColor Cyan
Write-Host "   (Esto puede tomar unos momentos)" -ForegroundColor Gray

git push origin main

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n⚠️  Error al hacer push. Intentando con --force..." -ForegroundColor Yellow
    
    Write-Host "⚠️  ADVERTENCIA: Esto sobrescribirá el historial remoto" -ForegroundColor Red
    Write-Host "¿Estás seguro? (S/N): " -ForegroundColor Yellow -NoNewline
    $confirmForce = Read-Host
    
    if ($confirmForce -eq 'S' -or $confirmForce -eq 's') {
        git push origin main --force
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "`n❌ Error al hacer push incluso con --force" -ForegroundColor Red
            Write-Host "Verifica tu autenticación y permisos en GitHub" -ForegroundColor Yellow
            exit 1
        }
    } else {
        Write-Host "❌ Push cancelado" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "✅ ¡Archivos subidos exitosamente a GitHub!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

Write-Host "`n📊 Próximos pasos:" -ForegroundColor Cyan
Write-Host "   1. Ve a tu repositorio en GitHub" -ForegroundColor White
Write-Host "   2. Verifica que los archivos estén subidos" -ForegroundColor White
Write-Host "   3. Ve a la pestaña 'Actions'" -ForegroundColor White
Write-Host "   4. Los workflows se ejecutarán automáticamente" -ForegroundColor White

Write-Host "`n🔗 Links útiles:" -ForegroundColor Cyan
$repoUrl = git remote get-url origin
$repoUrl = $repoUrl -replace '\.git$', ''
Write-Host "   Repositorio: $repoUrl" -ForegroundColor Gray
Write-Host "   Actions:     $repoUrl/actions" -ForegroundColor Gray

Write-Host "`n🎉 ¡Todo listo! Los workflows comenzarán a ejecutarse en GitHub." -ForegroundColor Green
