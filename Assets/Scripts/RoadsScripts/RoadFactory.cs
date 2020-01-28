using UnityEngine;
using static ButtonConstants;
using static RoadConstants;

public class RoadFactory : MonoBehaviour
{
    [SerializeField] private Road[] roads;
    [SerializeField] private GameObject roadScaler;
    // Start is called before the first frame update
    private void Start()
    {
        roads = GetComponentsInChildren<Road>(true);
    }

    private GameObject InstantiateObject(GameObject reference)
    {
        GameObject road= Instantiate(reference, reference.transform.position, reference.transform.rotation);
        road.transform.parent = roadScaler.transform;
        road.transform.localScale = roadScaler.transform.localScale;
        return road;
    }

    public Road GetRoadInstance(in RoadType id)
    {
        foreach (Road road in roads)
        {
            if (road.RoadType == id)
            {
                GameObject reference = road.gameObject;
                return InstantiateObject(reference).GetComponent<Road>();
            }
        }
        return null;
    }

    public Road GetVerticalConnectorWithButton(in Buttons button)
    {
        Road road = GetRoadInstance(RoadType.Vertical);
        if (road != null)
        {
            int[] desiredButton = { (int)button };
            road.ExecuteAction(Actions.SetButton, desiredButton);
        }
        return road;
    }
}