using UnityEngine;
using UnityEditor;

public class PlayerControllerWindow : EditorWindow
{

    [MenuItem("Window/Player Controller")]
    public static void ShowWindow ()
    {
        GetWindow<PlayerControllerWindow>("Player Controller");
    }



    private void OnGUI()
    {
        GUILayout.Label("Controller", EditorStyles.boldLabel);
        GUILayout.Box("player stuff");
        
    }
}
