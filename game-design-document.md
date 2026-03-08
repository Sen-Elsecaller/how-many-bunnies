# How Many Bunnies to Conquer France
## Game Design Document

---

## Concepto

Incremental game cozy sobre una granja de conejos que se multiplican exponencialmente hasta conquistar Francia.

**Inspiraciones:**
- "A Game About Feeding a Black Hole" - estetica visual limpia, rondas cortas (10-30s), tienda entre rondas
- "Gnorp Apologue" - caos controlado con miles de entidades en pantalla

**Objetivo final:** X millones de conejos para "conquistar Francia"

**Estetica:** Pixel art (sprite bunny.png como referencia), topdown estilo Stardew Valley

---

## Layout

```
+---------------------------+
|                           |
|     PASTO (3/4 pantalla)  |
|     [conejos caminando]   |
|        [casita]           |
|                           |
+---------------------------+
|~~~~~~~~~ CERCA ~~~~~~~~~~~|
+---------------------------+
|   CULTIVO ZANAHORIAS      |
|      [zona caca]          |
+---------------------------+
```

---

## Core Loop

```
┌─────────────────────────────────────────────────────────┐
│  DIA (30s, se alarga con upgrades)                      │
│  ├── Zanahorias crecen solas                            │
│  ├── Conejos wandering                                  │
│  ├── Jugador agarra zanahoria → suelta sobre conejo     │
│  ├── Conejo come → crece (4 zanahorias = adulto)        │
│  └── Conejos van a zona de caca (1-3 veces)             │
├─────────────────────────────────────────────────────────┤
│  FIN DE DIA                                             │
│  ├── Comprador llega → vende caca → ganas $             │
│  └── Conejos entran a casita                            │
├─────────────────────────────────────────────────────────┤
│  NOCHE                                                  │
│  ├── parejas = floor(adultos / 2)                       │
│  ├── crias = parejas * criasBase                        │
│  └── Tienda disponible (árbol + compras)                │
├─────────────────────────────────────────────────────────┤
│  AMANECER                                               │
│  └── Conejos salen de casita (incluye nuevas crias)     │
└─────────────────────────────────────────────────────────┘
```

---

## Ciclo de Vida del Conejo

1. **Nace pequeño** - sale de casita al amanecer
2. **Come zanahorias** - crece visualmente
3. **Adulto** - al llegar a tamaño adulto, deja de comer
4. **Noche** - entra a casita, se empareja, cria
5. **Repeat**

---

## Sistema Economico

### Moneda: Caca de Conejo
- Conejos van solos a zona designada (1-3 veces por dia)
- Caca se acumula automaticamente
- Al final del dia, alguien viene a comprarla
- Dinero ($) se usa en tienda

### Dos Sistemas de Upgrades

**1. Arbol de Habilidades (por milestones de cantidad de conejos)**

Desbloqueado al alcanzar X conejos. 3 ramas iniciales:

| Rama | Efecto |
|------|--------|
| Crecimiento | Probabilidad de que zanahoria dé x2 crecimiento |
| Produccion | Mayor probabilidad de hacer caca |
| Velocidad | Conejos van más rapido a las zanahorias |

**2. Tienda (con dinero $)**

Compras con $ de vender caca:
- Hover-pickup: agarrar zanahoria sin click (solo pasar mouse)
- [Más upgrades por definir]

---

## Escalado Visual

- Mas conejos = cerca mas grande = zoom out
- Conejos se ven mas pequeños pero hay mas
- Sensacion de "hormiguero" creciente
- Expansion automatica al llegar a X conejos

---

## Mecanica del Halcon (Futuro)

- Una casita grande donde van todos en la noche
- Conejo despistado puede quedarse afuera → halcon se lo lleva
- Contramedidas: upgrades O jugador tira zanahoria para salvarlo
- (Implementar despues de core loop)

---

## Decisiones Confirmadas

| Aspecto | Decision |
|---------|----------|
| Camara | Topdown, zoom out dinamico |
| IA conejos | Wandering simple al inicio |
| Zanahorias | Auto-crecen con el tiempo |
| Crecimiento visual | Si, conejos crecen visualmente |
| Performance | Codigo optimo para cientos de conejos |
| Persistencia | Upgrades permanentes. NG+ a futuro |
| Conejos iniciales | 4 |
| Duracion dia | 30s inicial, se alarga con upgrades |
| Expansion cerca | Automatica al llegar a X conejos |
| Muerte conejos | Solo por halcon (mecanica futura) |
| Parejas | floor(adultos/2), matching automatico |
| Crias por pareja | Variable, empieza en 1 |
| Plataforma | PC primero, movil despues |
| UI stats | Contador de conejos siempre visible |

---

## Pendiente Definir

- Audio/musica
- Visual de transicion dia/noche
- Thresholds exactos (X conejos para expansion, milestones árbol, etc.)
- Mas upgrades para tienda
- Balanceo economico (precio caca, costo upgrades)
