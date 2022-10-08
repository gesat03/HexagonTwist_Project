using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridEditor : EditorWindow
{

    [MenuItem("Window/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<GridEditor>("Grid Editor");
    }


    private void OnGUI()
    {

        GUILayout.Label("This field will be done if it necessary...", EditorStyles.boldLabel);

    }

}
