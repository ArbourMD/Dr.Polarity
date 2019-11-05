using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EngineEventOptionController : EngineEventOption
{
    public enum AffectControllerType { Target, State };
    public enum TargetType { Chase, Flee, LookAt };
    public enum ImplementType { Add, Remove }

    public AffectControllerType affectController;
    //state
    public UnitController.MovementStateType state;
    //target
    public TargetType targetType;
    public ImplementType implementType;

    public override void DoEvent(EngineEvent _event)
    {
        base.DoEvent(_event);
        var controller = _event.Source.GetComponent<UnitController>();
        if (controller)
        {
            if (affectController == AffectControllerType.Target)
            {
                if (targetType == TargetType.Chase)
                {
                    if (implementType == ImplementType.Add)
                        controller.ChaseTarget(objToUse.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveChaseTarget(objToUse.transform);
                }
                else if (targetType == TargetType.Flee)
                {
                    if (implementType == ImplementType.Add)
                        controller.FleeTarget(objToUse.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveFleeTarget(objToUse.transform);
                }
                else if (targetType == TargetType.LookAt)
                {
                    if (implementType == ImplementType.Add)
                        controller.LookAtTarget(objToUse.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveLookAtTarget(objToUse.transform);
                }
            }
            else if (affectController == AffectControllerType.State)
            {
                controller.SetCurMovementState(state);
            }
        }
    }
}
