using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCharacter : Character
{
    [SerializeField] private GameObject inventoryMarker;
    /*  [SerializeField] private Animator animator;*/
    [SerializeField] private EventAggregator eventAggregator;

    /// <summary>
    /// Velocidad a la que van las acciones
    /// </summary>
    public float actionSpeed = 1f;

    /// <summary>
    /// Porcentaje de la altura del salto hacia arriba respecto a la del bloque
    /// </summary>
    [Range(0.0f, 10.0f)]
    public float jumpPct = 1f;

    private float jumpHeight;

    /// <summary>
    /// Porcentaje de la altura del salto hacia abajo respecto a la altura total del salto
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float descendJumpPct = 0.3f;

    /// <summary>
    /// Porcentaje del tiempo de salto que se pasa ascendiendo
    /// </summary>
    [Range(0, 1)]
    public float takeOff = 0.8f;

    /// <summary>
    /// Tiempo que dura la rotacion
    /// </summary>
    public float rotationTime = 1f;

    /// <summary>
    /// Capacidad inicial de la lista de acciones pendientes
    /// </summary>
    public int initialActionCapacity = 20;

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
        eventAggregator.Subscribe<MsgBigRobotAction>(ReceiveAction);
        eventAggregator.Subscribe<MsgPlaceCharacter>(PlaceCharacter);
    }

    /// <summary>
    /// Inicializacion de variables
    /// </summary>
    private void Start()
    {

        actionList = new List<IEnumerator>(20);
        lastActionFinished = true;
        jumpHeight = LevelManager.instance.MapRenderer.BlockLength * jumpPct;
    }

    private void PlaceCharacter(MsgPlaceCharacter msg)
    {
        gameObject.transform.position = msg.Position;
        gameObject.transform.Rotate(msg.Rotation);
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        StartNextAction();
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

    private void ReceiveAction(MsgBigRobotAction msg)
    {
        Debug.Log("Hola");
        switch (msg.Action)
        {
            case MsgBigRobotAction.BigRobotActions.Jump:
                DoJump(msg.Target);
                break;

            case MsgBigRobotAction.BigRobotActions.Move:
                DoMove(msg.Target);
                break;

            case MsgBigRobotAction.BigRobotActions.TurnLeft:
                DoTurnLeft();
                break;

            case MsgBigRobotAction.BigRobotActions.TurnRight:
                DoTurnRight();
                break;
        }
    }

    /// <summary>
    /// Inicia la primera accion de la lista
    /// </summary>
    private void StartNextAction()
    {
        if (actionList.Count > 0 && lastActionFinished)
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

    private void NotifyStartOfAction()
    {
        lastActionFinished = false;
    }

    private void NotifyEndOfAction()
    {
        lastActionFinished = true;
    }

    private void DoJump(Vector3 target)
    {
        float finalHeight = transform.position.y > target.y ? jumpHeight * descendJumpPct : jumpHeight;
        AddAction(JumpCoroutine(target, actionSpeed, finalHeight, takeOff));
    }

    private void DoTurnRight()
    {
        AddAction(TurnCoroutine(rotationTime, 90f));
    }

    private void DoTurnLeft()
    {
        AddAction(TurnCoroutine(rotationTime, -90f));
    }

    private void DoMove(in Vector3 target)
    {
        AddAction(MoveCoroutine(actionSpeed, target));
    }

    private IEnumerator JumpCoroutine(Vector3 target, float speed, float jumpHeight, float takeOff)
    {
        NotifyStartOfAction();

        //Porcentaje del movimiento en el que nos encontramos
        float percent = 0;

        //Posicion de partida
        Vector3 originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Mientras el porcentaje sea menor o igual que 1
        while (percent <= 1)
        {
            //Para la x y la z simplemente hacemos lerp entre el punto original y el final
            Vector3 newPos = Vector3.Lerp(originalPos, target, percent);

            //Para la y tomamos en cuenta si estamos subiendo o bajando
            if (percent < takeOff)
            {
                newPos.y = Mathf.Lerp(originalPos.y, originalPos.y + jumpHeight, percent / takeOff);
            }
            else
            {
                newPos.y = Mathf.Lerp(originalPos.y + jumpHeight, target.y, (percent - takeOff) / (1 - takeOff));
            }

            //Actualizamos la posicion del jugador
            transform.position = newPos;

            //Actualizamos el porcentaje recorrido
            percent += speed * Time.deltaTime;

            yield return null;
        }

        //Transportamos al personaje a su posicion final para evitar errores de exactitud
        transform.position = target;
        NotifyEndOfAction();
    }

    private IEnumerator TurnCoroutine(float time, float degrees)
    {
        NotifyStartOfAction();

        //La rotacion es la rotacion actual + los grados en el eje 'y' (por eso multiplico
        //por Vector3.up (0,1,0)

        Vector3 finalRotation = transform.eulerAngles + Vector3.up * degrees;

        LTDescr lTDescr = LeanTween.rotate(this.gameObject, finalRotation, time).setEaseInOutSine();

        while (LeanTween.isTweening(lTDescr.id))
        {
            yield return null;
        }

        NotifyEndOfAction();
    }

    private IEnumerator MoveCoroutine(float speed, Vector3 target)
    {
        NotifyStartOfAction();
        //Se repiten el primer y ultimo punto ya que se usan para calcular angulos
        Vector3[] track = new Vector3[4];
        track[0] = transform.position;
        track[1] = transform.position;
        track[2] = target;
        track[3] = target;

        LTSpline lTSpline = new LTSpline(track);

        //El tiempo es la distancia dividida entre la velocidad
        LTDescr ltDescr = LeanTween.moveSpline(gameObject, lTSpline, lTSpline.distance / speed).setOrientToPath(true).setEaseInOutSine();

        while (LeanTween.isTweening(ltDescr.id))
        {
            yield return null;
        }

        NotifyEndOfAction();
    }

    public Vector3 GetInventoryPosition()
    {
        return inventoryMarker.transform.position;
    }
}