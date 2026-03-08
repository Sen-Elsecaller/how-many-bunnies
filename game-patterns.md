# Patrones de Programacion para Juegos

> **Referencia completa:** Si necesitas detalles de implementacion o ejemplos mas extensos, consulta https://gameprogrammingpatterns.com/contents.html

---

## Command

**Problema:** Input hardcodeado, acciones no reutilizables, imposible hacer undo/redo.

**Solucion:** Encapsular acciones como objetos con metodo `Execute()`.

**Cuando usarlo:**
- Configuracion de controles remapeables
- Sistema de undo/redo
- IA que ejecuta las mismas acciones que el jugador
- Replay systems / networking

**En Godot/C#:**
```csharp
public interface ICommand
{
    void Execute(Node actor);
    void Undo();  // opcional
}

public class JumpCommand : ICommand
{
    private Vector2 _previousPosition;

    public void Execute(Node actor)
    {
        _previousPosition = ((CharacterBody2D)actor).Position;
        // logica de salto
    }

    public void Undo() { /* restaurar _previousPosition */ }
}

// InputHandler retorna comandos, no ejecuta directamente
public ICommand HandleInput()
{
    if (Input.IsActionJustPressed("jump")) return _jumpCommand;
    if (Input.IsActionJustPressed("attack")) return _attackCommand;
    return null;
}
```

**Aplicacion en proyecto:** VirtualJoystick podria retornar Commands en lugar de activar Input Actions directamente.

---

## Flyweight

**Problema:** Miles de objetos similares consumen demasiada memoria (arboles, balas, tiles).

**Solucion:** Separar estado intrinseco (compartido) del extrinseco (unico por instancia).

| Intrinseco (compartir) | Extrinseco (por instancia) |
|------------------------|----------------------------|
| Mesh, Textura, Sonidos | Posicion, Rotacion, Color  |
| Stats base, Animaciones | HP actual, Estado          |

**Cuando usarlo:**
- Cientos/miles de objetos similares
- Datos mayormente identicos entre instancias
- Memoria es cuello de botella

**En Godot/C#:**
```csharp
// Resource como Flyweight (intrinseco)
public partial class EnemyData : Resource
{
    [Export] public int BaseHealth;
    [Export] public float Speed;
    [Export] public Texture2D Sprite;
    [Export] public PackedScene DeathEffect;
}

// Instancia (extrinseco)
public partial class Enemy : CharacterBody2D
{
    [Export] public EnemyData Data;  // compartido entre todos del mismo tipo

    private int _currentHealth;      // unico por instancia
    private Vector2 _position;       // unico por instancia
}
```

**Aplicacion en proyecto:** `PotionData`, `Attack` ya son Resources compartidos (Flyweight). Enemigos del mismo tipo deberian compartir un `EnemyData` Resource.

---

## Observer

**Problema:** Sistemas independientes necesitan comunicarse sin acoplarse (UI, logros, audio).

**Solucion:** Subject mantiene lista de observers y notifica eventos; observers se suscriben voluntariamente.

**Cuando usarlo:**
- Comunicacion entre sistemas independientes (UI ↔ Gameplay)
- Multiples sistemas reaccionan al mismo evento
- Desacoplar emisor de receptores

**Cuando NO usarlo:**
- Dentro de un modulo cohesivo (dificulta debug)
- Cuando necesitas saber quien recibe la notificacion

**En Godot/C#:**
```csharp
// Godot Signals SON el patron Observer
[Signal]
public delegate void HealthChangedEventHandler(int current, int max);

// Subject (emite)
public partial class HealthComponent : Node
{
    public void TakeDamage(int amount)
    {
        _health -= amount;
        EmitSignal(SignalName.HealthChanged, _health, _maxHealth);
    }
}

// Observers (escuchan)
// En LifeUI._Ready():
healthComponent.HealthChanged += UpdateHearts;

// En AudioManager._Ready():
healthComponent.HealthChanged += PlayDamageSound;

// En AchievementSystem._Ready():
healthComponent.HealthChanged += CheckLowHealthAchievement;
```

**Aplicacion en proyecto:** Ya usado extensivamente:
- `PotionComponent.CraftingChanged` → `IngredientsContainer`
- `PotionComponent.PotionCreated` → `PotionContainer`
- `HealthComponent.TookDamage` → `Player` (cambio de estado)
- `Utils.UtilsReady` → Containers (setup diferido)

---

## Prototype

**Problema:** Necesitas spawners para diferentes tipos de objetos, terminando con jerarquias paralelas de clases.

**Solucion:** Objetos se clonan a si mismos via metodo `Clone()`.

**Cuando usarlo:**
- Spawners genericos que crean copias de un "template"
- Variantes de un mismo tipo (enemigo rapido vs lento)
- Data modeling con herencia (JSON con prototipos)

**Cuando NO usarlo:**
- Rara vez es la mejor opcion en motores modernos
- Preferir Component o Type Object para entidades complejas

**En Godot/C#:**
```csharp
// Godot ya tiene esto: PackedScene ES un prototype
[Export] public PackedScene EnemyPrototype;

// Spawner generico
public Node SpawnEnemy(Vector2 position)
{
    var instance = EnemyPrototype.Instantiate<Enemy>();
    instance.Position = position;
    AddChild(instance);
    return instance;
}

// Resource.Duplicate() para clonar resources
var clonedData = originalData.Duplicate() as EnemyData;
```

**Aplicacion en proyecto:** `PackedScene` para enemigos, pociones, efectos. No necesitas implementar Clone() manualmente.

---

## Singleton

**Problema:** Necesitas acceso global a una instancia unica (audio, save system).

**Solucion:** Clase que garantiza una sola instancia con punto de acceso global.

**Problemas que causa:**
- Estado global = codigo dificil de razonar
- Acoplamiento oculto entre sistemas
- Dificulta testing y concurrencia
- Inflexible si luego necesitas multiples instancias

**Alternativas preferibles:**
- Pasar dependencias como parametros (DI)
- Service Locator para acceso flexible
- Acceder via objeto Game/Root existente

**En Godot/C#:**
```csharp
// Godot Autoloads YA son singletons manejados por el engine
// En Project Settings > Autoload:
// Utils.cs -> /root/Utils
// Database.cs -> /root/Database

// Acceso desde cualquier script:
var utils = GetNode<Utils>("/root/Utils");

// O guardar referencia en _Ready():
private Utils _utils;
public override void _Ready()
{
    _utils = GetNode<Utils>("/root/Utils");
}
```

**Aplicacion en proyecto:** `Utils`, `Database` son Autoloads (singletons de Godot). Evitar crear singletons propios; usar el sistema de Autoload.

---

## State

**Problema:** Multiples booleanos (`isJumping`, `isDucking`) generan estados invalidos y codigo fragil con if/switch enormes.

**Solucion:** FSM donde cada estado es un objeto con su propia logica.

**Variantes:**
- **FSM simple**: Un estado activo a la vez
- **Concurrent**: Multiples FSM independientes (movimiento + equipamiento)
- **Hierarchical**: Estados heredan comportamiento comun
- **Pushdown Automata**: Pila de estados (disparar → volver al anterior)

**Cuando usarlo:**
- Comportamiento cambia segun estado interno
- Estados son mutuamente excluyentes
- Entidad responde a eventos/input

**En Godot/C#:**
```csharp
// Interfaz base
public abstract partial class PlayerState : Node
{
    public CharacterBody2D Parent;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public abstract void Update(double delta);

    [Signal]
    public delegate void StateTransitionEventHandler(PlayerState from, string to);
}

// Estado concreto
public partial class PlayerWalkState : PlayerState
{
    public override void Update(double delta)
    {
        var input = Input.GetVector(...);
        Parent.Velocity = input * Speed;

        if (input == Vector2.Zero)
            EmitSignal(SignalName.StateTransition, this, "idle");
    }
}

// FSM delega al estado actual
public override void _PhysicsProcess(double delta)
{
    _currentState.Update(delta);
}
```

**Aplicacion en proyecto:** Ya implementado en `PlayerState`, `EnemyState`, y sus FSMs. Sistema robusto con `ForceChangeState()` para interrupciones.

---

## Double Buffer

**Problema:** Leer datos mientras se escriben causa artefactos (tearing en graficos, orden de update afecta gameplay).

**Solucion:** Dos buffers: uno para lectura (actual), otro para escritura (siguiente). Swap al terminar.

**Cuando usarlo:**
- Estado se modifica incrementalmente
- Se lee durante la modificacion
- Necesitas cambios atomicos/simultaneos
- Renderizado grafico (framebuffers)

**Consideraciones:**
- Duplica uso de memoria
- Swap es rapido (solo intercambiar punteros)
- Datos tienen 1-2 frames de antiguedad

**En Godot/C#:**
```csharp
// Godot maneja double buffering de graficos automaticamente
// Para gameplay (ej: sistema de turnos simultaneos):

public class SimultaneousActions
{
    private List<Action> _currentBuffer = new();
    private List<Action> _nextBuffer = new();

    public void QueueAction(Action action)
    {
        _nextBuffer.Add(action);
    }

    public void ExecuteAll()
    {
        // Swap
        (_currentBuffer, _nextBuffer) = (_nextBuffer, _currentBuffer);
        _nextBuffer.Clear();

        // Execute all from current (estado consistente)
        foreach (var action in _currentBuffer)
            action.Execute();
    }
}
```

**Aplicacion en proyecto:** Godot maneja el rendering. Podria usarse si implementas combate por turnos simultaneos o sistemas donde el orden no debe importar.

---

## Game Loop

**Problema:** El juego debe correr a velocidad consistente independiente del hardware.

**Solucion:** Bucle infinito que procesa input, actualiza estado, y renderiza.

**Enfoques de timestep:**

| Tipo | Pros | Contras |
|------|------|---------|
| **Fixed simple** | Simple | Velocidad depende del hardware |
| **Fixed + sync** | Consistente, ahorra bateria | Se ralentiza si update tarda mucho |
| **Variable** | Suave | No-deterministico, fisica inestable |
| **Catch-up** (recomendado) | Estable + suave | Mas complejo |

**En Godot/C#:**
```csharp
// Godot maneja el game loop por ti:
// - _Process(delta)        → update variable (render, UI)
// - _PhysicsProcess(delta) → update fijo (fisica, gameplay)

// El delta de _PhysicsProcess es fijo (Project Settings > Physics > Common > Physics Fps)
// El delta de _Process es variable segun framerate

// Para logica de gameplay critica, usar _PhysicsProcess:
public override void _PhysicsProcess(double delta)
{
    // delta es consistente (~0.016 a 60fps)
    _stateMachine.Update(delta);
}

// Para visual/UI, usar _Process:
public override void _Process(double delta)
{
    // delta varia segun framerate
    _animationTime += delta;
}
```

**Aplicacion en proyecto:** FSMs usan `_PhysicsProcess` para updates consistentes. Godot abstrae el game loop; no necesitas implementarlo.

---

## Update Method

**Problema:** Multiples entidades necesitan actualizarse cada frame sin acoplar su logica al game loop.

**Solucion:** Cada entidad implementa `Update(delta)`, el game loop itera y llama a todos.

**Consideraciones:**
- Orden de update importa (A ve estado viejo de B, B ve estado nuevo de A)
- No es verdadera concurrencia, solo lo parece
- Entidades deben ser mayormente independientes

**Cuando usarlo:**
- Multiples objetos simulandose en tiempo real
- Comportamientos independientes entre entidades
- Juegos con simulacion continua (no turnos abstractos)

**En Godot/C#:**
```csharp
// Godot lo hace automaticamente via _Process/_PhysicsProcess
// Cada Node con estos metodos se actualiza

// Para FSM customizada:
public partial class EnemyStateMachine : Node
{
    private EnemyState _currentState;

    public override void _PhysicsProcess(double delta)
    {
        // Delega al estado actual
        _currentState?.Update(delta);
    }
}

// Todos los enemigos en el arbol se actualizan automaticamente
// sin necesidad de mantener lista manual
```

**Aplicacion en proyecto:** Cada `Enemy`, `Player`, componente con `_PhysicsProcess` usa este patron. Godot maneja la iteracion; solo defines el comportamiento.

---

## Resumen Rapido

| Patron | Usa cuando... | En Godot |
|--------|---------------|----------|
| **Command** | Acciones como objetos (undo, remap, replay) | Clases con `Execute()` |
| **Flyweight** | Miles de objetos similares | `Resource` compartido |
| **Observer** | Sistemas independientes se comunican | `[Signal]` + `+=` |
| **Prototype** | Spawners genericos, clonar objetos | `PackedScene.Instantiate()` |
| **Singleton** | Acceso global unico (con cuidado) | Autoloads |
| **State** | Comportamiento cambia segun estado | FSM con clases Estado |
| **Double Buffer** | Lectura/escritura simultanea | Automatico en rendering |
| **Game Loop** | Ejecucion continua del juego | `_Process` / `_PhysicsProcess` |
| **Update Method** | Entidades se actualizan cada frame | `_PhysicsProcess` en cada Node |

---

# Behavioral Patterns

## Bytecode

**Problema:** Comportamiento hardcodeado requiere recompilar; designers no pueden iterar rapido; mods/UGC necesitan sandbox seguro.

**Solucion:** Maquina virtual que ejecuta instrucciones codificadas como bytes. Stack para valores intermedios.

**Cuando usarlo:**
- Iteracion rapida importa mas que simplicidad
- Sandbox para contenido no confiable (mods)
- Lenguaje principal es muy bajo nivel para designers

**Cuando NO usarlo:**
- Requiere construir herramientas de authoring (compilador o editor visual)
- Necesitas infraestructura de debugging custom
- Complejidad no justificada para proyectos simples

**En Godot/C#:**
```csharp
// Godot tiene GDScript que ya es interpretado
// Para sistemas de habilidades data-driven, considera:

// Opcion simple: Resources como "bytecode"
public partial class SpellData : Resource
{
    [Export] public string[] Actions;  // ["damage:10", "heal:5", "spawn:fireball"]
}

// Interpreter basico
public void ExecuteSpell(SpellData spell, Node target)
{
    foreach (var action in spell.Actions)
    {
        var parts = action.Split(':');
        switch (parts[0])
        {
            case "damage": target.TakeDamage(int.Parse(parts[1])); break;
            case "heal": target.Heal(int.Parse(parts[1])); break;
            case "spawn": SpawnEffect(parts[1]); break;
        }
    }
}
```

**Aplicacion en proyecto:** Podria usarse para sistema de habilidades/pociones configurable via JSON. Actualmente no necesario.

---

## Subclass Sandbox

**Problema:** Muchas subclases similares duplican codigo y se acoplan a sistemas externos.

**Solucion:** Clase base provee operaciones protegidas; subclases solo implementan metodo sandbox usando esas operaciones.

**Cuando usarlo:**
- Muchas clases derivan de una base comun
- Subclases comparten patrones de comportamiento
- Quieres aislar subclases de sistemas externos

**En Godot/C#:**
```csharp
// Clase base con operaciones protegidas
public abstract partial class Superpower : Node
{
    // Operaciones que subclases pueden usar
    protected void PlaySound(string name) { /* ... */ }
    protected void SpawnParticles(PackedScene effect) { /* ... */ }
    protected void ApplyForce(Vector2 force) { /* ... */ }
    protected void DealDamage(int amount) { /* ... */ }

    // Metodo sandbox que subclases implementan
    protected abstract void Activate();
}

// Subclase solo usa operaciones de la base
public partial class FireballPower : Superpower
{
    protected override void Activate()
    {
        PlaySound("fireball_cast");
        SpawnParticles(_fireEffect);
        DealDamage(25);
    }
}
```

**Aplicacion en proyecto:** `PlayerState` ya usa este patron parcialmente. Estados usan `MoveComponent`, `Animations`, `Parent` provistos por la base.

---

## Type Object

**Problema:** Crear subclase por cada variante (100 tipos de enemigos = 100 clases) requiere recompilar y es inflexible.

**Solucion:** Separar "tipo" como objeto de datos. Instancias referencian su tipo en lugar de heredar.

**Cuando usarlo:**
- No sabes que tipos necesitaras
- Quieres agregar tipos sin recompilar (data-driven)
- Designers definen tipos via archivos de datos

**En Godot/C#:**
```csharp
// Type Object (define el "tipo")
public partial class MonsterBreed : Resource
{
    [Export] public string Name;
    [Export] public int MaxHealth;
    [Export] public int Attack;
    [Export] public float Speed;
    [Export] public Texture2D Sprite;

    // Comportamiento compartido
    public int CalculateDamage() => Attack + _rng.Next(5);
}

// Typed Object (instancia)
public partial class Monster : CharacterBody2D
{
    [Export] public MonsterBreed Breed;  // referencia al tipo

    private int _currentHealth;

    public override void _Ready()
    {
        _currentHealth = Breed.MaxHealth;  // usa datos del tipo
        _sprite.Texture = Breed.Sprite;
    }
}

// Crear variantes sin codigo nuevo:
// - res://breeds/goblin.tres (MaxHealth=30, Attack=5)
// - res://breeds/orc.tres (MaxHealth=100, Attack=15)
```

**Aplicacion en proyecto:** `EnemyData` Resource podria ser Type Object para variantes de enemigos. `PotionData` ya funciona similar.

---

# Decoupling Patterns

## Component

**Problema:** Clase monolitica toca muchos dominios (fisica, graficos, IA, audio) = acoplamiento, dificil mantener.

**Solucion:** Dividir en componentes especializados. Contenedor mantiene referencias sin conocer detalles.

**Comunicacion entre componentes:**
1. Estado compartido del contenedor (Position, Velocity)
2. Referencias directas entre componentes
3. Sistema de mensajes via contenedor

**Cuando usarlo:**
- Clase toca multiples dominios desacoplados
- Necesitas reutilizar capacidades en diferentes entidades
- Equipos trabajan en paralelo en diferentes sistemas

**En Godot/C#:**
```csharp
// Godot ES component-based por naturaleza
// Player.tscn:
//   Player (CharacterBody2D)
//     ├── HealthComponent
//     ├── HitboxComponent
//     ├── PotionComponent
//     ├── PlayerMoveComponent
//     └── AnimatedSprite2D

// Comunicacion via signals (Observer) o referencias:
public partial class Player : CharacterBody2D
{
    public HealthComponent Health { get; private set; }
    public PotionComponent Potion { get; private set; }

    public override void _Ready()
    {
        Health = GetNode<HealthComponent>("HealthComponent");
        Potion = GetNode<PotionComponent>("PotionComponent");

        // Componentes se comunican via signals
        Health.Died += OnDied;
    }
}
```

**Aplicacion en proyecto:** Arquitectura actual ES Component-based. Player tiene HealthComponent, PotionComponent, HitboxComponent, etc.

---

## Event Queue

**Problema:** Observer es sincrono; necesitas desacoplar temporalmente emisor de receptor, o procesar eventos en batch.

**Solucion:** Cola FIFO almacena eventos. Emisor encola y continua. Procesador consume cuando conviene.

**Diferencia con Observer:**
- Observer: sincrono, respuesta inmediata
- Event Queue: asincrono, procesamiento diferido

**Cuando usarlo:**
- Desacoplamiento temporal (no solo de identidad)
- Procesador controla cuando atender eventos
- Necesitas agregar/priorizar requests
- Comunicacion entre hilos

**En Godot/C#:**
```csharp
public partial class EventQueue : Node
{
    private Queue<GameEvent> _events = new();

    public void Enqueue(GameEvent evt)
    {
        _events.Enqueue(evt);
        // Retorna inmediatamente, no bloquea
    }

    public override void _Process(double delta)
    {
        // Procesar N eventos por frame para evitar hitches
        int processed = 0;
        while (_events.Count > 0 && processed < 10)
        {
            var evt = _events.Dequeue();
            ProcessEvent(evt);
            processed++;
        }
    }
}

// Uso: audio requests, damage numbers, achievements
_eventQueue.Enqueue(new PlaySoundEvent("explosion"));
```

**Aplicacion en proyecto:** No implementado actualmente. Podria usarse para sistema de audio o achievements sin bloquear gameplay.

---

## Service Locator

**Problema:** Necesitas acceso global a servicios (audio, logging) sin acoplar a implementacion concreta.

**Solucion:** Localizador central donde se registran servicios. Consumidores piden servicio por interfaz.

**Diferencia con Singleton:**
- Singleton: clase gestiona su propia instancia
- Service Locator: codigo externo registra implementacion

**Ventajas sobre Singleton:**
- Cambiar implementacion en runtime (null service para testing)
- No afecta diseno de la clase servicio
- Mas flexible para testing/mocking

**En Godot/C#:**
```csharp
// Service Locator simple
public static class Services
{
    private static IAudioService _audio;
    private static ILogService _log;

    public static IAudioService Audio => _audio;
    public static ILogService Log => _log;

    public static void Register(IAudioService audio) => _audio = audio;
    public static void Register(ILogService log) => _log = log;
}

// Registro en inicializacion
Services.Register(new GameAudioService());
Services.Register(new FileLogService());

// Uso en cualquier parte
Services.Audio.PlaySound("explosion");

// Testing: registrar null/mock service
Services.Register(new NullAudioService());
```

**Aplicacion en proyecto:** Godot Autoloads funcionan similar. `Utils`, `Database` son servicios globales accesibles via `/root/`.

---

# Optimization Patterns

## Data Locality

**Problema:** CPU procesa datos mas rapido de lo que puede obtenerlos de RAM. Cache misses = cientos de ciclos perdidos.

**Solucion:** Organizar datos contiguos en memoria para maximizar cache hits.

**Estrategias:**
- Arrays contiguos en lugar de punteros dispersos
- Separar datos "hot" (frecuentes) de "cold" (raros)
- Ordenar entidades activas al inicio del array

**Cuando usarlo:**
- Profiling confirma cache misses como problema
- Miles de entidades procesadas por frame
- Performance critica (mobile, consolas)

**En Godot/C#:**
```csharp
// MAL - objetos dispersos en memoria
List<Enemy> enemies;  // cada Enemy es referencia, datos dispersos

// MEJOR - datos contiguos para hot path
struct EnemyTransform { public Vector2 Position; public float Rotation; }
EnemyTransform[] transforms = new EnemyTransform[1000];

// Procesar solo activos (contiguos al inicio)
int activeCount = 500;
for (int i = 0; i < activeCount; i++)
{
    transforms[i].Position += velocity * delta;
}
```

**Aplicacion en proyecto:** Godot maneja esto internamente. Solo relevante si tienes miles de entidades y profiling muestra problemas.

---

## Dirty Flag

**Problema:** Recalcular datos derivados en cada cambio es costoso; muchos calculos intermedios nunca se usan.

**Solucion:** Flag indica si datos derivados estan desactualizados. Solo recalcular cuando se necesitan Y estan dirty.

**Cuando usarlo:**
- Datos primarios cambian mas frecuente de lo que se leen los derivados
- Calculo derivado es costoso
- Updates incrementales no son factibles

**Consideraciones:**
- Cada modificacion debe setear el flag
- Datos cacheados ocupan memoria
- Recalculo diferido puede causar hitches

**En Godot/C#:**
```csharp
public partial class TransformNode : Node2D
{
    private bool _worldTransformDirty = true;
    private Transform2D _cachedWorldTransform;

    public new Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
            _worldTransformDirty = true;  // marcar dirty
            PropagateToChildren();
        }
    }

    public Transform2D WorldTransform
    {
        get
        {
            if (_worldTransformDirty)
            {
                _cachedWorldTransform = CalculateWorldTransform();
                _worldTransformDirty = false;
            }
            return _cachedWorldTransform;
        }
    }
}
```

**Aplicacion en proyecto:** Godot usa dirty flags internamente para transforms. Podria usarse para UI que solo actualiza cuando datos cambian.

---

## Object Pool

**Problema:** Crear/destruir objetos frecuentemente es lento y fragmenta memoria (balas, particulas, enemigos).

**Solucion:** Preasignar pool de objetos. Reutilizar en lugar de crear/destruir.

**Free List:** Objetos inactivos forman lista enlazada usando su propia memoria = O(1) para obtener/devolver.

**Cuando usarlo:**
- Crear/destruir objetos frecuentemente
- Objetos de tamano similar
- Fragmentacion o GC es problema
- Objetos encapsulan recursos costosos

**Consideraciones:**
- Tamano fijo limita objetos simultaneos
- Requiere reinicializar al reutilizar
- Puede desperdiciar memoria con objetos sin usar

**En Godot/C#:**
```csharp
public partial class BulletPool : Node
{
    [Export] public PackedScene BulletScene;
    [Export] public int PoolSize = 100;

    private Queue<Bullet> _available = new();

    public override void _Ready()
    {
        // Preasignar
        for (int i = 0; i < PoolSize; i++)
        {
            var bullet = BulletScene.Instantiate<Bullet>();
            bullet.Visible = false;
            bullet.SetProcess(false);
            AddChild(bullet);
            _available.Enqueue(bullet);
        }
    }

    public Bullet Get(Vector2 position, Vector2 direction)
    {
        if (_available.Count == 0) return null;

        var bullet = _available.Dequeue();
        bullet.Initialize(position, direction);
        bullet.Visible = true;
        bullet.SetProcess(true);
        return bullet;
    }

    public void Return(Bullet bullet)
    {
        bullet.Visible = false;
        bullet.SetProcess(false);
        _available.Enqueue(bullet);
    }
}
```

**Aplicacion en proyecto:** Ideal para proyectiles de pociones, particulas, enemigos spawneados frecuentemente.

---

## Spatial Partition

**Problema:** "Que objetos estan cerca?" requiere comparar todos vs todos = O(n²), prohibitivo con muchas entidades.

**Solucion:** Estructuras que dividen espacio para queries O(log n) o O(1).

**Estructuras:**

| Tipo | Caracteristicas | Mejor para |
|------|-----------------|------------|
| **Grid** | Celdas fijas, simple, memoria fija | Objetos moviles, distribucion uniforme |
| **Quadtree** | Subdivide areas pobladas, adaptativo | Distribucion no uniforme, muchos objetos |
| **BSP/k-d Tree** | Division basada en objetos, balanceado | Geometria estatica |

**Cuando usarlo:**
- Deteccion de colisiones con muchas entidades
- Queries espaciales frecuentes (enemigos cercanos, etc.)
- Performance de O(n²) es inaceptable

**En Godot/C#:**
```csharp
// Grid simple para queries espaciales
public class SpatialGrid
{
    private Dictionary<Vector2I, List<Node2D>> _cells = new();
    private int _cellSize = 64;

    public void Add(Node2D obj)
    {
        var cell = GetCell(obj.Position);
        if (!_cells.ContainsKey(cell))
            _cells[cell] = new List<Node2D>();
        _cells[cell].Add(obj);
    }

    public List<Node2D> GetNearby(Vector2 position, int radius = 1)
    {
        var result = new List<Node2D>();
        var center = GetCell(position);

        for (int x = -radius; x <= radius; x++)
        for (int y = -radius; y <= radius; y++)
        {
            var cell = center + new Vector2I(x, y);
            if (_cells.TryGetValue(cell, out var list))
                result.AddRange(list);
        }
        return result;
    }

    private Vector2I GetCell(Vector2 pos) =>
        new Vector2I((int)(pos.X / _cellSize), (int)(pos.Y / _cellSize));
}
```

**Aplicacion en proyecto:** Godot tiene collision layers/masks y Area2D para deteccion. Solo implementar custom si necesitas queries mas especificas o tienes cientos de entidades.

---

## Resumen Rapido

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

---

*Documento completo. Referencia: https://gameprogrammingpatterns.com/contents.html*
