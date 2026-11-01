using UnityEngine;

public class DetectorCasa : MonoBehaviour
{
    public CasaNivel casaNivel;

    private void OnTriggerEnter(Collider other)
    {
        if (casaNivel == null)
        {
            Debug.LogError("DetectorCasa: Falta asignar CasaNivel.");
            return;
        }

        Debug.Log("DetectorCasa: Entró " + other.name);

        casaNivel.ProcesarEntrada(other);
    }
}