#!/bin/bash

# Script para ejecutar OWASP ZAP Baseline Scan
# Uso: ./run-zap-scan.sh

set -e

echo "🔒 Iniciando OWASP ZAP Security Scan..."

# Verificar que Docker esté corriendo
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker no está corriendo. Por favor inicia Docker."
    exit 1
fi

# Crear directorio para reportes si no existe
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPORT_DIR="$SCRIPT_DIR/zap"
mkdir -p "$REPORT_DIR"
echo "📁 Directorio de reportes: $REPORT_DIR"

# Limpiar reportes anteriores
echo "🧹 Limpiando reportes anteriores..."
rm -f "$REPORT_DIR/zap-report."*

# Navegar al directorio raíz del proyecto
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

echo "🚀 Levantando servicios con Docker Compose..."
docker-compose -f docker-compose.zap.yml up --build --abort-on-container-exit

# Capturar el código de salida
EXIT_CODE=$?

# Detener y limpiar contenedores
echo ""
echo "🛑 Deteniendo servicios..."
docker-compose -f docker-compose.zap.yml down

# Verificar si se generaron reportes
HTML_REPORT="$REPORT_DIR/zap-report.html"
JSON_REPORT="$REPORT_DIR/zap-report.json"
MD_REPORT="$REPORT_DIR/zap-report.md"

echo ""
echo "📊 Resultados del Escaneo:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ -f "$HTML_REPORT" ]; then
    echo "✅ Reporte HTML: $HTML_REPORT"
    echo "   Abre este archivo en tu navegador para ver los detalles"
fi

if [ -f "$JSON_REPORT" ]; then
    echo "✅ Reporte JSON: $JSON_REPORT"
    
    # Analizar el reporte JSON para mostrar resumen
    if command -v jq &> /dev/null; then
        HIGH=$(jq '[.site[0].alerts[] | select(.riskcode == "3")] | length' "$JSON_REPORT")
        MEDIUM=$(jq '[.site[0].alerts[] | select(.riskcode == "2")] | length' "$JSON_REPORT")
        LOW=$(jq '[.site[0].alerts[] | select(.riskcode == "1")] | length' "$JSON_REPORT")
        INFO=$(jq '[.site[0].alerts[] | select(.riskcode == "0")] | length' "$JSON_REPORT")
        
        echo ""
        echo "📈 Resumen de Alertas:"
        echo "   🔴 Alta:   $HIGH"
        echo "   🟡 Media:  $MEDIUM"
        echo "   🟢 Baja:   $LOW"
        echo "   ℹ️  Info:   $INFO"
        
        if [ "$HIGH" -gt 0 ]; then
            echo ""
            echo "⚠️  ADVERTENCIA: Se encontraron $HIGH vulnerabilidades de severidad ALTA"
            echo "   Revisa el reporte HTML para más detalles"
        elif [ "$MEDIUM" -gt 0 ]; then
            echo ""
            echo "⚠️  Se encontraron $MEDIUM vulnerabilidades de severidad MEDIA"
        else
            echo ""
            echo "✅ No se encontraron vulnerabilidades de alta o media severidad"
        fi
    else
        echo "   (Instala 'jq' para ver el resumen de alertas)"
    fi
fi

if [ -f "$MD_REPORT" ]; then
    echo "✅ Reporte Markdown: $MD_REPORT"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Determinar el resultado final
if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "✅ Escaneo completado exitosamente"
    exit 0
elif [ $EXIT_CODE -eq 1 ]; then
    echo ""
    echo "⚠️  Escaneo completado con advertencias"
    exit 1
else
    echo ""
    echo "❌ Escaneo falló con errores"
    exit $EXIT_CODE
fi
