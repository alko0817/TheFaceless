using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(playerController))]
public class controllerToWindow : Editor
{
    bool showInsp = true; 
    public override void OnInspectorGUI()
    {
        if (showInsp)
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        if (GUILayout.Button("Open Player Controller"))
        {
            PlayerControllerWindow.ShowWindow();
        }
        if (GUILayout.Button("Show/Hide in Inspector (here)"))
        {
            showInsp = !showInsp;
        }

        EditorGUILayout.Space();
    }
}
