using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class LevelController : MonoBehaviour
{
    [Header("Configuración de Stages")]
    [SerializeField] private StageConfig[] stages;
    [SerializeField] private int currentStageIndex = 0;
    public PlayerController player;

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
            player.PasadoPorlapuerta = false;
            if (stages[currentStageIndex].map != null && stages[currentStageIndex].map.volteada)
            {
                stages[currentStageIndex].map.vuelta();
            }

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
                player.PasadoPorlapuerta = true;
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
            // Guardamos la referencia del stage que estamos dejando atrás
            // ANTES de mover el índice, para poder limpiarlo después.
            StageConfig stageAnterior = stages[currentStageIndex];

            currentStageIndex++;
            InicializarStageActual();
            print($"[Level] Avanzando al Stage {currentStageIndex + 1}");

            // Si ese stage está marcado para autodestruirse al avanzar, lo programamos.
            if (stageAnterior != null && stageAnterior.eliminarAlAvanzar)
            {
                StartCoroutine(EliminarUbicacionAnterior(stageAnterior));
            }
        }
        else
        {
            print("¡Has completado todos los niveles del juego!");
        }
    }

    /// <summary>
    /// Espera el tiempo configurado en el StageConfig y luego destruye la
    /// habitación/ubicación anterior, verificando null en cada paso para
    /// evitar errores si el objeto ya no existe o nunca fue asignado.
    /// </summary>
    private IEnumerator EliminarUbicacionAnterior(StageConfig stageAEliminar)
    {
        if (stageAEliminar == null) yield break;

        float espera = Mathf.Max(0f, stageAEliminar.tiempoEsperaEliminar);
        yield return new WaitForSeconds(espera);

        // Puede que en el medio alguien ya lo haya destruido o desactivado el flag.
        if (stageAEliminar.habitacionRoot != null)
        {
            print($"[Level] Eliminando ubicación anterior: {stageAEliminar.stageName}");
            Destroy(stageAEliminar.habitacionRoot);
        }
        else
        {
            print($"[Level] No se eliminó nada: '{stageAEliminar.stageName}' no tiene habitacionRoot asignado o ya fue destruido.");
        }
    }

    // =========================================================================
    // MÉTODOS REQUERIDOS POR EL PLAYERCONTROLLER
    // =========================================================================

    /// <summary>
    /// Acepta cualquier argumento enviado por el PlayerController (ej. objeto, bool o id)
    /// para evitar el error de sobrecarga CS1501.
    /// </summary>
    /// <summary>
    /// Acciona la palanca de UNA habitación específica. Recibe explícitamente
    /// a qué MapRoating pertenece (y opcionalmente a dónde mover al jugador),
    /// en vez de asumir que siempre es la del stage "actual". Así, activar
    /// una palanca nunca afecta a otra habitación por error.
    /// </summary>
    public void ActivarPalanca(MapRoating habitacionDeEstaPalanca, Transform posicionAlVoltear = null)
    {
        if (habitacionDeEstaPalanca == null)
        {
            Debug.LogWarning("[LevelController] Se llamó a ActivarPalanca sin asignar a qué habitación pertenece.");
            return;
        }

        print($"[LevelController] Palanca accionada para la habitación: {habitacionDeEstaPalanca.gameObject.name}");

        habitacionDeEstaPalanca.vuelta();

        if (posicionAlVoltear != null)
        {
            player.transform.DOMove(posicionAlVoltear.position, 1f);
        }
    }

    /// <summary>
    /// Muestra u oculta el texto o canvas de interacción (ej. "Presiona E").
    /// </summary>
    public void SetInteractUIVisible(bool visible)
    {
        // Lógica de UI si la usas
    }
}