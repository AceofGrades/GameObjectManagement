using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObjects
{
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        
        set
        {
            if(shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
    }
    int shapeId = int.MinValue;

    public int MaterialId
    {
        get; private set;
    }

    public void SetMaterial(Material material, int materialId)
    {
        GetComponent<MeshRenderer>().material = material;
        MaterialId = materialId;
    }

    Color color;

    public void SetColor(Color color)
    {
        this.color = color;
        GetComponent<MeshRenderer>().material.color = color;
    }
}
