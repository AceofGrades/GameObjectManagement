using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Game : PersistableObjects
{
    public KeyCode createKey = KeyCode.C;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;
    public PersistableObjects prefab;
    public PersistentStorage storage;
    

    private List<PersistableObjects> objects;
    private string savePath;

    private void Awake()
    {
        objects = new List<PersistableObjects>();
    }

    private void Update()
    {
        // If the Create Object Key is pressed
        if (Input.GetKeyDown(createKey))
        {
            // Spawns the prefab
            CreateObject();
        }
        // If the New Game Key is pressed
        else if (Input.GetKey(newGameKey))
        {
            BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    private void CreateObject()
    {
        PersistableObjects o  = Instantiate(prefab);
        Transform t = o.transform;
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;
        // t.Lookat(Vector3.zero);
        objects.Add(o); // Record new object to List
    }
    private void BeginNewGame()
    {
        // Loop through all list items
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i].gameObject);
        }
        // Clear the list 
        objects.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for(int i = 0; i < count; i++)
        {
            PersistableObjects o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }
}
