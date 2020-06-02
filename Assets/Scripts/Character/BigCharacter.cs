using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Block;

public class BigCharacter : Character
{
    [SerializeField] private GameObject inventoryMarker;
    [SerializeField] private Vector3 itemScale = new Vector3(1, 1, 1);

    /// <summary>
    /// Velocidad a la que van las acciones
    /// </summary>
    public float actionSpeed = 0.5f;

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

    private MessageWarehouse msgWar;
    private bool loaded = false;
    private float blockLenght;

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgBigRobotAction>(ReceiveAction);
        EventAggregator.Instance.Subscribe<MsgPlaceCharacter>(PlaceCharacter);
        EventAggregator.Instance.Subscribe<MsgTakeItem>(TakeItem);
        EventAggregator.Instance.Subscribe<MsgBigRobotIdle>(IsRobotIdle);
        EventAggregator.Instance.Subscribe<MsgUseItem>(UseObject);
        EventAggregator.Instance.Subscribe<MsgBigCharacterAllActionsFinished>(ServeAllActionsFinished);
        msgWar = new MessageWarehouse(EventAggregator.Instance);
    }

    /// <summary>
    /// Inicializacion de variables
    /// </summary>
    private void Start()
    {
        StartCoroutine(StartCrt());
    }

    private IEnumerator StartCrt()
    {
        loaded = false;
        actionList = new List<IEnumerator>(20);
        lastActionFinished = true;

        MsgBlockLength msg = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(msg, out blockLenght));
        jumpHeight = blockLenght * jumpPct;
        loaded = true;
    }

    private void ServeAllActionsFinished(MsgBigCharacterAllActionsFinished msg)
    {
        StartCoroutine(ServeAllActionsFinishedCrt(msg));
    }

    private IEnumerator ServeAllActionsFinishedCrt(MsgBigCharacterAllActionsFinished msg)
    {
        yield return new WaitUntil(() => AreAllActionsFinished());
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigCharacterAllActionsFinished, bool>(msg, true));
    }

    private void PlaceCharacter(MsgPlaceCharacter msg)
    {
        if (msg.NewParent != null)
        {
            gameObject.transform.parent = msg.NewParent;
        }
        gameObject.transform.position = new Vector3();
        gameObject.transform.position = msg.Position;
        gameObject.transform.rotation = new Quaternion();
        gameObject.transform.Rotate(msg.Rotation);
        EventAggregator.Instance.Publish(new ResponseWrapper<MsgPlaceCharacter, GameObject>(msg, this.gameObject));
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

    private void IsRobotIdle(MsgBigRobotIdle msg)
    {
        if (AreAllActionsFinished())
        {
            EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigRobotIdle, bool>(msg, true));
        }
        else
        {
            EventAggregator.Instance.Publish(new ResponseWrapper<MsgBigRobotIdle, bool>(msg, false));
        }
    }

    private void ReceiveAction(MsgBigRobotAction msg)
    {
        if (loaded)
        {
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

    private void TakeItem(MsgTakeItem msg)
    {
        AddAction(TakeItemCoroutine(msg.item, msg.numberOfItems));
    }

    private void UseObject(MsgUseItem msg)
    {
        AddAction(UseItemCoroutine(msg.frontBlock, msg.reaction, msg.replaceBlock, msg.itemPos, msg.item));
    }

    private IEnumerator UseItemCoroutine(Block frontBlock, EffectReaction reaction, LevelObject newlySpawnedObject, Vector3 posNew, Item item)
    {
        NotifyStartOfAction();

        foreach (BlockActions blockAction in reaction.actionsToExecute)
        {
            frontBlock.ExecuteAction(blockAction);
        }
        if (item.ParentToBlockParent)
        {
            Transform blockParent = frontBlock.transform.parent;
            if (blockParent != null)
            {
                item.transform.parent = blockParent;
            }
        }

        item.transform.localScale = new Vector3(1, 1, 1);
        item.transform.position = posNew;
        item.Use();
        //
        foreach (string trigger in reaction.animationTriggers)
        {
            frontBlock.SetAnimationTrigger(trigger);
        }
        if (reaction.replaceBlock && newlySpawnedObject != null)
        {
            newlySpawnedObject.gameObject.SetActive(true);
            frontBlock.gameObject.SetActive(false);
            Destroy(frontBlock.gameObject);

            if (newlySpawnedObject != null && newlySpawnedObject.IsBlock())
            {
                Block newlySpawnedBlock = (Block)newlySpawnedObject;
                newlySpawnedBlock.ExecuteAction(BlockActions.Place);
            }
        }
        yield return null;
        NotifyEndOfAction();
    }

    private IEnumerator TakeItemCoroutine(Item item, int numberOfItems)
    {
        NotifyStartOfAction();
        item.transform.parent = transform;
        item.transform.localScale = itemScale;
        item.transform.position = new Vector3(GetInventoryPosition().x, GetInventoryPosition().y + 0.45f * (numberOfItems - 1), GetInventoryPosition().z);
        yield return null;
        NotifyEndOfAction();
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
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target);

        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            transform.position = Vector3.Lerp(startPos, target, i);
            yield return null;
        }
        transform.position = target;

        NotifyEndOfAction();
    }

    public Vector3 GetInventoryPosition()
    {
        return inventoryMarker.transform.position;
    }
}