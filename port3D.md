# Mudanza a 3D

How Many Bunnies pasa de 2D pixel art a **3D low-poly**, con **PC como plataforma principal** y
móvil después. El concepto no cambia: un incremental cozy donde se alimenta a conejos que se
multiplican, con el árbol que se abre por cantidad de conejos criados.

Este doc es la referencia viva de la mudanza: qué se decidió, qué regla nueva impone el cambio,
qué sistemas pasan y cuáles hay que volver a pensar, y qué sigue abierto. **El *porqué* de cada
decisión va a [`bitacora.md`](bitacora.md); acá vive el estado.** Lo que falta analizar tiene
ficha en [`pendientes-analizar.md`](pendientes-analizar.md).

**El juego está en fase muy temprana.** Las primeras sesiones del proyecto nuevo van a ser de
lluvia de ideas y diseño; tener el proyecto ya en 3D sirve para asentar esas ideas sobre algo
que se ve, no para cerrarlas antes de tiempo.

---

## 1. Lo que ya está decidido

| Decisión | Qué significa |
|---|---|
| **Repo nuevo, no una rama** | Igual que con Little Big Potions: `how-many-bunnies` queda como **referencia de lectura** y cada sistema entra al proyecto nuevo cuando alguien lo necesita, con sus decisiones vueltas a tomar |
| **Por qué 3D** | Por dos razones de juego —**el espectáculo de la masa** y **la granja como maqueta**— y una de equipo: toda la forma de trabajar se está pasando a 3D |
| **Estilo low-poly** | El pixel art se abandona. Los modeladores ayudan, **al inicio sólo con props** |
| **PC primero, móvil después** | Pesa en el renderer y en cuánta masa se aguanta (§2) |
| **Techo de 1.000 conejos en pantalla, por ahora** | Y un **contador de cuántos se criaron** que sigue creciendo aparte. Los millones del GDD eran irreales; decenas de miles ya es difícil |
| **Los hitos de conejos abren el árbol** | Es lo que sobrevive de la economía. **El árbol es el de LBP**, no el de este repo (§5.4) |
| **Parte del trabajo se automatiza** | Por ejemplo, contratar granjeros que lancen zanahorias. Es el eje incremental: lo que al principio se hace a mano, después lo hace el juego |
| **El águila sigue** | Y la muerte de conejos sigue siendo la presión del juego. Puede sumarse **otra amenaza** |
| **Godot 4.7, C#, Forward+, Jolt** | Lo mismo que LBP. Este repo está en 4.6 |

**Lo que se busca de las referencias:**

- **A Game About Feeding a Black Hole**: rondas cortas con tienda entre medio. Acá la ronda es el
  día.
- **Gnorp Apologue**: caos controlado con muchas entidades en pantalla.
- **Bills Must Be Paid** y los incrementales de estos años: **el sentir del incremental bien
  hecho**. Árbol de habilidades, perks, y la sensación de que cada compra se nota.

---

## 2. La regla que impone la mudanza: la masa tiene techo

En LBP, la cámara impuso una regla de autoría (*nada alto donde se pelea*). Acá lo que impone la
regla es **la masa**, y se paga igual de caro si se descubre tarde:

> **El número que crece y el número que se ve son dos números distintos.** El contador de
> criados puede subir sin techo; lo que hay en la granja en un momento dado, no.

- **Techo provisional: 1.000 en pantalla.** Es un número de trabajo para dimensionar la
  simulación, no uno de balance.
- **Falta el motivo de juego para no tenerlos todos a la vez.** No puede ser un tope que se note
  como tope: tiene que haber una **mecánica o una razón de ficción** por la que los conejos
  salen de la granja. Ficha [`PA-001`](pendientes-analizar.md). El GDD ya tenía una semilla:
  *conquistar Francia*. Un conejo que se va puede irse a algo.
- **La simulación se dimensiona contra el techo, no contra el contador.** Eso cambia la
  arquitectura del conejo (§6).

---

## 3. Lo que la mudanza le puede dar al juego

Esto es **diseño, y candidato**: ninguna de estas ideas está decidida. Van acá porque son la
razón de juego de la mudanza y conviene tenerlas a la vista en las sesiones de ideas.

- **La zanahoria vuela de verdad.** La personalidad del conejo ya depende del vuelo: el
  **espabilado** sale mientras la zanahoria está en el aire y el **normal** espera a que toque el
  suelo. En 2D esa diferencia pasa sin que nadie la vea. Con un arco y una sombra en el piso, se
  lee: el jugador ve quién salió antes y aprende que sus conejos son distintos.
- **La granja es una maqueta, y la cámara se aleja.** El GDD ya pedía el *hormiguero creciente*:
  más conejos, cerca más grande, zoom out. En 3D alejarse es un **momento**: la granja se ve más
  chica, desde más arriba, y es la recompensa visual de haber crecido.
- **El día se lee en la luz.** Sombras que se alargan al atardecer, la casa que se enciende de
  noche. Además del color, el ciclo tiene dirección: el sol se mueve.
- **El águila se anuncia con su sombra.** El GDD ya prevé que el jugador salve al conejo tirándole
  una zanahoria. Una sombra que crece en el piso antes de la picada es la ventana para hacerlo, y
  hace de la amenaza algo que se ve venir y se puede responder, en vez de una pérdida al azar.

---

## 4. La forma de trabajar que se trae de LBP

La mudanza también trae el **método**. Estas son las piezas que funcionaron en LBP y cómo
aplican acá.

### 4.1 El flujo

- **Diseño, después técnica, después código.** Qué le hace sentir al jugador, qué decisión le
  da; después cómo se construye; después el código y el mapa que registra lo que quedó. En la
  conversación de diseño **el costo no es un argumento**.
- **Antes de portar un sistema, se revisa entero.** El veredicto es por pieza, y *"esto está bien
  como está"* es un resultado válido. En LBP la revisión del sistema de efectos encontró que *la
  arquitectura ya estaba desacoplada y el vocabulario no*: el problema que se buscaba no estaba y
  apareció otro.
- **Las reglas salen del código y se escriben arriba.** *"La altura es decorativa"* apareció
  cuatro veces en LBP antes de tener nombre. Cuando una corrección se repite, se le pone nombre y
  sube al `CLAUDE.md` como decisión de arquitectura.
- **Los casos concretos primero; la infraestructura sale de duplicados reales.** Nada de
  centralizar por adelantado.
- **No se documenta una implementación hasta que el usuario la prueba.** Todo número de balance
  es provisional y no se pregunta por él.

### 4.2 La documentación como sistema

Hoy este repo tiene un `CLAUDE.md` que mezcla instrucciones, estado y enciclopedia, y un GDD. En
LBP el reparto es:

| Qué | Dónde |
|---|---|
| Instrucciones + a dónde ir | `CLAUDE.md`, corto, porque se carga en cada turno |
| El *por qué* de cada decisión | la bitácora, cronológica y con tags |
| Qué falta analizar | `pendientes-analizar`, con ids `PA-0XX` que no se reutilizan |
| Qué falta hacer | `tareas` |
| Qué hace cada clase | los mapas (`code-map`, `ui-map`, `data-map`) |
| Por qué una mecánica es así | su `-design` |
| Lo que se aprendió dos veces | un skill de proyecto |

En esta mudanza se siembran sólo la bitácora y los pendientes. El resto entra con el repo nuevo,
cuando haga falta.

### 4.3 Las piezas de código que funcionaron

| Pieza de LBP | Qué resuelve | Dónde entra acá |
|---|---|---|
| `BaseFSM<TState>` + `IFSMState` | Una FSM genérica, los estados salen de la escena | La cámara, la UI, un granjero. **No el conejo** (§6) |
| Composición por componentes | Nodos hijos que no se conocen; los coordina el dueño | Granjeros, águila, casa |
| Resources como Type Object + `Database` por Id | Contenido en `.tres`, no en clases | Mejoras, tipos de conejo, amenazas |
| `SkillTreeManager` + el addon `skill_tree` | El estado del árbol vive **fuera** de la UI y se guarda | Reemplaza `UpgradeTree`/`UpgradeNode` enteros |
| Agregado de bonuses (`PlayerStats` y sus snapshots) | Muchas fuentes suman a una estadística, y se recalcula una vez | Es el motor del incremental (§5.4) |
| `SaveManager` | Guardado con su bloque de progresión | El incremental lo necesita desde el día uno |
| `UiLayer` + su pila de pantallas | Pantallas con push/pop y transiciones | Reemplaza `UIManager` |
| `AdaptivePool<T>` | Pool que crece bajo demanda | Audio, números flotantes, zanahorias |
| `CameraSpace` | El único lugar donde la pantalla se mapea al piso | Agarrar y lanzar la zanahoria con el mouse |
| Autoloads que se resuelven **tarde** | Nada depende del orden de `_Ready` | Hoy `ShaderManager` depende de que `GameManager` exista antes |

---

## 5. Inventario de sistemas

Como en LBP, la aclaración sin la cual esta tabla miente: **todo lo que toca posición, física o
colisión cambia de tipo** (`Node2D` -> `Node3D`, `Area2D` -> `Area3D`, `Vector2` -> `Vector3`).
Ese costo es mecánico. Lo que se categoriza acá es **cuánto diseño hay que volver a decidir**.

### 5.1 Pasan limpios

- **`AnimatedButton`**: el squish de botón con su sonido.
- **`TooltipPanel` + `TooltipManager`**: BBCode, fade y clamp al viewport. Un `Control` es
  idéntico en 3D.
- **`MainMenu`** y la música del **`AudioManager`**: el ciclo aleatorio sin repetir track, con el
  delay entre canciones al estilo Minecraft. Hay que compararlo con el `AudioManager` de LBP antes
  de elegir cuál viaja.
- **`SoundEffect`** como Resource de configuración, sin el contador (§5.5).
- **Los enums** `SoundEffectType` y `UpgradeEffectType`, aunque el segundo probablemente muera con
  el motor de estadísticas (§5.4).

### 5.2 Cambios leves

- **El reloj del día** (`GameManager`): Day -> Sunset -> Night y sus señales sobreviven. Lo que
  cambia es con quién vive (§5.4).
- **La reproducción** (`ProcessReproduction`): es una cuenta y no se entera de la dimensión.
- **La intención de los estados del conejo**: idle, wander, watch, ir a la zanahoria, comer,
  ir a dormir, dormir. Sobrevive como **comportamiento**; la forma de implementarlo es otra (§6).
- **La personalidad** (`IsAlert`, 30%): sobrevive entera, y el 3D la hace visible (§3).
- **La casa** (`BunnyHouse`): un punto de destino. Pasa a ser un prop.

### 5.3 Adaptables, con decisiones adentro

- **El cielo** (`ShaderManager`): `CanvasModulate` no existe en 3D. Pasa a ser
  `DirectionalLight3D` + `WorldEnvironment`, con **color y ángulo del sol** recorriendo el día. Es
  donde está el premio de §3.
- **Agarrar y lanzar la zanahoria** (`CarrotEdible`): seguir al mouse pasa a ser un rayo contra el
  plano del piso (`CameraSpace.PointerToPlane`). El vuelo con fricción puede volverse un arco con
  altura real. **Es la primera excepción candidata a "todo a una cota"**: la altura de la
  zanahoria sí le importa al conejo.
- **El campo** (`CarrotField` + `CarrotCrop`): los slots con `Marker2D` pasan a `Marker3D`; las
  tres etapas de crecimiento pasan de textura a modelo o escala. El click sobre el `Area2D` pasa a
  picking 3D.
- **Los límites** (`BunnyBounds`): el polígono pasa a XZ. Con la simulación en datos (§6) es un
  test de punto en polígono sobre el plano, igual que hoy.
- **El águila** (`Eagle`): la picada con tween sobrevive; se suma la sombra que anuncia. El
  reparent del conejo depende de cómo exista el conejo (§6).
- **La cámara** (`Camera2D` fija): todo por decidir. Ficha [`PA-003`](pendientes-analizar.md).

### 5.4 No sobreviven, o piden revisión completa

- **El conejo como nodo.** Hoy cada conejo es un `Node2D` con **8 nodos de estado** y un
  `Area2D`, con su FSM de strings y señales por estado. Con techo de 1.000 no llega, y en 3D menos.
  Ver §6.
- **El árbol de mejoras** (`UpgradeTree` + `UpgradeNode`). Se reemplaza por el de LBP. Además
  tenía un problema de raíz: **el estado vive adentro de un `Control`**, y `BunniesManager` lo sale
  a buscar por grupo. Sin estado fuera de la UI no hay guardado limpio. Cómo encaja el costo del
  árbol de LBP con hitos de conejos que no se gastan: ficha [`PA-004`](pendientes-analizar.md).
- **Los efectos de las mejoras.** Hoy cada efecto nuevo toca **tres `switch`**: `ApplyEffect`, la
  caché de `BunniesManager` y el texto del tooltip. Un incremental **es** apilar estadísticas, así
  que ese motor tiene que salir de datos: una estadística con id, fuentes que suman o multiplican,
  y un recálculo cuando algo cambia. Es el patrón **Dirty Flag**, y LBP ya lo tiene resuelto con
  los snapshots de bonuses.
- **`GameManager` tal cual está.** Hace cuatro trabajos: reloj, aparición del águila, cambio de
  escena y disparo de la reproducción. En LBP eso se reparte entre el flujo de escenas y un
  director. Las amenazas en particular quieren su propio dueño, más si se suma una segunda (§1).
- **`Utils` como registro de nodos de escena.** `BunnyHouse`, `BunniesContainer`, `UIManager` y
  `BunnyBounds` se anotan desde la escena en un autoload. Funciona; lo que hay que decidir es qué
  sigue siendo global cuando la simulación tenga dueño propio.
- **El arte entero.** Del spritesheet al low-poly. Con modelos estáticos al inicio, el bamboleo
  procedural de LBP (`WaddleComponent`) es precedente de cómo se ve vivo algo sin animaciones.

### 5.5 Lo que no se copia: bugs del 2D

Vistos al leer el repo, para que no viajen:

- **Las mejoras no se aplican entrando desde el menú, probablemente.** La escena principal es
  `main_menu.tscn`, así que `BunniesManager.ConnectToUpgradeTree` corre al arrancar el juego y no
  encuentra el árbol. Sólo funcionaría abriendo `main.tscn` directo.
- **`AudioManager.OnTrackFinished`**: el timer del delay llama a `PlayRandomTrack` aunque en el
  medio se haya llamado `StopMusicLoop`.
- **`SoundEffect._activeCount` vive en un `.tres`.** Un Resource es data compartida y cacheada:
  mutarlo en runtime sobrevive al cambio de escena. El contador va en el `AudioManager`.
- **El águila puede llevarse un conejo que ya está dormido en la casa.** El GDD dice que se lleva
  al **despistado que quedó afuera**, y eso no existe todavía.
- **`ProcessReproduction` usa `AdultsPerBaby` para la base y un `/ 2` fijo para el bonus.** Si se
  cambia uno, se desincronizan.
- **Cada `PointsChanged` recalcula todo**: cada `UpgradeNode` recorre el grupo entero de conejos y
  el árbol completo. Con 10 nodos y 4 conejos no se nota; contra el techo de §2, sí.

---

## 6. La simulación de la masa

**Candidata a analizar e implementar**, no decidida. Ficha [`PA-002`](pendientes-analizar.md).

Hay tres formas de que los conejos existan:

| | Cómo es | Techo práctico |
|---|---|---|
| **(a) Nodos** | Como hoy: un nodo por conejo, FSM de nodos | Unos cientos |
| **(b) Datos** | Un array de structs con el estado como enum, un manager que los actualiza a todos, y un `MultiMeshInstance3D` que los dibuja | Miles |
| **(c) Híbrido** | Unos pocos "de verdad" cerca de la cámara, el resto como datos | Miles, con dos caminos de código |

**(b) es la candidata.** Lo que implica:

- **El conejo deja de ser un nodo.** Es una fila: posición, estado, nutrición, personalidad,
  timer. La FSM pasa a ser un `switch` sobre un enum; la intención de los estados de §5.2
  sobrevive entera y la `BaseFSM` de LBP no aplica acá.
- **Dos patrones del libro dejan de ser teoría**: **Data Locality** (arrays contiguos, sin saltar
  por el heap) y **Update Method** llevado a un solo manager en vez de mil `_PhysicsProcess`.
- **La detección de zanahorias deja de ser un `Area2D` por conejo.** Con pocas zanahorias en el
  aire, recorrerlas por conejo es barato; si crecen, entra **Spatial Partition** (una grilla).
- **Lo que necesita ser nodo pide un puente.** El águila agarra a *un* conejo: ese conejo sale del
  array y pasa a ser un nodo por el tiempo que dure la picada. El mismo puente sirve para cualquier
  cosa que quiera un conejo individual.
- **La animación sale del shader.** Con `MultiMesh` no hay `AnimationPlayer` por conejo: el salto,
  el bamboleo y el crecimiento se hacen con datos por instancia (`INSTANCE_CUSTOM`) y el vertex
  shader.

---

## 7. Preguntas abiertas

Ninguna bloquea arrancar el repo.

1. **¿Por qué los conejos salen de la granja?** El motivo de juego del techo (§2). Ficha
   [`PA-001`](pendientes-analizar.md).
2. **¿Qué cámara?** Fija o alejándose con la población, perspectiva u ortográfica tipo maqueta,
   y cuánto la controla el jugador. Ficha [`PA-003`](pendientes-analizar.md).
3. **¿Sigue la caca como moneda?** Lo que se confirmó es que los hitos de conejos abren el árbol.
   El GDD tiene además la caca que se vende por dinero para una tienda. Ficha
   [`PA-004`](pendientes-analizar.md).
4. **¿Qué se automatiza y en qué orden?** Los granjeros son el primer ejemplo. Ficha
   [`PA-005`](pendientes-analizar.md).
5. **¿Cuál es la segunda amenaza?** Ficha [`PA-006`](pendientes-analizar.md).
6. **¿El vuelo de la zanahoria tiene altura jugable?** Es la excepción candidata a "todo a una
   cota" (§5.3). Se decide con la cámara.

---

## 8. Lo que la mudanza NO cambia

Para que no se re-litigue de paso:

- Es un incremental cozy: alimentar conejos para que se multipliquen.
- El día corto con pausa entre días. El día es la ronda.
- Los conejos crecen al comer y se reproducen de noche en la casa.
- La personalidad: el espabilado y el normal.
- La cantidad de conejos criados abre el árbol.
- El águila como amenaza.
- PC primero.
