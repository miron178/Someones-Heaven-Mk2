using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using Unity.Collections.LowLevel.Unsafe;

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

                    levelGen.GenerateLevel();
                }

                ImGui.SameLine();

                if(ImGui.Button("Clear Level"))
                {
                    levelGen.ClearLevel();
                }

                ImGui.Spacing();

                if(ImGui.CollapsingHeader("Settings"))
                {
                    bool mNOBFB = levelGen.ManualNOBFB;
                    ImGui.Checkbox("Set Number of Branches from Base?", ref mNOBFB);
                    levelGen.ManualNOBFB = mNOBFB;

                    if(mNOBFB)
                    {
                        int nOBFB = levelGen.NumberOfBranchesFromBase;
                        ImGui.SliderInt("Number of Branches from Base", ref nOBFB, 1, 4);
                        levelGen.NumberOfBranchesFromBase = nOBFB;
                    }

                    int mBS = levelGen.MaxBranchySize;
                    ImGui.InputInt("Max Branch Size", ref mBS);
                    levelGen.MaxBranchySize = mBS;

                    ImGui.Spacing();

                    int oBC = levelGen.OffBranchChance;
                    ImGui.SliderInt("Off Branching Chance", ref oBC, 0, 100);
                    levelGen.OffBranchChance = oBC;

                    ImGui.Spacing();

                    bool aC = levelGen.AllowConnections;
                    ImGui.Checkbox("Allow Connections?", ref aC);
                    levelGen.AllowConnections = aC;
                }

                ImGui.Spacing();

                if(ImGui.CollapsingHeader("Infomation"))
                {
                    ImGui.Text($"Level Seed: {levelGen.GetLevelSeed}");
                    ImGui.Text($"Number of Rooms Generated: {levelGen.GetFloorCount}");
                }

                

                ImGui.End();
            }
        }
    }
}
