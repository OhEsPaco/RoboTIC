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
                
            }
        }
    }

    [Serializable]
    public struct Path
    {
        public string name;
        public Transform[] points;
        public Color color;
        public bool drawPreview;
    }
}