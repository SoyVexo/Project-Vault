using UnityEngine;
using TMPro;

[System.Serializable]
public class StageConfig
{
    [Header("Identificador")]
    public string stageName = "Stage 1";

    [Header("Configuración de Elementos")]
    public Button[] botones;
    public Door door;               // Puerta de salida que se ABRE al completar botones
    public GameObject bloqueoEntrada; // Muro/Puerta que se CIERRA tras entrar al stage para evitar volver atrás
    public Collider exitTrigger;    // El Trigger físico en la puerta para pasar de nivel
    public GameObject palanca;
    public LayerMask palancaLayer;
    public float interactDistance = 2f;

    [Header("Opciones")]
    public bool isTutorial;
    public MapRoating map;

    [Header("UI del Stage")]
    public CanvasGroup interactPanelCanvasGroup;
    public TextMeshProUGUI intPanelText;

    [HideInInspector] public bool palancaPresionada;
}