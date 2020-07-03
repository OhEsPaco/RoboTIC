// MapMenuLogic.cs
// Francisco Manuel García Sánchez - Belmonte
// 2020

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Esta clase <see cref="MapMenuLogic" /> contiene la lógica del menú de mapas del juego.
/// </summary>
public class MapMenuLogic : MonoBehaviour
{
    /// <summary>
    /// Los niveles por defecto que incluye el juego.
    /// </summary>
    [SerializeField] private TextAsset[] storyLevels = new TextAsset[0];

    /// <summary>
    /// La flecha de pasar los mapas hacia la izquierda.
    /// </summary>
    [SerializeField] private SelectArrow leftArrow;

    /// <summary>
    /// La flecha de pasar los mapas hacia la derecha.
    /// </summary>
    [SerializeField] private SelectArrow rightArrow;

    /// <summary>
    /// Botón para volver al menú principal.
    /// </summary>
    [SerializeField] private GenericButton mainMenuButton;

    /// <summary>
    /// GameObject del escenario.
    /// </summary>
    [SerializeField] private GameObject placeableMap;

    /// <summary>
    /// Botón para elegir mapa.
    /// </summary>
    [SerializeField] private GenericButton mapBounds;

    /// <summary>
    /// Instancia de la clase MessageWarehouse.
    /// </summary>
    private MessageWarehouse msgWar;

    /// <summary>
    /// Diccionario que guarda referencias a los niveles cargados.
    /// </summary>
    private Dictionary<LevelData, LevelObject[]> loadedLevels = new Dictionary<LevelData, LevelObject[]>();

    /// <summary>
    /// Los niveles del juego.
    /// </summary>
    private List<LevelData> levels;

    /// <summary>
    /// Longitud del lado de un bloque.
    /// </summary>
    private float blockLength;

    /// <summary>
    /// Numero del mapa en el que nos encontramos.
    /// </summary>
    private int indexC = 0;

    /// <summary>
    /// ¿Se han terminado todas las acciones?
    /// </summary>
    private bool allDone = true;

    /// <summary>
    /// ¿Primera iteración del método Update?
    /// </summary>
    private bool firstIt = false;

    /// <summary>
    /// ¿Se ha cargado el menú?
    /// </summary>
    private bool loaded = false;

    /// <summary>
    /// Velocidad a la que se ejecutan las acciones.
    /// </summary>
    [Range(0f, 5000f)]
    public float speed = 1f;

    /// <summary>
    /// Instancia de la clase a la que pueden acceder otros objetos.
    /// </summary>
    private static MapMenuLogic mapMenuLogic;

    /// <summary>
    /// Retorna la instancia de la clase.
    /// </summary>
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

    /// <summary>
    /// Start.
    /// </summary>
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

        StartCoroutine(RenderAllLevels(storyLevelsLoaded));
    }

    /// <summary>
    /// Renderiza todos los niveles.
    /// </summary>
    /// <param name="storyLevelsLoaded">La lista de los niveles a cargar<see cref="List{LevelData}"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator RenderAllLevels(List<LevelData> storyLevelsLoaded)
    {
        MsgBlockLength msg = new MsgBlockLength();
        msgWar.PublishMsgAndWaitForResponse<MsgBlockLength, float>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgBlockLength, float>(msg, out blockLength));

        foreach (LevelData level in storyLevelsLoaded)
        {
            if (!loadedLevels.ContainsKey(level))
            {
                loadedLevels.Add(level, null);
            }

            StartCoroutine(RenderALevel(level));
        }
        levels = storyLevelsLoaded;
    }

    /// <summary>
    /// Añade un nuevo nivel a la lista.
    /// </summary>
    /// <param name="newLevel">The newLevel<see cref="LevelData"/>.</param>
    public void AddNewLevel(LevelData newLevel)
    {
        levels.Add(newLevel);
        loadedLevels.Add(newLevel, null);
        StartCoroutine(RenderALevel(newLevel));
    }

    /// <summary>
    /// Muestra el menú de niveles.
    /// </summary>
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

    /// <summary>
    /// Esconde el menú de niveles.
    /// </summary>
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

    /// <summary>
    /// El usuario ha hecho click en el botón de volver al menú principal.
    /// </summary>
    private void UserClickedOnMainMenu()
    {
        //Do nothing
    }

    /// <summary>
    /// El jugador ha seleccionado un mapa.
    /// </summary>
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

    /// <summary>
    /// Update.
    /// </summary>
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

    /// <summary>
    /// Mover los mapas hacia la izquierda.
    /// </summary>
    private void InputLeft()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexRight(indexC), speed));
            indexC = GetIndexRight(indexC);
        }
    }

    /// <summary>
    /// Mover los mapas hacia la derecha.
    /// </summary>
    private void InputRight()
    {
        if (!firstIt && allDone)
        {
            allDone = false;
            StartCoroutine(ShiftMap(indexC, GetIndexLeft(indexC), speed));
            indexC = GetIndexLeft(indexC);
        }
    }

    /// <summary>
    /// Centra el primer mapa.
    /// </summary>
    /// <param name="speed">Velocidad a la que aparece el mapa<see cref="float"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator CenterFirstMap(float speed)
    {
        //Pedimos el primer mapa
        LevelData centerObj = levels[0];
        yield return new WaitUntil(() => loadedLevels[centerObj] != null);
        GameObject centerParent = loadedLevels[centerObj][0].transform.parent.gameObject;

        //Lo hacemos hijo del escenario
        centerParent.transform.parent = placeableMap.transform;
        centerParent.transform.position = new Vector3();
        centerParent.transform.localRotation = new Quaternion();

        Vector3 mapCenter = placeableMap.GetComponent<MapController>().MapControllerCenter;

        centerParent.GetComponent<MapContainer>().MoveMapTo(mapCenter, placeableMap.transform.position.y, blockLength);

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

    /// <summary>
    /// Cambia el mapa en pantalla por otro de la lista.
    /// </summary>
    /// <param name="index">El indice del mapa actual<see cref="int"/>.</param>
    /// <param name="nextIndex">El indice del mapa al que moverse<see cref="int"/>.</param>
    /// <param name="speed">La velocidad del cambio<see cref="float"/>.</param>
    /// <returns>The <see cref="IEnumerator"/>.</returns>
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

        rightParent.transform.parent = placeableMap.transform;

        rightParent.transform.position = new Vector3();
        rightParent.transform.localRotation = new Quaternion();
        Vector3 mapCenter = placeableMap.GetComponent<MapController>().MapControllerCenter;

        rightParent.GetComponent<MapContainer>().MoveMapTo(mapCenter, placeableMap.transform.position.y, blockLength);

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
        centerParent.transform.parent = transform;
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

    /// <summary>
    /// Retorna el indice del mapa a la izquierda.
    /// </summary>
    /// <param name="index">El indice actual<see cref="int"/>.</param>
    /// <returns>El indice del mapa a la izquierda <see cref="int"/>.</returns>
    private int GetIndexLeft(int index)
    {
        int indexLeft = index - 1;
        if (indexLeft < 0)
        {
            indexLeft = levels.Count - 1;
        }
        return indexLeft;
    }

    /// <summary>
    /// Retorna el indice del mapa a la derecha.
    /// </summary>
    /// <param name="index">El indice actual<see cref="int"/>.</param>
    /// <returns>El indice del mapa a la derecha <see cref="int"/>.</returns>
    private int GetIndexRight(int index)
    {
        int indexRight = index + 1;
        if (indexRight >= levels.Count)
        {
            indexRight = 0;
        }
        return indexRight;
    }

    /// <summary>
    /// Toma la estructura de datos de un nivel y genera los objetos apropiados.
    /// </summary>
    /// <param name="level">La estructura del nivel<see cref="LevelData"/>.</param>
    /// <returns><see cref="IEnumerator"/>.</returns>
    private IEnumerator RenderALevel(LevelData level)
    {
        GameObject parent = new GameObject();
        MsgRenderMapAndItems msg = new MsgRenderMapAndItems(level.mapAndItems, level.levelSize, level.goal, parent);
        LevelObject[] loadedLevel = null;
        msgWar.PublishMsgAndWaitForResponse<MsgRenderMapAndItems, LevelObject[]>(msg);
        yield return new WaitUntil(() => msgWar.IsResponseReceived<MsgRenderMapAndItems, LevelObject[]>(msg, out loadedLevel));

        if (loadedLevel != null)
        {
            MapContainer mcont = parent.AddComponent<MapContainer>();

            parent.transform.position = placeableMap.transform.position;
            parent.name = System.Guid.NewGuid().ToString();

            yield return null;

            if (loadedLevels.ContainsKey(level))
            {
                foreach (LevelObject lo in loadedLevel)
                {
                    lo.gameObject.SetActive(true);
                }
                mcont.UpdateMapCenter(level.levelSize, blockLength);
                parent.SetActive(false);
                loadedLevels[level] = loadedLevel;
            }
            else
            {
                Destroy(parent);
            }
        }
    }

    /// <summary>
    /// Carga una lista con los nombres de archivo de los mapas creados por el usuario.
    /// </summary>
    /// <returns>La lista con los nombres de archivo <see cref="string[]"/>.</returns>
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

    /// <summary>
    /// Carga los niveles predefinidos.
    /// </summary>
    /// <param name="levels">Los niveles como strings<see cref="string[]"/>.</param>
    /// <returns>Los niveles como LevelData <see cref="List{LevelData}"/>.</returns>
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

    /// <summary>
    /// Carga los niveles del usuario.
    /// </summary>
    /// <param name="files">Los nombres de archivo de los niveles<see cref="string[]"/>.</param>
    /// <returns>Los niveles cargados como LevelData <see cref="List{LevelData}"/>.</returns>
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

    /// <summary>
    /// Lee un archivo y lo retorna como un string.
    /// </summary>
    /// <param name="path">La ruta del archivo<see cref="string"/>.</param>
    /// <returns>El archivo leído <see cref="string"/>.</returns>
    private string ReadFileAsString(in string path)
    {
        StreamReader sr = new StreamReader(new FileStream(path, FileMode.Open));

        string output = sr.ReadToEnd();

        return output;
    }
}