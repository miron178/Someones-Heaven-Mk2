using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;

public class DearIMGUI : MonoBehaviour
{
    bool uiOpen = false;

    bool levelGenWindowOpen = false;
    bool roomGenWindowOpen = false;
    bool allInOneGenWindowOpen = false;

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
        RoomGenerator roomGen = RoomGenerator.Instance;

        ImGui.SetWindowSize(new Vector2(400, 400));

        if (!ImGui.BeginMainMenuBar()) return;
        else
        {
            if(!levelGenWindowOpen)
            {
                if(!roomGenWindowOpen)
                {
                    if(ImGui.Button("All-In-One-Generator"))
                    {
                        if(!allInOneGenWindowOpen) { allInOneGenWindowOpen = true; }
                        else { allInOneGenWindowOpen = false; }
                    }
                }
            }

            if(!allInOneGenWindowOpen)
            {
                if(ImGui.Button("Level Generator"))
                {
                    if(!levelGenWindowOpen) { levelGenWindowOpen = true; }
                    else { levelGenWindowOpen = false; } 
                }

                if(levelGen.RoomCount > 1)
                {
                    if(ImGui.Button("Room Generator"))
                    {
                        if(!roomGenWindowOpen) { roomGenWindowOpen = true; }
                        else { roomGenWindowOpen = false; }
                    }
                }
                else { if(roomGenWindowOpen) { roomGenWindowOpen = false; } }
            }

            ImGui.EndMainMenuBar();
        }

        if(allInOneGenWindowOpen)
        {
            if(ImGui.Begin("All-in-One Generator", ref allInOneGenWindowOpen))
            {
                if(levelGen.RoomCount == 0)
                {
                    if(ImGui.Button("Generate"))
                    {
                        levelGen.GenerateLevel();
                        roomGen.GenerateTraps();
                        levelGen.GenerateNavMesh();
                        roomGen.GenerateEnemies();
                        levelGen.SpawnPlayer();
                    }

                    ImGui.Spacing();
                    ImGui.Text("Settings");

                    if(ImGui.CollapsingHeader("Level Settings"))
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

                    if(ImGui.CollapsingHeader("Room Settings"))
                    {
                        ImGui.Text("Traps");

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

                        ImGui.Spacing();
                        ImGui.Text("Enemies");

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
                else
                {
                    if(ImGui.Button("Next Level"))
                    {
                        roomGen.ClearRoom();
                        levelGen.ClearLevel();

                        levelGen.GenerateLevel(true);
                        roomGen.GenerateTraps(true);
                        levelGen.GenerateNavMesh();
                        roomGen.GenerateEnemies(true);
                        levelGen.SpawnPlayer();
                    }

                    ImGui.SameLine();

                    if(ImGui.Button("Clear Level"))
                    {
                        levelGen.DeletePlayer();
                        roomGen.ClearRoom();
                        levelGen.ClearLevel();
                    }

                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        ImGui.Text($"Number of Rooms: {levelGen.RoomCount}");
                        ImGui.Text($"Number of Traps: {roomGen.TrapCount}");
                        ImGui.Text($"Number of Enemies: {roomGen.EnemyCount}");
                    }
                }
            
                ImGui.End();
            }
        }

        if(levelGenWindowOpen)
        {
            if(ImGui.Begin("Level Generator", ref levelGenWindowOpen))
            {
                if(levelGen.RoomCount == 0)
                {
                    if (ImGui.Button("Generate Level Seed")) { levelGen.GenerateSeed(); }

                    ImGui.SameLine();

                    if(ImGui.Button("Generate Level")) { levelGen.GenerateLevel(); }

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

                }
                else if(levelGen.RoomCount > 1)
                {

                    if(ImGui.Button("Next Level")) 
                    { 
                        levelGen.ClearLevel();

                        if(roomGen.TrapCount > 0 || roomGen.EnemyCount > 0) { roomGen.ClearRoom(); }

                        levelGen.GenerateLevel(true);
                    }

                    if(ImGui.Button("Clear Level")) { levelGen.ClearLevel(); }

                    ImGui.SameLine();

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

                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        ImGui.Text($"Level Seed: {levelGen.LevelSeed}");
                        ImGui.Text($"Number of Rooms Generated: {levelGen.RoomCount}");
                    }
                }

                ImGui.End();
            }
        }

        if(roomGenWindowOpen)
        {
            if(ImGui.Begin("Room Generator", ref roomGenWindowOpen))
            {
                if(roomGen.TrapCount == 0) { if(ImGui.Button("Generate Traps")) { roomGen.GenerateTraps(); } }
                else { if(ImGui.Button("Clear Traps")) { roomGen.ClearTraps(); } }

                ImGui.SameLine();

                if(roomGen.EnemyCount == 0) { if(ImGui.Button("Generate Enemies")) { roomGen.GenerateEnemies(); } }
                else { if(ImGui.Button("Clear Enemies")) { roomGen.ClearEnemies(); } }
                
                if(roomGen.TrapCount == 0)
                {
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Trap Settings"))
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
                }

                if(roomGen.EnemyCount == 0)
                {
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Enemy Settings"))
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

                if(roomGen.TrapCount > 1 || roomGen.EnemyCount > 1) 
                { 
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        if(roomGen.TrapCount > 1) { ImGui.Text($"Number of Traps: {roomGen.TrapCount}"); }
                        if(roomGen.EnemyCount > 1) { ImGui.Text($"Number of Enemies: {roomGen.EnemyCount}"); }
                    }
                }

                ImGui.End();
            }
        }
    }
}
