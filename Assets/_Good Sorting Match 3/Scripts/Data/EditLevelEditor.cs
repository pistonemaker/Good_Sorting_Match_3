using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(EditLevelManager))]
public class EditLevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditLevelManager manager = (EditLevelManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Data"))
        {
            manager.SaveData();
        }

        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}

#endif