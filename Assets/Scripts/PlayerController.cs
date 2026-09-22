using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput pi;

    [Header("Cámara")]
    public Camera camara;
    public GameObject cameraReference;
    public float sencibilidad = 10f;
    private float CamRotationY;

    [Header("Movimiento y Gravedad Personal")]
    public float speed = 8.5f;
    public float gravedad = 9.81f;
    public bool volteada;

    private LevelController levelController;
    private bool palancaCol;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pi = GetComponent<PlayerInput>();

        rb.useGravity = false;
        Application.targetFrameRate = 60;

        levelController = FindObjectOfType<LevelController>();
    }

    private void Update()
    {
        DetectarPalanca();

        ControlCamera();
        ControlMovement();

        // RB: Cambiar gravedad del personaje
        if (pi.actions["InvertPlayer"].WasPressedThisFrame())
        {
            InvertirGravedadPropia();
        }

        // Interacción con elementos del Stage actual
        if (palancaCol && pi.actions["Interact"].WasPressedThisFrame())
        {
            if (levelController != null)
            {
                levelController.ActivarPalanca(transform);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravedad * rb.mass);
    }

    private void DetectarPalanca()
    {
        if (camara == null || levelController == null || levelController.CurrentStage == null) return;

        StageConfig config = levelController.CurrentStage;

        bool colisionPrevia = palancaCol;

        // Usa la distancia y la LayerMask definidas en la config del Stage
        palancaCol = Physics.Raycast(
            camara.transform.position,
            camara.transform.forward,
            config.interactDistance,
            config.palancaLayer
        );

        Debug.DrawLine(
            camara.transform.position,
            camara.transform.position + (camara.transform.forward * config.interactDistance),
            palancaCol ? Color.green : Color.red
        );

        if (colisionPrevia != palancaCol)
        {
            levelController.SetInteractUIVisible(palancaCol);
        }
    }

    private void InvertirGravedadPropia()
    {
        volteada = !volteada;
        float angulo = volteada ? 180f : 0f;

        gravedad = -gravedad;
        transform.DORotate(new Vector3(0, 0, angulo), 0.8f);
    }

    private void ControlCamera()
    {
        Vector2 cameraInput = pi.actions["Look"].ReadValue<Vector2>();

        transform.Rotate(Vector3.up * cameraInput.x * sencibilidad * Time.deltaTime);

        CamRotationY -= cameraInput.y * sencibilidad * Time.deltaTime;
        CamRotationY = Mathf.Clamp(CamRotationY, -80, 80);

        if (camara != null && cameraReference != null)
        {
            camara.transform.position = cameraReference.transform.position;
            camara.transform.rotation = transform.rotation * Quaternion.Euler(CamRotationY, 0f, 0f);
        }
    }

    private void ControlMovement()
    {
        Vector2 movInput = pi.actions["Move"].ReadValue<Vector2>();

        Vector3 adelante = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 derecha = new Vector3(transform.right.x, 0f, transform.right.z).normalized;

        Vector3 direccion = adelante * movInput.y + derecha * movInput.x;

        rb.velocity = new Vector3(direccion.x * speed, rb.velocity.y, direccion.z * speed);
    }
}