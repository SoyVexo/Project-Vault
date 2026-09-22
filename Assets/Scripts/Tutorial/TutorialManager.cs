using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // 1. Referencias a tus scripts base (para leer sus variables)
    public PlayerController playerScript;
    public TextMeshProUGUI textoTutorial;
    public GameObject Door;
    public Image BlackPanel;
    public TextMeshProUGUI textLoading;
    private float DoorPostIni = 5.13f;
    private float DoorPostEnd = 13.01f;

    // 2. Sonidos de Pixabay
    public AudioSource fuenteAudio;
    public AudioClip sonidoClick;
    public AudioClip sonidoTurururu;
    public AudioClip DoorOpen;

    // 3. Control de pasos de la secuencia
    private int pasoActual = 0;
    private bool pasoCompletado = false;

    void Start()
    {
        // Texto inicial
        textoTutorial.text = "Usa el stick izquierdo para caminar.";
        BlackPanel.DOFade(0, 0);
        textLoading.DOFade(0, 0);
    }

    void Update()
    {
        switch (pasoActual)
        {
            case 0: // PASO 1: Caminar
                // Accedemos al Rigidbody del jugador para ver si se está moviendo físicamente
                if (playerScript.GetComponent<Rigidbody>().velocity.magnitude > 0.5f && !pasoCompletado)
                {
                    AvanceTutorial("¡Muy bien! Ahora mueve la cámara para mirar a tu alrededor.");
                    Door.transform.DOMoveY(DoorPostEnd, 2);
                    fuenteAudio.PlayOneShot(DoorOpen);
                }
                break;

            case 1: // PASO 2: Mirar (Comprobamos si tu variable de rotación cambia)
                // Usamos Input del ratón/stick directamente o una propiedad de tu script
                if (playerScript.GetComponent<PlayerInput>().actions["Look"].ReadValue<Vector2>().magnitude > 0.1)
                {
                    AvanceTutorial("Perfecto. Pulsa el botón para invertir la gravedad.");
                }
                break;

            case 2: // PASO 3: Invertir Gravedad
                // Leemos directamente tu variable booleana 'volteada' del PlayerController
                if (playerScript.volteada && !pasoCompletado)
                {

                    AvanceTutorial("Toca la palanca con X para voltear la habiracion");
                }
                break;
            case 3:
                if (true)
                {
                    pasoCompletado = true;

                    fuenteAudio.PlayOneShot(sonidoClick);

                    pasoActual = 4;
                    textoTutorial.text = "Has completado el tutorial felicidades";
                    StartCoroutine(TutorialCompletado());
                }
                break;

                
        }
    }

    void AvanceTutorial(string siguienteTexto)
    {
        pasoCompletado = true;

        // Suena el CLICK seco de Pixabay al cumplir la orden
        fuenteAudio.PlayOneShot(sonidoClick);

        // Actualizamos la interfaz y pasamos al siguiente caso
        textoTutorial.text = siguienteTexto;
        pasoActual++;
        pasoCompletado = false;
    }
    IEnumerator TutorialCompletado()
    {
        yield return new WaitForSeconds(1.75f);
        
        BlackPanel.DOFade(1, 1);
        textLoading.DOFade(1, 1);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("SampleScene");
    }
}
