using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{

    private static Game instance;

    public static Game Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("Game").GetComponent<Game>();
            }

            return instance;
        }
    }


    public const int nowSaveVersion = 6;
    public const int version_1 = 1; //版本1储存的是shape的shapeId
    public const int version_2 = 2; //版本2储存的是shape的materialId
    public const int version_3 = 3; //版本3储存的是shape的颜色
    public const int version_4 = 4; //版本4储存的是loadedLevelBuildIndex
    public const int version_5 = 5; //版本5储存的是生成的随机数种子
    public const int version_6 = 6; //版本6储存的是生成毁灭的速度和进度

    [SerializeField] private ShapeFactory shapeFactory;
    public PersistenStorage storage;
    [SerializeField] private bool reSeedOnLoad;

    public KeyCode createKey = KeyCode.C;
    public KeyCode destoryKey = KeyCode.X;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;


    private List<Shape> shapes;

    private float creationSpeed;
    private float creationProgress;
    private float destructionSpeed;
    private float destructionProgress;

    private int levelCount;
    private int loadedLevelBuildIndex; //当前加载的场景的BuildIndex

    private Random.State mainRandomState;

    private void Awake()
    {
    }


    /// <summary>
    /// 为什么用start,因为Awake的时候一些东西还没有准备就绪
    /// </summary>
    private void Start()
    {
        shapes = new List<Shape>();

        mainRandomState = Random.state;

        levelCount = SceneManager.sceneCountInBuildSettings - 1; //这里暂时暂时只用排除主场景

#if UNITY_EDITOR
        //只有在Editor的情况下 一开始就有场景叠加
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name.Contains("Level"))
            {
                SceneManager.SetActiveScene(loadedScene);
                loadedLevelBuildIndex = loadedScene.buildIndex;
                return;
            }
        }
#endif

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    private void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            CreateShape();
        }
        else if (Input.GetKeyDown(destoryKey))
        {
            DestroyShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, nowSaveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        creationProgress += Time.deltaTime * creationSpeed;
        while (creationProgress >= 1)
        {
            creationProgress -= 1;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * destructionSpeed;
        while (destructionProgress >= 1)
        {
            destructionProgress -= 1;
            DestroyShape();
        }
    }

    public void ChangeCreationSpeed(float val)
    {
        creationSpeed = val;
    }

    public void ChangeDestructionSpeed(float val)
    {
        destructionSpeed = val;
    }


    private void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = GameLevel.Current.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(
            hueMin: 0, hueMax: 1
            , saturationMin: 0.5f, saturationMax: 1
            , valueMin: 0.25f, valueMax: 1
            , alphaMin: 1, alphaMax: 1));
        shapes.Add(instance);
    }

    private void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    private void BeginNewGame()
    {
        Random.state = mainRandomState;
        //这样能是种子更难被预测
        int seed = Random.Range(0, int.MaxValue) ^ (int) Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);

        creationSpeed = 0;
        UIManager.Instance.SetCreationSpeedValue(creationSpeed);
        destructionSpeed = 0;
        UIManager.Instance.SetDestructionSpeedValue(destructionSpeed);

        foreach (var obj in shapes)
        {
            shapeFactory.Reclaim(obj);
        }

        shapes.Clear();
    }


    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(creationSpeed);
        writer.Write(creationProgress);
        writer.Write(destructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
        foreach (var item in shapes)
        {
            writer.Write(item.ShapeId);
            writer.Write(item.MaterialId);
            item.Save(writer);
        }
    }


    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > nowSaveVersion)
        {
            //防止版本错误
            Debug.LogError("Unsupported future save version " + version);
            return;
        }

        StartCoroutine(LoadGame(reader));
    }

    private IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        //之前没有储存版本,现在加了,所以用负号,
        int count = version <= 0 ? -version : reader.ReadInt();

        if (version >= version_5)
        {
            Random.State state = reader.ReadRandomState();
            if (!reSeedOnLoad)
            {
                Random.state = state;
            }

        }

        if (version >= version_6)
        {
            creationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            UIManager.Instance.SetCreationSpeedValue(creationSpeed);
            destructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
            UIManager.Instance.SetDestructionSpeedValue(destructionSpeed);
        }


        yield return LoadLevel(version<version_4?1:reader.ReadInt());
        if (version >= version_5)
        {
            GameLevel.Current.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int shapedId = version >= version_1 ? reader.ReadInt() : 0;
            int materialId = version >= version_2 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapedId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    private IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }

        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }
}