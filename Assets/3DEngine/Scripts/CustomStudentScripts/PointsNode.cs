using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsNode : MonoBehaviour
{
    public float pointsAmount;

    public void DoPointsDelta()
    {
        PointsManager.instance.PointsDelta(pointsAmount);
    }
}
