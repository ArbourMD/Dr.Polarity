using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MEC;

public class DetectZoneLink : DetectZoneTrigger
{
    [SerializeField] private bool sendFSMEvent;
    [SerializeField] private PlayMakerFSM fsmScript = null;
    [SerializeField] private string enteredGlobalEventName = default;
    [SerializeField] private string exitedGlobalEventName = default;

    private struct GameObjectWrapper
    {
        public GameObject gameObjectValue;
    }

    private GameObjectWrapper entered;
    private GameObjectWrapper exited;

    public GameObject EnteredObject { get { return entered.gameObjectValue; } }
    public GameObject ExitObject { get { return exited.gameObjectValue; } }

    public GameObject ClosestEnteredObject()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        for (int i = 0; i < EnteredColliders.Length; i++)
        {
            var go = EnteredColliders[i].gameObject;
            var dist = Vector3.Distance(go.transform.position, transform.position);
            if (dist < distance)
            {
                closest = go;
                distance = dist;
            }
        }
        return closest;
    }

    protected override void OnEnter(Collider _col)
    {
        base.OnEnter(_col);
        entered.gameObjectValue = _col.gameObject;
        if (sendFSMEvent && fsmScript != null)
            fsmScript.SendEvent(enteredGlobalEventName);
        Timing.RunCoroutine(NullAfterOneFrame(entered).CancelWith(gameObject));
    }

    protected override void OnExit(Collider _col)
    {
        base.OnExit(_col);
        exited.gameObjectValue = _col.gameObject;
        if (sendFSMEvent && fsmScript != null)
            fsmScript.SendEvent(exitedGlobalEventName);
        Timing.RunCoroutine(NullAfterOneFrame(exited).CancelWith(gameObject));
    }

    IEnumerator<float> NullAfterOneFrame(GameObjectWrapper _go)
    {
        yield return Timing.WaitForOneFrame;
        _go.gameObjectValue = null;
    }
        
}
