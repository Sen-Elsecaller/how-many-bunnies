# Shaders - Efectos Visuales

## Sistema Actual: ShaderManager + CanvasModulate

### Arquitectura
```
ShaderManager (Autoload)
    ↓ escucha signals
GameManager.DayStarted / SunsetStarted / NightStarted
    ↓ tween de 5s
CanvasModulate.Color → modula color de toda la escena
```

### Configuración en Editor
1. **Project Settings > Autoload**: Agregar ShaderManager
2. **Escena principal**: Agregar nodo `CanvasModulate`
3. **Script de escena**: Llamar `ShaderManager.Instance.RegisterSkyLight(canvasModulate)`

### Exports de ShaderManager
| Propiedad | Default | Descripción |
|-----------|---------|-------------|
| DayColor | #ffffff | Blanco, sin modulación |
| SunsetColor | #ff9966 | Naranja atardecer |
| NightColor | #1a1a2e | Azul oscuro noche |
| TransitionDuration | 5f | Segundos de transición |

### Flujo de Colores
```
Day (#fff) ──5s──> Sunset (#ff9966) ──5s──> Night (#1a1a2e)
     ↑                                            │
     └────────────────── 5s ──────────────────────┘
```

---

## Shaders Adicionales (No Implementados)

Archivos en `resources/shaders/` para uso futuro.

### 1. Shadows.gdshader - Sombras Dinámicas

**Técnica:** Ray-marching sobre SDF (Signed Distance Field)

**Cómo funciona:**
1. Requiere nodos `LightOccluder2D` en la escena
2. Godot genera SDF automáticamente con los occluders
3. Shader "camina" desde cada pixel en dirección del sol
4. Si choca con obstáculo → sombra, si no → luz

**Parámetros:**
```glsl
shadow_color   // Color y opacidad de sombra
shadow_angle   // Dirección del sol (0-360°)
shadow_length  // Largo máximo de sombras
```

**Aplicación:**
- ColorRect que cubra todo el viewport
- Material: ShaderMaterial con Shadows.gdshader
- Animar `shadow_angle` para simular movimiento del sol

**Valores típicos por fase:**
| Fase | Angle | Length |
|------|-------|--------|
| Amanecer | 72° | 15 |
| Mediodía | 120° | 12 |
| Atardecer | 270° | 15 |
| Noche | 1° | 1 |

### 2. LightRays.gdshader - Rayos de Luz

**Técnica:** Ruido Perlin procedural + animación por TIME

**Cómo funciona:**
1. Genera patrón de ruido en dos capas (diferentes densidades)
2. Rota según `angle` para dirección de rayos
3. Anima con `sin(TIME)` para efecto de movimiento
4. `smoothstep` suaviza bordes

**Parámetros principales:**
```glsl
color              // Color y alpha de rayos
angle              // Rotación de rayos
movement_speed     // Velocidad de animación
ray_1_density      // Densidad capa 1 (20 típico)
ray_2_density      // Densidad capa 2 (80 típico)
ray_1_intensity    // Brillo capa 1 (0-1)
ray_2_intensity    // Brillo capa 2 (0-1)
```

**Aplicación:**
- ColorRect sobre todo el viewport (después de sombras)
- Material: ShaderMaterial con LightRays.gdshader
- Solo animar `color` para cambiar intensidad/tono

**Valores típicos por fase:**
| Fase | Color (RGBA) |
|------|--------------|
| Día | (1, 1, 0.9, 0.3) |
| Atardecer | (1, 0.6, 0.2, 0.4) |
| Noche | (0.4, 0.6, 0.8, 0.1) |

---

## Implementación Futura

Para agregar estos shaders:

1. **Estructura de escena:**
```
Main
├── ... (juego)
├── ShadowCaster (ColorRect)     ← Shadows.gdshader
│   └── ShaderMaterial
├── SkyLight (CanvasModulate)    ← Ya implementado
└── LightRays (ColorRect)        ← LightRays.gdshader
    └── ShaderMaterial
```

2. **Agregar a ShaderManager:**
   - Referencias a ColorRect
   - Tweens para parámetros de shader
   - Usar `TweenMethod` con `SetShaderParameter`

3. **LightOccluder2D:**
   - Agregar a objetos que proyectan sombra (árboles, casa)
   - Dibujar polígono de oclusión

---

## Referencia

Proyecto original: `C:\Users\Sen\Documents\GameExperiments\DayNightCycleGodot-main`
- Usa AnimationPlayer con ciclo de 24s
- Tracks paralelos para sky, shadows, rays
- Interpolación lineal entre keyframes
