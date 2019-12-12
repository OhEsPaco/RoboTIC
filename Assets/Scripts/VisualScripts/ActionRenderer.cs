using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the <see cref="ActionRenderer" />
/// </summary>
public class ActionRenderer : MonoBehaviour
{
    /// <summary>
    /// Velocidad a la que van las acciones
    /// </summary>
    public float actionSpeed = 1f;

    /// <summary>
    /// Multiplicador para la velocidad de rotacion
    /// </summary>
    public float rotationSpeedMultiplier = 100f;

    /// <summary>
    /// Capacidad inicial de la lista de acciones pendientes
    /// </summary>
    public int initialActionCapacity = 20;

    /// <summary>
    /// Altura del arco del salto
    /// </summary>
    public float arcHeight=1f;

    /// <summary>
    /// Angulo del salto
    /// </summary>
    public float angle=1f;

    /// <summary>
    /// Referencia para el LevelManager
    /// </summary>
    private LevelManager manager;

    /// <summary>
    /// Lista de acciones
    /// </summary>
    private List<IEnumerator> actionList;

    /// <summary>
    /// false si se esta ejecutando una accion,
    /// true si no
    /// </summary>
    private bool lastActionFinished;

    /// <summary>
    /// Aqui tomamos la referencia al LevelManager
    /// </summary>
    private void Awake()
    {
        manager = LevelManager.instance;
    }

    /// <summary>
    /// Inicializacion de variables
    /// </summary>
    private void Start()
    {
        actionList = new List<IEnumerator>(20);
        lastActionFinished = true;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        if (actionList.Count > 0 && lastActionFinished)
        {
            StartNextAction();
        }
    }

    /// <summary>
    /// True si han terminado todas las acciones
    /// </summary>
    /// <returns>The <see cref="bool"/></returns>
    public bool AreAllActionsFinished()
    {
        if (actionList.Count == 0 && lastActionFinished)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Inicia la primera accion de la lista
    /// </summary>
    private void StartNextAction()
    {
        if (actionList.Count > 0)
        {
            IEnumerator coroutine = actionList[0];
            actionList.RemoveAt(0);
            if (coroutine != null)
            {
                StartCoroutine(coroutine);
            }
        }
    }

    /// <summary>
    /// Añade una accion a la lista
    /// </summary>
    /// <param name="action">The action<see cref="IEnumerator"/></param>
    private void AddAction(IEnumerator action)
    {
        actionList.Add(action);
    }

    /// <summary>
    /// Metodo para notificar que empieza una accion
    /// </summary>
    private void NotifyStartOfAction()
    {
        lastActionFinished = false;
    }

    /// <summary>
    /// Metodo para notificar que acaba una accion
    /// </summary>
    private void NotifyEndOfAction()
    {
        lastActionFinished = true;
    }

    /// <summary>
    /// Añade a la lista una accion de saltar
    /// </summary>
    /// <param name="player">The player<see cref="GameObject"/></param>
    /// <param name="currentBlock">The currentBlock<see cref="List{int}"/></param>
    /// <param name="intendedBlock">The intendedBlock<see cref="List{int}"/></param>
    /// <param name="playerOrientation">The playerOrientation<see cref="int"/></param>
    public void DoJump(GameObject player, List<int> currentBlock, List<int> intendedBlock, int playerOrientation)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);

        AddAction(JumpCoroutine(player, target, actionSpeed, arcHeight, angle));
    }

    /// <summary>
    /// Añade a la lista una accion de girar a la derecha
    /// </summary>
    /// <param name="player">The player<see cref="GameObject"/></param>
    public void DoTurnRight(GameObject player)
    {
        AddAction(TurnCoroutine(rotationSpeedMultiplier * actionSpeed, player, 90f, false));
    }

    /// <summary>
    /// Añade a la lista una accion de girar a la izquierda
    /// </summary>
    /// <param name="player">The player<see cref="GameObject"/></param>
    public void DoTurnLeft(GameObject player)
    {
        AddAction(TurnCoroutine(rotationSpeedMultiplier * actionSpeed, player, 90f, true));
    }

    /// <summary>
    /// Añade a la lista una accion de avanzar
    /// </summary>
    /// <param name="player">The player<see cref="GameObject"/></param>
    /// <param name="currentBlock">The currentBlock<see cref="List{int}"/></param>
    /// <param name="intendedBlock">The intendedBlock<see cref="List{int}"/></param>
    public void DoMove(GameObject player, List<int> currentBlock, List<int> intendedBlock)
    {
        Vector3 target = new Vector3(player.transform.position.x + (intendedBlock[0] - currentBlock[0]) * manager.MapRenderer.blockLength,
            player.transform.position.y + (intendedBlock[1] - currentBlock[1]) * manager.MapRenderer.blockLength,
            player.transform.position.z + (intendedBlock[2] - currentBlock[2]) * manager.MapRenderer.blockLength);

        AddAction(MoveCoroutine(actionSpeed, player, target));
    }

    /// <summary>
    /// Rutina que lleva a cabo el salto
    /// </summary>
    /// <param name="player">The player<see cref="GameObject"/></param>
    /// <param name="target">The target<see cref="Vector3"/></param>
    /// <param name="speed">The speed<see cref="float"/></param>
    /// <param name="arcHeight">The arcHeight<see cref="float"/></param>
    /// <param name="angle">The angle<see cref="float"/></param>
    /// <returns>The <see cref="IEnumerator"/></returns>
    private IEnumerator JumpCoroutine(GameObject player, Vector3 target, float speed, float arcHeight, float angle)
    {
        NotifyStartOfAction();
        for (float currentTime = 0; currentTime <= 1; currentTime += speed * Time.deltaTime)
        {
            Vector3 newPoint = Vector3.Lerp(player.transform.position, target, currentTime);

            player.transform.position = new Vector3(newPoint.x, (-angle * arcHeight * currentTime * currentTime + angle * arcHeight * currentTime) + Mathf.Lerp(player.transform.position.y, target.y, currentTime), newPoint.z);

            yield return null;
        }

        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();
    }

    /// <summary>
    /// Rutina que lleva a cabo el giro
    /// </summary>
    /// <param name="speed">The speed<see cref="float"/></param>
    /// <param name="player">The player<see cref="GameObject"/></param>
    /// <param name="degrees">The degrees<see cref="float"/></param>
    /// <param name="left">The left<see cref="bool"/></param>
    /// <returns>The <see cref="IEnumerator"/></returns>
    private IEnumerator TurnCoroutine(float speed, GameObject player, float degrees, bool left)
    {
        NotifyStartOfAction();
        int sign = left ? -1 : 1;
        float currentDegrees = 0f;
        for (; currentDegrees < degrees; currentDegrees += speed * Time.deltaTime)
        {
            float step = sign * (speed * Time.deltaTime);
            player.transform.Rotate(0, step, 0);
            yield return null;
        }

        //error correction
        player.transform.Rotate(0, sign * (degrees - currentDegrees), 0);
        NotifyEndOfAction();
    }

    /// <summary>
    /// Rutina que lleva a cabo la accion de avanzar
    /// </summary>
    /// <param name="speed">The speed<see cref="float"/></param>
    /// <param name="player">The player<see cref="GameObject"/></param>
    /// <param name="target">The target<see cref="Vector3"/></param>
    /// <returns>The <see cref="IEnumerator"/></returns>
    private IEnumerator MoveCoroutine(float speed, GameObject player, Vector3 target)
    {
        NotifyStartOfAction();
        float distance = Vector3.Distance(player.transform.position, target);
        for (float currentDistance = 0f; currentDistance < distance; currentDistance += speed * Time.deltaTime)
        {
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(player.transform.position, target, step);
            yield return null;
        }
        //error correction
        player.transform.position = new Vector3(target.x, target.y, target.z);
        NotifyEndOfAction();
    }
}
