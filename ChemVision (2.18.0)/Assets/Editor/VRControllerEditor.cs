
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//      VR Controller Editor - Customizes VR Controller Inspector
//
//      Last Updated:               10/15/2017
//      Oldest Compatible Version:  2.2.0
//      Updated By:                 Kaleb Cole
//      Project:                    Chem Vision
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////

[CustomEditor(typeof(VRController))]
public class VRControllerEditor : Editor
{
    //  Privates
    private bool playerFoldout = true, prefabsFoldout = false, textFileFoldout = true;
    private VRController vrControllerScript;

    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void OnInspectorGUI()
    {
        vrControllerScript = (VRController)target;

        prefabsFoldout = EditorGUILayout.Foldout(prefabsFoldout, "Prefabs");
        if (prefabsFoldout)
        {
            serializedObject.FindProperty("INFO_PREFAB").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Information Prefab", vrControllerScript.INFO_PREFAB, typeof(GameObject), true);
            serializedObject.FindProperty("QUESTION_PREFAB").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Question Prefab", vrControllerScript.QUESTION_PREFAB, typeof(GameObject), true);
            serializedObject.FindProperty("EMPTY_PREFAB").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Empty Prefab", vrControllerScript.EMPTY_PREFAB, typeof(GameObject), true);
            EditorGUILayout.Space();
            serializedObject.FindProperty("playerVR_Prefab").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("VR Player Prefab", vrControllerScript.playerVR_Prefab, typeof(GameObject), true);
            serializedObject.FindProperty("playerNon_Prefab").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("NonVR Player Prefab", vrControllerScript.playerNon_Prefab, typeof(GameObject), true);
            EditorGUILayout.Space();
            serializedObject.FindProperty("gvrViewerMain").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("GvrViewerMain", vrControllerScript.gvrViewerMain, typeof(GameObject), true);
            serializedObject.FindProperty("eventSystem").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("EventSystem", vrControllerScript.eventSystem, typeof(GameObject), true);
            serializedObject.FindProperty("persistantObject").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("PersistantObject", vrControllerScript.persistantObject, typeof(GameObject), true);
            EditorGUILayout.Space();
        }

        textFileFoldout = EditorGUILayout.Foldout(textFileFoldout, "Text Files");
        if (textFileFoldout)
        {
            serializedObject.FindProperty("questionTextFile").objectReferenceValue = (TextAsset)EditorGUILayout.ObjectField("Question Text", vrControllerScript.questionTextFile, typeof(TextAsset), true);
            serializedObject.FindProperty("informationTextFile").objectReferenceValue = (TextAsset)EditorGUILayout.ObjectField("Information Text", vrControllerScript.informationTextFile, typeof(TextAsset), true);
        }

        playerFoldout = EditorGUILayout.Foldout(playerFoldout, "Player");
        if (playerFoldout)
        {
            serializedObject.FindProperty("playerPosition").vector3Value = EditorGUILayout.Vector3Field("Starting Position", vrControllerScript.playerPosition);
            serializedObject.FindProperty("playerRotationY").floatValue = EditorGUILayout.FloatField("Starting Y Rotation", vrControllerScript.playerRotationY);
            serializedObject.FindProperty("letPlayerMove").boolValue = EditorGUILayout.Toggle("Allow Movement", vrControllerScript.letPlayerMove);
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
}
