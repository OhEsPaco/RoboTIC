using System;
using UnityEditor;
using UnityEngine;

public class PathContainer : MonoBehaviour
{
    [SerializeField] private Path[] paths = new Path[0];

    private void OnDrawGizmos()
    {
        //iTween.DrawPath(paths,Color.red);
        foreach (Path p in paths)
        {
            if (p.drawPreview)
            {
               
                iTween.DrawPath(p.points, p.color);
                if (p.pathName != null)
                {

                    int nPoints = 0;
                    Vector3 total = new Vector3(0, 0, 0);
                    foreach(Transform point in p.points)
                    {
                        nPoints++;
                        total = total + point.position;
                    }

                    if (nPoints > 0)
                    {
                        total = total / nPoints;
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = p.color;
                       Handles.Label(total,p.pathName,style);
                    }
                }
                /*Transform firstPoint = p.points[0];
                Transform lastPoint = p.points[p.points.Length - 1];
                Handles.Label(firstPoint.position, p.ioBegin.name);
                Handles.Label(lastPoint.position, p.ioEnd.name);*/

            }
        }
    }

    public bool GetPathByName(in string name, out Path path)
    {
        foreach(Path p in paths)
        {
            if (p.pathName.Equals(name))
            {
                path = p;
                return true;
            }
        }
        path = new Path();
        return false;
    }

    [Serializable]
    public struct Path
    {
        public string pathName;
        public Transform[] points;
        public Color color;
        public bool drawPreview;
        public RoadInput ioBegin;
        public RoadOutput ioEnd;
    }
}