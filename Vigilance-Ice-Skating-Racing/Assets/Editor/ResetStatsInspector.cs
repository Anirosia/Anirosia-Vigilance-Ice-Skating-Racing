using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class ResetStatsInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager)target;
        if (GUILayout.Button("Reset Stats"))
            gameManager.ResetSave();
    }
}