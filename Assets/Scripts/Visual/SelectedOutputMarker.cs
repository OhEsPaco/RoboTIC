using System.Collections.Generic;
using UnityEngine;

public class SelectedOutputMarker : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    [SerializeField] private Transform floor;
    [SerializeField] private GameObject sphere;
    [SerializeField] private RoadPlacementLogic RoadPlacementLogic;

    private void OnMouseDown()
    {
        Debug.Log(transform.position);

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
        sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
        sphere.SetActive(true);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;

        if (floor != null)
        {
            if (transform.position.y < floor.position.y)
            {
                transform.position = new Vector3(transform.position.x, floor.position.y, transform.position.z);
            }

            //OPTIMIZAR
            sphere.transform.position = SearchClosestsIO(RoadPlacementLogic.FirstInput).transform.position;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        //Coordinadas en pixeles
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseUp()
    {
        FindAndSelectClosestIO();
    }

    public void FindAndSelectClosestIO()
    {
        RoadIO pivotIO = RoadPlacementLogic.FirstInput;

        if (pivotIO != null)
        {
            RoadIO closests = SearchClosestsIO(pivotIO);
            if (closests != null)
            {
                RoadPlacementLogic.SelectedIO=closests;
                gameObject.transform.position = closests.transform.position;
                sphere.SetActive(false);
                /*if(closests is RoadInput)
                {
                    Debug.Log("input");
                    LevelManager.instance.RoadPlacementLogic.PivotIO = closests;
                }*/
            }
        }
    }

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