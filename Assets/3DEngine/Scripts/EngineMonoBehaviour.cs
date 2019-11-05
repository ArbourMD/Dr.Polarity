using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public abstract class EngineMonoBehaviour : MonoBehaviour
{
    public abstract void LateStart();

    protected virtual void Start()
    {
        Timing.RunCoroutine(StartLateStart());
    }

    IEnumerator<float> StartLateStart()
    {
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            yield return Timing.WaitForOneFrame;
            LateStart();
        }    
    }
}
