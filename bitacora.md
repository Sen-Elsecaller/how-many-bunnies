# Bitácora — How Many Bunnies

Registro de decisiones de diseño, bugs resueltos y contexto de sesión — orden
**reverso-cronológico** (más reciente arriba), agrupado por semana. Acá va el *por qué* de cada
decisión; el estado vivo está en los docs de referencia.

**Al inicio de sesión:** leer las últimas 3 entradas. Para historial más viejo, buscar por texto
o por tag.

**Tags:** revisar esta lista antes de crear uno nuevo — evitar sinónimos. Formato: kebab-case,
sin tildes, a nivel sistema/módulo.

`#port-3d` `#docs` `#economy` `#skill-tree` `#bunny` `#camera` `#threats` `#automation`
`#process-rules`

**La fecha es la del día.** Si ya hay una entrada de hoy, la nueva es `(Sesión 2)`, `(Sesión 3)`
en el título.

---

## Semana 22 - 28 de septiembre

### Sesión: 2026-09-25 — El juego se muda a 3D, y el conejo deja de ser un nodo

**Tags:** #port-3d #docs #economy #skill-tree #bunny #process-rules

Sesión de lectura y conversación, **sin una línea de código**. Se leyó el repo entero junto con
la forma de trabajar de Little Big Potions, y salió [`port3D.md`](port3D.md), el doc vivo de la
mudanza.

#### Por qué 3D

Dos razones de juego y una de equipo. Las de juego: **el espectáculo de la masa** y **la granja
como maqueta**. La de equipo: toda la forma de trabajar se está pasando a 3D, y LBP ya hizo el
camino.

Lo que el 3D le da al juego no es el look. La **personalidad del conejo** ya dependía del vuelo
de la zanahoria —el espabilado sale mientras vuela, el normal espera a que caiga—, y en 2D esa
diferencia pasa sin que nadie la vea. Con un arco y una sombra en el piso, el jugador ve quién
salió antes. Lo mismo con el águila: una sombra que crece antes de la picada convierte una
pérdida al azar en algo que se ve venir y se responde. Todo eso quedó como **candidato** en
`port3D.md` §3, no como decisión.

#### Por qué un repo nuevo

Por la misma razón que en LBP: casi nada se copia tal cual, porque todo lo que toca posición,
física o colisión cambia de tipo, y con eso cambian también las decisiones. Una rama invita a
portar a ciegas para que compile. Empezando limpio, cada sistema entra cuando alguien lo
necesita. Este repo queda como referencia de lectura.

#### El número que crece y el número que se ve

El GDD hablaba de **millones** de conejos. Es irreal; decenas de miles ya es difícil. Quedó un
**techo provisional de 1.000 en pantalla** y un **contador de criados** aparte, que sigue
creciendo.

Eso es una regla de diseño que se paga cara si se descubre tarde, igual que *"nada alto donde se
pelea"* en LBP: dimensiona la simulación. Lo que falta es el **motivo de juego** para que los
conejos salgan de la granja sin que se sienta como un tope ([`PA-001`](pendientes-analizar.md)).
*Conquistar Francia* ya estaba en el GDD como semilla.

#### El conejo deja de ser un nodo, probablemente

Hoy cada conejo es un `Node2D` con 8 nodos de estado y un `Area2D`. Contra el techo de 1.000 no
llega. La candidata es **simulación en datos**: un array de structs con el estado como enum, un
manager que los actualiza, y `MultiMeshInstance3D` para dibujarlos. Queda para analizar e
implementar ([`PA-002`](pendientes-analizar.md)).

La consecuencia contraintuitiva: la `BaseFSM<TState>` de LBP, que es de lo mejor que tiene ese
proyecto, **no aplica al conejo**. Sigue sirviendo para todo lo que es uno solo: la cámara, la
UI, un granjero.

#### La economía: los hitos sobreviven, el árbol se reemplaza

Lo que se mantiene es que **la cantidad de conejos abre el árbol**. El árbol que viaja es **el de
LBP**, no `UpgradeTree`. Tenía un problema de raíz: el estado de las mejoras vivía adentro de un
`Control` y `BunniesManager` lo salía a buscar por grupo, así que no había forma limpia de
guardarlo.

Lo que no cierra todavía: el árbol de LBP **gasta** una moneda, y un hito de conejos **no se
gasta** ([`PA-004`](pendientes-analizar.md)). La caca como segunda moneda quedó sin confirmar ni
descartar.

De Bills Must Be Paid y los incrementales de estos años, lo que se busca es **el sentir del
incremental bien hecho**: árbol, perks, y que cada compra se note. Eso pide que los efectos de
las mejoras salgan de datos. Hoy cada efecto nuevo toca tres `switch` en tres archivos.

#### La automatización y las amenazas

**Parte del trabajo se automatiza**, empezando por granjeros que lancen zanahorias
([`PA-005`](pendientes-analizar.md)). Las primeras sesiones del repo nuevo van a ser de lluvia de
ideas y diseño; tener el proyecto ya en 3D sirve para asentarlas.

**El águila sigue** y puede sumarse otra amenaza ([`PA-006`](pendientes-analizar.md)).

#### Lo que se decidió sin discusión

Low-poly, con los modeladores ayudando al inicio sólo con props. PC primero, móvil después. Godot
4.7 como LBP (este repo está en 4.6).

#### Bugs vistos al leer, que no tienen que viajar

Listados en `port3D.md` §5.5. El más serio: **las mejoras probablemente no se aplican entrando
desde el menú**, porque `BunniesManager` busca el árbol al arrancar el juego, cuando la escena
cargada es el menú. No se arregló: el repo pasa a ser referencia de lectura.

#### El método viaja con el juego

Del flujo de LBP se trae todo (diseño -> técnica -> código, revisión antes de portar, reglas que
suben al `CLAUDE.md`, documentar después de probar). De la documentación, por ahora **sólo la
bitácora y los pendientes**: el resto entra con el repo nuevo, cuando haga falta.
