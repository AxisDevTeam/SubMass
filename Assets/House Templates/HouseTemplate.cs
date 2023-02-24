using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New House", menuName = "House Templates/House Template")]

public class HouseTemplate : ScriptableObject
{
    public List<RoomTemplate> rooms = new List<RoomTemplate>();

    public int length = 10;
    public int width = 10;

    [HideInInspector]
    public bool randomSize = false;

    [HideInInspector]
    public Vector2Int lengthRange = new Vector2Int(10, 15);
    [HideInInspector]
    public Vector2Int widthRange = new Vector2Int(10, 15);
}



[CustomEditor(typeof(HouseTemplate))]
public class MyScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myScript = target as HouseTemplate;

        if (myScript.randomSize) { 
            
        }

        myScript.randomSize = EditorGUILayout.BeginToggleGroup("Random Size Range?", myScript.randomSize);
        myScript.lengthRange = EditorGUILayout.Vector2IntField("Length Range:", myScript.lengthRange);
        myScript.widthRange = EditorGUILayout.Vector2IntField("Width Range:", myScript.widthRange);
        EditorGUILayout.EndToggleGroup();

    }
}