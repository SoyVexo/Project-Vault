using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MapRoating : MonoBehaviour
{
    public enum EjeRotacion { X, Y, Z }

    public GameObject map;
    PlayerInput pi;

    public bool volteada;

    [Header("Eje de giro")]
    [Tooltip("Coordenada sobre la que va a rotar la habitación.")]
    public EjeRotacion ejeDeGiro = EjeRotacion.Z;

    [Header("Piso de seguridad")]
    [Tooltip("Cubo extra que tapa el hueco mientras la habitación está volteada. Opcional.")]
    public SafetyFloor pisoSeguridad;

    void Start()
    {
        pi = GetComponent<PlayerInput>();

        if (pi == null)
        {
            Debug.LogWarning($"[MapRoating] '{gameObject.name}' no tiene un componente PlayerInput asignado. La tecla de test para invertir el mapa no funcionará en este objeto.");
        }

        // Sincroniza el piso de seguridad con el estado real de la habitación
        // al arrancar la escena, sin importar cómo haya quedado tildado en el Inspector.
        if (pisoSeguridad != null)
        {
            pisoSeguridad.SincronizarEstadoInicial(volteada);
        }
    }
    void Update()
    {
        // Si no hay PlayerInput (o no tiene la acción configurada), no hacemos nada
        // en vez de tirar NullReferenceException.
        if (pi == null || pi.actions == null) return;

        if (pi.actions["InvertMap(Testing)"].WasPressedThisFrame())
        {
            vuelta();
        }
    }
    public void vuelta()
    {
        float angulo = volteada ? 0 : 180f;

        Vector3 rotacionObjetivo = ObtenerRotacionObjetivo(angulo);

        // true si esta rotación nos lleva HACIA el estado volteado,
        // false si nos está devolviendo al estado original.
        bool vaHaciaVolteado = !volteada;

        if (pisoSeguridad != null)
        {
            if (vaHaciaVolteado)
            {
                // Aparece con fade in y la colisión se activa YA MISMO,
                // para cubrir el hueco durante toda la rotación.
                pisoSeguridad.Activar();
            }
            else
            {
                // Desaparece con fade out (a la inversa), pero la colisión
                // se mantiene activa hasta que termine de rotar.
                pisoSeguridad.Desactivar();
            }
        }

        Tween rotacion = map.transform.DORotate(rotacionObjetivo, 5);

        rotacion.OnComplete(() =>
        {
            // Solo sacamos la colisión cuando termina de rotar Y estamos
            // volviendo al estado original, tal como se pidió.
            if (pisoSeguridad != null && !vaHaciaVolteado)
            {
                pisoSeguridad.QuitarColision();
            }
        });

        volteada = !volteada;
    }

    private Vector3 ObtenerRotacionObjetivo(float angulo)
    {
        switch (ejeDeGiro)
        {
            case EjeRotacion.X:
                return new Vector3(angulo, 0, 0);
            case EjeRotacion.Y:
                return new Vector3(0, angulo, 0);
            case EjeRotacion.Z:
            default:
                return new Vector3(0, 0, angulo);
        }
    }
}