using UnityEngine;
using DG.Tweening;

/// <summary>
/// Cubo de seguridad que se activa junto con la palanca: aparece con un
/// fade de visibilidad gradual y su colisión se controla por separado,
/// para poder sacarla justo cuando termina de rotar la habitación
/// (y no antes, para evitar que el jugador se caiga a mitad de la animación).
/// </summary>
public class SafetyFloor : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si lo dejás vacío, busca automáticamente los Renderer en los hijos.")]
    public Renderer[] renderers;
    [Tooltip("Si lo dejás vacío, busca automáticamente un Collider en este mismo objeto.")]
    public Collider colisionador;

    [Header("Configuración")]
    [Tooltip("Duración del fade de aparición/desaparición, en segundos.")]
    public float duracionFade = 1f;

    private Tween fadeTween;

    private void Awake()
    {
        if (colisionador == null)
        {
            colisionador = GetComponent<Collider>();
        }

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
    }

    /// <summary>
    /// Fuerza el estado del piso de seguridad de forma INSTANTÁNEA (sin fade),
    /// para usar al arrancar la escena y que quede sincronizado con el estado
    /// real de la habitación (MapRoating.volteada), sin importar cómo haya
    /// quedado tildado el GameObject en el Inspector.
    /// </summary>
    public void SincronizarEstadoInicial(bool debeEstarActivo)
    {
        fadeTween?.Kill();

        gameObject.SetActive(debeEstarActivo);

        if (debeEstarActivo)
        {
            if (colisionador != null)
            {
                colisionador.enabled = true;
            }
            AplicarAlphaInstantaneo(1f);
        }
        else
        {
            if (colisionador != null)
            {
                colisionador.enabled = false;
            }
            AplicarAlphaInstantaneo(0f);
        }
    }

    /// <summary>
    /// Activa el cubo: lo habilita, pone la colisión ON de inmediato
    /// y sube la visibilidad (alpha) de forma gradual.
    /// </summary>
    public void Activar()
    {
        gameObject.SetActive(true);

        if (colisionador != null)
        {
            colisionador.enabled = true;
        }

        fadeTween?.Kill();
        AplicarAlphaInstantaneo(0f);
        fadeTween = DOTween.To(() => 0f, AplicarAlphaInstantaneo, 1f, duracionFade);
    }

    /// <summary>
    /// Baja la visibilidad de forma gradual (inversa a Activar).
    /// OJO: esto NO toca el GameObject ni la colisión. Esas se apagan
    /// aparte con QuitarColision(), llamado cuando termine de rotar
    /// la habitación, para que la colisión dure TODA la rotación.
    /// </summary>
    public void Desactivar()
    {
        fadeTween?.Kill();
        float alphaActual = ObtenerAlphaActual();
        fadeTween = DOTween.To(() => alphaActual, AplicarAlphaInstantaneo, 0f, duracionFade);
    }

    /// <summary>
    /// Saca la colisión del cubo y recién ahí apaga el GameObject entero.
    /// Llamalo justo cuando termina de rotar la habitación, no antes:
    /// para ese momento el fade visual (más corto que la rotación) ya
    /// terminó hace rato, así que es seguro apagar todo de una.
    /// </summary>
    public void QuitarColision()
    {
        fadeTween?.Kill();

        if (colisionador != null)
        {
            colisionador.enabled = false;
        }

        gameObject.SetActive(false);
    }

    private void AplicarAlphaInstantaneo(float alpha)
    {
        if (renderers == null) return;

        foreach (var r in renderers)
        {
            if (r == null) continue;

            // .material crea una instancia propia (no afecta a otros objetos
            // que compartan el material original).
            Color c = r.material.color;
            c.a = alpha;
            r.material.color = c;
        }
    }

    private float ObtenerAlphaActual()
    {
        if (renderers != null && renderers.Length > 0 && renderers[0] != null)
        {
            return renderers[0].material.color.a;
        }
        return 1f;
    }
}