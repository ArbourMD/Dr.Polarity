using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DetectZoneTrigger : MonoBehaviour
{
    [SerializeField] protected DetectZone detectZone;
    public DetectZone DetectZone { get { return detectZone; } }

    public bool IsDetected { get { return enteredCols.Count > 0; } }

    private List<Collider> enteredCols = new List<Collider>();

    public Collider[] EnteredColliders { get { return detectZone.EnteredColliders; } }

    protected virtual void Start()
    {
        detectZone.AddEnterTrigger(gameObject, OnEnter);
        detectZone.AddStayTrigger(gameObject, OnStay);
        detectZone.AddExitTrigger(gameObject, OnExit);
    }

    protected virtual void OnEnter(Collider _col)
    {
        if (!enteredCols.Contains(_col))
            enteredCols.Add(_col);
    }

    protected virtual void OnStay(Collider _col)
    {
    }

    protected virtual void OnExit(Collider _col)
    {
        if (enteredCols.Contains(_col))
            enteredCols.Remove(_col);
    }

}
