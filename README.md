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


