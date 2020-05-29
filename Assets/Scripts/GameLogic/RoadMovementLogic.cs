using UnityEngine;
using static PathContainer;

public class RoadMovementLogic : MonoBehaviour
{
    //El robot pequeño
    [SerializeField] private MiniCharacter player;

    //La velocidad de movimiento del robot
    [SerializeField] private float speed = 20;

    //Comprueba que se haya iniciado el movimiento
    private bool movementStarted = false;

    //Output final de la carretera
    private RoadOutput finalOutput;

    //Output de la pieza de carretera que estamos recorriendo
    private RoadOutput nextOutput;

    //El camino en si
    private LTSpline track;

    //Descriptor del tween
    private LTDescr tweenDescr;

    private void Start()
    {
        //Hacemos que el robot sea hijo de este objeto y lo marcamos como inactivo
        //player.transform.parent = transform;
        //player.gameObject.SetActive(false);
    }

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgStartRoadMovement>(StartMovement);
        EventAggregator.Instance.Subscribe<MsgStopMovement>(StopMovement);
    }

    //Inicia el movimiento dado el input y el output de la carretera
    private void StartMovement(MsgStartRoadMovement msg)
    {
        //Resetemos todo
        movementStarted = false;
        finalOutput = msg.output;
        nextOutput = null;
        player.gameObject.SetActive(true);
        player.transform.position = msg.input.transform.position;
        track = null;
        tweenDescr = null;

        //Tomamos el camino que empieza en input
        if (StartNewPath(msg.input, out tweenDescr))
        {
            movementStarted = true;
        }
        else
        {
            Debug.LogError("No path available");
        }
    }

    //Busca el siguiente camino posible y crea una LTSpline combinando los puntos nuevos y los anteriores
    private bool StartNewPath(RoadInput input, out LTDescr ltDescr)
    {
        Path path;
        RoadOutput outp;

        //Pedimos el camino correspondiente al input
        if (input.GetParentRoad().GetPathAndOutput(input, out path, out outp))
        {
            //Duplicamos el primer y el ultimo punto
            /*http://dentedpixel.com/LeanTweenDocumentation/classes/LTSpline.html */
            /*LTSpline ( pts )
            Defined in LeanTween.cs:2858
            Parameters:
            pts Vector3 Array
            A set of points that define the points the path will pass through (starting with starting
            control point, and ending with a control point)
            Note: The first and last item just define the angle of the end points, they are not actually
            used in the spline path itself. If you do not care about the angle you can jus set the first
            two items and last two items as the same value.*/

            Vector3[] rawPath = new Vector3[path.points.Length + 2];

            rawPath[0] = path.points[0].position;
            for (int i = 0; i < path.points.Length; i++)
            {
                rawPath[i + 1] = path.points[i].position;
            }
            rawPath[rawPath.Length - 1] = path.points[path.points.Length - 1].position;

            //Si hemos recorrido un camino anteriormente, reemplazamos los dos primeros puntos por los finales
            //del camino anterior para evitar saltos
            if (track != null)
            {
                rawPath[0] = track.pts[track.pts.Length - 3];
                rawPath[1] = track.pts[track.pts.Length - 2];
            }

            //Creamos el camino
            track = new LTSpline(rawPath);

            //Ponemos como siguiente output la que resulta de procesar el camino nuevo
            nextOutput = outp;

            //Iniciamos el movimiento
            ltDescr = LeanTween.moveSpline(player.gameObject, track, track.distance / speed).setOrientToPath(true);

            return true;
        }

        ltDescr = null;
        return false;
    }

    private void StopMovement(MsgStopMovement msg)
    {
        //Resetemos todo
        movementStarted = false;
        if (tweenDescr != null)
        {
            if (LeanTween.isTweening(tweenDescr.id))
            {
                LeanTween.cancel(tweenDescr.id);
            }
        }
    }

    private void Update()
    {
        if (movementStarted)
        {
            //Comprueba si se ha acabado el proceso de tweening
            if (!LeanTween.isTweening(tweenDescr.id))
            {
                //Final de la carretera
                if (nextOutput == finalOutput)
                {
                    Debug.Log("End of the road!!");
                    movementStarted = false;
                }
                else
                {
                    //Pedir mas camino
                    if (!StartNewPath((RoadInput)nextOutput.ConnectedTo, out tweenDescr))
                    {
                        movementStarted = false;
                        Debug.LogError("No path available");
                    }
                }
            }
        }
    }
}