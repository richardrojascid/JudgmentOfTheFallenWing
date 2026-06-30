@echo off
REM Sincroniza la carpeta local con github.com/richardrojascid/artemisa (main)
echo.
echo  Artemisa - sincronizar con GitHub
echo  Repo: https://github.com/richardrojascid/artemisa
echo.

set TARGET=%~1
if "%TARGET%"=="" set TARGET=C:\Users\Richard Rojas\artemisa

if not exist "%TARGET%\.git" (
    echo Clonando en %TARGET% ...
    git clone https://github.com/richardrojascid/artemisa.git "%TARGET%"
    if errorlevel 1 (
        echo Error al clonar.
        pause
        exit /b 1
    )
)

cd /d "%TARGET%"
echo Carpeta: %CD%
git pull origin main

if errorlevel 1 (
    echo.
    echo Error al actualizar. Verifica acceso a GitHub.
    pause
    exit /b 1
)

echo.
echo Listo. Ultimo commit:
git log -1 --oneline
echo.
echo Abre: http://localhost:8080/login.php
echo (Ejecuta scripts\serve-local.bat desde esta carpeta)
pause
