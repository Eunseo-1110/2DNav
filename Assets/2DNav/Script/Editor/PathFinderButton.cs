using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//��ó: https://wergia.tistory.com/165 [������ ���α׷��� ��Ʈ]
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
