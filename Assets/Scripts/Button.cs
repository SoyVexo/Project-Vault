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

    private void Start()
    {
        InicialAlture = transform.position.y;
        UnPressed = InicialAlture;
        pressed = UnPressed - 0.381386f;
        print(pressed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsPressed) return;
        
        transform.DOMoveY(pressed, 0.3f);
        IsPressed = true;
    }
    public void OnTriggerExit(Collider other)
    {
        transform.DOMoveY(UnPressed, 0.3f);
        IsPressed = false;
    }
}
