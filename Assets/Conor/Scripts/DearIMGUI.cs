using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;

public class DearIMGUI : MonoBehaviour
{
    bool uiOpen = false;

    bool levelGenWindowOpen = false;

    void Update()
    {
        if (Keyboard.current[Key.Slash].wasPressedThisFrame)
        {
            if(!uiOpen) { ImGuiUn.Layout += OnLayout; uiOpen = true; }
            else { ImGuiUn.Layout -= OnLayout; uiOpen = false; }
        }
    }

    void OnLayout()
    {
        if (!ImGui.BeginMainMenuBar()) return;
        else
        {
            if(ImGui.Button("Level Generator"))
            {
                levelGenWindowOpen = true;
            }

            ImGui.EndMainMenuBar();
        }

        if(levelGenWindowOpen)
        {
            if(ImGui.Begin("Level Generator", ref levelGenWindowOpen))
            {
                LevelGenerator levelGen = LevelGenerator.Instance;

                if (ImGui.Button("Generate Level Seed"))
                {
                    levelGen.GenerateSeed();
                }

                ImGui.SameLine();

                if(ImGui.Button("Generate Level"))
                {
                    levelGen.ClearLevel();

                    levelGen.GenerateBranchyLevel();
                }

                ImGui.SameLine();

                if(ImGui.Button("Clear Level"))
                {
                    levelGen.ClearLevel();
                }

                ImGui.Spacing();

                ImGui.Text($"Level Seed: {levelGen.GetLevelSeed}");

                ImGui.Spacing();

                int mBS = levelGen.MaxBranchySize;

                ImGui.InputInt("Max Branch Size", ref mBS);

                levelGen.MaxBranchySize = mBS;

                bool aC = levelGen.AllowConnections;
                ImGui.Checkbox("Allow Connections?", ref aC);
                levelGen.AllowConnections = aC;

                ImGui.End();
            }
        }
    }
}
