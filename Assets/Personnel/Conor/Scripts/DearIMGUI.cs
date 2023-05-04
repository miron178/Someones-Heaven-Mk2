using UnityEngine;
using ImGuiNET;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ImGuiNET.Unity.DearImGui))]
public class DearIMGUI : MonoBehaviour
{
    bool m_uiOpen = false;

    bool m_levelGenWindowOpen = false;
    bool m_roomGenWindowOpen = false;
    bool m_allInOneGenWindowOpen = false;

    GameManager m_gameManager;
    LevelGenerator m_levelGenerator;
    RoomGenerator m_roomGenerator;
    PlayerManager m_playerManager;

    void Start()
    {
        m_gameManager = GameManager.Instance;
        m_levelGenerator = LevelGenerator.Instance;
        m_roomGenerator = RoomGenerator.Instance;
        m_playerManager = PlayerManager.Instance;
    }

    void Update()
    {
        if(m_gameManager.EnableDevMenu)
        {
            if (Keyboard.current[Key.Slash].wasPressedThisFrame)
            {
                if(!m_uiOpen) { ImGuiUn.Layout += OnLayout; m_uiOpen = true; }
                else { ImGuiUn.Layout -= OnLayout; m_uiOpen = false; }
            }
        }
    }

    void OnLayout()
    {
        ImGui.SetWindowSize(new Vector2(400, 400));

        if (!ImGui.BeginMainMenuBar()) return;
        else
        {
            if(!m_levelGenWindowOpen)
            {
                if(!m_roomGenWindowOpen)
                {
                    if(ImGui.Button("All-In-One-Generator"))
                    {
                        if(!m_allInOneGenWindowOpen) { m_allInOneGenWindowOpen = true; }
                        else { m_allInOneGenWindowOpen = false; }
                    }
                }
            }

            if(!m_allInOneGenWindowOpen)
            {
                if(ImGui.Button("Level Generator"))
                {
                    if(!m_levelGenWindowOpen) { m_levelGenWindowOpen = true; }
                    else { m_levelGenWindowOpen = false; } 
                }

                if(m_levelGenerator.RoomCount > 1)
                {
                    if(ImGui.Button("Room Generator"))
                    {
                        if(!m_roomGenWindowOpen) { m_roomGenWindowOpen = true; }
                        else { m_roomGenWindowOpen = false; }
                    }
                }
                else { if(m_roomGenWindowOpen) { m_roomGenWindowOpen = false; } }
            }

            ImGui.EndMainMenuBar();
        }

        if(m_allInOneGenWindowOpen)
        {
            if(ImGui.Begin("All-in-One Generator", ref m_allInOneGenWindowOpen))
            {
                if(m_levelGenerator.RoomCount == 0)
                {
                    if(ImGui.Button("Generate"))
                    {
                        m_levelGenerator.GenerateLevel();
                        m_roomGenerator.GenerateTraps();
                        m_levelGenerator.GenerateNavMesh();
                        m_roomGenerator.GenerateEnemies();
                        m_playerManager.SpawnPlayer();
                    }

                    ImGui.Spacing();
                    ImGui.Text("Settings");

                    if(ImGui.CollapsingHeader("Level Settings"))
                    {
                        bool manualBranches = m_levelGenerator.ManualNumOfBranches;
                        ImGui.Checkbox("Set Number of Branches?", ref manualBranches);
                        m_levelGenerator.ManualNumOfBranches = manualBranches;

                        if(manualBranches)
                        {
                            int numOfBranches = m_levelGenerator.NumberOfBranches;
                            ImGui.SliderInt("Number of Branches", ref numOfBranches, 1, 4);
                            m_levelGenerator.NumberOfBranches = numOfBranches;
                        }

                        int branchSize = m_levelGenerator.BranchLength;
                        ImGui.InputInt("Branch Length", ref branchSize);
                        m_levelGenerator.BranchLength = branchSize;

                        ImGui.Spacing();

                        int offBranchChance = m_levelGenerator.OffBranchChance;
                        ImGui.SliderInt("Off Branching Chnace", ref offBranchChance, 0, 100);
                        m_levelGenerator.OffBranchChance = offBranchChance;

                        ImGui.Spacing();

                        bool doubleUp = m_levelGenerator.DoubleUpRooms;
                        ImGui.Checkbox("Double Up Rooms?", ref doubleUp);
                        m_levelGenerator.DoubleUpRooms = doubleUp;
                    }

                    if(ImGui.CollapsingHeader("Room Settings"))
                    {
                        ImGui.Text("Traps");

                        bool setTrapAmount = m_roomGenerator.SetTrapAmount;
                        ImGui.Checkbox("Set Trap Count?", ref setTrapAmount);
                        m_roomGenerator.SetTrapAmount = setTrapAmount;

                        if(setTrapAmount)
                        {
                            int trapAmount = m_roomGenerator.TrapAmount;
                            ImGui.InputInt("Number of Traps", ref trapAmount);
                            m_roomGenerator.TrapAmount = trapAmount;
                        }
                        else
                        {
                            int minTrapAmount = m_roomGenerator.MinTrapAmount;
                            ImGui.InputInt("Minimum Number of Traps", ref minTrapAmount);
                            m_roomGenerator.MinTrapAmount = minTrapAmount;

                            int maxTrapAmount = m_roomGenerator.MaxTrapAmount;
                            ImGui.InputInt("Maximum Number of Traps", ref maxTrapAmount);
                            m_roomGenerator.MaxTrapAmount = maxTrapAmount;
                        }

                        ImGui.Spacing();
                        ImGui.Text("Enemies");

                        bool setEnemyAmount = m_roomGenerator.SetEnemyAmount;
                        ImGui.Checkbox("Set Enemy Count?", ref setEnemyAmount);
                        m_roomGenerator.SetEnemyAmount = setEnemyAmount;

                        if(setEnemyAmount)
                        {
                            int enemyAmount = m_roomGenerator.EnemyAmount;
                            ImGui.InputInt("Number of Enemies", ref enemyAmount);
                            m_roomGenerator.EnemyAmount = enemyAmount;
                        }
                        else
                        {
                            int minEnemyAmount = m_roomGenerator.MinEnemyAmount;
                            ImGui.InputInt("Minimum Number of Enemies", ref minEnemyAmount);
                            m_roomGenerator.MinEnemyAmount = minEnemyAmount;

                            int maxEnemyAmount = m_roomGenerator.MaxEnemyAmount;
                            ImGui.InputInt("Maximum Number of Enemies", ref maxEnemyAmount);
                            m_roomGenerator.MaxEnemyAmount = maxEnemyAmount;
                        }
                    }                
                }
                else
                {
                    if(ImGui.Button("Next Level"))
                    {
                        m_roomGenerator.ClearRoom();
                        m_levelGenerator.ClearLevel();

                        m_levelGenerator.GenerateLevel(true);
                        m_roomGenerator.GenerateTraps(true);
                        m_levelGenerator.GenerateNavMesh();
                        m_roomGenerator.GenerateEnemies(true);
                        m_playerManager.SpawnPlayer();
                    }

                    ImGui.SameLine();

                    if(ImGui.Button("Clear Level"))
                    {
                        m_playerManager.DeletePlayer();
                        m_roomGenerator.ClearRoom();
                        m_levelGenerator.ClearLevel();
                    }

                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        ImGui.Text($"Number of Rooms: {m_levelGenerator.RoomCount}");
                        ImGui.Text($"Number of Traps: {m_roomGenerator.TrapCount}");
                        ImGui.Text($"Number of Enemies: {m_roomGenerator.EnemyCount}");
                    }
                }
            
                ImGui.End();
            }
        }

        if(m_levelGenWindowOpen)
        {
            if(ImGui.Begin("Level Generator", ref m_levelGenWindowOpen))
            {
                if(m_levelGenerator.RoomCount == 0)
                {
                    if (ImGui.Button("Generate Level Seed")) { m_gameManager.GenerateSeed(); }

                    ImGui.SameLine();

                    if(ImGui.Button("Generate Level")) { m_levelGenerator.GenerateLevel(); }

                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Settings"))
                    {
                        bool manualBranches = m_levelGenerator.ManualNumOfBranches;
                        ImGui.Checkbox("Set Number of Branches?", ref manualBranches);
                        m_levelGenerator.ManualNumOfBranches = manualBranches;

                        if(manualBranches)
                        {
                            int numOfBranches = m_levelGenerator.NumberOfBranches;
                            ImGui.SliderInt("Number of Branches", ref numOfBranches, 1, 4);
                            m_levelGenerator.NumberOfBranches = numOfBranches;
                        }

                        int branchSize = m_levelGenerator.BranchLength;
                        ImGui.InputInt("Branch Length", ref branchSize);
                        m_levelGenerator.BranchLength = branchSize;

                        ImGui.Spacing();

                        int offBranchChance = m_levelGenerator.OffBranchChance;
                        ImGui.SliderInt("Off Branching Chnace", ref offBranchChance, 0, 100);
                        m_levelGenerator.OffBranchChance = offBranchChance;

                        ImGui.Spacing();

                        bool doubleUp = m_levelGenerator.DoubleUpRooms;
                        ImGui.Checkbox("Double Up Rooms?", ref doubleUp);
                        m_levelGenerator.DoubleUpRooms = doubleUp;
                    }

                }
                else if(m_levelGenerator.RoomCount > 1)
                {

                    if(ImGui.Button("Next Level")) 
                    { 
                        m_levelGenerator.ClearLevel();

                        if(m_roomGenerator.TrapCount > 0 || m_roomGenerator.EnemyCount > 0) { m_roomGenerator.ClearRoom(); }

                        m_levelGenerator.GenerateLevel(true);
                    }

                    if(ImGui.Button("Clear Level")) { m_levelGenerator.ClearLevel(); }

                    ImGui.SameLine();

                    if(ImGui.Button("Generate Nav Mesh")) { m_levelGenerator.GenerateNavMesh(); }

                    ImGui.SameLine();

                    if(m_levelGenerator.Player == null) 
                    { 
                        if(ImGui.Button("Spawn Player")) { m_playerManager.SpawnPlayer(); }
                    }
                    else
                    {
                        if(ImGui.Button("Delete Player")) { m_playerManager.DeletePlayer(); }
                    }

                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        ImGui.Text($"Level Seed: {m_gameManager.LevelSeed}");
                        ImGui.Text($"Number of Rooms Generated: {m_levelGenerator.RoomCount}");
                    }
                }

                ImGui.End();
            }
        }

        if(m_roomGenWindowOpen)
        {
            if(ImGui.Begin("Room Generator", ref m_roomGenWindowOpen))
            {
                if(m_roomGenerator.TrapCount == 0) { if(ImGui.Button("Generate Traps")) { m_roomGenerator.GenerateTraps(); } }
                else { if(ImGui.Button("Clear Traps")) { m_roomGenerator.ClearTraps(); } }

                ImGui.SameLine();

                if(m_roomGenerator.EnemyCount == 0) { if(ImGui.Button("Generate Enemies")) { m_roomGenerator.GenerateEnemies(); } }
                else { if(ImGui.Button("Clear Enemies")) { m_roomGenerator.ClearEnemies(); } }
                
                if(m_roomGenerator.TrapCount == 0)
                {
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Trap Settings"))
                    {
                        bool setTrapAmount = m_roomGenerator.SetTrapAmount;
                        ImGui.Checkbox("Set Trap Count?", ref setTrapAmount);
                        m_roomGenerator.SetTrapAmount = setTrapAmount;

                        if(setTrapAmount)
                        {
                            int trapAmount = m_roomGenerator.TrapAmount;
                            ImGui.InputInt("Number of Traps", ref trapAmount);
                            m_roomGenerator.TrapAmount = trapAmount;
                        }
                        else
                        {
                            int minTrapAmount = m_roomGenerator.MinTrapAmount;
                            ImGui.InputInt("Minimum Number of Traps", ref minTrapAmount);
                            m_roomGenerator.MinTrapAmount = minTrapAmount;

                            int maxTrapAmount = m_roomGenerator.MaxTrapAmount;
                            ImGui.InputInt("Maximum Number of Traps", ref maxTrapAmount);
                            m_roomGenerator.MaxTrapAmount = maxTrapAmount;
                        }
                    }
                }

                if(m_roomGenerator.EnemyCount == 0)
                {
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Enemy Settings"))
                    {
                        bool setEnemyAmount = m_roomGenerator.SetEnemyAmount;
                        ImGui.Checkbox("Set Enemy Count?", ref setEnemyAmount);
                        m_roomGenerator.SetEnemyAmount = setEnemyAmount;

                        if(setEnemyAmount)
                        {
                            int enemyAmount = m_roomGenerator.EnemyAmount;
                            ImGui.InputInt("Number of Enemies", ref enemyAmount);
                            m_roomGenerator.EnemyAmount = enemyAmount;
                        }
                        else
                        {
                            int minEnemyAmount = m_roomGenerator.MinEnemyAmount;
                            ImGui.InputInt("Minimum Number of Enemies", ref minEnemyAmount);
                            m_roomGenerator.MinEnemyAmount = minEnemyAmount;

                            int maxEnemyAmount = m_roomGenerator.MaxEnemyAmount;
                            ImGui.InputInt("Maximum Number of Enemies", ref maxEnemyAmount);
                            m_roomGenerator.MaxEnemyAmount = maxEnemyAmount;
                        }
                    }
                }

                if(m_roomGenerator.TrapCount > 1 || m_roomGenerator.EnemyCount > 1) 
                { 
                    ImGui.Spacing();

                    if(ImGui.CollapsingHeader("Infomation"))
                    {
                        if(m_roomGenerator.TrapCount > 1) { ImGui.Text($"Number of Traps: {m_roomGenerator.TrapCount}"); }
                        if(m_roomGenerator.EnemyCount > 1) { ImGui.Text($"Number of Enemies: {m_roomGenerator.EnemyCount}"); }
                    }
                }

                ImGui.End();
            }
        }
    }
}
