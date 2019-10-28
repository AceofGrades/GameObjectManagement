using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObjects
{
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;
    [SerializeField] ShapeFactory shapeFactory;
    public PersistentStorage storage;
    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    public int levelCount;
    public SpawnZone SpawnZoneOfLevel { get; set; }
    public static Game Instance { get; private set; }

    float creationProgress, destructionProgress;
    int loadedLevelBuildIndex;

    const int saveVersion = 2;
    
    private string savePath;

    List<Shape> shapes;

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        shapes = new List<Shape>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            if (loadedScene.name.Contains("Level "))
            {
                SceneManager.SetActiveScene(loadedScene);
                loadedLevelBuildIndex = loadedScene.buildIndex;
                return;
            }
        }

        StartCoroutine(LoadLevel(1));
    }

    private void Update()
    {
        // If the Create Object Key is pressed
        if (Input.GetKeyDown(createKey))
        {
            // Spawns the prefab
            CreateShape();
        }
        // If the New Game Key is pressed
        else if (Input.GetKey(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else
        {
            for (int i = 0; i < levelCount; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while(destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    private void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = SpawnZoneOfLevel.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;
        instance.SetColor(Random.ColorHSV(hueMin: 0f, hueMax: 1f, saturationMin: 0.5f, saturationMax: 1f, valueMin: 0.25f, valueMax: 1f, alphaMin: 1f, alphaMax: 1f));
        shapes.Add(instance);
    }
    private void BeginNewGame()
    {
        // Loop through all list items
        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }
        // Clear the list 
        shapes.Clear();
    }

    void DestroyShape()
    {
        if(shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(loadedLevelBuildIndex);
        for (int i = 0; i < shapes.Count; i++)
        {
            writer.Write(shapes[i].ShapeId);
            writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if(version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
            return;
        }
        int count = version <= 0 ? -version : reader.ReadInt();
        StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        for(int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if(loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        enabled = true;
    }
}
