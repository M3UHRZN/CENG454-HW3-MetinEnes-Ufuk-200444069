using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugDamager))]
public class DebugDamagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("💥 Deal Damage", GUILayout.Height(35)))
        {
            var damager = (DebugDamager)target;
            damager.DealDamage();
        }
        GUI.backgroundColor = Color.white;
    }
}
