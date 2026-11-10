1. Actividad 1: Ficha del juego
1.1. Información general
Campo	Contenido
Título y género	Grox: Limpieza Espacial. Género principal: educativo / arcade. Subgénero: puzzle interactivo de recolección y clasificación de residuos.
Descripción	Grox, un marciano pacífico, debe limpiar el planeta Nimbus-072, cuyo ecosistema se encuentra en riesgo debido a la basura espacial que cayó sobre él. En el Nivel 1, el jugador recorre un entorno 3D en primera persona y captura residuos utilizando un rayo recolector. En el Nivel 2, debe clasificar cada residuo (papel, vidrio o plástico) en el contenedor correspondiente según su color.
Condiciones de victoria y derrota	Para ganar el Nivel 1, se debe alcanzar la meta de puntaje dentro de los 60 segundos disponibles. En el Nivel 2, se deben conseguir 1050 puntos en un máximo de 30 segundos. Si no se alcanza el objetivo antes de que termine el tiempo, se muestra la escena «Contaminación inminente» (E7).
Mecánica principal y ciclo de juego	Nivel 1: cinemática de la nave → exploración → apuntar → capturar residuos con el rayo → visualizar resultados → caminar hasta la casa para avanzar al siguiente nivel. Nivel 2: observar el residuo → identificar su tipo → dirigirlo al contenedor correspondiente → sumar puntos.
Sistema de puntuación	En el Nivel 2, cada acierto otorga 100 puntos. La pantalla de victoria muestra 16 aciertos y 1600 puntos (E6). Por lo tanto, para alcanzar los 1050 puntos necesarios se requieren al menos 11 aciertos sin errores (cálculo propio).
Contenido y duración	El juego cuenta con 2 niveles e incluye menú de inicio, selector de niveles, pausa, pantalla de resultados y pantalla de derrota (E1 a E8). Según la configuración del código, el Nivel 1 dispone de 60 segundos de generación de basura, más aproximadamente 5 segundos de cinemática. El Nivel 2 tiene una duración de 30 segundos (C7). Una partida completa tiene una duración estimada de aproximadamente 2 minutos. Pendiente de cronometraje en celular (UNKNOWN).
Controles actuales (Android)	Los controles táctiles y de movimiento se detallan en la sección 1.2.
Orientación de pantalla	Horizontal (apaisada), en pantalla completa y sin rotación (E1, E3 a E7). VERIFICADO. Las capturas realizadas en el celular tienen una resolución de 1600 × 702 píxeles (aproximadamente 2,28:1, cercana a 20:9). La resolución de referencia de la interfaz (Canvas Scaler) no fue verificada (UNKNOWN).
Assets y licencias	Los recursos gráficos, modelos 3D, música, efectos de sonido y fuentes tipográficas se detallan en la sección 1.3.
Estado general	El funcionamiento de las distintas partes del juego se detalla en la sección 1.4.
1.2. Controles actuales (Android)
Acción	Cómo se realiza actualmente
Mover a Grox (Nivel 1)	Se utiliza un joystick táctil ubicado en la parte inferior izquierda de la pantalla (E3). Alimenta una acción de movimiento del Input System (C5). VERIFICADO / APLICADO.
Girar la cámara (Nivel 1)	Se realiza arrastrando el dedo sobre la mitad derecha de la pantalla. También se utiliza el giroscopio del dispositivo (C4). APLICADO.
Apuntar y disparar (Nivel 1)	Se utiliza una mira fija ubicada en el centro de la pantalla (E3). Para disparar, se debe tocar la mitad derecha de la pantalla (C3). VERIFICADO / APLICADO.
Clasificar residuos (Nivel 2)	Se inclina el celular utilizando el acelerómetro (C2). También están implementadas las teclas A, W y D en el código (C1). APLICADO.
Pausa	Botón «Pause» ubicado en la esquina superior derecha de la pantalla. Está disponible únicamente en el Nivel 1 (E3, E8; C6). VERIFICADO. El Nivel 2 no cuenta actualmente con pausa.
Sensibilidad y movimiento	El control deslizante «Sens» ajusta únicamente la sensibilidad del giroscopio. La opción «Movement» permite activar o desactivar el joystick en pantalla (E8, C6). APLICADO.
Menús	La navegación se realiza tocando los botones correspondientes (E1, E2, E7). VERIFICADO.
1.3. Assets y licencias
Tipo de recurso	Origen / Autor	Licencia	¿Es propio?
Modelos 3D y arte	Flores Ruben	Derechos reservados / Uso autorizado para el proyecto.	Sí
Música y efectos de sonido	Jiménez Jesús Gabriel	Derechos reservados / Uso autorizado para el proyecto.	Sí
Fuentes tipográficas	Google Fonts	SIL Open Font License (OFL) 1.1.	No
1.4. Estado de las partes del juego
Parte o funcionalidad	Estado actual	Evidencia
Menú → selector de niveles → Nivel 1 → Nivel 2 → victoria o derrota	Funciona correctamente. VERIFICADO.	E1 a E7
Menú de pausa del Nivel 1	Funciona correctamente. VERIFICADO.	E8
Barra de progreso del Nivel 1 (ObjetivoUI)	Está implementada en el código, pero no se visualiza en las capturas E3 y E4. UNKNOWN.	—
Teclas A, W y D del Nivel 2	Están implementadas en el código, pero no cuentan con indicaciones visibles en pantalla. APLICADO.	C1
Pausa en el Nivel 2	No está implementada actualmente. VERIFICADO.	E5
Engranaje de opciones	Actualmente no tiene ninguna funcionalidad. VERIFICADO.	E1, E7
Versión para PC (teclado y mouse)	Todavía no está desarrollada. PROPUESTO.	—

Nota sobre los estados:

VERIFICADO: funcionalidad comprobada mediante pruebas o evidencias.
APLICADO: funcionalidad implementada en el código.
UNKNOWN: aspecto pendiente de comprobar o verificar.
PROPUESTO: funcionalidad planificada, pero todavía no implementada.
