using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;

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
                    bool randNumBranches = levelGen.RandomNumOfBranches;
                    ImGui.Checkbox("Set Number of Branches?", ref randNumBranches);
                    levelGen.RandomNumOfBranches = randNumBranches;

                    if(randNumBranches)
                    {
                        int numOfBranches = levelGen.NumberOfBranches;
                        ImGui.SliderInt("Number of Branches", ref numOfBranches, 1, 4);
                        levelGen.NumberOfBranches = numOfBranches;
                    }

                    int branchSize = levelGen.BranchLength;
                    ImGui.InputInt("Branch Length", ref branchSize);
                    levelGen.BranchLength = branchSize;

                    ImGui.Spacing();

                    int offBranchChance = levelGen.OffBranchChance;
                    ImGui.SliderInt("Off Branching Chnace", ref offBranchChance, 0, 100);
                    levelGen.OffBranchChance = offBranchChance;

                    ImGui.Spacing();

                    bool doubleUp = levelGen.DoubleUpRooms;
                    ImGui.Checkbox("Double Up Rooms?", ref doubleUp);
                    levelGen.DoubleUpRooms = doubleUp;
                }

                ImGui.Spacing();

                if(ImGui.CollapsingHeader("Infomation"))
                {
                    ImGui.Text($"Level Seed: {levelGen.LevelSeed}");
                    ImGui.Text($"Number of Rooms Generated: {levelGen.RoomCount}");
                }

                

                ImGui.End();
            }
        }
    }
}
