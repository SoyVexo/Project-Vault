using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{ 
    [Header("Configs")]
    public Transform rejas;

    public AudioSource AudioEmit;

    public AudioClip DoorSoundOpen;

    [Header("State")]
    public bool Isopen = false;

    public void Open()
    {
        rejas.DOLocalMoveY(7.38f, 2);

        AudioEmit.PlayOneShot(DoorSoundOpen);

        Isopen = true;
    }

}
