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

                    if(levelGen.IsBlockyGeneration) { levelGen.GenerateBlockyLevel(); }
                    else { levelGen.GenerateBranchyLevel(); }
                }

                ImGui.SameLine();

                if(ImGui.Button("Clear Level"))
                {
                    levelGen.ClearLevel();
                }

                ImGui.Spacing();

                bool blG = levelGen.IsBlockyGeneration;
                bool brG = levelGen.IsBranchyGeneration;

                ImGui.Checkbox("Blocky Generation", ref blG);

                ImGui.SameLine();

                ImGui.Checkbox("Branchy Generation", ref brG);

                levelGen.IsBlockyGeneration = blG;
                levelGen.IsBranchyGeneration = brG;

                ImGui.Spacing();

                ImGui.Text($"Level Seed: {levelGen.GetLevelSeed}");

                ImGui.Spacing();

                if(blG)
                {
                    int mBS = levelGen.MaxBlockySize;

                    ImGui.InputInt("Max Blocky Size", ref mBS);

                    levelGen.MaxBlockySize = mBS;
                }
                else if(brG)
                {
                    int mBS = levelGen.MaxBranchySize;

                    ImGui.InputInt("Max Branchy Size", ref mBS);

                    levelGen.MaxBranchySize = mBS;
                }

                ImGui.End();
            }
        }
    }
}
