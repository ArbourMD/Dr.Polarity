using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.AI;

public class ItemWeaponPush : ItemFinite
{
    public new ItemWeaponPushData Data { get { return (ItemWeaponPushData)data; } }
    [SerializeField] private InputProperty fireButton = null;

    private bool pushed;

    private void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        if (IsPaused)
            return;

        if (inputType == UnitEquip.InputType.None)
            return;

        if (fireButton.GetInputDown())
            Use();
    }

    protected override void DoItemUse()
    {
        pushed = false;
        DetectObjects();
        curAmmo.ShootAmmo(transform.position, Vector3.zero, transform, 0, 0, 0, 0);
        base.DoItemUse();
    }

    void DetectObjects()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, Data.radius, Data.affectedMask);

        for (int i = 0; i < cols.Length; i++)
        {
            var pos = cols[i].transform.position;
            var diff = pos - transform.position;
            var dist = diff.magnitude;
            var dir = diff.normalized;
            bool valid = !Physics.Linecast(transform.position, pos + (Vector3.up * 0.5f), Data.obstacleMask);
            if (Data.setAngle)
            {
                var dirFlat = dir;
                dirFlat.y = 0;
                valid = Vector3.Dot(dirFlat, curUnitOwner.transform.forward) > Mathf.Cos(Data.angle * 0.5f * Mathf.Deg2Rad);
            }
            if (valid)
            {
                if (Data.XZOnly)
                    dir.y = 0;
                PushObject(cols[i], dir, dist);
            }

        }

    }

    void PushObject(Collider _col, Vector3 _direction, float _distance)
    {
        var cont = _col.GetComponent<UnitController>();
        if (cont)
        {
            cont.DisableNavAgent(Data.disableNavTime, false, true);
        }
        var rb = _col.GetComponent<Rigidbody>();
        if (rb)
        {
            var perc = _distance / Data.radius;
            var force = Data.force * Data.fallOffCurve.Evaluate(perc);
            _direction.y += Data.upwardForce;
            rb.AddForce(_direction * force, ForceMode.Impulse);

            var points = _col.GetComponent<PointsNode>();
            if (points)
                points.DoPointsDelta();
        }
        if (_col.gameObject.layer.IsInMask(Data.recoilMask) && !pushed)
        {
            var ownRB = curUnitOwner.GetComponent<Rigidbody>();
            if (ownRB)
            {
                var dir = -_direction;
                dir.y = 0;
                ownRB.AddForce(dir * Data.recoilForce, ForceMode.Impulse);
                pushed = true;
            }
        }


    }

}
