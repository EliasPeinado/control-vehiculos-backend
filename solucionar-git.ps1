# Script para solucionar problemas de git
# Uso: .\solucionar-git.ps1

Write-Host "🔧 Solucionando problemas de git..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# 1. Eliminar archivos bloqueados
Write-Host "🗑️  Eliminando archivos bloqueados de .vs/" -ForegroundColor Yellow
Remove-Item -Force -Recurse .vs -ErrorAction SilentlyContinue

# 2. Renombrar branch a main
Write-Host "🔄 Renombrando branch master a main" -ForegroundColor Cyan
if (git branch --show-current -eq "master") {
    git branch -m main
}

# 3. Agregar archivos importantes
git add .gitignore
git add .github/
git add security/
git add *.md
git add docker-compose.zap.yml
git add subir-a-github.ps1
git add ControlVehiculos/Dockerfile

Write-Host "📦 Archivos agregados correctamente" -ForegroundColor Green

# 4. Hacer commit
Write-Host "💬 Creando commit" -ForegroundColor Cyan
git commit -m "Implementación completa de CI/CD con GitHub Actions y OWASP ZAP"

# 5. Subir a GitHub
Write-Host "🚀 Subiendo a GitHub" -ForegroundColor Cyan
git push -u origin main --force

Write-Host "✅ ¡Problemas solucionados y código subido!" -ForegroundColor Green
Write-Host "🔗 Ve a: https://github.com/EliasPeinado/control-vehiculos-backend/actions" -ForegroundColor Cyan
