using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EngineValueCategory
{
    public string categoryName;
    public EngineValueData[] engineValueDatas;

    public string[] GetEngineValueDataNames()
    {
        string[] names = new string[engineValueDatas.Length];
        for (int i = 0; i < engineValueDatas.Length; i++)
        {
            if (engineValueDatas[i])
                names[i] = engineValueDatas[i].name;
        }
            
        return names;
    }
}
