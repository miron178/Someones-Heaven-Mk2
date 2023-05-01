using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;

public class DearIMGUI : MonoBehaviour
{
    bool uiOpen = false;

    bool levelGenWindowOpen = false;
    bool roomGenWindowOpen = false;

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
        LevelGenerator levelGen = LevelGenerator.Instance;
        RoomGeneration roomGen = RoomGeneration.Instance;

        if (!ImGui.BeginMainMenuBar()) return;
        else
        {
            if(ImGui.Button("Level Generator"))
            {
                levelGenWindowOpen = true;
            }

            if(levelGen.RoomCount > 1)
            {
                if(ImGui.Button("Room Generator"))
                {
                    roomGenWindowOpen = true;
                }
            }

            ImGui.EndMainMenuBar();
        }

        if(levelGenWindowOpen)
        {
            if(ImGui.Begin("Level Generator", ref levelGenWindowOpen))
            {
                if (ImGui.Button("Generate Level Seed")) { levelGen.GenerateSeed(); }

                ImGui.SameLine();

                if(ImGui.Button("Generate Level"))
                {
                    levelGen.ClearLevel();

                    levelGen.GenerateLevel();
                }

                if(levelGen.RoomCount > 1)
                {
                    ImGui.SameLine();

                    if(ImGui.Button("Clear Level")) { levelGen.ClearLevel(); }

                    if(ImGui.Button("Generate Nav Mesh")) { levelGen.GenerateNavMesh(); }

                    ImGui.SameLine();

                    if(levelGen.Player == null) 
                    { 
                        if(ImGui.Button("Spawn Player")) { levelGen.SpawnPlayer(); }
                    }
                    else
                    {
                        if(ImGui.Button("Delete Player")) { levelGen.DeletePlayer(); }
                    }
                }

                ImGui.Spacing();

                if(ImGui.CollapsingHeader("Settings"))
                {
                    bool manualBranches = levelGen.ManualNumOfBranches;
                    ImGui.Checkbox("Set Number of Branches?", ref manualBranches);
                    levelGen.ManualNumOfBranches = manualBranches;

                    if(manualBranches)
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

        if(roomGenWindowOpen)
        {
            if(ImGui.Begin("Room Generator", ref roomGenWindowOpen))
            {
                if(ImGui.Button("Generate Rooms"))
                {
                    roomGen.GenerateRooms();
                }

                ImGui.Spacing();

                if(ImGui.CollapsingHeader("Settings"))
                {
                    if(ImGui.CollapsingHeader("Traps"))
                    {
                        bool setTrapAmount = roomGen.SetTrapAmount;
                        ImGui.Checkbox("Set Trap Count?", ref setTrapAmount);
                        roomGen.SetTrapAmount = setTrapAmount;

                        if(setTrapAmount)
                        {
                            int trapAmount = roomGen.TrapAmount;
                            ImGui.InputInt("Number of Traps", ref trapAmount);
                            roomGen.TrapAmount = trapAmount;
                        }
                        else
                        {
                            int minTrapAmount = roomGen.MinTrapAmount;
                            ImGui.InputInt("Minimum Number of Traps", ref minTrapAmount);
                            roomGen.MinTrapAmount = minTrapAmount;

                            int maxTrapAmount = roomGen.MaxTrapAmount;
                            ImGui.InputInt("Maximum Number of Traps", ref maxTrapAmount);
                            roomGen.MaxTrapAmount = maxTrapAmount;
                        }
                    }

                    if(ImGui.CollapsingHeader("Enemies"))
                    {
                        bool setEnemyAmount = roomGen.SetEnemyAmount;
                        ImGui.Checkbox("Set Enemy Count?", ref setEnemyAmount);
                        roomGen.SetEnemyAmount = setEnemyAmount;

                        if(setEnemyAmount)
                        {
                            int enemyAmount = roomGen.EnemyAmount;
                            ImGui.InputInt("Number of Enemies", ref enemyAmount);
                            roomGen.EnemyAmount = enemyAmount;
                        }
                        else
                        {
                            int minEnemyAmount = roomGen.MinEnemyAmount;
                            ImGui.InputInt("Minimum Number of Enemies", ref minEnemyAmount);
                            roomGen.MinEnemyAmount = minEnemyAmount;

                            int maxEnemyAmount = roomGen.MaxEnemyAmount;
                            ImGui.InputInt("Maximum Number of Enemies", ref maxEnemyAmount);
                            roomGen.MaxEnemyAmount = maxEnemyAmount;
                        }
                    }
                }

                if(ImGui.CollapsingHeader("Infomation"))
                {
                    ImGui.Text($"Number of Traps: {roomGen.TrapCount}");
                    ImGui.Text($"Number of Enemies: {roomGen.EnemyCount}");
                }

                ImGui.End();
            }
        }
    }
}
