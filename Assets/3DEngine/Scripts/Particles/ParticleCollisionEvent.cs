using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleCollisionEvent : MonoBehaviour
{
    [Header("Optional: Send an FSM event")]
    [SerializeField] private PlayMakerFSM fsmScript = null;
    [SerializeField] private string globalEventName = default;

    public bool Collided { get; private set; }
    public GameObject CollidedGO { get; private set; }

    private CoroutineHandle collisionHandle = new CoroutineHandle();

    private ParticleSystem system;

    private void Awake()
    {
        system = GetComponent<ParticleSystem>();
        var collision = system.collision;
        collision.sendCollisionMessages = true;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (collisionHandle != null)
            Timing.KillCoroutines(collisionHandle);
        collisionHandle = Timing.RunCoroutine(StartCollided(other));
    }

    IEnumerator<float> StartCollided(GameObject go)
    {
        if (gameObject != null)
        {
            Collided = true;
            CollidedGO = go;
            if (fsmScript != null)
                fsmScript.SendEvent(globalEventName);
            //Debug.Log(CollidedGO);
            yield return Timing.WaitForOneFrame;
            Collided = false;
            CollidedGO = null;
        } 
    }
}
