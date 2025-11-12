# Script para ejecutar OWASP ZAP Baseline Scan
# Uso: .\run-zap-scan.ps1

Write-Host "🔒 Iniciando OWASP ZAP Security Scan..." -ForegroundColor Cyan

# Verificar que Docker esté corriendo
$dockerRunning = docker info 2>$null
if (-not $dockerRunning) {
    Write-Host "❌ Error: Docker no está corriendo. Por favor inicia Docker Desktop." -ForegroundColor Red
    exit 1
}

# Crear directorio para reportes si no existe
$reportDir = Join-Path $PSScriptRoot "zap"
if (-not (Test-Path $reportDir)) {
    New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    Write-Host "📁 Directorio de reportes creado: $reportDir" -ForegroundColor Green
}

# Limpiar reportes anteriores
Write-Host "🧹 Limpiando reportes anteriores..." -ForegroundColor Yellow
Remove-Item "$reportDir\zap-report.*" -ErrorAction SilentlyContinue

# Navegar al directorio raíz del proyecto
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $projectRoot

Write-Host "🚀 Levantando servicios con Docker Compose..." -ForegroundColor Cyan
docker-compose -f docker-compose.zap.yml up --build --abort-on-container-exit

# Capturar el código de salida
$exitCode = $LASTEXITCODE

# Detener y limpiar contenedores
Write-Host "`n🛑 Deteniendo servicios..." -ForegroundColor Yellow
docker-compose -f docker-compose.zap.yml down

# Verificar si se generaron reportes
$htmlReport = Join-Path $reportDir "zap-report.html"
$jsonReport = Join-Path $reportDir "zap-report.json"
$mdReport = Join-Path $reportDir "zap-report.md"

Write-Host "`n📊 Resultados del Escaneo:" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

if (Test-Path $htmlReport) {
    Write-Host "✅ Reporte HTML: $htmlReport" -ForegroundColor Green
    Write-Host "   Abre este archivo en tu navegador para ver los detalles" -ForegroundColor Gray
}

if (Test-Path $jsonReport) {
    Write-Host "✅ Reporte JSON: $jsonReport" -ForegroundColor Green
    
    # Analizar el reporte JSON para mostrar resumen
    try {
        $zapData = Get-Content $jsonReport -Raw | ConvertFrom-Json
        $alerts = $zapData.site[0].alerts
        
        $high = ($alerts | Where-Object { $_.riskcode -eq "3" }).Count
        $medium = ($alerts | Where-Object { $_.riskcode -eq "2" }).Count
        $low = ($alerts | Where-Object { $_.riskcode -eq "1" }).Count
        $info = ($alerts | Where-Object { $_.riskcode -eq "0" }).Count
        
        Write-Host "`n📈 Resumen de Alertas:" -ForegroundColor Cyan
        Write-Host "   🔴 Alta:   $high" -ForegroundColor $(if ($high -gt 0) { "Red" } else { "Gray" })
        Write-Host "   🟡 Media:  $medium" -ForegroundColor $(if ($medium -gt 0) { "Yellow" } else { "Gray" })
        Write-Host "   🟢 Baja:   $low" -ForegroundColor $(if ($low -gt 0) { "Green" } else { "Gray" })
        Write-Host "   ℹ️  Info:   $info" -ForegroundColor Gray
        
        if ($high -gt 0) {
            Write-Host "`n⚠️  ADVERTENCIA: Se encontraron $high vulnerabilidades de severidad ALTA" -ForegroundColor Red
            Write-Host "   Revisa el reporte HTML para más detalles" -ForegroundColor Yellow
        } elseif ($medium -gt 0) {
            Write-Host "`n⚠️  Se encontraron $medium vulnerabilidades de severidad MEDIA" -ForegroundColor Yellow
        } else {
            Write-Host "`n✅ No se encontraron vulnerabilidades de alta o media severidad" -ForegroundColor Green
        }
    } catch {
        Write-Host "⚠️  No se pudo analizar el reporte JSON" -ForegroundColor Yellow
    }
}

if (Test-Path $mdReport) {
    Write-Host "✅ Reporte Markdown: $mdReport" -ForegroundColor Green
}

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Determinar el resultado final
if ($exitCode -eq 0) {
    Write-Host "`n✅ Escaneo completado exitosamente" -ForegroundColor Green
    exit 0
} elseif ($exitCode -eq 1) {
    Write-Host "`n⚠️  Escaneo completado con advertencias" -ForegroundColor Yellow
    exit 1
} else {
    Write-Host "`n❌ Escaneo falló con errores" -ForegroundColor Red
    exit $exitCode
}
