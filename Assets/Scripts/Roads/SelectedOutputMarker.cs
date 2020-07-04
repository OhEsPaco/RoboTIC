// SelectedOutputMarker.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase de la flecha para seleccionar carretera.
/// </summary>
public class SelectedOutputMarker : MonoBehaviour
{
    /// <summary>
    /// Offset del movimiento.
    /// </summary>
    private Vector3 mOffset;

    /// <summary>
    /// Profundidad del movimiento.
    /// </summary>
    private float mZCoord;

    /// <summary>
    /// Objeto que se muestra marcando la carretera seleccionada.
    /// </summary>
    [SerializeField] private GameObject sphere;

    /// <summary>
    /// Lógica de posicionamiento de carreteras.
    /// </summary>
    [SerializeField] private RoadPlacementLogic RoadPlacementLogic;

    /// <summary>
    /// ¿Se está usando la flecha?
    /// </summary>
    private bool placing = false;

    /// <summary>
    /// Start.
    /// </summary>
    private void Start()
    {
        EventAggregator.Instance.Subscribe<MsgSomethingTapped>(SomethingTapped);
    }

    /// <summary>
    /// Para mejorar la usabilidad el gesto de tap sirve para parar la colocación del marcador
    /// aunque no se haya hecho tap directamente sobre el mismo.
    /// </summary>
    /// <param name="msg">The msg<see cref="MsgSomethingTapped"/>.</param>
    private void SomethingTapped(MsgSomethingTapped msg)
    {
        if (placing)
        {
            placing = false;
            tappedMsg = true;
            FindAndSelectClosestIO();
        }
    }

    /// <summary>
    /// True si se ha recibido un mensaje de tap.
    /// </summary>
    private bool tappedMsg = false;

    /// <summary>
    /// OnSelect.
    /// </summary>
    private void OnSelect()
    {
        if (!placing && !tappedMsg)
        {
            Debug.Log(transform.position);

            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
            sphere.SetActive(true);
            placing = true;
        }
        else if (!placing && tappedMsg)
        {
            tappedMsg = false;
        }
        else
        {
            placing = false;
            FindAndSelectClosestIO();
        }
    }

    /// <summary>
    /// Update.
    /// </summary>
    private void Update()
    {
        if (placing)
        {
            OnMouseDrag();
        }
    }

    /// <summary>
    /// Se activa cuando se arrastra con el ratón.
    /// </summary>
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
        sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
    }

    /// <summary>
    /// Convierte las coordenadas del ratón a coordenadas globales.
    /// </summary>
    /// <returns>Las coordenadas globlales.</returns>
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    /// <summary>
    /// OnMouseUp.
    /// </summary>
    private void OnMouseUp()
    {
        FindAndSelectClosestIO();
    }

    /// <summary>
    /// Busca y selecciona la IO más cercana.
    /// </summary>
    public void FindAndSelectClosestIO()
    {
        RoadIO pivotIO = RoadPlacementLogic.FirstInput;

        if (pivotIO != null)
        {
            RoadIO closests = SearchClosestsIO(pivotIO);
            if (closests != null)
            {
                RoadPlacementLogic.SelectedIO = closests;
                gameObject.transform.position = closests.transform.position;
                sphere.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Busca la IO más cercana.
    /// </summary>
    /// <param name="pivotIO">IO por la que empezar a buscar.</param>
    /// <returns>La IO más cercana.</returns>
    private RoadIO SearchClosestsIO(RoadIO pivotIO)
    {
        if (pivotIO != null)
        {
            RoadIO closestIO = pivotIO;

            List<RoadIO> processedIO = new List<RoadIO>();

            Stack<RoadIO> ioToProc = new Stack<RoadIO>();

            RoadIO[] tmpe;

            ioToProc.Push(pivotIO);

            while (ioToProc.Count > 0)
            {
                RoadIO toProc = ioToProc.Pop();
                RoadIO connectedTo = toProc.ConnectedTo;

                tmpe = toProc.GetParentRoad().GetAllIO();

                foreach (RoadIO rio in tmpe)
                {
                    if (!processedIO.Contains(rio))
                    {
                        ioToProc.Push(rio);
                    }
                }

                if (connectedTo != null)
                {
                    tmpe = connectedTo.GetParentRoad().GetAllIO();

                    foreach (RoadIO rio in tmpe)
                    {
                        if (!processedIO.Contains(rio))
                        {
                            ioToProc.Push(rio);
                        }
                    }
                }

                if (Vector3.Distance(closestIO.transform.position, transform.position) > Vector3.Distance(toProc.transform.position, transform.position))
                {
                    if (toProc.CanBeSelected)
                    {
                        closestIO = toProc;
                    }
                }
                if (!processedIO.Contains(toProc))
                {
                    processedIO.Add(toProc);
                }
            }

            if (closestIO.ConnectedTo != null)
            {
                if (closestIO.ConnectedTo is RoadOutput && closestIO.ConnectedTo.CanBeSelected)
                {
                    closestIO = closestIO.ConnectedTo;
                }
            }
            return closestIO;
        }
        return null;
    }
}