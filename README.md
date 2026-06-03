# Clase 17/18 - Roulette
**Alumno:** Federico Bazán
**Clase:** 17 y 18 (Funciones Matemáticas y Funciones Útiles)

---

## Descripción

Juego de ruleta de casino con vista cenital. El jugador presiona un botón para girar la ruleta y según el sector donde caiga el indicador, sus monedas se modifican de distintas formas. La ruleta y las fichas fueron modeladas en Blender y exportadas como FBX.

---

## Uso

- **GIRAR** — Gira la ruleta y aplica el resultado al contador de monedas.
- **SALIR** — Cierra el juego.
- El texto central muestra el sector activo en tiempo real mientras la ruleta gira.
- El resultado final aparece cuando la ruleta se detiene.

---

## Sectores

0 - Verde - JACKPOT (Monedas X3)
1 - Rojo - +10 monedas
2 - Negro - (-10) monedas
3 - Rojo - +25 monedas
4 - Negro - (-25) monedas
5 - Rojo - Monedas x2
6 - Negro - Monedas %2
7 - Amarillo - Perdiste (Monedas = =0)

---

## Scripts

- **MonedaManager.cs** — Maneja el contador de monedas del jugador.
- **GirarRuleta.cs** — Controla el giro de la ruleta.

---