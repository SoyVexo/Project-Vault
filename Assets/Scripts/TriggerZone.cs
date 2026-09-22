using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. Imprime el nombre de CUALQUIER objeto que toque la zona
        Debug.Log($"[TRIGGER] ¡Algo entró en la zona! Objeto: {other.gameObject.name} | Tag: {other.tag}");

        // 2. Si es el jugador, le avisa al controlador
        if (other.CompareTag("Player"))
        {
            Debug.Log("[TRIGGER] ¡El objeto es el Player!");
            FindObjectOfType<LevelController>()?.OnJugadorLlegoAZona(gameObject);
        }
    }
}