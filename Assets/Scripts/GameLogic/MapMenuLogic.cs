using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapMenuLogic : MonoBehaviour
{
    [SerializeField] private TextAsset[] storyLevels = new TextAsset[0];
    [SerializeField] private SelectArrow leftArrow;
    [SerializeField] private SelectArrow rightArrow;
    [SerializeField] private GenericButton mainMenuButton;
    [SerializeField] private GameObject placeableMap;
    [SerializeField] private GenericButton mapBounds;

    [Range(0, 100f)]
    [SerializeField] private float arrowDistance = 0.3f;

    // Start is called before the first frame update
    private MessageWarehouse msgWar;

    private Dictionary<LevelData, LevelObject[]> loadedLevels = new Dictionary<LevelData, LevelObject[]>();
    private List<LevelData> levels;

    [Range(0f, 5000f)]
    public float speed = 1f;

    private static MapMenuLogic mapMenuLogic;

    public static MapMenuLogic Instance
    {
        get
        {
            if (!mapMenuLogic)
            {
                mapMenuLogic = FindObjectOfType(typeof(MapMenuLogic)) as MapMenuLogic;

                if (!mapMenuLogic)
                {
                    Debug.LogError("There needs to be one active MapMenuLogic script on a GameObject in your scene.");
                }
            }

            return mapMenuLogic;
        }
    }

    private void Start()
    {
        msgWar = new MessageWarehouse(EventAggregator.Instance);

        mainMenuButton.ClickCalbacks = UserClickedOnMainMenu;
        mapBounds.ClickCalbacks += UserClickedOnMap;
        leftArrow.InformMeOfClickedArrow(InputLeft);
        rightArrow.InformMeOfClickedArrow(InputRight);
        string[] storyLevelsString = new string[storyLevels.Length];

        for (int i = 0; i < storyLevels.Length; i++)
        {
            storyLevelsString[i] = storyLevels[i].ToString();
        }
        List<LevelData> storyLevelsLoaded = LoadStoryLevels(storyLevelsString);
        foreach (LevelData level in LoadImportedLevels(GetListOfImportedLevels()))
        {
            storyLevelsLoaded.Add(level);
        }

        foreach (LevelData level in storyLevelsLoaded)
        {
            if (!loadedLevels.ContainsKey(level))
            {
                loadedLevels.Add(level, null);
            }

            StartCoroutine(RenderALevel(level));
        }
        levels = storyLevelsLoaded;

        //DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.streamingAssetsPath);
        //Debug.Log(levelDirectoryPath.FullName);
        //FileInfo[]fileInfo="ASDf".
        /*foreach (string file in System.IO.Directory.GetFiles((System.IO.Directory.GetCurrentDirectory())))
        {
            Debug.Log(file);
        }*/

        /*List<LevelData> userLevelsLoaded = LoadImportedLevels(GetListOfImportedLevels());
        foreach (LevelData level in userLevelsLoaded)
        {
            if (!loadedLevels.ContainsKey(level))
            {
                loadedLevels.Add(level, null);
            }

            StartCoroutine(RenderALevel(level));
        }*/
        //eventAggregator.Publish(new MsgRenderMapAndItems(userLevelsLoaded[0].mapAndItems, userLevelsLoaded[0].levelSize));

        //SpaceCollectionManager.Instance.PlaceItemInWorld(placeableMap);
    }

    public void AddNewLevel(LevelData newLevel)
    {
        levels.Add(newLevel);
        loadedLevels.Add(newLevel, null);
        StartCoroutine(RenderALevel(newLevel));
    }

    public void ShowMapMenu()
    {
        placeableMap.GetComponent<MapController>().EnableMenuControls();
        if (!loaded)
        {
            firstIt = true;
        }
        else
        {
            LevelData centerObj = levels[indexC];
            foreach (LevelObject l in loadedLevels[centerObj])
            {
                l.gameObject.SetActive(true);
            }
        }
    }

    public void HideMapMenu()
    {
        if (loaded)
        {
            LevelData centerObj = levels[indexC];

            foreach (LevelObject l in loadedLevels[centerObj])
            {
                l.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    private int indexC = 0;

    private bool allDone = true;

    private bool firstIt = false;

    private bool loaded = false;

    private void UserClickedOnMainMenu()
    {
        //SceneManager.LoadScene("Main");
    }

    private void UserClickedOnMap()
    {
        if (allDone && !firstIt && levels[indexC] != null)
        {
            Debug.Log("User clicked");
            LevelData centerObj = levels[indexC];

            foreach (LevelObject l in loadedLevels[centerObj])
            {
                l.gameObject.SetActive(false);
            }

            GameLogic.Instance.StartLevel(centerObj, loadedLevels[centerObj][0].transform.parent.gameObject);
        }
    }

    private void Update()
    {
        if (firstIt)
        {
            loaded = true;
            firstIt = false;
            allDone = false;
            StartCoroutine(CenterFirstMap(speed));
        }
    }

    private void InputLeft()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexRight(indexC), speed));
            indexC = GetIndexRight(indexC);
        }
    }

    private void InputRight()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexLeft(indexC), speed));
            indexC = GetIndexLeft(indexC);
        }
    }

    private IEnumerator CenterFirstMap(float speed)
    {
        //Pedimos el primer mapa
        LevelData centerObj = levels[0];
        yield return new WaitUntil(() => loadedLevels[centerObj] != null);
        GameObject centerParent = loadedLevels[centerObj][0].transform.parent.gameObject;

        Vector3 lpos = placeableMap.transform.position;
        Quaternion lrot = placeableMap.transform.rotation;

        placeableMap.transform.position = new Vector3();
        placeableMap.transform.rotation = new Quaternion();

        centerParent.transform.position = new Vector3();
        centerParent.transform.rotation = new Quaternion();

        centerParent.GetComponent<MapContainer>().MoveMapTo(placeableMap.GetComponent<MapController>().MapControllerCenter);

        //Lo hacemos padre del escenario
        centerParent.transform.parent = placeableMap.transform;

        placeableMap.transform.position = lpos;
        placeableMap.transform.rotation = lrot;

        //Lo hacemos hijo del escenario
        centerParent.transform.parent = placeableMap.transform;

        //Contiene el centro del mapa
        //MapContainer mcont = centerParent.GetComponent<MapContainer>();

        // mcont.MoveMapTo(placeableMap.GetComponent<MapController>().MapControllerCenter);
        //Ponemos el escenario
        //yield return new WaitUntil(() => SpaceCollectionManager.Instance.IsReady());
        //SpaceCollectionManager.Instance.PlaceItemInWorld(placeableMap);

        Vector3 mapScale = centerParent.transform.localScale;

        centerParent.transform.localScale = new Vector3();
        centerParent.SetActive(true);

        //Hacemos grande el mapa
        Vector3 lastPos = centerParent.transform.localScale;
        float distance = Vector3.Distance(lastPos, mapScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            centerParent.transform.localScale = Vector3.Lerp(lastPos, mapScale, i);
            yield return null;
        }
        centerParent.transform.localScale = mapScale;

        allDone = true;
    }

    private IEnumerator ShiftMap(int index, int nextIndex, float speed)
    {
        allDone = false;
        //Tomamos el mapa actual y esperamos si no esta cargado
        LevelData centerObj = levels[index];
        yield return new WaitUntil(() => loadedLevels[centerObj] != null);

        //Sacamos el padre del mapa actual
        GameObject centerParent = loadedLevels[centerObj][0].transform.parent.gameObject;

        //Tomamos el siguiente objeto
        LevelData rightObj = levels[nextIndex];
        yield return new WaitUntil(() => loadedLevels[rightObj] != null);
        GameObject rightParent = loadedLevels[rightObj][0].transform.parent.gameObject;

        rightParent.SetActive(false);

        placeableMap.SetActive(false);
        Vector3 lpos = placeableMap.transform.position;
        Quaternion lrot = placeableMap.transform.rotation;

        placeableMap.transform.position = new Vector3();
        placeableMap.transform.rotation = new Quaternion();

        rightParent.transform.position = new Vector3();
        rightParent.transform.rotation = new Quaternion();

        rightParent.GetComponent<MapContainer>().MoveMapTo(placeableMap.GetComponent<MapController>().MapControllerCenter);

        //Lo hacemos padre del escenario
        rightParent.transform.parent = placeableMap.transform;

        placeableMap.transform.position = lpos;
        placeableMap.transform.rotation = lrot;

        //Lo activamos
        placeableMap.SetActive(true);

        //Hacemos pequeño el que esta
        Vector3 mapScale = centerParent.transform.localScale;
        Vector3 goalScale = new Vector3();

        float distance = Vector3.Distance(goalScale, mapScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            centerParent.transform.localScale = Vector3.Lerp(mapScale, goalScale, i);
            yield return null;
        }

        centerParent.SetActive(false);
        centerParent.transform.localScale = mapScale;

        goalScale = rightParent.transform.localScale;
        mapScale = new Vector3();

        rightParent.transform.localScale = mapScale;

        rightParent.SetActive(true);

        //Hacemos grande el mapa

        distance = Vector3.Distance(goalScale, mapScale);
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            rightParent.transform.localScale = Vector3.Lerp(mapScale, goalScale, i);
            yield return null;
        }
        rightParent.transform.localScale = goalScale;

        allDone = true;
    }

    private int GetIndexLeft(int index)
    {
        int indexLeft = index - 1;
        if (indexLeft < 0)
        {
            indexLeft = levels.Count - 1;
        }
        return indexLeft;
    }

    private int GetIndexRight(int index)
    {
        int indexRight = index + 1;
        if (indexRight >= levels.Count)
        {
            indexRight = 0;
        }
        return indexRight;
    }

    private IEnumerator RenderALevel(LevelData level)
    {
        // eventAggregator.Publish(new ResponseWrapper<MsgRenderMapAndItems, LevelObject[]>(msg, objectReferences));

        MsgRenderMapAndItems msg = new MsgRenderMapAndItems(level.mapAndItems, level.levelSize, level.goal);
        LevelObject[] loadedLevel = null;
        msgWar.PublishMsgAndWaitForResponse<MsgRenderMapAndItems, LevelObject[]>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMapAndItems, LevelObject[]>(msg, out loadedLevel));

        if (loadedLevel != null)
        {
            GameObject parent = new GameObject();
            MapContainer mcont = parent.AddComponent<MapContainer>();

            parent.transform.position = placeableMap.transform.position;
            parent.name = System.Guid.NewGuid().ToString();

            yield return null;
            foreach (LevelObject obj in loadedLevel)
            {
                obj.gameObject.transform.parent = parent.transform;
            }

            if (loadedLevels.ContainsKey(level))
            {
                foreach (LevelObject lo in loadedLevel)
                {
                    lo.gameObject.SetActive(true);
                }
                mcont.UpdateMapCenter();
                parent.SetActive(false);
                loadedLevels[level] = loadedLevel;
            }
            else
            {
                Destroy(parent);
            }
        }
    }

    private string[] GetListOfImportedLevels()
    {
        if (Application.isEditor)
        {
            if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "/UserLevels"))
            {
                Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "/UserLevels");
                return new string[0];
            }

            return System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "/UserLevels", "*.json");
        }
        else
        {
            if (!Directory.Exists(Application.persistentDataPath + "/UserLevels"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/UserLevels");
                return new string[0];
            }

            return System.IO.Directory.GetFiles(Application.persistentDataPath + "/UserLevels", "*.json");
        }
    }

    private List<LevelData> LoadStoryLevels(string[] levels)
    {
        List<LevelData> loadedLevels = new List<LevelData>();
        foreach (string level in levels)
        {
            try
            {
                LevelData levelData = new LevelData();
                JsonUtility.FromJsonOverwrite(level, levelData);
                loadedLevels.Add(levelData);
            }
            catch
            {
                Debug.LogError("Unable to load: " + level);
            }
        }
        return loadedLevels;
    }

    private List<LevelData> LoadImportedLevels(string[] files)
    {
        List<LevelData> loadedLevels = new List<LevelData>();
        foreach (string path in files)
        {
            try
            {
                LevelData levelData = new LevelData();
                string readedString = ReadFileAsString(path);
                JsonUtility.FromJsonOverwrite(readedString, levelData);
                loadedLevels.Add(levelData);
            }
            catch
            {
                Debug.LogError("Unable to load: " + path);
            }
        }
        return loadedLevels;
    }

    private string ReadFileAsString(in string path)
    {
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string output = reader.ReadToEnd();
        reader.Close();
        return output;
    }
}