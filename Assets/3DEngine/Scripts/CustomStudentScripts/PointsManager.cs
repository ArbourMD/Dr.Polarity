using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public EngineFloatData pointsData;
    public float CurPoints { get { return pointsData.FloatValue; } }
    public bool dontDestroyOnLoad;

    public static PointsManager instance;

    private void Awake()
    {
        instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this);
    }

    public void PointsDelta(float _amount)
    {
        var amount = (float)pointsData.Value;
        amount += _amount;
        pointsData.Value = amount;
    }
}
