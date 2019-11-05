using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Interacts/ControllerTarget")]
public class InteractFXControllerTarget : InteractFXDynamic
{
    public enum ImplementType { Add, Remove }
    public enum TargetType { Chase, Flee, Lookat }
    [SerializeField] private TargetType targetType;
    [SerializeField] private ImplementType implementType;
    [SerializeField] private SceneObjectProperty objToAdd;

    protected override void AffectObject()
    {
        var controller = affectedGameObject.GetComponent<UnitController>();
        if (controller)
        {
            var objTrans = objToAdd.GetSceneObject(sender, receiver).transform;
            if (objTrans)
            {
                if (targetType == TargetType.Chase)
                {
                    if (implementType == ImplementType.Add)
                        controller.ChaseTarget(objTrans);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveChaseTarget(objTrans);
                }
                else if (targetType == TargetType.Flee)
                {
                    if (implementType == ImplementType.Add)
                        controller.FleeTarget(objTrans);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveFleeTarget(objTrans);
                }
                else if (targetType == TargetType.Lookat)
                {
                    if (implementType == ImplementType.Add)
                        controller.LookAtTarget(objTrans);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveLookAtTarget(objTrans);
                }
            }
                
        }
    }
}
