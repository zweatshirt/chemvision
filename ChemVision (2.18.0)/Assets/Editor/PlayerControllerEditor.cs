﻿
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      Player Controller Editor - Customizes Player Controller Inspector
//
//      Last Updated:               2/12/2018
//      Oldest Compatible Version:  2.10.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEditor;

////////////////////////////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(VRPlayerController))]
public class PlayerControllerEditor : Editor
{

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
