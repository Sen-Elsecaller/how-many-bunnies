# Proyecto Godot C# - How Many Bunnies

## Reglas de Interaccion
- Be concise. Sacrifice grammar for concision
- If unsure about GD/C# implementation, search docs online before answering
- End of each plan: list unresolved questions (extremely concise)
- NO editar archivos .tscn - usuario los maneja en editor Godot
- NO ejecutar proyecto - usuario envia errores de consola
- Enfoque didactico al escribir C# para familiarizar al usuario
- Actualizar documentacion (CLAUDE.md) solo cuando mecanicas esten finalizadas, no en primera escritura

## Documentos del Proyecto
- `game-design-document.md` - Diseño del juego, mecanicas, decisiones
- `game-patterns.md` - Patrones de programacion para juegos (referencia)
- `godot-csharp-guide.md` - Convenciones C#, errores comunes, patrones
- `shaders-skill.md` - Sistema de shaders y efectos visuales

> **IMPORTANTE:** Al inicio de cada sesión, consultar `godot-csharp-guide.md`. Actualizarlo cuando se aprenda algo nuevo.

## Estructura del Proyecto
```
how-many-bunnies/
├── assets/
│   └── references/          # Imagenes de referencia
├── scripts/                 # Codigo C#
│   ├── actors/bunny/        # Bunny + estados FSM
│   ├── actors/carrot/       # CarrotField, Crop, Edible
│   └── ui/                  # UIManager, GameUI, UpgradesMenu
├── resources/
│   └── shaders/             # Shadows.gdshader, LightRays.gdshader
├── scenes/                  # Escenas (.tscn)
├── util/                    # Utilidades y componentes reutilizables
├── CLAUDE.md                # Este archivo (tecnico)
├── game-design-document.md  # Diseño del juego
├── game-patterns.md         # Patrones de programacion
└── shaders-skill.md         # Documentacion de shaders
```

---

## Comandos Utiles
```bash
# Build del proyecto (desde la carpeta del proyecto)
dotnet build
```

---

## Patrones de Programacion para Juegos (Referencia Rapida)

> **IMPORTANTE:** Cuando veas oportunidad de aplicar alguno de estos patrones, lee la seccion completa en `game-patterns.md` y comentalo al usuario. Asi ambos aprenden los patrones mientras los aplicamos. En caso de no entender el patron con la seccion de game-patterns, busca el patron completo en https://gameprogrammingpatterns.com/contents.html

| Patron | Usa cuando... | En Godot | Lineas en game-patterns.md |
|--------|---------------|----------|---------------------------|
| **Command** | Acciones como objetos (undo, remap) | Clases con `Execute()` | 7-50 |
| **Flyweight** | Miles de objetos similares | `Resource` compartido | 53-91 |
| **Observer** | Sistemas independientes comunican | `[Signal]` + `+=` | 94-141 |
| **Prototype** | Spawners genericos | `PackedScene.Instantiate()` | 144-178 |
| **Singleton** | Acceso global unico | Autoloads | 181-217 |
| **State** | Comportamiento por estado | FSM con clases Estado | 220-273 |
| **Double Buffer** | Lectura/escritura simultanea | Automatico en rendering | 276-322 |
| **Game Loop** | Ejecucion continua | `_Process` / `_PhysicsProcess` | 325-365 |
| **Update Method** | Entidades update cada frame | `_PhysicsProcess` por Node | 368-406 |
| **Bytecode** | Comportamiento data-driven | Resources + interpreter | 427-471 |
| **Subclass Sandbox** | Muchas subclases similares | Base provee operaciones | 474-514 |
| **Type Object** | Variantes sin subclases | Resource como "tipo" | 517-562 |
| **Component** | Entidad multi-dominio | Nodos hijos especializados | 567-612 |
| **Event Queue** | Desacoplamiento temporal | Queue + procesar en _Process | 615-661 |
| **Service Locator** | Servicios globales flexibles | Autoloads o static class | 664-706 |
| **Data Locality** | Cache misses son problema | Arrays contiguos | 711-745 |
| **Dirty Flag** | Evitar recalculos innecesarios | Bool + cache | 748-798 |
| **Object Pool** | Crear/destruir frecuente | Pool preasignado | 801-863 |
| **Spatial Partition** | Queries espaciales O(n²) | Grid, Quadtree, Area2D | 866-922 |

Referencia completa: https://gameprogrammingpatterns.com/contents.html

---

## Progreso Actual
- [x] Estructura de carpetas creada
- [x] Documentacion base (CLAUDE.md, game-design-document.md)
- [x] FSM conejo completa (8 estados)
- [x] Sistema de zanahorias (CarrotField, CarrotCrop, CarrotEdible)
- [x] Física top-down para CarrotEdible (fricción, sin gravedad)
- [x] Sistema de personalidad (IsAlert 30%)
- [x] Detección de zanahorias via Area2D
- [x] Movimiento centralizado en Bunny.Move() con speedMultiplier
- [x] Ciclo día/noche (GameManager con fases Day/Sunset/Night)
- [x] Autoloads con static Instance (Utils, GameManager, ShaderManager, BunniesManager)
- [x] Sistema de reproducción (adultos generan crías)
- [x] BunnyBounds con Polygon2D (limita área de movimiento)
- [x] Sistema de UI (UIManager, GameUI, UpgradesMenu)
- [x] ShaderManager (CanvasModulate para transiciones día/noche)
- [x] BunniesManager (Facade: población, modifiers, spawn)
- [x] Sistema de Upgrades (UpgradeTree, UpgradeNode, UpgradeData)
- [x] TooltipManager (singleton + panel custom con BBCode)
- [x] AudioManager (SFX + BGM, auto-carga desde carpetas, delay random estilo Minecraft)
- [x] Cursores personalizados (Utils: SetDefaultCursor, SetPointingCursor, SetGrabbingCursor)
- [x] MainMenu con StartGame/ReturnToMenu en GameManager
- [x] AnimatedButton con [Export] ClickSound
- [x] Efectos de upgrades conectados (BunniesManager escucha UpgradePurchased, aplica a velocidad/twins/babies)
- [x] Eagle (aparece en sunset, roba bunny random, tween-based swoop)

## Collision Layers (sugerido)
```
Layer 1: Bunnies
Layer 2: Carrots (CarrotEdible)
```
- CarrotDetection (Area2D del bunny): mask = Layer 2
- CarrotEdible Area2D: layer = Layer 2

## Archivos Creados
```
scripts/actors/bunny/
├── Bunny.cs
├── BunnyHouse.cs
├── BunnyStateMachine.cs
└── states/
    ├── BunnyState.cs
    ├── BunnyIdleState.cs
    ├── BunnyWanderingState.cs
    ├── BunnyWatchingState.cs
    ├── BunnyGoingToCarrotState.cs
    ├── BunnyEatingState.cs
    ├── BunnyGoingToSleepState.cs
    └── BunnySleepingState.cs

scripts/actors/carrot/
├── CarrotField.cs
├── CarrotCrop.cs
└── CarrotEdible.cs

scripts/actors/eagle/
└── Eagle.cs          # Roba conejos en sunset, tween-based

scripts/
├── BunnyBounds.cs    # Polygon2D que define área de movimiento
└── Main.cs           # Script escena principal, registra SkyLight

scripts/ui/
├── UIManager.cs      # CanvasLayer, maneja visibilidad, se registra en Utils
├── GameUI.cs         # HUD gameplay, timer, botón StartDay
├── UpgradesMenu.cs   # Menú nocturno, botón NewDay
├── AnimatedButton.cs # Botón con animaciones hover/press (GlobalClass)
└── TooltipPanel.cs   # Panel tooltip con BBCode, colores [Export]

scripts/upgrades/
├── UpgradeTree.cs    # Manager: puntos, compras, efectos acumulados
└── UpgradeNode.cs    # Visual: botón, estados, líneas de conexión

util/autoload/
├── Utils.cs          # Autoload: referencias a nodos (BunnyHouse, UIManager, etc.)
├── GameManager.cs    # Autoload: ciclo día/noche, fases, timer
├── ShaderManager.cs  # Autoload: transiciones visuales día/noche
├── BunniesManager.cs # Autoload: población, spawn, reproducción, modifiers
├── TooltipManager.cs # Autoload: muestra tooltips via TooltipPanel
└── AudioManager.cs   # Autoload: reproduce SFX con límites y pitch random

util/types/
├── UpgradeData.cs    # Resource: datos de cada upgrade
└── SoundEffect.cs    # Resource: config de efecto de sonido

resources/
├── UpgradeEffect.cs    # Enum: tipos de efecto (TwinChance, BabiesPerCouple, etc.)
├── SoundEffectType.cs  # Enum: tipos de SFX (ButtonClick, BunnyEat, etc.)
└── shaders/
    ├── Shadows.gdshader    # Sombras ray-marching (no implementado)
    └── LightRays.gdshader  # Rayos de luz procedurales (no implementado)
```

---

## Arquitectura Implementada

### FSM del Conejo
**Patrón:** State (game-patterns.md líneas 220-273)

**Estructura:**
```
Bunny (Node2D)
├── AnimatedSprite2D
├── CarrotDetection (Area2D)
└── BunnyStateMachine
    └── [Estados como nodos hijos]
```

**Estados:** Idle ↔ Wandering → Watching → GoingToCarrot → Eating → Idle
            ↘ GoingToSleep → Sleeping ↗

**Bunny.cs signals:**
- `BecameAdult(Bunny)` - cuando alcanza NutritionToAdult
- `Ate(Bunny, int currentNutrition)` - cada vez que come

**BunnyStateMachine:** Escucha `SunsetStarted`/`DayStarted`, maneja `TargetCarrot`

**Personalidad:** `IsAlert` (30%) reacciona inmediato, resto espera que zanahoria toque suelo.

**Performance:** Node2D en vez de CharacterBody2D (no colisiones, escala a cientos).

---

### Sistema de Zanahorias

**CarrotField:** Spawner con Marker2D como slots, respawn automático.

**CarrotCrop:** 3 etapas de crecimiento (GrowthTime: 3s), click en Stage3 genera CarrotEdible.

**CarrotEdible:**
- Estados: Held (sigue mouse) → Flying (física top-down) → Grounded
- Signals: `Grounded(CarrotEdible)`, `Consumed(CarrotEdible)`
- Exports: NutritionValue, ThrowForceMultiplier, Friction

---

### Ciclo Día/Noche

**Fases:** Day (30s) → Sunset (5s) → Night (menú) → Day...

**GameManager.cs signals:**
- `PhaseChanged(int phase)` - cada cambio de fase
- `SunsetStarted` - conejos van a dormir
- `NightStarted` - abre menú upgrades
- `DayStarted` - conejos salen de casa

**Utils.cs referencias:**
- `BunnyHouse`, `BunniesContainer`, `UIManager`
- `BunnyBounds` (Vector2[] polígono área válida)

**Autoloads (orden en Project Settings):**
1. Utils, 2. GameManager, 3. ShaderManager, 4. BunniesManager, 5. TooltipManager, 6. AudioManager

---

### BunniesManager (Facade)

**Propósito:** Centraliza toda lógica de población.

**Propiedades:** `BunnyCount`

**Signal:** `BunnyCountChanged(int count)`

**Métodos:**
- `RefreshCount()`, `SpawnBunny(Vector2)`, `RemoveBunny(Bunny)`
- `ProcessReproduction()` - genera crías según adultos

**Modificadores (propiedades cacheadas, actualizadas via `UpgradePurchased`):**
- `SpeedMultiplier`, `TwinChance`, `BonusBabiesPerCouple`
- Patrón: evita lookups repetidos por frame, refresca solo al comprar upgrade

---

### Sistema de Upgrades

**Patrón:** Type Object (UpgradeData Resource) + jerarquía visual

**UpgradeData (Resource):** Id, DisplayName, Description, Cost, Icon, EffectType, EffectValue

**UpgradeEffectType:** None, TwinChance, BabiesPerCouple, BunnySpeed

**EffectValue en editor (conversión en ApplyEffect):**
- TwinChance: 10 → +10% probabilidad
- BunnySpeed: 15 → +15% velocidad (1.15x)
- BabiesPerCouple: 1 → +1 bebé por pareja (sin conversión)

**UpgradeTree.cs:**
- Signals: `UpgradePurchased(UpgradeData)`, `PointsChanged(int available, int total)`
- Propiedades acumuladas: `TwinChance`, `BonusBabiesPerCouple`, `BunnySpeedMultiplier`
- Métodos: `GetTotalPoints()`, `GetAvailablePoints()`, `CanPurchase()`, `TryPurchase()`

**UpgradeNode.cs:**
- Prerequisitos por jerarquía: padre (GetParent as UpgradeNode) debe estar comprado
- Line2D conexión visual al padre
- Colores: gris=bloqueado, blanco=disponible, verde=comprado

**Estructura en editor:**
```
UpgradeTree (Control)
├── RootUpgrade (UpgradeNode)      ← Sin padre, siempre disponible
│   ├── Child1 (UpgradeNode)       ← Requiere RootUpgrade
│   └── Child2 (UpgradeNode)       ← Requiere RootUpgrade
```

---

### Sistema de UI

**Estructura:**
```
UIManager (CanvasLayer)
├── GameUI (%TimeLabel, %StartButton)
└── UpgradesMenu (%NewDayButton, %BunnyCounter, UpgradeTree)
```

**UIManager:** Solo visibilidad, escucha `NightStarted`/`DayStarted`

**GameUI:** Timer countdown, botón iniciar primer día

**UpgradesMenu:** Botón nuevo día, contador de conejos reactivo

---

### ShaderManager

**Propósito:** Transiciones visuales día/noche via CanvasModulate.

**Colores:** Day=#ffffff, Sunset=#ff9966, Night=#1a1a2e

**Método:** `RegisterSkyLight(CanvasModulate)` - llamado desde Main.cs

---

### TooltipManager

**Patrón:** Singleton (Autoload) + Panel en escena

**Arquitectura:**
```
TooltipManager (Autoload)     ← Persiste, recibe llamadas
TooltipPanel (en Main)        ← UI, se re-registra al cargar escena
```

**TooltipPanel [Export]:**
- Colores: TitleColor, DescColor, EffectColor, CostColor, PurchasedColor
- Offset, FadeDuration

**Uso:**
```csharp
TooltipManager.Instance.ShowUpgrade(data, isPurchased);
TooltipManager.Instance.Hide();
```

**Características:**
- BBCode para formato rico
- Fade in/out con Tween
- Sigue mouse con offset
- Clamp a viewport

---

### AudioManager

**Patrón:** Singleton + Type Object (SoundEffect Resource)

**SoundEffect (Resource):**
- Type (enum), Stream, VolumeDb, PitchScale, PitchRandomness
- MaxSimultaneous (límite de instancias concurrentes)

**SoundEffectType (Enum):** ButtonClick, CarrotPick, BunnyEat, etc.

**Uso:**
```csharp
AudioManager.Instance.CreateAudio(SoundEffectType.ButtonClick);
AudioManager.Instance.Create2DAudioAt(pos, SoundEffectType.BunnyEat);
```

**Características SFX:**
- Límite de sonidos simultáneos por tipo
- Pitch randomness automático
- Auto-cleanup al terminar
- AudioStreamPlayer (global) o AudioStreamPlayer2D (posicional)

**BGM (estilo Minecraft):**
- Auto-carga .wav desde OstFolder (res://assets/ost/)
- Delay aleatorio entre tracks (10-30s configurable)
- No repite track consecutivo
- Fade out con callback

**Uso BGM:**
```csharp
AudioManager.Instance.StartMusicLoop();  // Inicia ciclo
AudioManager.Instance.FadeOutMusic(() => { /* callback */ });
AudioManager.Instance.StopMusicLoop();   // Detiene todo
AudioManager.Instance.PlayMenuMusic();   // Track específico del menú (loop)
```

---

### Eagle

**Comportamiento:** Aparece durante sunset, roba un bunny random.

**Estructura:**
```
Eagle (Node2D)
├── Sprite2D
└── HitArea (Area2D)  # Para futura colisión con zanahorias
```

**Flujo:**
1. GameManager detecta `TimeRemaining <= EagleSpawnTime` durante Sunset
2. `SpawnEagle()` → `BunniesManager.GetRandomBunny()`
3. Eagle hace tween diagonal hacia bunny
4. `GrabTarget()` → reparenta bunny, sonido `EagleTakingBunny`
5. `FlyAway()` → tween salida
6. `Cleanup()` → `RemoveBunny()` + `QueueFree()`

**Exports:**
- `SwoopDuration`: 1.2s (entrada)
- `EscapeDuration`: 0.8s (salida)
- `EntryOffset`: (400, -300) desde dónde aparece

**GameManager Exports:**
- `EagleSpawnTime`: 3f (segundos restantes en sunset)
