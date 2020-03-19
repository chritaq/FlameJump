using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialougeManagerV2))]
public class DialougeManagerV2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DialougeManagerV2 myScript = (DialougeManagerV2)target;
        if (GUILayout.Button("Start Test Dialouge"))
        {
            myScript.StartDialouge(myScript.testDialouge);
        }
        if (GUILayout.Button("Next Sentence"))
        {
            myScript.DisplayNextSentence();
        }
    }
}

