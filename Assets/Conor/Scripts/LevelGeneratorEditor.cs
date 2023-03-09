using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGenerator levelGen = (LevelGenerator)target;

        if(Application.isPlaying)
        {
            if(GUILayout.Button("Generate Seed")) { levelGen.GenerateSeed(); }
            if(GUILayout.Button("Generate Level")) 
            {
                levelGen.ClearLevel();

                if (levelGen.IsBlockyGeneration) { levelGen.GenerateBlockyLevel(); }
                else if (levelGen.IsBranchyGeneration) { levelGen.GenerateBranchyLevel(); }
            }
            if(GUILayout.Button("Clear Level")) { levelGen.ClearLevel(); }
        }
    }
}
