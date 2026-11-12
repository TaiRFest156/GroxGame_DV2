using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Herramientas de menu para configurar el manual con un clic.
// Este archivo debe estar dentro de una carpeta llamada "Editor".
public static class ManualMenuTools
{
    private const string Carpeta = "Assets/Manual";
    private const string RutaAsset = "Assets/Manual/ManualGrox.asset";

    // ------------------------------------------------------------------ 1. datos
    [MenuItem("Grox/Manual/1. Crear datos del manual (contenido inicial)")]
    public static void CrearDatos()
    {
        ManualData data = AssetDatabase.LoadAssetAtPath<ManualData>(RutaAsset);

        if (data != null)
        {
            bool ok = EditorUtility.DisplayDialog("Manual de Grox",
                "Ya existe ManualGrox.\n\n¿Reemplazar su contenido por el texto inicial? " +
                "Perderás los cambios que hayas hecho en las páginas.",
                "Reemplazar", "Cancelar");
            if (!ok) return;
            data.paginas.Clear();
        }
        else
        {
            if (!AssetDatabase.IsValidFolder(Carpeta)) AssetDatabase.CreateFolder("Assets", "Manual");
            data = ScriptableObject.CreateInstance<ManualData>();
            AssetDatabase.CreateAsset(data, RutaAsset);
        }

        CargarContenidoInicial(data);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Selection.activeObject = data;
        EditorGUIUtility.PingObject(data);
        Debug.Log("Manual de Grox: datos creados en " + RutaAsset + " (" + data.paginas.Count + " páginas).");
    }

    // ------------------------------------------------------------------ 2 y 3. agregar a escena
    [MenuItem("Grox/Manual/2. Agregar manual a esta escena (NIVEL)")]
    public static void AgregarNivel() { Agregar(true); }

    [MenuItem("Grox/Manual/3. Agregar manual a esta escena (MENU / selector)")]
    public static void AgregarMenu() { Agregar(false); }

    private static void Agregar(bool esNivel)
    {
        ManualData datos = AssetDatabase.LoadAssetAtPath<ManualData>(RutaAsset);
        if (datos == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:ManualData");
            if (guids.Length > 0)
                datos = AssetDatabase.LoadAssetAtPath<ManualData>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (datos == null)
        {
            EditorUtility.DisplayDialog("Manual de Grox",
                "Primero crea los datos: menú Grox > Manual > 1. Crear datos del manual.", "OK");
            return;
        }

        ManualSystem existente = Object.FindFirstObjectByType<ManualSystem>();
        if (existente != null)
        {
            Selection.activeGameObject = existente.gameObject;
            EditorUtility.DisplayDialog("Manual de Grox", "Esta escena ya tiene un ManualSystem.", "OK");
            return;
        }

        GameObject go = new GameObject("ManualSystem");
        Undo.RegisterCreatedObjectUndo(go, "Agregar ManualSystem");

        ManualSystem ms = go.AddComponent<ManualSystem>();
        ms.datos = datos;
        ms.introAutomatica = esNivel;
        ms.crearBotonFlotante = esNivel;

        if (esNivel)
        {
            // Pagina inicial segun el nombre de la escena (Nivel_1, Nivel_2, Nivel_3)
            string n = go.scene.name;
            if (n.EndsWith("1")) ms.paginaInicial = 5;
            else if (n.EndsWith("2"))
            {
                ms.paginaInicial = 7;
                ms.esperaInicial = 4.5f;   // el Nivel 2 tiene un fundido de pantalla de ~4 s
            }
            else if (n.EndsWith("3")) ms.paginaInicial = 9;
        }

        Selection.activeGameObject = go;
        EditorSceneManager.MarkSceneDirty(go.scene);
    }

    // ------------------------------------------------------------------ contenido inicial
    private static ManualPage P(string etiqueta, string titulo, string texto)
    {
        return new ManualPage { etiqueta = etiqueta, titulo = titulo, texto = texto };
    }

    private static void CargarContenidoInicial(ManualData d)
    {
        // Paginas 1-2
        d.paginas.Add(P("Bienvenida", "Manual Intergaláctico de Reciclaje",
            "Hola, Grox. Este libro te enseña todo lo que necesitas para limpiar Nimbus-072: " +
            "cómo moverte, cómo jugar cada nivel y, lo más importante, cómo reciclar de verdad.\n\n" +
            "Pasa las hojas con las flechas o deslizando el dedo.\n\n" +
            "Puedes abrir este manual cuando quieras con el botón del libro."));

        d.paginas.Add(P("Historia", "Nimbus-072 necesita ayuda",
            "En un futuro lejano, la humanidad produjo tanta basura que decidió mandarla al espacio. " +
            "Por accidente, los desechos cayeron sobre Nimbus-072, un planeta pacífico.\n\n" +
            "Grox, un marciano curioso y responsable, es el encargado de limpiarlo con su " +
            "<b>Pistola de Rayo Recolector</b> antes de que el ecosistema colapse."));

        // Paginas 3-4
        d.paginas.Add(P("Controles", "Moverte y recolectar",
            "<b>Joystick táctil:</b> a la izquierda de la pantalla, para mover a Grox.\n\n" +
            "<b>Apuntar:</b> dirige la cámara hacia la basura. El rayo sale justo del centro de la pantalla.\n\n" +
            "<b>Tocar la pantalla:</b> toca la mitad derecha para disparar el Rayo Recolector."));

        d.paginas.Add(P("Controles", "Inclinar, combinar y pausar",
            "<b>Nivel 2:</b> inclina el celular hacia la izquierda, la derecha o arriba para lanzar cada residuo a su contenedor.\n\n" +
            "<b>Nivel 3:</b> toca los 4 botones de colores para fabricar objetos.\n\n" +
            "<b>Pausa:</b> botón de la esquina superior derecha.\n" +
            "<b>Manual:</b> el icono del libro."));

        // Paginas 5-6 (Nivel 1)
        d.paginas.Add(P("Nivel 1", "Rayo Recolector",
            "La basura cae sobre el planeta. ¡Atrápala antes de que lo contamine!\n\n" +
            "<b>Cómo ganar:</b> recolecta suficiente basura antes de que se acabe el tiempo y llena la barra de objetivo.\n\n" +
            "<b>Cómo perder:</b> si el tiempo termina y no llegaste a la meta, Nimbus-072 se contamina."));

        d.paginas.Add(P("Nivel 1", "Consejos del Nivel 1",
            "Apunta con calma: el rayo necesita un breve descanso entre disparos.\n\n" +
            "No todos los residuos valen lo mismo: algunos dan más puntos.\n\n" +
            "Mira el color del planeta: <b>verde</b> es limpio y <b>gris</b> es contaminado."));

        // Paginas 7-8 (Nivel 2)
        d.paginas.Add(P("Nivel 2", "Clasificación gestual",
            "Cada residuo que aparece debe ir a su contenedor.\n\n" +
            "<b>Inclina a la izquierda:</b> papel.\n" +
            "<b>Inclina a la derecha:</b> plástico.\n" +
            "<b>Inclina hacia arriba:</b> vidrio.\n\n" +
            "<b>Cómo ganar:</b> llega a 1050 puntos antes de que termine el tiempo."));

        d.paginas.Add(P("Nivel 2", "Consejos del Nivel 2",
            "Mira bien el residuo antes de inclinar: un error puede restarte puntos.\n\n" +
            "<b>Cuidado:</b> si fallas 4 veces seguidas, pierdes el nivel.\n\n" +
            "Si al terminar el tiempo no llegaste a 1050 puntos, tendrás que intentarlo otra vez."));

        // Paginas 9-10 (Nivel 3)
        d.paginas.Add(P("Nivel 3", "La cinta transportadora",
            "Una cinta lleva plásticos reciclados, desordenados, de izquierda a derecha.\n\n" +
            "<b>Salta</b> sobre la primera máquina en el momento justo para convertir la pila en cubos de plástico compacto.\n\n" +
            "Cuidado: la cinta se acelera poco a poco."));

        d.paginas.Add(P("Nivel 3", "La máquina de fabricar",
            "Los cubos llegan a la segunda máquina, donde Grox se teletransporta. La pantalla pide un objeto: una bolsa, una botella, una remera...\n\n" +
            "Ingresa la secuencia correcta con los 4 botones de colores.\n\n" +
            "<b>Tienes 3 vidas.</b> Pierdes si fallas 3 veces o si el material llega al final de la cinta sin procesar."));

        // Paginas 11-12
        d.paginas.Add(P("Reciclaje", "Los residuos de Nimbus",
            "En tu aventura encontrarás residuos de varios tipos:\n\n" +
            "<b>Papel y cartón</b>, <b>plástico</b> y <b>vidrio</b>: se pueden reciclar.\n" +
            "<b>Metal</b> (como las latas): también se recicla.\n" +
            "<b>Orgánicos</b> (restos de comida): pueden convertirse en compost.\n" +
            "<b>Tóxicos</b> (como las pilas): necesitan un cuidado especial."));

        d.paginas.Add(P("Reciclaje · Nivel 1", "Recoger y no ensuciar",
            "En el Nivel 1 limpias el planeta. En la vida real, la basura tirada en el suelo llega a alcantarillas, ríos y mares, y daña a los animales.\n\n" +
            "Se estima que un papel tarda semanas o meses en desaparecer, una botella de plástico cientos de años y el vidrio miles.\n\n" +
            "<b>Siempre usa un cesto.</b>"));

        // Paginas 13-14
        d.paginas.Add(P("Reciclaje · Nivel 2", "Separar bien",
            "En el Nivel 2 clasificas. En casa también:\n\n" +
            "<b>Papel y cartón:</b> limpios y secos, sin grasa.\n" +
            "<b>Plástico:</b> envases vacíos, enjuagados y aplastados.\n" +
            "<b>Vidrio:</b> botellas y frascos vacíos. ¡Se puede reciclar muchas veces!\n" +
            "<b>Pilas:</b> llévalas a un punto de recolección.\n\n" +
            "Los colores de los contenedores cambian según la ciudad: pregunta en tu municipio."));

        d.paginas.Add(P("Reciclaje · Nivel 3", "De residuo a recurso",
            "En el Nivel 3 el plástico se compacta y se transforma en objetos nuevos: bolsas, prendas, envases. " +
            "Eso se llama <b>economía circular</b>: lo que ya usamos vuelve a ser útil en vez de terminar como basura.\n\n" +
            "Reciclar ahorra materias primas y energía, y crea trabajo. Separar bien ayuda a quienes clasifican los residuos."));

        // Paginas 15-16
        d.paginas.Add(P("Reciclaje", "Las 3R",
            "<b>Reducir:</b> usa menos cosas descartables. Lleva tu propia botella y tu bolsa.\n\n" +
            "<b>Reutilizar:</b> dale una segunda vida a frascos, cajas y ropa.\n\n" +
            "<b>Reciclar:</b> separa lo que ya no puedes reutilizar.\n\n" +
            "El mejor residuo es el que nunca se genera."));

        d.paginas.Add(P("¡Tu turno!", "Ahora te toca a ti",
            "Grox ya limpió Nimbus-072, pero la Tierra también cuenta contigo.\n\n" +
            "1. Ten un cesto para reciclables y otro para el resto.\n" +
            "2. Enjuaga y aplasta los envases.\n" +
            "3. Lleva tu propia bolsa.\n" +
            "4. Cuéntale a alguien lo que aprendiste.\n\n" +
            "Puedes volver a leer este manual cuando quieras."));
    }
}
