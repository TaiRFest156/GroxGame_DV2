using System.Collections;         
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Llamás este método cuando tu spawner termina
    public void OnSpawnerFinished()
    {
        StartCoroutine(CargarEscenaDespuesDeTiempo(10f, "testNivel2"));
    }

    private IEnumerator CargarEscenaDespuesDeTiempo(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
