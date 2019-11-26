using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (DetectZoneLink))]
public class DetectZoneLinkEditor : DetectZoneTriggerEditor
{

    protected SerializedProperty sendFSMEvent;
    protected SerializedProperty fsmScript;
    protected SerializedProperty enteredGlobalEventName;
    protected SerializedProperty exitedGlobalEventName;


    protected override void GetProperties ()
    {
        base.GetProperties();
        sendFSMEvent = sourceRef.FindProperty ("sendFSMEvent");
        fsmScript = sourceRef.FindProperty ("fsmScript");
        enteredGlobalEventName = sourceRef.FindProperty ("enteredGlobalEventName");
        exitedGlobalEventName = sourceRef.FindProperty ("exitedGlobalEventName");
    }

    protected override void SetProperties ()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField (sendFSMEvent);
        if (sendFSMEvent.boolValue)
        {
            EditorGUILayout.PropertyField(fsmScript);
            EditorGUILayout.PropertyField(enteredGlobalEventName);
            EditorGUILayout.PropertyField(exitedGlobalEventName);
        }
        base.SetProperties();

    }

}