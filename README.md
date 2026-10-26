<div align="center">

# 👽 Grox: Limpieza Espacial

### *Salvá Nimbus-072 recolectando y clasificando la basura espacial*

![Género](https://img.shields.io/badge/G%C3%A9nero-Educativo%20%7C%20Arcade%20%7C%20Puzzle-7C4DFF?style=for-the-badge)
![Plataforma](https://img.shields.io/badge/Versi%C3%B3n%20actual-Android-2E7D32?style=for-the-badge)
![PC](https://img.shields.io/badge/Porteo%20a%20PC-Propuesto-FFA000?style=for-the-badge)

**Grupo Gambitos** · Balcarce Rafael · Jiménez Jesús

</div>

---

## 📑 Índice

1. [Ficha del juego](#-actividad-1--ficha-del-juego)
2. [Análisis según género y plataforma](#-actividad-2--análisis-según-género-y-plataforma)
3. [Supuestos de móvil en el proyecto](#-actividad-3--supuestos-de-móvil-en-el-proyecto)
4. [Plataforma destino y decisiones de porteo](#-actividad-4--plataforma-destino-y-decisiones-de-porteo)

## 🏷️ Etiquetas de estado

| Etiqueta | Significado |
|---|---|
| ✅ **FUNCIONA** | Implementado y probado en Android |
| 🚧 **EN DESARROLLO** | Parcialmente implementado, aún no estable |
| 💡 **PROPUESTO** | Diseñado en este análisis, **no implementado** |
| 📏 **SIN MEDIR** | Falta tomar la medición o la evidencia |

> ⚠️ **Aclaración importante:** el juego está desarrollado y probado **en Android** (controles táctiles y giroscopio).
> El porteo a **PC** es solo una propuesta: en este trabajo no se implementa,
> por lo que todo lo referido a PC es **💡 PROPUESTO**.

---

# 🎮 Actividad 1 · Ficha del juego

| Campo | Contenido |
|---|---|
| **Título y género** | **Grox: Limpieza Espacial**. Género principal: educativo / arcade. Subgénero: puzzle interactivo de recolección y clasificación. |
| **Descripción** | Grox, un marciano pacífico, debe limpiar el planeta Nimbus-072, cuyo ecosistema está al borde del colapso por la basura espacial que cayó sobre él. El jugador recorre un entorno 3D capturando residuos con un rayo recolector y luego los clasifica (papel, plástico y vidrio) en su contenedor de color. **Se gana** recolectando la cantidad requerida dentro del tiempo límite y alcanzando 1050 puntos de clasificación. **Se pierde** si la basura se acumula y el indicador ambiental llega al límite, o si se acaba el tiempo sin llegar a los 1050 puntos. |
| **Mecánica principal y ciclo de juego** | Apuntar y capturar residuos con el rayo recolector. Ciclo completo: **explorar → apuntar → capturar → clasificar → sumar puntos**. |
| **Contenido y duración** | 2 niveles: **N1** Recolección e intercepción espacial (entorno 3D) y **N2** Clasificación y reciclaje gestual. Diseño previsto: 2 a 5 min por nivel. Duración de partida completa **medida**: `[completar con cronómetro: N1 __ min · N2 __ min]` 📏 |
| **Controles actuales (Android)** | Ver tabla de abajo. |
| **Pantalla** | `[completar: resolución y relación de aspecto de diseño, p. ej. 2400×1080 · 20:9]`. Orientación `[horizontal / vertical]`. 📏 |
| **Assets y licencias** | Ver tabla de abajo. |
| **Estado** | Ver tabla de abajo. |

### 📱 Controles actuales (Android)

| Acción | Control actual ✅ | PC (propuesto) 💡 |
|---|---|---|
| Mover a Grox y girar la cámara (N1) | Joystick táctil en pantalla | `W A S D` + mouse |
| Disparar el rayo / interactuar | Toque en pantalla | Clic izquierdo |
| Apuntar el residuo al contenedor (N2) | Inclinación del giroscopio | Movimiento del mouse |
| Pausa / menú | Botón en la esquina superior derecha | `Esc` (y botón en pantalla) |

### 🎨 Assets y licencias

| Tipo | Origen | Licencia | ¿Propio? |
|---|---|---|---|
| Modelos 3D / arte | `completar` | `completar` | `sí algunos prefabs` |
| Música y efectos de sonido | `freesound` | `libre de copyright` | ` no` |
| Fuentes tipográficas | `2 exo` | `Google Corporation` | ` no` |

### 🧭 Estado

| Parte | Estado |
|---|---|
| Nivel 1: Recolección con rayo (3D) | ✅ FUNCIONA (Android) |
| Nivel 2: Clasificación con giroscopio | ✅ FUNCIONA (Android) |
| Puntuación ecológica y feedback visual (verde/gris) | `[✅ / 🚧]` |
| Versión para PC (teclado y mouse) | 💡 PROPUESTO (no existe todavía) |

---

# 🔬 Actividad 2 · Análisis según género y plataforma

Plataforma destino analizada: **PC (Windows / Steam)**.

| Aspecto | Cómo está hoy (Android) | Qué exige el género | Problema en PC | Evidencia | Estado |
|---|---|---|---|---|---|
| **Orientación** | `[horizontal / vertical]` según el diseño actual. | Ver el entorno en el N1 y tener espacio para los contenedores en el N2. | El monitor es siempre horizontal: si el juego está pensado en vertical, hay que rediseñar la composición. El N2 usa el giroscopio, que no existe en PC (no se puede "inclinar" un monitor). | 📷 `[captura de la orientación actual]` | 📏 SIN MEDIR |
| **Relación de aspecto y escala** | Diseñado para un celular de `[__:__]`. | Que el HUD y la zona jugable se vean completos. | En 16:9 y 16:10 sobra o falta ancho respecto del celular. En 4:3 y en ultrawide 21:9 el HUD puede quedar pegado a bordes o estirado. Los botones táctiles se ven enormes en un monitor. | 📷 `[capturas en Game View con 16:9, 16:10, 4:3 y 21:9]` | 📏 SIN MEDIR |
| **Controles** | Joystick táctil + toque + giroscopio: se mueve, mira y dispara con los dedos. | N1: movimiento, cámara y disparo simultáneos. N2: apuntado fino con puntería. | El mouse apunta con más precisión que el dedo y el giroscopio, pero hay que decidir quién gira la cámara (mouse) y quién mueve (teclado). Se necesita bloquear y ocultar el cursor en el N1. Sin multitáctil, hay que reasignar acciones. | 🧪 `[prueba: manejar el N1 con teclado y mouse en un prototipo]` | 💡 PROPUESTO (traducción a teclado y mouse) |
| **HUD e interfaz** | Joystick y botón de acción en pantalla, botón de pausa arriba a la derecha, indicadores de puntos, tiempo y salud del planeta. | Info imprescindible: tiempo, puntaje, indicador ambiental y objetivo del nivel. | El joystick y el botón táctil sobran y deben ocultarse. Falta mostrar las teclas de ayuda. Los elementos pensados para la muesca y la zona segura del celular quedan mal ubicados. En TV (modo sofá) habría que respetar el *safe area* del borde. | 📷 `[captura del HUD actual y boceto adaptado]` | 💡 PROPUESTO |
| **Sesión** | `[__ min]` por nivel, pausa con botón en pantalla. | Partidas rápidas (2 a 5 min por nivel). | En PC la ventana puede perder el foco (Alt+Tab) sin que el juego se pause. Hay que definir si se guarda el progreso entre niveles. | 🧪 `[probar: qué pasa al perder el foco la ventana]` | 📏 SIN MEDIR |
| **Rendimiento** | **`[__] fps`** medidos en `[modelo de celular]`. | Respuesta fluida para que la captura y el apuntado resulten justos. | Si el juego fija un límite de fps pensando en el móvil, en PC quedaría limitado o cambiaría el comportamiento de la física. La calidad gráfica de móvil se verá baja en monitores grandes. En PC hay que revisar VSync y resolución. | 📷 `[captura del Profiler o contador de fps en el celular]` | 📏 SIN MEDIR |
| **Accesibilidad** | Textos e íconos de tamaño `[__]`. Los contenedores se distinguen por color. | Público escolar: texto legible y feedback claro. | Los contenedores de color (papel, plástico, vidrio) solo se distinguen por color: problema para daltonismo. Se debe permitir remapear teclas y ajustar la sensibilidad del mouse. | 🧪 `[prueba con simulador de daltonismo]` | 📏 SIN MEDIR |

**Alternativas de accesibilidad (💡 PROPUESTO):** agregar íconos o formas a cada contenedor además del color, sensibilidad ajustable para el apuntado con mouse, teclas remapeables y textos escalables.

---

# 🔍 Actividad 3 · Supuestos de móvil en el proyecto

> Las ubicaciones son hipótesis a **verificar con búsqueda en el código** (Ctrl+Shift+F sobre `Input.GetTouch`, `Input.gyro`, `Screen.`, `Application.`). Reemplazá `[archivo:línea]` con el dato real.

| # | Ubicación | Qué asume | Qué pasaría en PC |
|---|---|---|---|
| 1 | `[archivo:línea]` · lectura de `Input.touchCount` / `Input.GetTouch(0)` para disparar el rayo | Que hay una pantalla táctil. | En PC la acción nunca se ejecutaría: el clic del mouse no se lee como toque. |
| 2 | `[archivo:línea]` · joystick virtual en el Canvas para mover a Grox y la cámara (N1) | Que el jugador mueve con el pulgar sobre la pantalla. | El joystick no respondería al teclado; el personaje quedaría inmóvil. |
| 3 | `[archivo:línea]` · `Input.gyro.enabled` / `Input.gyro.attitude` para apuntar en el N2 | Que el dispositivo tiene giroscopio y se puede inclinar. | En PC no hay sensor: el N2 sería injugable sin un control alternativo (mouse). |
| 4 | `[archivo:línea]` · slider de **sensibilidad del giroscopio** del menú | Que el jugador regula la sensibilidad de un sensor. | La opción no tendría efecto; debe pasar a sensibilidad del mouse. |
| 5 | `[archivo:línea]` · botón de pausa en la esquina superior derecha | Que se toca con el pulgar. | Sin teclado asignado (`Esc`), solo se pausaría haciendo clic en un botón pequeño. |
| 6 | `Project Settings > Player > Resolution and Presentation` | Orientación y resolución de un celular (`[__×__]`). | Barras negras o recortes en 16:9, 16:10 y 21:9; ventana mal dimensionada. |
| 7 | `Canvas Scaler` del HUD (`[referencia de celular]`) | Una resolución de referencia de móvil y *safe area* para la muesca. | HUD desproporcionado, botones gigantes o elementos fuera de lugar en un monitor. |
| 8 | `[archivo:línea]` · `Application.targetFrameRate` (por ejemplo 30 o 60) | Que se limita el rendimiento para cuidar la batería. | En PC se limitarían los fps sin necesidad, o cambiaría la sensación del juego. |
| 9 | `[archivo:línea]` · `Screen.sleepTimeout = SleepTimeout.NeverSleep` y gestión de `OnApplicationPause` | Que el sistema puede suspender o interrumpir la app. | No aplica en PC: en cambio habría que gestionar la pérdida de foco de la ventana. |
| 10 | `Project Settings > Player` (Android) y `Quality` | Configuración y calidad gráfica de móvil (compresión de texturas, calidad baja). | El build debe regenerarse para Standalone y subir la calidad para monitores. |

---

# 🛠️ Actividad 4 · Plataforma destino y decisiones de porteo

> 💡 **Todo lo de esta actividad es PROPUESTO.** En este trabajo no se implementa el porteo.

## 1. Plataforma destino: **PC (Windows / Steam)** 💡

**Justificación:** el público son estudiantes de nivel escolar, y el proyecto se pensa para usarse en el aula: en las escuelas es muy común contar con computadoras de laboratorio, y un docente puede proyectar el juego para toda la clase. El género (partidas cortas, clasificación y puntería) funciona muy bien con mouse. Además, el GDD ya contempla a PC y Steam como plataforma.

## 2. Orientación, controles y HUD 💡

- **Orientación:** pantalla horizontal, relación base 16:9, que se adapta a 16:10 y 21:9 mediante un `Canvas Scaler` configurado en "Scale With Screen Size". Soporta ventana y pantalla completa.
- **Controles:** `W A S D` para mover, mouse para girar la cámara y apuntar, clic izquierdo para disparar, `Esc` para pausar. Se sugiere soporte opcional de gamepad.
- **HUD:** se eliminan el joystick y el botón de acción virtuales. Se muestran ayudas de teclas y el cursor visible solo en los menús.

```text
┌──────────────────────────────────────────────────────┐
│  🎯 Objetivo   ⏱ 01:24   🌍 [██████░░░░]   ⏸️ (Esc) │  ← HUD superior
│                                                      │
│                                                      │
│                   ZONA DE JUEGO                      │
│                       ✛  ← mira (mouse)              │
│                                                      │
│                                                      │
│  [W A S D] Mover      [Clic] Disparar     [Esc] Pausa│  ← ayuda de teclas
└──────────────────────────────────────────────────────┘
```

## 3. Plan de expansión 💡

Con 2 niveles de 2 a 5 minutos, la partida completa dura aproximadamente 4 a 10 minutos (`[ajustar con la duración medida]`), así que se considera **juego corto**. Se propone:

| Propuesta | Duración que suma | Por qué encaja en PC |
|---|---|---|
| **Nivel 3: Planta de Procesamiento** 🚧 *(actualmente en desarrollo)*. Cinta transportadora que traslada plásticos recolectados; salto preciso sobre la primera máquina para compactarlos y secuencia de 4 botones de colores para fabricar el objeto pedido (bolsa, botella, remera). 3 intentos fallidos como máximo. | +3 a 5 min | El teclado permite precisión de tiempo en el salto y las secuencias se ingresan rápido con 4 teclas. Además refuerza la economía circular, que es parte del triple impacto. |
| **Nuevos residuos** (metal y orgánico) en la clasificación | +2 a 3 min | Reutiliza la mecánica del Nivel 2 y amplía el contenido educativo. |
| **Modo "Desafío rápido"** (puntaje máximo en 2 minutos) | Rejugabilidad | Permite competir entre compañeros en el aula con ranking local. |

## 4. Registro de decisiones 💡

**Decisión 1: Esquema de movimiento y cámara**
- **Contexto:** en Android el joystick táctil mueve a Grox y rota la cámara; en PC existen dos dispositivos independientes.
- **Opciones:** (a) todo con teclado, (b) teclado para mover y mouse para la cámara, (c) gamepad como único control.
- **Decisión:** `W A S D` para mover y mouse para la cámara, con gamepad opcional.
- **Consecuencias:** es el estándar del género y da mayor precisión, pero hay que bloquear el cursor en el N1 y ofrecer sensibilidad ajustable.

**Decisión 2: Reemplazo del giroscopio en el Nivel 2**
- **Contexto:** el apuntado con giroscopio no existe en PC.
- **Opciones:** (a) mouse, (b) teclas de dirección, (c) arrastrar y soltar con el mouse.
- **Decisión:** apuntado con el mouse, con la sensibilidad en el menú (reemplaza el slider del giroscopio).
- **Consecuencias:** se conserva la lógica del apuntado y se gana precisión, aunque se pierde la sensación de "inclinar" el juego.

**Decisión 3: Interfaz y relación de aspecto**
- **Contexto:** el HUD está pensado para la pantalla y la muesca de un celular.
- **Opciones:** (a) fijar 16:9 con barras negras, (b) interfaz adaptable con anclas, (c) resolución fija en ventana.
- **Decisión:** interfaz adaptable con anclas y `Canvas Scaler`, retirando los controles táctiles.
- **Consecuencias:** se ve bien en distintos monitores, pero requiere probar varias resoluciones y reubicar todos los elementos del HUD.

**Decisión 4: Pausa y pérdida de foco**
- **Contexto:** en PC la ventana puede perder el foco sin aviso.
- **Opciones:** (a) ignorarlo, (b) pausar automáticamente, (c) preguntar al jugador.
- **Decisión:** pausa automática al perder el foco y `Esc` para pausar manualmente.
- **Consecuencias:** evita perder partidas en medio de una clase, pero hay que reanudar manualmente.

---

<div align="center">


