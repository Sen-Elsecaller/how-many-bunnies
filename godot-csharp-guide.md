# Guía C# para Godot

> **Mantener actualizado:** Agregar nuevos aprendizajes, errores encontrados y soluciones.

## Convenciones de Código
- PascalCase: propiedades públicas, métodos
- _camelCase: campos privados
- Signals: delegates con sufijo `EventHandler`
- `[Export]`: exponer al editor
- `[GlobalClass]`: nodos agregados desde editor (componentes reutilizables, UI containers)
- `partial class`: requerido para clases que heredan de Node

## Conversión GDScript → C#

| GDScript | C# |
|----------|-----|
| `@onready var` | campo privado + GetNode en _Ready() |
| `@export var` | `[Export] public Tipo Nombre { get; set; }` |
| `signal nombre` | `[Signal] public delegate void NombreEventHandler(...)` |
| `emit_signal("nombre")` | `EmitSignal(SignalName.Nombre, ...)` |
| `class_name Nombre` | `[GlobalClass] public partial class Nombre` |
| `to_local(pos)` | `GetGlobalTransform().AffineInverse() * pos` |
| `preload()` | `GD.Load<T>()` o `ResourceLoader.Load<T>()` |

## Errores Comunes

### Llaves {} obligatorias en if/else
```csharp
// MAL - Bug silencioso
if (condition)
    GD.Print("debug");
    DoSomething();          // SIEMPRE se ejecuta

// BIEN
if (condition)
{
    GD.Print("debug");
    DoSomething();
}
```

### Ambigüedad Dictionary (CS0104)
`using Godot.Collections;` + `using System.Collections.Generic;` causa conflicto.

**Solución:** Aliases
```csharp
using Godot.Collections;  // Array<T>, Dictionary<K,V> de Godot
using SysDict = System.Collections.Generic.Dictionary<string, float>;
```

- `Godot.Collections`: datos exportados o signals
- `System.Collections.Generic`: datos internos

### AddChild durante _Ready()
Error: "Parent node is busy setting up children"

**Solución:** `CallDeferred()`
```csharp
// MAL
parent.AddChild(newNode);

// BIEN
parent.CallDeferred(Node.MethodName.AddChild, newNode);
```

## Signals - Buenas Prácticas
- Conectar en `_Ready()` con `+=`
- Desconectar en `_ExitTree()` con `-=` (prevenir memory leaks)
- Acceder via `SignalName.NombreSignal`

## Autoloads - Patrón static Instance
```csharp
public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }
}

// Uso:
GameManager.Instance.StartDay();
```

**NO** guardar referencias privadas a autoloads.

## Acceso entre Sistemas con Groups
Cuando un nodo necesita encontrar otro que aún no existe en `_Ready()`:
```csharp
// En _EnterTree() del nodo a encontrar
AddToGroup("upgrade_tree");

// Para encontrarlo
var tree = GetTree().GetFirstNodeInGroup("upgrade_tree") as UpgradeTree;
```

## Estructura de Métodos
```csharp
private void Move(Vector2 inputDir)
{
    // === LÓGICA FUNCIONAL (primero) ===
    Parent.Velocity = inputDir * Speed;
    Parent.MoveAndSlide();

    // === DEBUG (al final, fácil de eliminar) ===
    // GD.Print($"Debug: {inputDir}");
}
```

## Colores en BBCode (RichTextLabel)
```csharp
// Convertir Color a hex para BBCode
var hexColor = myColor.ToHtml(false);  // false = sin alpha
var bbcode = $"[color=#{hexColor}]Texto[/color]";
```

## [ExportGroup] para organizar inspector
```csharp
[ExportGroup("Position")]
[Export] public Vector2 Offset { get; set; }

[ExportGroup("Colors")]
[Export] public Color TitleColor { get; set; }
```

## Patrón: Nodo en escena + Autoload
Cuando necesitas [Export] en inspector pero también acceso global:
```csharp
// Autoload (persiste entre escenas)
public partial class TooltipManager : Node
{
    public static TooltipManager Instance { get; private set; }
    private TooltipPanel _panel;

    public void RegisterPanel(TooltipPanel panel) => _panel = panel;
}

// Nodo en escena (tiene [Export], se re-registra cada vez)
public partial class TooltipPanel : PanelContainer
{
    [Export] public Color TitleColor { get; set; }

    public override void _Ready()
    {
        TooltipManager.Instance?.RegisterPanel(this);
    }
}
```

## Principios
1. Lógica funcional primero, debug al final
2. Una responsabilidad por método
3. Delegar a nodos hijos (no crear exports redundantes)
4. Solo `[Export]` si se necesita cambiar en runtime
