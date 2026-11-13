using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Herramientas de menu para armar el Nivel 3 con un par de clics.
// Este archivo debe estar dentro de una carpeta llamada "Editor".
public static class Nivel3Tools
{
    // ------------------------------------------------------------------ 1. crear todo
    [MenuItem("Grox/Nivel 3/1. Crear sistema y objetos base en esta escena")]
    public static void Crear()
    {
        if (Object.FindFirstObjectByType<Nivel3Manager>() != null)
        {
            EditorUtility.DisplayDialog("Nivel 3", "Esta escena ya tiene un Nivel3Manager.", "OK");
            return;
        }

        LeverMachine palanca = Object.FindFirstObjectByType<LeverMachine>();
        ObjectSpawner spawner = Object.FindFirstObjectByType<ObjectSpawner>();
        Vector3 origen = palanca != null ? palanca.transform.position : Vector3.zero;

        // ---- Sistema (scripts)
        GameObject sistema = new GameObject("Nivel3_Sistema");
        Undo.RegisterCreatedObjectUndo(sistema, "Crear Nivel 3");
        sistema.transform.position = origen;
        Nivel3Manager manager = sistema.AddComponent<Nivel3Manager>();
        SimonWall simon = sistema.AddComponent<SimonWall>();
        ProductLine linea = sistema.AddComponent<ProductLine>();
        sistema.AddComponent<Nivel3HUD>();

        // ---- Objetos base (para mover a su lugar)
        GameObject props = new GameObject("Nivel3_Props");
        props.transform.SetParent(sistema.transform, false);
        props.transform.position = origen;

        // Pared de colores: 6 cuadros en 2 filas de 3
        Transform pared = Vacio("Pared_Colores", props.transform, new Vector3(0f, 0f, 6f));
        Renderer[] cuadros = new Renderer[6];
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = new Vector3(-1.2f + (i % 3) * 1.2f, 2.6f - (i / 3) * 1.0f, 0f);
            cuadros[i] = Pieza("Cuadro_" + (i + 1), pared, pos, new Vector3(0.8f, 0.8f, 0.12f)).GetComponent<Renderer>();
        }

        // Placas del piso: 6 en 2 filas de 3
        Transform pisoRoot = Vacio("Placas_Piso", props.transform, new Vector3(0f, 0f, 2.5f));
        Transform[] placas = new Transform[6];
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = new Vector3(-2f + (i % 3) * 2f, 0.03f, -(i / 3) * 2f);
            placas[i] = Pieza("Placa_" + (i + 1), pisoRoot, pos, new Vector3(1.3f, 0.05f, 1.3f)).transform;
        }

        // Donde aparece el jugador al teletransportarse (mira hacia +Z, hacia la pared)
        Transform destino = Vacio("Destino_Simon", props.transform, new Vector3(0f, 0f, -1.5f));

        // Cinta 2 y estanteria
        Transform c2i = Vacio("Cinta2_Inicio", props.transform, new Vector3(2f, 1f, 0f));
        Transform c2f = Vacio("Cinta2_Fin", props.transform, new Vector3(4f, 1f, 0f));
        Transform[] slots = new Transform[3];
        for (int i = 0; i < 3; i++)
            slots[i] = Pieza("Estanteria_Slot_" + (i + 1), props.transform,
                             new Vector3(-1f + i, 1.2f, -3f), new Vector3(0.5f, 0.05f, 0.5f)).transform;

        // ---- Conexiones
        simon.cuadrosPared = cuadros;
        simon.placas = placas;
        linea.inicio = c2i;
        linea.fin = c2f;
        linea.slotsEstanteria = slots;

        manager.palanca = palanca;
        manager.spawnerCubos = spawner;
        manager.simon = simon;
        manager.linea = linea;
        manager.destinoSimon = destino;
        manager.hud = sistema.GetComponent<Nivel3HUD>();
        manager.lucesDelCuarto = LucesDeLaEscena(sistema);

        Selection.activeGameObject = props;
        EditorGUIUtility.PingObject(props);
        EditorSceneManager.MarkSceneDirty(sistema.scene);

        string aviso = "Listo. Creé 'Nivel3_Sistema' con todo conectado.\n\n";
        if (palanca == null) aviso += "- No encontré LeverMachine en la escena.\n";
        if (spawner == null) aviso += "- No encontré ObjectSpawner en la escena.\n";
        aviso += "\nSiguiente: mueve 'Nivel3_Props' (o cada pieza) a su lugar en el cuarto y " +
                 "usa el menú 2 para convertir tu teleport.";
        Debug.Log("Nivel 3: " + aviso);
    }

    // ------------------------------------------------------------------ 2. teleport
    [MenuItem("Grox/Nivel 3/2. Convertir objeto seleccionado en Teleport")]
    public static void ConvertirEnTeleport()
    {
        GameObject sel = Selection.activeGameObject;
        if (sel == null)
        {
            EditorUtility.DisplayDialog("Nivel 3", "Selecciona primero tu objeto de teleport en la Hierarchy.", "OK");
            return;
        }

        TeleportPad pad = sel.GetComponent<TeleportPad>();
        if (pad == null) pad = Undo.AddComponent<TeleportPad>(sel);

        Nivel3Manager manager = Object.FindFirstObjectByType<Nivel3Manager>();
        if (manager != null)
        {
            Undo.RecordObject(manager, "Asignar teleport");
            manager.teleport = pad;
            EditorUtility.SetDirty(manager);
        }
        else
        {
            Debug.LogWarning("Nivel 3: no hay Nivel3Manager todavía. Usa primero el menú 1.");
        }

        EditorSceneManager.MarkSceneDirty(sel.scene);
    }

    // ------------------------------------------------------------------ ayudas
    private static Transform Vacio(string nombre, Transform padre, Vector3 localPos)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.transform.localPosition = localPos;
        return go.transform;
    }

    private static GameObject Pieza(string nombre, Transform padre, Vector3 localPos, Vector3 escala)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = nombre;
        go.transform.SetParent(padre, false);
        go.transform.localPosition = localPos;
        go.transform.localScale = escala;
        Collider col = go.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);   // que no frene al jugador
        return go;
    }

    private static Light[] LucesDeLaEscena(GameObject excluir)
    {
        Light[] todas = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        System.Collections.Generic.List<Light> lista = new System.Collections.Generic.List<Light>();
        foreach (Light l in todas)
            if (!l.transform.IsChildOf(excluir.transform)) lista.Add(l);
        return lista.ToArray();
    }
}
