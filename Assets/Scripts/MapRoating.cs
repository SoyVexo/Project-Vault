using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MapRoating : MonoBehaviour
{
    public GameObject map;
    PlayerInput pi;

    public bool volteada;

    void Start()
    {
        pi = GetComponent<PlayerInput>();
    }
    void Update()
    {
        if (pi.actions["InvertMap(Testing)"].WasPressedThisFrame())
        {

            vuelta();
        }
    }
    public void vuelta()
    {
        float angulo = volteada ? 0 : 180f;

        map.transform.DORotate(new Vector3(0, 0, angulo), 5);

        if (volteada)
        {
            volteada = false;
        }
        else
        {
            volteada = true;
        }
    }
}
