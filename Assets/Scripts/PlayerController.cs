using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput pi;

    public Camera camara;
    public GameObject cameraReference;
    private float CamRotationY;

    public float sencibilidad = 10;
    public bool volteada;
    public float speed = 8.5f;

    public float gravedad = 9.81f;

    public bool palancaCol;
    private MapRoating map;
    public GameObject InteractPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pi = GetComponent<PlayerInput>();
        map = GetComponent<MapRoating>();
        rb.useGravity = false;
        InteractPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        palancaCol = Physics.Raycast(camara.transform.position, camara.transform.forward, 2f, LayerMask.GetMask("Palanca"));
        Debug.DrawLine(camara.transform.position, camara.transform.position + (camara.transform.forward * 2f), palancaCol ? Color.green : Color.red);


        Camera();
        MOV();

        

        if (pi.actions["InvertPlayer"].WasPressedThisFrame())
        {
            float angulo = volteada ? 0 : 180f;

            gravedad = gravedad * -1;
            transform.DORotate(new Vector3(0, 0, angulo), 0.8f);

            if (volteada)
            {
                volteada = false;
            }
            else
            {
                volteada = true;
            }

        }
        if (palancaCol) {InteractPanel.SetActive(true);} else { InteractPanel.SetActive(false); }
        if (palancaCol && pi.actions["Interact"].WasPressedThisFrame())
        {
            map.vuelta();
            transform.DOMove(new Vector3(0, transform.position.y, 0), 1f);
        }

    }
    
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravedad * rb.mass);
    }
    void Camera()
    {
        Vector2 CameraInput = pi.actions["Look"].ReadValue<Vector2>();
        float hor = CameraInput.x;
        float vert = CameraInput.y;

        transform.Rotate(Vector3.up * hor * sencibilidad * Time.deltaTime);

        CamRotationY -= vert * sencibilidad * Time.deltaTime;
        CamRotationY = Mathf.Clamp(CamRotationY, -80, 80);
        

        camara.transform.position = cameraReference.transform.position;
        camara.transform.rotation = transform.rotation * Quaternion.Euler(CamRotationY, 0f, 0f);

    }

    void MOV()
    {
        Vector2 MovInput = pi.actions["Move"].ReadValue<Vector2>();

        Vector3 adelante = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 derecha = new Vector3(transform.right.x, 0f, transform.right.z).normalized;

        Vector3 direccion = adelante * MovInput.y + derecha * MovInput.x;

        rb.velocity = new Vector3(direccion.x * speed, rb.velocity.y, direccion.z * speed);
    }

}
