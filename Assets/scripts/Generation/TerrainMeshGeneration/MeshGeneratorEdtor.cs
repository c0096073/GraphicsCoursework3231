using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGenerator meshGen = (MeshGenerator)target;

        if (DrawDefaultInspector())
        {
            if (meshGen.autoUpdate)
            {
                meshGen.CreateVertices();
                meshGen.UpdateMesh();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            meshGen.CreateVertices();
            meshGen.UpdateMesh();
        }
    }
}
