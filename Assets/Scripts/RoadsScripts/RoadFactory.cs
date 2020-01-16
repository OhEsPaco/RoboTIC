using UnityEngine;
using static ButtonConstants;
using static RoadConstants;

public class RoadFactory : MonoBehaviour
{
    [SerializeField] private Road[] roads;

    // Start is called before the first frame update
    private void Start()
    {
        roads = GetComponentsInChildren<Road>(true);
    }

    private GameObject InstantiateObject(GameObject reference)
    {
        return Instantiate(reference, reference.transform.position, reference.transform.rotation);
    }

    public GameObject GetGameObjectInstance(in RoadType id)
    {
        foreach (Road road in roads)
        {
            if (road.RoadType == id)
            {
                GameObject reference = road.gameObject;
                return InstantiateObject(reference);
            }
        }
        return null;
    }

    public GameObject GetVerticalConnectorWithButton(in Buttons button)
    {
        GameObject road = GetGameObjectInstance(RoadType.Vertical);
        if (road != null)
        {
            int[] desiredButton = { (int)button };
            road.GetComponent<Road>().ExecuteAction(Actions.SetButton, desiredButton);
        }
        return road;
    }
}