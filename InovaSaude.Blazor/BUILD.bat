@echo off
echo ========================================
echo    InovaSaude - Compilar Projeto
echo ========================================
echo.
echo Compilando...
echo.

cd /d "%~dp0"

dotnet build

echo.
echo Compilacao concluida!
echo.

pause
