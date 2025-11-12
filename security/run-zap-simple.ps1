# Script simplificado para ejecutar OWASP ZAP contra API local
# Uso: .\run-zap-simple.ps1 -TargetUrl "http://localhost:5000"

param(
    [string]$TargetUrl = "http://localhost:5000",
    [switch]$GenerateReport = $true
)

Write-Host "🔒 OWASP ZAP Baseline Scan" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Verificar que Docker esté corriendo
$dockerRunning = docker info 2>$null
if (-not $dockerRunning) {
    Write-Host "❌ Error: Docker no está corriendo." -ForegroundColor Red
    exit 1
}

# Crear directorio para reportes
$reportDir = Join-Path $PSScriptRoot "zap"
if (-not (Test-Path $reportDir)) {
    New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
}

Write-Host "🎯 Target: $TargetUrl" -ForegroundColor Yellow
Write-Host "📁 Reportes: $reportDir" -ForegroundColor Yellow
Write-Host ""

# Limpiar reportes anteriores
Remove-Item "$reportDir\zap-report.*" -ErrorAction SilentlyContinue

Write-Host "🚀 Ejecutando escaneo ZAP..." -ForegroundColor Cyan
Write-Host "   (Esto puede tomar varios minutos)" -ForegroundColor Gray
Write-Host ""

# Ejecutar ZAP en contenedor Docker
$zapCommand = @"
docker run --rm ``
    -v "${reportDir}:/zap/wrk:rw" ``
    -t ghcr.io/zaproxy/zaproxy:stable ``
    zap-baseline.py ``
    -t $TargetUrl ``
    -r zap-report.html ``
    -J zap-report.json ``
    -w zap-report.md ``
    -I
"@

Invoke-Expression $zapCommand

$exitCode = $LASTEXITCODE

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Verificar reportes generados
$htmlReport = Join-Path $reportDir "zap-report.html"
$jsonReport = Join-Path $reportDir "zap-report.json"

if (Test-Path $htmlReport) {
    Write-Host "✅ Reporte HTML generado" -ForegroundColor Green
    Write-Host "   📄 $htmlReport" -ForegroundColor Gray
    
    # Abrir reporte en navegador
    if ($GenerateReport) {
        Start-Process $htmlReport
    }
}

if (Test-Path $jsonReport) {
    Write-Host "✅ Reporte JSON generado" -ForegroundColor Green
    Write-Host "   📄 $jsonReport" -ForegroundColor Gray
    
    # Analizar resumen
    try {
        $zapData = Get-Content $jsonReport -Raw | ConvertFrom-Json
        $alerts = $zapData.site[0].alerts
        
        $high = ($alerts | Where-Object { $_.riskcode -eq "3" }).Count
        $medium = ($alerts | Where-Object { $_.riskcode -eq "2" }).Count
        $low = ($alerts | Where-Object { $_.riskcode -eq "1" }).Count
        $info = ($alerts | Where-Object { $_.riskcode -eq "0" }).Count
        
        Write-Host ""
        Write-Host "📈 Resumen de Vulnerabilidades:" -ForegroundColor Cyan
        Write-Host "   🔴 Alta:   $high" -ForegroundColor $(if ($high -gt 0) { "Red" } else { "Gray" })
        Write-Host "   🟡 Media:  $medium" -ForegroundColor $(if ($medium -gt 0) { "Yellow" } else { "Gray" })
        Write-Host "   🟢 Baja:   $low" -ForegroundColor $(if ($low -gt 0) { "Green" } else { "Gray" })
        Write-Host "   ℹ️  Info:   $info" -ForegroundColor Gray
        
        Write-Host ""
        if ($high -gt 0) {
            Write-Host "⚠️  CRÍTICO: $high vulnerabilidades de severidad ALTA encontradas" -ForegroundColor Red
            exit 2
        } elseif ($medium -gt 0) {
            Write-Host "⚠️  ADVERTENCIA: $medium vulnerabilidades de severidad MEDIA encontradas" -ForegroundColor Yellow
            exit 1
        } else {
            Write-Host "✅ No se encontraron vulnerabilidades críticas" -ForegroundColor Green
            exit 0
        }
    } catch {
        Write-Host "⚠️  No se pudo analizar el reporte" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

exit $exitCode
