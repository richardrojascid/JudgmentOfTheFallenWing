@echo off
setlocal
set "PROYECTO=C:\Users\Richard Rojas\artemisa"

echo ============================================
echo  Subir codigo a github.com/richardrojascid/artemisa
echo ============================================
echo.

if not "%~1"=="" set "PROYECTO=%~1"
cd /d "%PROYECTO%" 2>nul
if errorlevel 1 (
    echo Error: no existe la carpeta:
    echo   %PROYECTO%
    pause
    exit /b 1
)

if not exist ".git" (
    echo Error: esta carpeta no es un repositorio git.
    echo Clona primero:
    echo   git clone https://github.com/richardrojascid/artemisa.git "%PROYECTO%"
    pause
    exit /b 1
)

git remote get-url export >nul 2>&1
if errorlevel 1 (
    echo Agregando remoto export ^(JudgmentOfTheFallenWing^)...
    git remote add export https://github.com/richardrojascid/JudgmentOfTheFallenWing.git
)

echo.
echo 1/3 Descargando ultimos cambios del agente ^(export/artemisa^)...
git fetch export artemisa
if errorlevel 1 (
    echo Error al hacer fetch de export/artemisa
    pause
    exit /b 1
)

echo.
echo 2/3 Actualizando rama main local...
git checkout -B main export/artemisa
if errorlevel 1 (
    echo Error al actualizar main
    pause
    exit /b 1
)

echo.
echo 3/3 Subiendo a https://github.com/richardrojascid/artemisa ...
git push -u origin main
if errorlevel 1 (
    echo.
    echo Error al hacer push. Inicia sesion en GitHub o usa un token personal.
    pause
    exit /b 1
)

echo.
echo Opcional: actualizar tambien la rama develop...
set /p SYNC_DEVELOP="¿Actualizar develop con el mismo codigo? (S/N): "
if /I "%SYNC_DEVELOP%"=="S" (
    git push origin main:develop
)

echo.
echo Listo. Verifica en:
echo   https://github.com/richardrojascid/artemisa/tree/main
echo.
echo Si GitHub muestra develop vacio, cambia la rama por defecto a main:
echo   Settings - General - Default branch - main
echo ============================================
pause
