# Script para inicializar Git con repo existente
Write-Host "Inicializando repositorio Git..." -ForegroundColor Cyan

# Limpiar .git existente
if (Test-Path ".git") {
    Remove-Item -Path ".git" -Recurse -Force
    Write-Host "Git anterior eliminado" -ForegroundColor Green
}

# Limpiar .git internos
if (Test-Path "src/ControlVehiculos/.git") {
    Remove-Item -Path "src/ControlVehiculos/.git" -Recurse -Force
}
if (Test-Path "src/ControlVehiculos/.github") {
    Remove-Item -Path "src/ControlVehiculos/.github" -Recurse -Force
}

# Inicializar
git init
git branch -M main

# Agregar archivos
git add -A

# Commit
git commit -m "Initial commit - Control de Vehiculos API - Estructura reorganizada con src/"

# Conectar con GitHub
git remote add origin https://github.com/EliasPeinado/control-vehiculos-backend.git

Write-Host ""
Write-Host "ADVERTENCIA: Vas a sobrescribir el repositorio existente" -ForegroundColor Red
Write-Host "https://github.com/EliasPeinado/control-vehiculos-backend" -ForegroundColor Yellow
Write-Host ""
$confirm = Read-Host "Escribe SI para continuar"

if ($confirm -eq "SI") {
    git push -u origin main --force
    Write-Host ""
    Write-Host "Repositorio actualizado!" -ForegroundColor Green
    Write-Host "Ver en: https://github.com/EliasPeinado/control-vehiculos-backend" -ForegroundColor Cyan
} else {
    Write-Host "Cancelado. Puedes hacer push manualmente con: git push -u origin main --force" -ForegroundColor Yellow
}
