using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSurface : MonoBehaviour
{
    private MessageWarehouse msgWar;
    private float blockLength;
    private List<int> levelSize;
    private EditorSurfacePoint[] points;

    [Range(0f, 1f)]
    [SerializeField] private float secondsBetweenUpdates;

    public float maxArrowDistance = 0.2f;
    private bool readyArrow = false;
    [SerializeField] private Material cubeMaterial;
    private bool readySurface = false;

    private void Awake()
    {
        EventAggregator.Instance.Subscribe<MsgResetEditorSurface>(ResetEditorSurface);
    }

    // Start is called before the first frame update
    private void Start()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);
        StartCoroutine(SetUpSurface());
    }

    private IEnumerator SetUpSurface()
    {
        readySurface = false;
        MsgBlockLength msg = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(msg, out blockLength));

        MsgEditorMapSize msg2 = new MsgEditorMapSize();
        msgWar.PublishMsgAndWaitForResponse<MsgEditorMapSize, List<int>>(msg2);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgEditorMapSize, List<int>>(msg2, out levelSize));
        points = new EditorSurfacePoint[levelSize[0] * levelSize[2]];
        int index = 0;
        EditorSurfacePoint editorSurfacePoint;
        GameObject objToSpawn;
        for (int x = 0; x < levelSize[0]; x++)
        {
            for (int z = 0; z < levelSize[2]; z++)
            {
                objToSpawn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                objToSpawn.GetComponent<Renderer>().material = cubeMaterial;
                objToSpawn.transform.localScale = new Vector3(objToSpawn.transform.localScale.x * (blockLength - 0.001f), 0.001f, objToSpawn.transform.localScale.z * (blockLength - 0.001f));
                objToSpawn.transform.parent = transform;
                objToSpawn.transform.rotation = transform.rotation;
                objToSpawn.transform.position = new Vector3(transform.position.x + blockLength * x, transform.position.y - blockLength / 2, transform.position.z + blockLength * z);
                editorSurfacePoint = objToSpawn.AddComponent<EditorSurfacePoint>();
                editorSurfacePoint.EditorSurface = transform;
                editorSurfacePoint.SetPosition(x, z);
                editorSurfacePoint.BlockLength = blockLength;
                points[index] = editorSurfacePoint;
                index++;
            }
        }
        readySurface = true;
        //centerOffset = new Vector3((levelSize[0] * blockLength) / 2, (levelSize[1] * blockLength) / 2, (levelSize[2] * blockLength) / 2);
    }

    private void ResetEditorSurface(MsgResetEditorSurface msg)
    {
        if (readySurface)
        {
            int index = 0;
            for (int x = 0; x < levelSize[0]; x++)
            {
                for (int z = 0; z < levelSize[2]; z++)
                {
                    points[index].gameObject.transform.parent = transform;
                    //points[index].gameObject.transform.rotation = transform.rotation;
                    points[index].gameObject.transform.position = new Vector3(transform.position.x + blockLength * x, transform.position.y + blockLength / 2, transform.position.z + blockLength * z);
                    points[index].ResetBox();
                    points[index].SetPosition(x, z);
                    points[index].BlockLength = blockLength;
                    points[index].EditorSurface = transform;
                    index++;
                }
            }
        }
    }

    public EditorSurfacePoint GetClosestSurfacePoint(Vector3 targetPosition, float maxDistance)
    {
        EditorSurfacePoint closest = null;
        if (points != null && points.Length > 0)
        {
            float distanceBetweenClosestAndTarget = 0;
            float currentDistance = 0;
            closest = points[0];
            distanceBetweenClosestAndTarget = Vector3.Distance(points[0].gameObject.transform.position, targetPosition);

            foreach (EditorSurfacePoint point in points)
            {
                currentDistance = Vector3.Distance(point.gameObject.transform.position, targetPosition);
                if (currentDistance < distanceBetweenClosestAndTarget)
                {
                    distanceBetweenClosestAndTarget = currentDistance;
                    closest = point;
                }
            }

            if (distanceBetweenClosestAndTarget > maxDistance)
            {
                closest = null;
            }
        }
        return closest;
    }
}