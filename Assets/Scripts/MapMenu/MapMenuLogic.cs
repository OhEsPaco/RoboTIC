using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapMenuLogic : MonoBehaviour
{
    private EventAggregator eventAggregator;
    [SerializeField] private TextAsset[] storyLevels = new TextAsset[0];
    [SerializeField] private Transform center;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private SelectArrow leftArrow;
    [SerializeField] private SelectArrow rightArrow;
    [SerializeField] private MapSelector mapSelector;
    public float arrowDistance = 4f;

    private float blockLength;

    // Start is called before the first frame update
    private MessageWarehouse msgWar;

    private Dictionary<LevelData, LevelObject[]> loadedLevels = new Dictionary<LevelData, LevelObject[]>();
    private List<LevelData> levels;

    private void Start()
    {
        eventAggregator = EventAggregator.instance;
        msgWar = new MessageWarehouse(eventAggregator);
        //DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.streamingAssetsPath);
        //Debug.Log(levelDirectoryPath.FullName);
        //FileInfo[]fileInfo="ASDf".
        /*foreach (string file in System.IO.Directory.GetFiles((System.IO.Directory.GetCurrentDirectory())))
        {
            Debug.Log(file);
        }*/

        leftArrow.InformMe(InputLeft);
        rightArrow.InformMe(InputRight);
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
    }

    // Update is called once per frame
    private int indexC = 0;

    private bool allDone = true;

    private bool firstIt = true;

    private void Update()
    {
        if (firstIt)
        {
            firstIt = false;
            allDone = false;
            StartCoroutine(CenterFirstMap(center.position, left.position, 1500f));
        }
        else
        {
            /*if (allDone)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    //StartCoroutine(ShiftLeft(indexC, 1500f));
                    StartCoroutine(ShiftMap(indexC, GetIndexRight(indexC), center.position, right.position, left.position, 1500f));
                    indexC = GetIndexRight(indexC);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    StartCoroutine(ShiftMap(indexC, GetIndexLeft(indexC), center.position, left.position, right.position, 1500f));
                    indexC = GetIndexLeft(indexC);
                }
            }*/
        }
    }

    private void InputLeft()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexRight(indexC), center.position, right.position, left.position, 1500f));
            indexC = GetIndexRight(indexC);

        }
    }

    private void InputRight()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexLeft(indexC), center.position, left.position, right.position, 1500f));
            indexC = GetIndexLeft(indexC);
        }
    }
    private void PlaceArrows(GameObject obj, float centerX, float centerY, float centerZ)
    {
        leftArrow.transform.parent = obj.transform;
        rightArrow.transform.parent = obj.transform;

        leftArrow.gameObject.SetActive(true);
        rightArrow.gameObject.SetActive(true);


        leftArrow.transform.position = new Vector3(centerX - arrowDistance, centerY, centerZ);
        rightArrow.transform.position = new Vector3(centerX + arrowDistance, centerY, centerZ);




    }
    private IEnumerator CenterFirstMap(Vector3 centerPos, Vector3 leftPos, float speed)
    {
        //Pone en el centro de la pantalla el mapa 0
       
        // eventAggregator.Publish<ResponseWrapper<MsgBlockLength, float>>(new ResponseWrapper<MsgBlockLength, float>(msg, blockLength));
        MsgBlockLength iNeedTheLength = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(iNeedTheLength);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(iNeedTheLength, out blockLength));

        LevelData centerObj = levels[0];
        yield return new WaitUntil(() => loadedLevels[centerObj] != null);
        GameObject centerParent = loadedLevels[centerObj][0].transform.parent.gameObject;

        centerParent.transform.position = leftPos;
        centerParent.SetActive(true);

        //Calculamos el offset necesario para centrarlo bien
        float xOffset = Math.Abs((blockLength * centerObj.levelSize[0]) / 2 - (blockLength / 2));
        Vector3 centerAdjusted = centerPos;
        centerAdjusted.x = centerAdjusted.x - xOffset;

        //Offset en z para las flechas
        float zOffset = Math.Abs((blockLength * centerObj.levelSize[2]) / 2 - (blockLength / 2));

        //Le colocamos las flechas
        PlaceArrows(centerParent, centerParent.transform.position.x + xOffset, centerParent.transform.position.y, centerParent.transform.position.z+zOffset);
        //Movemos el mapa desde la izquierda de la pantalla
        float distance = Vector3.Distance(leftPos, centerAdjusted);

        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            centerParent.transform.position = Vector3.Lerp(leftPos, centerAdjusted, i);
            yield return null;
        }
        centerParent.transform.position = centerAdjusted;
        leftArrow.transform.parent = gameObject.transform;
        rightArrow.transform.parent = gameObject.transform;
        mapSelector.SelectedObject = centerParent;
        allDone = true;

    }

    private IEnumerator ShiftMap(int index, int nextIndex, Vector3 centerPos, Vector3 lastPos, Vector3 nextPos, float speed)
    {
        allDone = false;
        //Tomamos el mapa actual y esperamos si no esta cargado
        LevelData centerObj = levels[index];
        yield return new WaitUntil(() => loadedLevels[centerObj] != null);

        //Sacamos el padre del mapa actual
        GameObject centerParent = loadedLevels[centerObj][0].transform.parent.gameObject;
        leftArrow.transform.parent = centerParent.transform;
        rightArrow.transform.parent = centerParent.transform;
        //Distancia entre el centro y a donde vamos a mover el mapa
        float distance = Vector3.Distance(centerParent.transform.position, nextPos);
        Vector3 initialPos = centerParent.transform.position;

        //Movemos el mapa
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            centerParent.transform.position = Vector3.Lerp(initialPos, nextPos, i);
            yield return null;
        }
        //Ajustamos la posicion por si acaso
        centerParent.transform.position = nextPos;
        //Lo desactivamos para no renderizarlo
        centerParent.SetActive(false);

        //Tomamos el siguiente objeto
        LevelData rightObj = levels[nextIndex];
        yield return new WaitUntil(() => loadedLevels[rightObj] != null);
        GameObject rightParent = loadedLevels[rightObj][0].transform.parent.gameObject;
        //Lo activamos
        rightParent.SetActive(true);
        //Movemos el objeto a la posicion de inicio
        rightParent.transform.position = lastPos;

        //Distancia entre este objeto y la posicion final
        distance = Vector3.Distance(centerPos, lastPos);

        //Calculamos el offset necesario para centrarlo bien
        float xOffset = Math.Abs((blockLength * rightObj.levelSize[0]) / 2 - (blockLength / 2));
        Vector3 centerAdjusted = centerPos;
        centerAdjusted.x = centerAdjusted.x - xOffset;

        //Offset en z para las flechas
        float zOffset = Math.Abs((blockLength * rightObj.levelSize[2]) / 2 - (blockLength / 2));

        //Le colocamos las flechas
        PlaceArrows(rightParent, rightParent.transform.position.x + xOffset, rightParent.transform.position.y, rightParent.transform.position.z + zOffset);

        //Movemos el mapa al centro
        for (float i = 0; i <= 1;)
        {
            i += ((speed * Time.deltaTime) / distance);
            rightParent.transform.position = Vector3.Lerp(lastPos, centerAdjusted, i);
            yield return null;
        }
        //Ajustamos la posicion
        rightParent.transform.position = centerAdjusted;

        leftArrow.transform.parent = gameObject.transform;
        rightArrow.transform.parent = gameObject.transform;
        mapSelector.SelectedObject = rightParent;
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

        MsgRenderMapAndItems msg = new MsgRenderMapAndItems(level.mapAndItems, level.levelSize);
        LevelObject[] loadedLevel = null;
        msgWar.PublishMsgAndWaitForResponse<MsgRenderMapAndItems, LevelObject[]>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMapAndItems, LevelObject[]>(msg, out loadedLevel));

        if (loadedLevel != null)
        {
            GameObject parent = new GameObject();

            parent.transform.position = left.position;
            parent.name = System.Guid.NewGuid().ToString();
            yield return null;
            foreach (LevelObject obj in loadedLevel)
            {
                obj.gameObject.transform.parent = parent.transform;
                
            }

            if (loadedLevels.ContainsKey(level))
            {
              
                loadedLevels[level] = loadedLevel;

                parent.SetActive(false);
            }
            else
            {
                Destroy(parent);
            }
        }
    }

    private string[] GetListOfImportedLevels()
    {
        if (!Directory.Exists(System.IO.Directory.GetCurrentDirectory() + "/UserLevels"))
        {
            Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "/UserLevels");
            return new string[0];
        }

        return System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "/UserLevels", "*.json");
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