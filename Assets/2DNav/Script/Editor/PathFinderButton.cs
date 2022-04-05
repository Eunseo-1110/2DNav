using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//출처: https://wergia.tistory.com/165 [베르의 프로그래밍 노트]
[CustomEditor(typeof(PathFinder))]
public class PathFinderButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathFinder generator = (PathFinder)target;
        if (GUILayout.Button("Scan"))
        {
            generator.Scan();
        }
    }
}
