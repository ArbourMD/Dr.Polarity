using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EngineEventOptionController : EngineEventOption
{
    public enum AffectControllerType { Target, State, Properties };
    public enum PropertyType { Speed, SpeedMultiplier, EnableMovement, EnableJump };
    public enum TargetType { Chase, Flee, LookAt };
    public enum ImplementType { Add, Remove }

    public AffectControllerType affectController;
    //state
    public UnitController.MovementStateType state;

    //target
    public TargetType targetType;
    public ImplementType implementType;

    //property
    public PropertyType propertyType;
    public float value;
    public bool enabled;

    public override void DoEvent(EngineEvent _event)
    {
        base.DoEvent(_event);

        var controller = _event.CurTarget.GetComponent<UnitController>();
        if (controller)
        {
            if (affectController == AffectControllerType.Target && _event.Visitor)
            {
                if (targetType == TargetType.Chase)
                {
                    if (implementType == ImplementType.Add)
                        controller.ChaseTarget(_event.Visitor.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveChaseTarget(_event.Visitor.transform);
                }
                else if (targetType == TargetType.Flee)
                {
                    if (implementType == ImplementType.Add)
                        controller.FleeTarget(_event.Visitor.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveFleeTarget(_event.Visitor.transform);
                }
                else if (targetType == TargetType.LookAt)
                {
                    if (implementType == ImplementType.Add)
                        controller.LookAtTarget(_event.Visitor.transform);
                    else if (implementType == ImplementType.Remove)
                        controller.RemoveLookAtTarget(_event.Visitor.transform);
                }
            }
            else if (affectController == AffectControllerType.State)
            {
                controller.SetCurMovementState(state);
            }
            else if (affectController == AffectControllerType.Properties)
            {
                switch (propertyType)
                {
                    case PropertyType.Speed:
                        controller.SetCurSpeed(value);
                        break;
                    case PropertyType.SpeedMultiplier:
                        controller.SpeedMultiplier = value;
                        break;
                    case PropertyType.EnableMovement:
                        controller.DisableMovement(!enabled);
                        break;
                    case PropertyType.EnableJump:
                        controller.JumpEnabled = enabled;
                        break;
                }
            }
        }
    }
}
