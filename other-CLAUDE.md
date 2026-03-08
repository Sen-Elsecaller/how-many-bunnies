# Little Big Potions - C# Port

## Reglas de Interaccion
- Be concise. Sacrifice grammar for concision
- If unsure about GD/C# implementation, search docs online before answering
- End of each plan: list unresolved questions (extremely concise)
- NO editar archivos .tscn - usuario los maneja en editor Godot
- NO ejecutar proyecto - usuario envia errores de consola
- Enfoque didactico al escribir C# para familiarizar al usuario

## Descripcion del Proyecto
Juego top-down sobre craftear pociones en tiempo real usando un virtual joystick o radial menu para defenderse de enemigos. Mecanica de supervivencia, recoleccion de materiales y crafteo de pociones.

## Progreso Actual
- ✅ Utils.cs (singleton global)
- ✅ Database.cs (singleton - texturas, particles, ElementData + modificadores runtime)
- ✅ Attack.cs (resource de ataque)
- ✅ HealthComponent.cs (sistema de vida)
- ✅ HitboxComponent.cs (detección de daño - recibe daño)
- ✅ HurtboxComponent.cs (inflige daño)
- ✅ LifeUi.cs (UI de corazones)
- ✅ Sistema de enemigos completo (Enemy, FSM, 6 estados)
- ✅ VirtualJoystick.cs (joystick base - diagonales + texturas opcionales)
- ✅ VJMagic.cs (joystick crafteo - segmentos + iconos)
- ✅ VJAim.cs (joystick apuntado - auto-aim + direcciones aim_*)
- ✅ VJContainer.cs (coordinador - Mediator, pasa PotionData a ShootComponent)
- ✅ PotionComponent.cs (sistema de crafteo, desacoplado de ShootComponent)
- ✅ PlayerMoveComponent.cs (movimiento del jugador)
- ✅ IngredientsContainer.cs (UI crafteo en progreso)
- ✅ PotionContainer.cs (UI poción creada)
- ✅ ShootComponent.cs (disparo de pociones, auto-aim, manual aim)
- ✅ BasePotion.cs (proyectil de poción)
- ✅ SpawnWaveData.cs (Resource - configuración de olas de enemigos)
- ✅ EnemySpawner.cs (spawner con olas configurables)
- ✅ EnemyShootState.cs, EnemyRepositionState.cs (estados para enemigos ranged)
- ✅ EnemyProjectile.cs (proyectil base para enemigos)
- ✅ Sistema de efectos de pociones (Strategy Pattern):
  - IPotionBehavior, PotionEffect, PotionEffectArea
  - AreaStaticBehavior (Fuego, Agua, Hielo)
  - AreaMovingBehavior (Vapor) - stub
  - RaycastBehavior (Rayo) - chain lightning implementado

## Estructura del Proyecto
- `scripts/player/` - Logica del jugador y state machine
  - `states/` - Estados: PlayerIdleState, PlayerWalkState, PlayerDashState, PlayerAttackState, PlayerHurtState, PlayerDeathState, PlayerThrowPotionState
  - `State.cs` - Clase base PlayerState
  - `Player.cs` - Nodo principal del jugador (PotionComponent, dash cooldown centralizado)
  - `FiniteStateMachine.cs` - Maquina de estados
  - `PlayerMoveComponent.cs` - Componente de movimiento (MoveSpeed, LastMoveDir, input)
- `scripts/enemies/` - Sistema de enemigos
  - `Enemy.cs` - Clase base genérica para todos los enemigos
  - `EnemyStateMachine.cs` - FSM con tracking de rangos (detection/attack)
  - `states/` - Estados base: EnemyState, EnemyIdleState, EnemyChaseState, EnemyAttackWindupState, EnemyAttackJumpState, EnemyHurtState, EnemyDeathState
- `scripts/ui/` - Componentes de interfaz
  - `LifeUi.cs` - Sistema de corazones dinámico
  - `VirtualJoystick.cs` - Joystick táctil base (diagonales, texturas opcionales, ResetAndHide)
  - `VJMagic.cs` - Joystick para crafteo (8 segmentos, iconos)
  - `VJAim.cs` - Joystick para apuntado (auto-aim + aim_up/down/left/right)
  - `VJContainer.cs` - Coordinador de joysticks (Mediator: visibilidad + pasa PotionData)
  - `IngredientsContainer.cs` - Muestra ingredientes durante crafteo
  - `PotionContainer.cs` - Muestra poción creada
- `scripts/projectiles/` - Proyectiles
  - `BasePotion.cs` - Proyectil de poción (implementa IPotionProjectile)
  - `EnemyProjectile.cs` - Proyectil base para enemigos (usa HurtboxComponent)
- `scripts/effects/` - Efectos de pociones (Strategy Pattern)
  - `IPotionBehavior.cs` - Interface para behaviors
  - `PotionEffect.cs` - Coordinador (implementa IPotionProjectile)
  - `PotionEffectArea.cs` - Área de daño (hereda HurtboxComponent)
  - `PotionBehaviorFactory.cs` - Factory para crear behaviors
  - `behaviors/` - Implementaciones de IPotionBehavior
    - `AreaStaticBehavior.cs` - Fuego, Agua, Hielo
    - `AreaMovingBehavior.cs` - Vapor (stub)
    - `RaycastBehavior.cs` - Rayo (stub)
- `scripts/spawners/` - Spawners
  - `EnemySpawner.cs` - Spawner de enemigos con olas configurables
- `util/components/` - Componentes reutilizables
  - `Attack.cs` - Resource con datos de ataque (Damage, Knockback, Attacker)
  - `HealthComponent.cs` - Sistema de vida con señales e invulnerabilidad (desacoplado)
  - `HitboxComponent.cs` - Recibe daño (Area2D que detecta colisiones)
  - `HurtboxComponent.cs` - Inflige daño (Area2D, emite HitDealt para feedback)
  - `PotionComponent.cs` - Sistema de crafteo (reacciones via ReactionData)
  - `ShootComponent.cs` - Lanzar pociones (usa IPotionProjectile)
  - `IPotionProjectile.cs` - Interfaz para proyectiles type-safe
- `util/autoload/` - Singletons
  - `Utils.cs` - Singleton global (señales, referencias, utilidades)
  - `Database.cs` - Singleton con texturas de elementos/items y particle configs
- `resources/scripts/` - Resources custom
  - `PotionData.cs` - Datos de poción (elementos + items, consulta Database para stats)
  - `ReactionData.cs` - Define reacciones entre elementos (patrón Type Object)
  - `ParticlesData.cs` - Config de partículas por elemento (material, amount, lifetime, DirectionMode)
  - `SpawnWaveData.cs` - Config de ola de spawn (patrón Type Object)
  - `ElementData.cs` - Stats base por elemento (patrón Type Object, configurable via .tres)
- `scenes/` - Escenas de Godot (.tscn)
- `resources/` - Recursos (sprites, animaciones, etc.)

## Proyecto Original (GDScript)
Ubicado en: `../little-big-potions/`
Usar como referencia para la conversion a C#.

## Arquitectura Actual

### State Machine del Player
- `PlayerState` (State.cs): Clase base con Enter(), Exit(), Update(delta)
- Estados emiten signal `StateTransition` para cambiar de estado
- FSM usa `_PhysicsProcess` para llamar Update() del estado actual
- FSM inicializa estados con: Parent, Animations, MoveComponent, StateMachine
- `TransitionTo()` centraliza logica de cambio de estado
- **Mejora futura**: Mover `Input.IsActionJustPressed()` de estados a métodos protegidos en PlayerState (Subclass Sandbox). Permitiría reutilizar estados para IA/replay.

### Componentes
- `PlayerMoveComponent`: Maneja input de movimiento y direccion via `Input.GetVector()`
- `HealthComponent`: Vida y daño con eventos de señal
- `HitboxComponent`: Recibe daño - emite señal Damaged(Attack)
- `HurtboxComponent`: Inflige daño - detecta Hitboxes y les aplica Attack
- `PotionComponent`: Sistema de crafteo (escucha Input Actions de VJMagic)
- `ShootComponent`: Lanzar pociones
- `Utils`: Singleton global (señales como VjMagicReleased, referencias)
- `Database`: Singleton con diccionarios de texturas y particle configs

### State Machine de Enemigos
- `EnemyState`: Clase base con Enter(), Exit(), Update(delta), referencia a Player
- `EnemyStateMachine`: FSM con tracking de rangos via Areas2D, usa `_PhysicsProcess`
  - `PlayerInDetectionRange`: jugador detectado
  - `PlayerInAttackRange`: jugador en rango de ataque
  - `SharedData`: diccionario para pasar datos entre estados (ej: last_attack)
- Estados: Idle → Chase → AttackWindup → AttackJump (loop o volver a Idle)
- `ForceChangeState()`: para interrupciones (Hurt, Death)
- `Enemy.cs`: clase base genérica, enemigos específicos heredan de ella
- **Mejora futura**: Unificar FiniteStateMachine y EnemyStateMachine en clase base genérica (código duplicado)

## Patrones de Programacion y Buenas Practicas

### Estructura de Metodos - Evitar Errores al Debuggear
Al agregar/quitar código de debug (prints, logs), estructurar métodos para que la lógica funcional no se vea afectada:

```csharp
// MAL - Debug intercalado con lógica (propenso a errores al eliminar)
private void Move(Vector2 inputDir)
{
    GD.Print($"Debug: {inputDir}");  // Si eliminas esto...
    Parent.Velocity = inputDir * Speed;  // ...puedes borrar esto por accidente

    if (inputDir.Length() > 0)
    // ...
}

// BIEN - Lógica funcional separada, debug al final o en bloque separado
private void Move(Vector2 inputDir)
{
    // === LÓGICA FUNCIONAL ===
    Parent.Velocity = inputDir * Speed;

    if (inputDir.Length() > 0)
    {
        // lógica de animación...
    }

    Parent.MoveAndSlide();

    // === DEBUG (eliminar en producción) ===
    // GD.Print($"Debug: {inputDir}");
}
```

### Principios Clave
1. **Lógica funcional primero**: Asignaciones críticas (Velocity, posiciones) van al inicio del método
2. **Debug separado**: Prints/logs al final o en bloque claramente marcado
3. **Una responsabilidad**: Cada método hace una cosa bien
4. **Líneas críticas nunca adyacentes a debug**: Dejar línea vacía entre lógica importante y debug

### Centralizar Recursos (Patron Singleton)
Usar singletons para recursos compartidos en lugar de duplicar exports:

```csharp
// MAL - Cada UI tiene sus propios exports de texturas
public partial class IngredientsContainer : HBoxContainer
{
    [Export] public Texture2D FireTexture;
    [Export] public Texture2D WaterTexture;
    // ... repetido en cada container
}

// BIEN - Database centraliza, containers solo consultan
public partial class Database : Node  // Autoload
{
    public Dictionary<int, Texture2D> ElementTextures { get; private set; }
    public Texture2D GetTextureFor(int element) => ElementTextures.GetValueOrDefault(element);
}

public partial class IngredientsContainer : HBoxContainer
{
    private Database _database;
    // Solo usa: _database.GetTextureFor(element)
}
```

### Exponer Componentes Correctamente
Para acceso entre sistemas, usar propiedades públicas con setter privado:

```csharp
// En Player.cs
public PotionComponent PotionComponent { get; private set; }

// En _Ready()
PotionComponent = GetNodeOrNull<PotionComponent>("PotionComponent");

// Otros sistemas acceden via: Utils.player.PotionComponent
```

## Comandos Utiles
```bash
# Build del proyecto (desde la carpeta del proyecto)
dotnet build
```

## Convenciones de Codigo C# en Godot
- Usar PascalCase para propiedades publicas y metodos
- Usar _camelCase para campos privados
- Signals se definen como delegates con sufijo EventHandler
- `[Export]` para exponer propiedades al editor
- `[GlobalClass]` para clases que se agregan como nodos desde el editor (componentes reutilizables)
- `partial class` requerido para clases que heredan de Node

### Cuando usar [GlobalClass]
Agregar a scripts cuyos nodos se agregan directamente desde el editor de Godot:
- Componentes reutilizables: HealthComponent, HitboxComponent, HurtboxComponent
- UI containers: IngredientsContainer, PotionContainer
- Clases base de entidades: Enemy, EnemyStateMachine
- NO necesario para: estados internos, scripts que solo se asignan via código

## Notas de Conversion GDScript -> C#
- `@onready var` -> campo privado + GetNode en _Ready()
- `@export var` -> `[Export] public Tipo Nombre { get; set; }`
- `signal nombre` -> `[Signal] public delegate void NombreEventHandler(...)`
- `emit_signal("nombre")` -> `EmitSignal(SignalName.Nombre, ...)`
- `class_name Nombre` -> `[GlobalClass] public partial class Nombre`
- snake_case -> PascalCase para publicos, _camelCase para privados
- `to_local(pos)` -> `GetGlobalTransform().AffineInverse() * pos` (no existe ToLocal en C#)
- `preload()` -> `GD.Load<T>()` o `ResourceLoader.Load<T>()`

## Errores Comunes C#/Godot

### Siempre usar llaves {} en bloques if/else
En C#, sin llaves solo la primera línea pertenece al bloque:

```csharp
// MAL - Bug silencioso
if (condition)
    GD.Print("debug");      // Solo esto está en el if
    DoSomething();          // SIEMPRE se ejecuta (fuera del if)

// BIEN - Siempre usar llaves
if (condition)
{
    GD.Print("debug");
    DoSomething();
}
```

### Ambigüedad Dictionary (CS0104)
Usar `using Godot.Collections;` junto con `using System.Collections.Generic;` causa error CS0104 porque ambos tienen `Dictionary<,>`.

**Solución:** Usar aliases para los tipos de System.Collections.Generic:
```csharp
using Godot;
using Godot.Collections;  // Array<T>, Dictionary<K,V> de Godot
using SysDict = System.Collections.Generic.Dictionary<string, float>;
using SysDictBool = System.Collections.Generic.Dictionary<string, bool>;

// Uso
private SysDict _timers = new();           // System.Collections.Generic
private Array<int> _elements = new();      // Godot.Collections
```

**Cuándo usar cada uno:**
- `Godot.Collections.Array<T>` / `Dictionary<K,V>`: Para datos que se exportan al editor o pasan por signals
- `System.Collections.Generic`: Para datos internos que no interactúan con Godot

## Buenas Practicas de Signals
- Conectar signals en `_Ready()` con `+=`
- Desconectar signals en `_ExitTree()` con `-=` para prevenir memory leaks
- Usar atributo `[Signal]` para signals tipo-seguro
- Acceder signals via `SignalName.SignalName` para seguridad de tipos

## Virtual Joysticks

Sistema de joysticks táctiles con herencia y coordinador:
```
VirtualJoystick (base)
├── VJMagic (crafteo)
└── VJAim (apuntado)

VJContainer (coordinador - Mediator pattern)
```

### VirtualJoystick (Base)
Joystick para movimiento con soporte de diagonales. Activa Input Actions, la FSM traduce a Vector2.

**Características:**
- Modos: Fixed, Dynamic, Following
- Movimiento diagonal (evalúa X e Y independientemente)
- Texturas opcionales para base y tip (fallback a círculos)
- Señales: JoystickPressed, JoystickReleased
- `ResetAndHide()`: Resetea estado y oculta (para cambios de modo UI)

**Exports:**
- `ActionUp/Down/Left/Right`: Nombres de Input Actions (default: ui_*)
- `DeadzoneSize` / `ClampzoneSize`: Zonas de input
- `BaseTexture`, `TipTexture`: Texturas opcionales
- `BaseRadius`, `TipRadius`: Tamaños visuales (usados si no hay textura)
- `BaseColor`, `TipColor`, `TipPressedColor`: Colores/tints

**Uso (VJMove):**
1. Agregar Control, asignar VirtualJoystick.cs directamente (no necesita clase propia)
2. Configurar acciones en Project Settings (move_left, move_right, move_up, move_down)
3. FSM/PlayerMoveComponent lee `Input.GetVector()` normalmente

### VJMagic (Crafteo)
Extiende VirtualJoystick. Segmentos configurables para elementos/items de pociones.

**Características:**
- Segmentos totalmente configurables desde inspector (sin tocar código)
- Sistema de iconos por segmento con feedback visual
- Señales: SegmentChanged (además de las heredadas)
- PotionComponent escucha los Input Actions

**Exports adicionales:**
- `SegmentCount`: Número de divisiones (default: 8, rango 4-16)
- `SegmentRotationOffset`: Rotación en grados (-90 = segmento 0 arriba)
- `SegmentActions`: Array de nombres de acciones por segmento
- `SegmentIcons`: Array de Texture2D por segmento
- `IconDistanceFromCenter`, `IconSize`, `IconTint`, `IconActiveTint`

**Flujo de Crafteo:**
1. Usuario arrastra hacia segmento → activa Input Action
2. PotionComponent detecta hold (0.25s) → agrega elemento a crafting
3. Usuario suelta VJ → Utils.VjMagicReleased → PotionComponent crea poción

**Input Actions en Project Settings:**
- `magic_1`, `magic_2`, `magic_3`, `magic_4` (elementos: Fire, Air, Water, reservado)
- `object_1`, `object_2`, `object_3`, `object_4` (items: Point, Bounce, Area, reservado)

### VJAim (Apuntado)
Extiende VirtualJoystick. Joystick para apuntar con soporte de auto-aim.

**Características:**
- Auto-aim cuando el tip está en deadzone mientras se presiona
- Apuntado manual cuando sale de deadzone
- Solo procesa input si está visible
- Señales: AutoAimStarted, AutoAimStopped

**Lógica Auto-Aim:**
- Presionar dentro de deadzone → `AutoAimStarted`
- Mover fuera de deadzone → `AutoAimStopped`
- Soltar joystick → `AutoAimStopped`

### VJContainer (Coordinador)
MarginContainer que coordina visibilidad entre VJMagic y VJAim. Patrón Mediator.

**Responsabilidades:**
- Escucha `PotionCreated(PotionData)` → pasa datos a ShootComponent, oculta VJMagic, muestra VJAim
- Escucha `AmmoFinished` → oculta VJAim, muestra VJMagic
- Forwarda signals de auto-aim a ShootComponent

**Setup en escena:**
```
VJContainer (MarginContainer) → script: VJContainer.cs
├── %VJMove   → script: VirtualJoystick (base)
├── %VJMagic  → script: VJMagic
└── %VJAim    → script: VJAim (inicialmente oculto)
```
Nota: Usar unique names (%) para que VJContainer encuentre los nodos.

### Arquitectura Crafteo → Disparo (Desacoplada)
PotionComponent NO conoce a ShootComponent. VJContainer actúa como Mediator:

```
PotionComponent --[PotionCreated(PotionData)]--> VJContainer ---> ShootComponent
                                              |                   (SetCurrentPotionData + ResetAmmo)
                                              +--> VJMagic/VJAim (visibilidad)

PotionComponent --[PotionCreated(PotionData)]--> PotionContainer (UI)
```

**API pública de PotionComponent:**
- `CreatePotionFromCrafting()` - crea poción con crafting actual
- `ForceCreatePotion(elements, items)` - fuerza poción específica (power-ups, testing)

## UI Containers (Crafteo)

### IngredientsContainer
Muestra ingredientes mientras se craftea. HBoxContainer que escucha `CraftingChanged`.

### PotionContainer
Muestra la poción finalizada. HBoxContainer que escucha `PotionCreated`.

**Ambos usan:**
- `Database.GetTextureFor(element)` para obtener texturas
- Export `IconSize` para tamaño de iconos
- Se conectan via `Utils.UtilsReady` → `Utils.player.PotionComponent`

## Database (Singleton)

Centraliza recursos del juego. Configurar como Autoload.

**Paths:**
```
res://assets/elements/  → fire.png, air.png, water.png, ice.png, cloud.png, thunder.png
res://assets/items/     → point.png, bounce.png, area.png
res://resources/particles_data/ → fire.tres, water.tres, ice.tres, steam.tres, lightning.tres, air.tres
res://resources/element_data/   → fire.tres, water.tres, air.tres, ice.tres, steam.tres, lightning.tres
```

**API - Texturas y Partículas:**
- `ElementTextures`: Dictionary<int, Texture2D>
- `ItemTextures`: Dictionary<int, Texture2D>
- `ParticleConfigs`: Dictionary<int, ParticlesData>
- `GetTextureFor(int elementOrItem)`: Busca en ambos diccionarios
- `GetParticleConfig(int element)`: Obtiene config de partículas

**API - Element Data (Type Objects):**
- `ElementDataConfigs`: Dictionary<int, ElementData> - Stats base por elemento
- `GetElementData(int element)`: Obtiene ElementData (sin modificadores)
- `GetModifiedDamage(int element)`: Daño base × modificador
- `GetModifiedAreaRadius(int element)`: Radio base × modificador
- `GetModifiedTickRate(int element)`: TickRate base / modificador (inverso)
- `GetModifiedMoveSpeed(int element)`: Velocidad base × modificador
- `GetElementBehavior(int element)`: BehaviorType del elemento

**API - Modificadores Runtime (Upgrades):**
```csharp
// TODO: Migrar a PlayerStats cuando exista
ApplyDamageModifier(int element, float bonus)   // +bonus% daño
ApplyAreaModifier(int element, float bonus)     // +bonus% radio
ApplyTickRateModifier(int element, float bonus) // +bonus% velocidad ticks
ResetAllModifiers()                              // Nueva run
```

**Ejemplo de upgrade:**
```csharp
var db = GetNode<Database>("/root/Database");
db.ApplyDamageModifier((int)Element.Fire, 0.3f); // +30% daño fuego
```

## Enemy Spawner

Sistema de spawn de enemigos con olas configurables (patrón Type Object).

### SpawnWaveData (Resource)
Configuración de una ola. Solo datos, sin estado runtime.

**Exports:**
- `EnemyScene`: PackedScene del enemigo
- `EnemyCount`: Cuántos spawnear por intervalo
- `SpawnInterval`: Segundos entre spawns
- `TimeStart`: Segundo donde empieza la ola
- `TimeEnd`: Segundo donde termina (-1 = infinito)

**Crear olas:**
1. FileSystem → Click derecho → New Resource → SpawnWaveData
2. Configurar y guardar como `.tres` (ej: `wave_slimes.tres`)

### EnemySpawner (Node)
Maneja el spawn con estado runtime separado.

**Exports:**
- `Enabled`: Activa/desactiva spawner
- `Waves`: Array de SpawnWaveData
- `SpawnDistanceMultiplier`: Qué tan lejos del borde visible (1.2 = 20% fuera)
- `SpawnDistanceVariation`: Variación aleatoria adicional

**Características:**
- Considera zoom de cámara para calcular área visible
- Spawn en bordes (arriba, abajo, izquierda, derecha) aleatorios
- Estado runtime separado de configuración (timers por ola)

**API pública:**
- `Reset()` - reinicia tiempo y timers
- `ForceSpawnWave(index)` - fuerza spawn de una ola
- `ForceSpawnEnemy(scene)` - fuerza spawn de un enemigo
- `ElapsedTime` - tiempo transcurrido

## Patrones de Programacion (Referencia Rapida)

> **IMPORTANTE:** Cuando veas oportunidad de aplicar alguno de estos patrones, lee la seccion completa en `.claude/rules/game-patterns.md` y comentalo al usuario. Asi ambos aprenden los patrones mientras los aplicamos. En caso de no entender el patron con la seccion de game-patterns, busca el patron completo en https://gameprogrammingpatterns.com/contents.html

| Patron | Usa cuando... | En Godot |
|--------|---------------|----------|
| **Command** | Acciones como objetos (undo, remap) | Clases con `Execute()` |
| **Flyweight** | Miles de objetos similares | `Resource` compartido |
| **Observer** | Sistemas independientes comunican | `[Signal]` + `+=` |
| **Prototype** | Spawners genericos | `PackedScene.Instantiate()` |
| **Singleton** | Acceso global unico | Autoloads |
| **State** | Comportamiento por estado | FSM con clases Estado |
| **Double Buffer** | Lectura/escritura simultanea | Automatico en rendering |
| **Game Loop** | Ejecucion continua | `_Process` / `_PhysicsProcess` |
| **Update Method** | Entidades update cada frame | `_PhysicsProcess` por Node |
| **Bytecode** | Comportamiento data-driven | Resources + interpreter |
| **Subclass Sandbox** | Muchas subclases similares | Base provee operaciones |
| **Type Object** | Variantes sin subclases | Resource como "tipo" |
| **Component** | Entidad multi-dominio | Nodos hijos especializados |
| **Event Queue** | Desacoplamiento temporal | Queue + procesar en _Process |
| **Service Locator** | Servicios globales flexibles | Autoloads o static class |
| **Data Locality** | Cache misses son problema | Arrays contiguos |
| **Dirty Flag** | Evitar recalculos innecesarios | Bool + cache |
| **Object Pool** | Crear/destruir frecuente | Pool preasignado |
| **Spatial Partition** | Queries espaciales O(n²) | Grid, Quadtree, Area2D |

Referencia completa: https://gameprogrammingpatterns.com/contents.html

## Sistema de Efectos de Pociones (Strategy Pattern)

> **Documentación completa:** `.claude/rules/potion-effects-system.md`

Resumen rápido:
- `PotionEffect.cs` - Coordinador (Context del Strategy)
- `IPotionBehavior.cs` - Interface para behaviors
- `PotionEffectArea.cs` - Área de daño con tick system
- `PotionBehaviorFactory.cs` - Factory que crea behaviors
- `behaviors/` - Implementaciones: AreaStatic, AreaMoving, Raycast

Para agregar nuevo behavior: crear clase → agregar en Factory → (opcional) nuevo BehaviorType en PotionData.

### Chain Lightning (Implementado)

Sistema para que RaycastBehavior (Lightning) salte entre enemigos.

**Componentes:**
- `IPotionProjectile.InitialTarget` - Propiedad para pasar enemigo impactado
- `BasePotion` guarda target al colisionar con HitboxComponent
- `ElementData` tiene campos: ChainRadius, MaxChainTargets, ChainDamageDecay
- `RaycastBehavior` - Lógica completa de chain lightning

**Flujo:**
```
ExecuteChainLightning()
    ├─→ Buscar HitboxComponent en InitialTarget
    │   └─→ Si es null (pared) → solo visual, sin daño
    ├─→ Aplicar daño al inicial
    ├─→ Loop: buscar más cercano en radio → aplicar daño con decay
    └─→ CreateVisualization() → Line2D entre puntos
```

**Métodos helper en RaycastBehavior:**
- `FindHitboxInTarget()` - Busca "HitboxComponent" como hijo directo
- `FindClosestHitboxInRadius()` - Query de física (Physics2D IntersectShape)
- `ApplyDamage()` - Crea Attack y llama hitbox.TakeDamage()
- `GetLightningElementData()` - Obtiene config de chain desde Database

---

## Pendiente para el Usuario

### Chain Lightning - Verificar en Editor

1. **Collision Layers:** La query de física (`IntersectShape`) detecta áreas según sus collision layers. Verificar que los `HitboxComponent` de enemigos estén en un layer que permita ser detectado (normalmente layer 1 o el que uses para "hurtboxes/hitboxes").

2. **lightning.tres:** El archivo usa valores por defecto. Para personalizarlos, abrir `resources/element_data/lightning.tres` y configurar:
   - `ChainRadius` (default: 150) - Radio para buscar enemigos
   - `MaxChainTargets` (default: 2) - Enemigos adicionales al inicial
   - `ChainDamageDecay` (default: 1.0) - Multiplicador por salto (1.0 = sin reducción)

3. **Probar:** Lanzar poción de Lightning a grupo de enemigos y verificar:
   - Daño al enemigo inicial
   - Chain a enemigos cercanos (máximo 2 por defecto)
   - Visualización Line2D entre puntos de impacto