using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridCreator))]
public class GridCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridCreator t_gridCreator = (GridCreator)target;

        if (!target)
            return;

        GUILayout.Space(15);

        if (GUILayout.Button("Create Grid"))
            t_gridCreator.CreateGrid();

        if (GUILayout.Button("Recreate Grid"))
            t_gridCreator.RecreateGrid();

        if (GUILayout.Button("Delete Grid"))
            t_gridCreator.DeleteGrid();
    }
}
