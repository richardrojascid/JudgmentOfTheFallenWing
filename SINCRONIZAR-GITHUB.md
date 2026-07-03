# Cómo sincronizar los cambios con GitHub

Hay **dos repositorios** involucrados. Eso suele causar confusión:

| Repositorio | Rama | Quién sube aquí |
|-------------|------|-----------------|
| [JudgmentOfTheFallenWing](https://github.com/richardrojascid/JudgmentOfTheFallenWing) | `artemisa` | **Cursor / agente** (automático) |
| [artemisa](https://github.com/richardrojascid/artemisa) | `main` | **Tú** (manual, con el script) |

El agente **no puede** escribir directamente en `richardrojascid/artemisa`. Por eso debes **importar** los cambios.

---

## Ver el código actualizado en GitHub

Abre esta URL (rama **main**, no develop):

**https://github.com/richardrojascid/artemisa/tree/main**

La rama `develop` puede estar vacía o desactualizada. El código completo está en **`main`**.

### Cambiar la rama por defecto en GitHub

1. Ve a https://github.com/richardrojascid/artemisa/settings  
2. **General** → **Default branch**  
3. Elige **`main`** → **Update**

Así al entrar al repo verás el código correcto.

---

## En tu PC — actualizar local (para probar)

Desde la carpeta `C:\Users\Richard Rojas\artemisa`:

```cmd
scripts\actualizar.bat
```

Eso trae cambios de `JudgmentOfTheFallenWing` rama `artemisa` a tu carpeta local.

---

## En tu PC — subir a github.com/richardrojascid/artemisa

Después de que el agente haya hecho cambios, ejecuta:

```cmd
scripts\importar-a-artemisa.bat
```

O el atajo:

```cmd
scripts\push-to-artemisa.bat
```

Eso hace:

1. `git fetch export artemisa` — descarga del repo del agente  
2. `git checkout -B main export/artemisa` — actualiza tu main local  
3. `git push origin main` — sube a **richardrojascid/artemisa**

---

## Primera vez — configurar remotos

Si clonaste solo `artemisa`:

```cmd
cd C:\Users\Richard Rojas\artemisa
git remote add export https://github.com/richardrojascid/JudgmentOfTheFallenWing.git
```

Verifica:

```cmd
git remote -v
```

Debe mostrar:

```
origin   https://github.com/richardrojascid/artemisa.git
export   https://github.com/richardrojascid/JudgmentOfTheFallenWing.git
```

---

## Resumen rápido

```
Cursor Agent  →  push  →  JudgmentOfTheFallenWing/artemisa
                              ↓
                    scripts\importar-a-artemisa.bat
                              ↓
                    richardrojascid/artemisa (main)
```

---

## Verificar que llegó todo

En https://github.com/richardrojascid/artemisa debes ver:

- Carpetas: `admin`, `api`, `assets`, `includes`
- Archivos: `index.php`, `login.php`, `install.php`
- Último commit con mensajes como `fix: búsqueda instantánea...`

Si solo ves un `README.md`, estás en la rama **`develop`** — cambia a **`main`**.
