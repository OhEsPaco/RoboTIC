using System.Collections.Generic;
using UnityEngine;
using static PathContainer;

public class RoadMovementLogic : MonoBehaviour
{
    //El robot pequeño
    [SerializeField] private MiniCharacter player;

    //Distancia minima a la que tengo que acercarme al ultimo punto de un camino
    //para considerarlo superado
    [SerializeField] private float minimumDist = 0.1f;

    //La velocidad de movimiento del robot
    [SerializeField] private float speed = 20;

    //Maxima cantidad de puntos en un camino cada vez
    [SerializeField] private int pathMaxPoints = 20;

    //Comprueba que se haya iniciado el movimiento
    private bool movementStarted = false;

    //Porcentaje del camino en el que nos encontramos
    private float pathPercent = 0;

    //Output final de la carretera
    private RoadOutput finalOutput;

    //Output de la pieza de carretera que estamos recorriendo
    private RoadOutput nextOutput;

    //Listaa con los puntos que forman el camino
    private List<Vector3> controlPath = new List<Vector3>();

    //El camino en si
    private LTSpline track;

    //Lo que vamos a avanzar en el camino en una unidad de tiempo
    private float percentAdd;

    private void Start()
    {
        //Hacemos que el robot sea hijo de este objeto y lo marcamos como inactivo
        player.transform.parent = transform;
        player.gameObject.SetActive(false);
    }

    //Inicia el movimiento dado el input y el output de la carretera
    public void StartMovement(RoadInput input, RoadOutput output)
    {
        //Resetemos todo
        movementStarted = false;
        pathPercent = 0;
        this.finalOutput = output;
        nextOutput = null;
        player.gameObject.SetActive(true);
        player.transform.position = input.transform.position;
        controlPath = new List<Vector3>();
        percentAdd = 0;
        track = null;

        //Tomamos el camino que empieza en input
        if (GetNewPath(input))
        {
            movementStarted = true;
        }
        else
        {
            Debug.LogError("No path available");
        }
    }

    //Busca el siguiente camino posible y crea una LTSpline combinando los puntos nuevos y los anteriores
    private bool GetNewPath(RoadInput input)
    {
        Path path;
        RoadOutput outp;

        //Pedimos el camino correspondiente al input
        if (input.GetParentRoad().GetPathAndOutput(input, out path, out outp))
        {
            //Estos son los puntos en bruto del camino
            Vector3[] trackPoints = new Vector3[path.points.Length];
            for (int i = 0; i < trackPoints.Length; i++)
            {
                trackPoints[i] = path.points[i].position;
            }

            //Si la lista no tiene ningun punto duplicamos el primero y añadimos los siguientes
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
            if (controlPath.Count == 0)
            {
                controlPath.Add(trackPoints[0]);
                foreach (Vector3 point in trackPoints)
                {
                    controlPath.Add(point);
                }
            }
            else
            {
                //Eliminamos el punto del final, pues esta duplicado por lo expuesto anteriormente
                controlPath.RemoveAt(controlPath.Count - 1);

                //Y otra vez eliminamos el del final ya que el punto final del camino anterior y el inicial
                //del siguiente deberian estar superpuestos y si por temas de precision el punto inicial del
                //camino nuevo se encuentra ligeramente más atras haría un giro extraño
                controlPath.RemoveAt(controlPath.Count - 1);
                for (int i = 0; i < trackPoints.Length; i++)
                {
                    controlPath.Add(trackPoints[i]);
                }
            }

            //Si nos hemos pasado, eliminamos los del principio
            bool someRemoved = false;
            while (controlPath.Count > pathMaxPoints)
            {
                controlPath.RemoveAt(0);
                someRemoved = true;
            }

            //Si hemos eliminado algo, duplicamos el inicial
            if (someRemoved)
            {
                controlPath.Insert(0, controlPath[0]);
            }

            //Duplicamos el final
            controlPath.Add(controlPath[controlPath.Count - 1]);

            //Creamos el camino
            track = new LTSpline(controlPath.ToArray());

            //Nos aseguramos de que la velocidad esté basada en la distancia del camino para que sea constante
            percentAdd = speed / track.distance;

            //Actualizamos el porcentaje recorrido del camino segun nuestra posicion actual
            pathPercent = track.ratioAtPoint(player.transform.position);

            //Ponemos como siguiente output la que resulta de procesar el camino nuevo
            nextOutput = outp;

            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (Vector3 v in controlPath.ToArray())
        {
            Gizmos.DrawSphere(v, 0.3f);
        }
    }

    private void Update()
    {
        if (movementStarted)
        {
            //Distancia entre el robot y el ultimo punto del camino
            float distance = Vector3.Distance(controlPath[controlPath.Count - 1], player.transform.position);

            //Si es menor que la distancia minima, hace falta alargar el camino
            if (distance < minimumDist)
            {
                //Final de la carretera
                if (nextOutput == finalOutput)
                {
                    Debug.Log("End of the road!!");
                }
                else
                {
                    //Pedir mas camino
                    if (!GetNewPath((RoadInput)nextOutput.connectedTo))
                    {
                        movementStarted = false;
                        Debug.LogError("No path available");
                    }
                }
            }

            //Movemos al robot
            track.place(player.transform, pathPercent);

            //Avanzamos el punto del camino en el que nos encontramos
            pathPercent += percentAdd * Time.deltaTime;

            //Si la siguiente output es la final y el porcentaje del camino pasa de 1, hemos acabado
            if (nextOutput == finalOutput && pathPercent >= 1)
            {
                movementStarted = false;
            }
        }
    }
}