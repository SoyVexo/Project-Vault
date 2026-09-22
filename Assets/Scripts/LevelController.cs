using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Configuración de Stages")]
    [SerializeField] private StageConfig[] stages;
    [SerializeField] private int currentStageIndex = 0;

    public StageConfig CurrentStage
    {
        get
        {
            if (stages == null || stages.Length == 0) return null;
            return stages[currentStageIndex];
        }
    }

    // Variable que indica si el stage actual ya fue resuelto
    public bool IsCurrentStageComplete { get; private set; }

    private void Start()
    {
        InicializarStageActual();
    }

    private void Update()
    {
        ComprobarBotones();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void InicializarStageActual()
    {
        if (CurrentStage == null) return;

        IsCurrentStageComplete = false;

        // Si el Stage tiene una pared/reja de bloqueo para cerrar la entrada
        if (CurrentStage.bloqueoEntrada != null)
        {
            CurrentStage.bloqueoEntrada.SetActive(true);
        }

        print($"[Level] Cargado {CurrentStage.stageName}");
    }

    private void ComprobarBotones()
    {
        // 1. SI YA SE COMPLETÓ EL STAGE, NO HACEMOS NADA MÁS.
        if (IsCurrentStageComplete) return;

        if (CurrentStage == null || CurrentStage.botones == null || CurrentStage.botones.Length == 0) return;

        // 2. Comprobamos si TODOS los botones del Stage están presionados EN ESTE FRAME
        bool todosPulsados = CurrentStage.botones.All(b => b != null && b.IsPressed);

        // 3. Si todos están activados a la vez:
        if (todosPulsados)
        {
            IsCurrentStageComplete = true; // Se bloquea permanentemente
            print($"[Level] ¡Todos los botones activados simultáneamente! Stage completado.");

            // Abrir la puerta
            if (CurrentStage.door != null)
            {
                CurrentStage.door.Open();
            }
        }
    }

    // Método llamado por el script puente (TriggerZone) al tocar la puerta de salida
    public void OnJugadorLlegoAZona(GameObject triggerQueToco)
    {
        if (CurrentStage == null) return;

        // Comprueba si tocó el Trigger de salida del Stage actual
        if (CurrentStage.exitTrigger != null && triggerQueToco == CurrentStage.exitTrigger.gameObject)
        {
            if (IsCurrentStageComplete)
            {
                NextStage();
            }
            else
            {
                print("[Level] No puedes pasar aún. Faltan botones por presionar.");
            }
        }
    }

    public void NextStage()
    {
        if (currentStageIndex < stages.Length - 1)
        {
            currentStageIndex++;
            InicializarStageActual();
            print($"[Level] Avanzando al Stage {currentStageIndex + 1}");
        }
        else
        {
            print("¡Has completado todos los niveles del juego!");
        }
    }

    // =========================================================================
    // MÉTODOS REQUERIDOS POR EL PLAYERCONTROLLER
    // =========================================================================

    /// <summary>
    /// Acepta cualquier argumento enviado por el PlayerController (ej. objeto, bool o id)
    /// para evitar el error de sobrecarga CS1501.
    /// </summary>
    public void ActivarPalanca(object objetoOEstado = null)
    {
        print($"[LevelController] Palanca accionada con el valor: {objetoOEstado}");
        stages[currentStageIndex].map.vuelta();
    }

    /// <summary>
    /// Muestra u oculta el texto o canvas de interacción (ej. "Presiona E").
    /// </summary>
    public void SetInteractUIVisible(bool visible)
    {
        // Lógica de UI si la usas
    }
}