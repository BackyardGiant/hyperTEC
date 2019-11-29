using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGenerator genTerrain = (TerrainGenerator)target;
        if (GUILayout.Button("Generate Terrain"))
        {
            genTerrain.generate();
        }
        if (GUILayout.Button("Clear Terrain"))
        {
            genTerrain.clear();
        }

    }
}
