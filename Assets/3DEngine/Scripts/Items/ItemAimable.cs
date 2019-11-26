using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public abstract class ItemAimable : ItemFinite
{
    public new ItemAimableData Data { get { return (ItemAimableData)data; } }
    protected Transform muzzle;
    public Transform Muzzle { get { return muzzle; } }
    protected PlayerController playerController;
    protected UnitController unitController;

    protected LineRenderer previewLine;
    protected GameObject[] previewHits;
    protected override void Start()
    {
        base.Start();
        if (Data.projectilePreview && !previewLine)
        {
            if (Data.aimLineFX)
            {
                previewLine = Instantiate(Data.aimLineFX.linePreview);
                previewLine.transform.SetParent(transform, false, true);
            }
            
        }

        previewHits = new GameObject[Data.ricochetAmount];
            
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

            //bullet line ricochet
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
                        if (previewHits[i] == null)
                        {
                            previewHits[i] = Instantiate(Data.previewHit);
                        }
                        else
                        {
                            previewHits[i].transform.position = info.point;
                            previewHits[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, info.normal);
                        }  
                        
                    }
                    else if (!lastHit)
                    {
                        lastHit = true;
                        previewLine.positionCount++;
                        points[i] = origin + dir * Data.aimDistance;
                    }
                }
                previewLine.SetPositions(points);
                var active = previewHits.Where(x => x).ToArray();
                if (previewLine.positionCount - 2 < active.Length)
                {
                    foreach (var hit in previewHits)
                        Destroy(hit);
                    previewHits = new GameObject[Data.ricochetAmount];
                }
                    
            }

            //bullet line color
            if (Data.aimLineFX)
            {
                var hit = unitController.AimHitObject;
                if (hit)
                {
                    for (int i = 0; i < Data.aimLineFX.colorSwaps.Length; i++)
                    {
                        var swap = Data.aimLineFX.colorSwaps[i];

                        if (hit)
                        {
                            if (hit.IsInLayerMask(swap.mask))
                            {
                                if (previewLine.colorGradient != swap.color)
                                    previewLine.colorGradient = swap.color;
                            }
                                
                        }
                        
                    }
                }
                else if (previewLine.colorGradient != Data.aimLineFX.defaultColor)
                    previewLine.colorGradient = Data.aimLineFX.defaultColor;

            }
            
        }
    }

}
