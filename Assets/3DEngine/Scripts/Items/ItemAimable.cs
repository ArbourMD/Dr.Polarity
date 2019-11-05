using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public abstract class ItemAimable : ItemFinite
{
    public new ItemAimableData Data { get { return (ItemAimableData)data; } }
    protected Transform muzzle;
    public Transform Muzzle { get { return muzzle; } }
    protected PlayerController playerController;
    protected UnitController unitController;

    protected LineRenderer previewLine;

    protected override void Start()
    {
        base.Start();
        if (Data.projectilePreview && !previewLine)
        {
            if (Data.previewLine)
            {
                previewLine = Instantiate(Data.previewLine);
                previewLine.transform.SetParent(transform, false, true);
            }
            
        }
            
    }

    protected override void OnDisable()
    {
        if (UIPlayer.instance && Data.aimFX)
            UIPlayer.instance.RemoveAimFXHandler(Data.aimFX);
        base.OnDisable();
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
        ShowProjectilePreview();
    }

    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        if (dropped)
            return;
        unitController = curUnitOwner.GetComponent<UnitController>();
        playerController = unitController as PlayerController;
        muzzle = transform.FindDeepChild(Data.muzzlePos);
        muzzle.forward = unitController.transform.forward;

        if (UIPlayer.instance && Data.aimFX)
            UIPlayer.instance.AddAimFXHandler(Data.aimFX);
    }

    protected virtual void ShowProjectilePreview()
    {
        if (!Data.projectilePreview)
            return;

        if (!curAmmo)
        {
            if (previewLine)
                previewLine.positionCount = 0;
            return;
        }
            
        var proj = curAmmo.Data.projectile;
        if (proj is ProjectileBulletData)
        {
            var bulData = proj as ProjectileBulletData;
            if (bulData.enableRicochet)
            {
                previewLine.positionCount = 1;

                var points = new Vector3[Data.ricochetAmount];
                points[0] = muzzle.position;
                var origin = muzzle.position;
                var dir = unitController.AimDirection;
                bool lastHit = false;
                for (int i = 1; i < points.Length; i++)
                {
                    var ray = new Ray(origin, dir);
                    if (Physics.Raycast(ray, out RaycastHit info, Data.aimDistance, Data.aimMask))
                    {
                        previewLine.positionCount++;
                        origin = info.point;
                        var reflect = Vector3.Reflect(dir, info.normal).normalized;
                        dir = reflect;
                        points[i] = origin;
                    }
                    else if (!lastHit)
                    {
                        lastHit = true;
                        previewLine.positionCount++;
                        points[i] = origin + dir * Data.aimDistance;
                    }
                }
                previewLine.SetPositions(points);
            }
        }
    }

}
