using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Button : MonoBehaviour
{
    public bool IsPressed;

    private float InicialAlture;
    private float pressed;
    private float UnPressed;
    public int ObjetEncim;

    private void Start()
    {
        InicialAlture = transform.position.y;
        UnPressed = InicialAlture;
        pressed = UnPressed - 0.2f;
    }

    // Es mucho mejor usar OnTriggerEnter/Exit para lanzar la animación UNA SOLA VEZ
    // en lugar de comprobarlo frames por segundo en el FixedUpdate.
    public void OnTriggerEnter(Collider other)
    {
        ObjetEncim++;
        ComprobarEstadoBoton();
    }

    public void OnTriggerExit(Collider other)
    {
        ObjetEncim--;
        // Por seguridad, evitamos que baje de 0 por si ocurre un bug físico raro
        if (ObjetEncim < 0) ObjetEncim = 0;

        ComprobarEstadoBoton();
    }

    private void ComprobarEstadoBoton()
    {
        // SI HAY OBJETOS ENCIMA y el botón NO estaba presionado aún...
        if (ObjetEncim > 0 && !IsPressed)
        {
            IsPressed = true;
            // .DOTween necesita un .DOKill() antes para detener animaciones previas si se solapan
            transform.DOKill();
            transform.DOLocalMoveY(-0.054f, 0.3f).SetEase(Ease.OutQuad);
        }
        // SI NO QUEDA NINGÚN OBJETO y el botón estaba presionado...
        else if (ObjetEncim == 0 && IsPressed)
        {
            IsPressed = false;
            transform.DOKill();
            transform.DOLocalMoveY(0.232f, 0.3f).SetEase(Ease.OutQuad);
        }
    }
}