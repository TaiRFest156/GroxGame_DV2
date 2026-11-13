using UnityEngine.SceneManagement;

public static class LoadingSceneData
{
    public static string escenaDestino;

    public static void Cargar(string nombreEscena)
    {
        escenaDestino = nombreEscena;
        SceneManager.LoadScene("LoadingScene");
    }
}