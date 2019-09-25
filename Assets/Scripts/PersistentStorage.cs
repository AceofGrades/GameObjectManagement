﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentStorage : MonoBehaviour
{
    string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void Save(PersistableObjects o)
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            o.Save(new GameDataWriter(writer));
        }
    }

    public void Load(PersistableObjects o)
    {
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            o.Load(new GameDataReader(reader));
        }
    }
}
