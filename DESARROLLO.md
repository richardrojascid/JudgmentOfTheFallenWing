# Desarrollo — repositorio y carpeta oficial

## Única fuente de verdad

| Qué | Dónde |
|-----|--------|
| **Repositorio GitHub** | https://github.com/richardrojascid/artemisa |
| **Rama principal** | `main` |
| **Carpeta local (PC Richard)** | `C:\Users\Richard Rojas\artemisa` |

Todo cambio nuevo (código, logos, carta, reportes, etc.) debe ir **solo** a ese repositorio y a esa carpeta.

## No usar para trabajo nuevo

- ~~`JudgmentOfTheFallenWing`~~ (repositorio antiguo)
- ~~rama `artemisa` en JudgmentOfTheFallenWing~~ (solo histórico; migración completada)
- ~~`C:\xampp\htdocs\artemisa`~~ como carpeta de edición (solo copia para Apache si la necesitas)

## Flujo diario en tu PC

```cmd
cd "C:\Users\Richard Rojas\artemisa"
git pull origin main
scripts\serve-local.bat
```

Abre: http://localhost:8080/install.php (primera vez) o http://localhost:8080/login.php

## Subir cambios a GitHub

```cmd
cd "C:\Users\Richard Rojas\artemisa"
git add .
git commit -m "Descripción del cambio"
git push origin main
```

## Si usas XAMPP Apache (opcional)

Después de `git pull`, copia a htdocs:

```cmd
xcopy "C:\Users\Richard Rojas\artemisa\*" "C:\xampp\htdocs\artemisa\" /E /Y /I
```

Luego: http://localhost/artemisa/login.php

## Para Cursor / agentes en la nube

- Destino de PRs y commits: **github.com/richardrojascid/artemisa** (`main`)
- Ruta local de referencia del usuario: **`C:\Users\Richard Rojas\artemisa`**
- No abrir PRs ni seguir trabajando en `JudgmentOfTheFallenWing`
