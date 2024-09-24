using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(EditLevelManager))]
public class EditLevelEditor : Editor
{
    private int totalItems;
    private int totalItemTypes;

    public override void OnInspectorGUI()
    {
        EditLevelManager manager = (EditLevelManager)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Data"))
        {
            manager.SaveData();
        }

        GUILayout.EndHorizontal();

        totalItems = EditorGUILayout.IntField("Total Items", totalItems);
        totalItemTypes = EditorGUILayout.IntField("Total Item Types", totalItemTypes);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Auto Generate Items"))
        {
            manager.AutoGenerateItems(totalItems, totalItemTypes);
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

#endif