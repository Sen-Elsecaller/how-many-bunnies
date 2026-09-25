# Pendientes de Analizar

Necesidades detectadas que requieren análisis antes de implementar. Cada ficha lleva un id
`PA-0XX` que **no se reutiliza nunca**, ni aunque la ficha se cierre. Al abrir una nueva, se toma
el número siguiente al mayor.

Cuando una ficha se resuelve, la decisión va a [`bitacora.md`](bitacora.md) y al doc que
corresponda, y la ficha se borra de acá.

---

## [PA-001] Por qué los conejos salen de la granja

**Planteado:** 2026-09-25, en la mudanza a 3D ([`port3D.md`](port3D.md) §2).

**El problema:** hay un techo provisional de **1.000 conejos en pantalla** y un contador de
**criados** que crece sin techo. Hace falta una **mecánica o una razón de ficción** para que
existan dos números, que no se sienta como un tope.

**Lo que ya hay para empezar:**

- El GDD ya tenía una semilla: *conquistar Francia*. Un conejo que se va puede irse a algo:
  una expedición, un ejército, otra granja.
- El águila ya saca conejos, pero es una pérdida, no un destino.
- En un incremental, **mandar algo afuera** puede ser también una decisión: cuántos se quedan
  para reproducirse y cuántos se van a producir otra cosa.

**Qué decide:** cómo se dimensiona la simulación ([`PA-002`](#pa-002-la-simulación-de-la-masa)).

---

## [PA-002] La simulación de la masa

**Planteado:** 2026-09-25. **Candidata**, sin decidir. Análisis en [`port3D.md`](port3D.md) §6.

**La opción que se analiza:** conejos como **datos** (array de structs, estado como enum, un
manager que los actualiza) dibujados con `MultiMeshInstance3D`.

**Lo que hay que contestar antes de escribirla:**

- **El puente dato -> nodo.** Qué pasa cuando algo necesita un conejo individual (el águila que lo
  agarra, un conejo que se clickea). Qué pasa al volver.
- **La animación por shader.** Cuánto se puede con datos por instancia y vertex shader: salto,
  bamboleo, crecimiento, orientación.
- **La detección de zanahorias.** Recorrido directo o grilla, según cuántas zanahorias haya a la vez.
- **Los límites.** Punto en polígono sobre XZ, o algo más barato si el área es simple.
- **Medir antes de decidir.** Un banco de prueba con 1.000 conejos en el repo nuevo contesta más
  que cualquier estimación.

---

## [PA-003] La cámara

**Planteado:** 2026-09-25. **Nada decidido.**

**Los ejes:**

- **Fija o viva.** El GDD pide que se aleje cuando crece la población (*hormiguero creciente*).
  ¿Es continua o por escalones, atada a los hitos?
- **Perspectiva u ortográfica.** La ortográfica lee como maqueta; la perspectiva da más
  profundidad al alejarse.
- **Cuánto la controla el jugador.** Nada, zoom, o rotar la maqueta.

**Qué depende de esto:** si el vuelo de la zanahoria tiene altura jugable
([`port3D.md`](port3D.md) §5.3), cómo se anuncia el águila, y cuánto se ve de la masa.

**Referencia:** la cámara de LBP (`docs/rules/camera-system.md` allá) es otro problema —sigue a
un personaje—, pero la deriva hacia el cursor medida en porcentaje de viewport es una idea que
puede servir.

---

## [PA-004] La economía: los hitos y el árbol de LBP

**Planteado:** 2026-09-25.

**Lo decidido:** la cantidad de conejos criados abre el árbol, y **el árbol es el de LBP**
(`SkillTreeManager` + el addon `skill_tree`), no `UpgradeTree` de este repo.

**Lo que hay que analizar:**

- **El costo del árbol de LBP se gasta** (conocimiento). Un hito de conejos **no se gasta**: los
  conejos siguen ahí. El repo 2D lo resolvía con `puntos = conejos - gastado`. ¿Se adopta eso, se
  pasa a hitos que desbloquean sin costo, o hay una moneda aparte que sí se gasta?
- **¿Sigue la caca?** El GDD tiene una segunda economía: caca que se vende al final del día por
  dinero, para una tienda. No se confirmó ni se descartó.
- **El motor de estadísticas** que alimenta el árbol: estadística con id, fuentes que suman o
  multiplican, recálculo al cambiar ([`port3D.md`](port3D.md) §5.4).

---

## [PA-005] La automatización

**Planteado:** 2026-09-25.

**Lo decidido:** parte del trabajo del jugador se automatiza. El primer ejemplo es **contratar
granjeros que lancen zanahorias**.

**Lo que hay que diseñar:**

- **Qué se automatiza y en qué orden**: cosechar, lanzar, repartir entre conejos, vender.
- **Qué le queda a la mano del jugador** cuando todo lo básico está automatizado. En un
  incremental bien hecho, el verbo del jugador cambia de escala; no desaparece.
- **Cómo se ve un granjero**: ¿es un actor en la granja, con cuerpo, o una estadística?

---

## [PA-006] La segunda amenaza

**Planteado:** 2026-09-25.

El águila sigue, y puede sumarse otra. Lo que conviene que tenga la nueva: una **forma distinta
de responderle** que la del águila (que se responde con una zanahoria a tiempo), y que se lea en
la maqueta 3D.

**Y lo que el águila todavía no hace:** según el GDD, se lleva al **despistado que quedó afuera**
de la casa. Hoy se lleva a cualquiera, incluso a uno dormido adentro.
