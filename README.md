# Judgment of the Fallen Wing

**Judgment of the Fallen Wing** es un Metroidvania 2D en Unity. La protagonista es **ALIA**, portadora del Ala Caída.

## Requisitos

- Unity 2022.3 LTS o superior (2D URP recomendado)
- Input Manager clásico (`Horizontal`, `Jump`) o migración futura al Input System

## Estructura del proyecto

```
Assets/_Project/
├── Scripts/
│   ├── Core/          # GameManager, SaveSystem, eventos globales
│   ├── Player/        # ALIA: movimiento, combate, salud, respawn
│   ├── Abilities/     # Sistema de habilidades (gating Metroidvania)
│   ├── Combat/        # Daño, hitboxes
│   ├── Enemies/       # IA base de enemigos
│   ├── World/         # Checkpoints, puertas, salas, pickups
│   ├── Camera/        # Cámara 2D con límites por sala
│   ├── UI/            # HUD y menú de pausa
│   └── Data/          # Constantes compartidas
├── Art/
├── Audio/
├── Prefabs/
├── Scenes/
└── ScriptableObjects/
```

## Configuración rápida de ALIA

1. Crear GameObject `ALIA` con tag **Player** y layer **Player**.
2. Añadir componentes:
   - `Rigidbody2D` (Dynamic, Freeze Rotation Z, Gravity Scale 3)
   - `CapsuleCollider2D`
   - `SpriteRenderer` + `Animator`
   - `AliaController`, `AliaHealth`, `AliaCombat`, `AliaAbilityController`, `AliaRespawn`
3. Crear hijo `GroundCheck` (posición bajo los pies) y asignarlo en `AliaController`.
4. Crear hijo `AttackPoint` con collider trigger + `Hitbox` para el ataque.
5. Asignar layer **Ground** a plataformas y suelo.

### Controles por defecto

| Acción   | Tecla              |
|----------|--------------------|
| Mover    | A/D o flechas      |
| Saltar   | Espacio            |
| Dash     | Left Shift         |
| Atacar   | J                  |
| Pausa    | Escape             |

## Sistemas implementados

### Movimiento Metroidvania (`AliaController`)
- Aceleración/desaceleración suave
- Coyote time y jump buffer
- Wall slide / wall jump (requiere habilidad)
- Double jump (requiere habilidad)
- Dash (requiere habilidad)

### Progresión (`AliaAbilityController` + `AbilityPickup` + `AbilityGate`)
Las habilidades se desbloquean con pickups en el mundo y abren puertas/pasajes bloqueados.

Habilidades definidas: `DoubleJump`, `Dash`, `WallJump`, `Glide`, `GroundPound`, `WingSlash`.

### Guardado (`SaveSystem`)
JSON en `Application.persistentDataPath`. Persiste:
- Checkpoint activo
- Habilidades desbloqueadas
- Puertas abiertas
- Items recolectados
- Salas visitadas
- Vida y moneda

### Eventos (`GameEvents`)
Bus de eventos estático para desacoplar UI, audio y gameplay.

## Escenas recomendadas

| Escena      | Propósito                    |
|-------------|------------------------------|
| MainMenu    | Menú principal               |
| GameWorld   | Mundo interconectado         |
| (por zona)  | Una escena por bioma/zona    |

## Próximos pasos de desarrollo

1. **Sincronizar** tu proyecto local con este repositorio (`git pull` + merge de scripts).
2. **Animaciones** de ALIA con parámetros: `Speed`, `VelocityY`, `IsGrounded`, `IsWallSliding`, `IsDashing`, `Attack`.
3. **Tilemaps** + Composite Collider 2D para nivel.
4. **Minimapa** basado en `RoomTrigger` + `SaveData.visitedRooms`.
5. **Boss fights** extendiendo `EnemyBase`.
6. **AudioManager** reaccionando a `GameEvents`.
7. **Input System** para soporte de mando.

## Sincronizar tu proyecto local

Si ya tienes código en `C:\Users\Richard Rojas\Documents\JudgmentOfTheFallenWing\`:

```bash
cd "C:\Users\Richard Rojas\Documents\JudgmentOfTheFallenWing"
git remote add origin https://github.com/richardrojascid/JudgmentOfTheFallenWing.git
git fetch origin
git checkout develop
git merge origin/cursor/aria-metroidvania-foundation-c5c3
```

O copia la carpeta `Assets/_Project/Scripts/` a tu proyecto Unity existente.

## Licencia

Proyecto privado — Richard Rojas.
