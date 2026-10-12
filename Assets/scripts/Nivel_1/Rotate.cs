using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float velocidadRotacion = 100f;
    private bool tocoSuelo = false;
    private Vector3 direccionRotacion;

    void Start()
    {
        // Genera una dirección de rotación aleatoria normalizada
        direccionRotacion = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void Update()
    {
        if (!tocoSuelo)
        {
            transform.Rotate(direccionRotacion * velocidadRotacion * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        tocoSuelo = true;
    }
}
