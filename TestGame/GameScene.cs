using Roblox;

namespace TestGame;

/// <summary>
/// A beautiful medieval village scene with castle, timber-framed houses,
/// church, marketplace, and detailed landscaping.
/// </summary>
public static class GameScene
{
    // ===========================================
    // COLOR PALETTE
    // ===========================================

    // Stone colors
    static readonly Color3 LightStone = Color3.fromRGB(180, 175, 165);
    static readonly Color3 MediumStone = Color3.fromRGB(140, 135, 125);
    static readonly Color3 DarkStone = Color3.fromRGB(95, 90, 85);
    static readonly Color3 MossyStone = Color3.fromRGB(110, 120, 100);

    // Wood colors
    static readonly Color3 DarkTimber = Color3.fromRGB(75, 50, 30);
    static readonly Color3 MediumTimber = Color3.fromRGB(101, 67, 33);
    static readonly Color3 LightWood = Color3.fromRGB(160, 130, 100);
    static readonly Color3 OldWood = Color3.fromRGB(90, 75, 60);

    // Building colors
    static readonly Color3 PlasterWhite = Color3.fromRGB(250, 245, 235);
    static readonly Color3 PlasterCream = Color3.fromRGB(245, 235, 210);
    static readonly Color3 PlasterPink = Color3.fromRGB(245, 225, 215);

    // Roof colors
    static readonly Color3 RoofTerracotta = Color3.fromRGB(180, 100, 70);
    static readonly Color3 RoofBrown = Color3.fromRGB(110, 75, 55);
    static readonly Color3 RoofSlate = Color3.fromRGB(70, 75, 80);
    static readonly Color3 ThatchYellow = Color3.fromRGB(190, 170, 110);

    // Nature colors
    static readonly Color3 GrassGreen = Color3.fromRGB(65, 130, 55);
    static readonly Color3 DarkGrass = Color3.fromRGB(45, 100, 40);
    static readonly Color3 TreeGreen = Color3.fromRGB(45, 95, 35);
    static readonly Color3 TreeDark = Color3.fromRGB(30, 70, 25);
    static readonly Color3 TrunkBrown = Color3.fromRGB(80, 55, 35);
    static readonly Color3 FlowerRed = Color3.fromRGB(200, 60, 70);
    static readonly Color3 FlowerYellow = Color3.fromRGB(240, 200, 80);
    static readonly Color3 FlowerPurple = Color3.fromRGB(150, 80, 170);

    // Other colors
    static readonly Color3 Water = Color3.fromRGB(70, 140, 180);
    static readonly Color3 WaterDeep = Color3.fromRGB(45, 100, 140);
    static readonly Color3 DirtPath = Color3.fromRGB(150, 125, 95);
    static readonly Color3 Cobblestone = Color3.fromRGB(130, 130, 130);
    static readonly Color3 HayYellow = Color3.fromRGB(210, 190, 120);
    static readonly Color3 MetalDark = Color3.fromRGB(50, 50, 55);
    static readonly Color3 GoldAccent = Color3.fromRGB(220, 180, 80);
    static readonly Color3 WindowBlue = Color3.fromRGB(170, 210, 240);
    static readonly Color3 BannerRed = Color3.fromRGB(180, 40, 45);
    static readonly Color3 BannerBlue = Color3.fromRGB(45, 80, 160);

    static int blockCount = 0;

    public static void Setup()
    {
        System.Console.WriteLine("Building medieval kingdom...");

        // Terrain
        BuildTerrain();

        // Castle complex
        BuildCastle();

        // Village
        BuildVillageLayout();
        BuildHouses();
        BuildChurch();
        BuildMarketplace();
        BuildBlacksmith();
        BuildTavern();
        BuildMill();

        // Landscaping
        BuildTrees();
        BuildGardens();
        BuildDecorations();
        BuildPond();
        BuildBridge();

        System.Console.WriteLine($"Medieval kingdom complete! ({blockCount} blocks)");
    }

    static void Block(string name, Vector3 position, Vector3 size, Color3 color)
    {
        var block = new SpawnableBox(name, position, size, color);
        block.Spawn();
        blockCount++;
    }

    // ===========================================
    // TERRAIN
    // ===========================================

    static void BuildTerrain()
    {
        // Main grass field
        Block("Terrain_Grass", Vector3.@new(0, -0.5, 0), Vector3.@new(600, 1, 600), GrassGreen);

        // Slight hill behind castle
        Block("Terrain_Hill1", Vector3.@new(0, 0.5, -60), Vector3.@new(120, 2, 80), DarkGrass);
        Block("Terrain_Hill2", Vector3.@new(0, 1.5, -70), Vector3.@new(80, 2, 50), DarkGrass);

        // Main cobblestone road from castle to village
        for (int i = 0; i < 12; i++)
        {
            double z = 25 + i * 12;
            double wobble = (i % 2 == 0) ? 0.5 : -0.5;
            Block($"Road_{i}", Vector3.@new(wobble, 0.02, z), Vector3.@new(12, 0.1, 13), Cobblestone);
        }

        // Village square cobblestones
        Block("Square_Main", Vector3.@new(0, 0.02, 110), Vector3.@new(50, 0.1, 40), Cobblestone);

        // Side paths
        Block("Path_Left", Vector3.@new(-30, 0.01, 95), Vector3.@new(25, 0.1, 8), DirtPath);
        Block("Path_Right", Vector3.@new(30, 0.01, 95), Vector3.@new(25, 0.1, 8), DirtPath);
        Block("Path_Back", Vector3.@new(0, 0.01, 145), Vector3.@new(8, 0.1, 35), DirtPath);
    }

    // ===========================================
    // CASTLE
    // ===========================================

    static void BuildCastle()
    {
        double baseY = 1; // Castle sits on the hill

        // Main walls with battlements
        BuildCastleWall("Wall_N", Vector3.@new(0, baseY, -35), 70, 16, true);  // Back
        BuildCastleWall("Wall_S_L", Vector3.@new(-22, baseY, 20), 26, 16, true); // Front left
        BuildCastleWall("Wall_S_R", Vector3.@new(22, baseY, 20), 26, 16, true);  // Front right
        BuildCastleWall("Wall_E", Vector3.@new(35, baseY, -7.5), 55, 16, false); // Right (rotated)
        BuildCastleWall("Wall_W", Vector3.@new(-35, baseY, -7.5), 55, 16, false); // Left (rotated)

        // Corner towers
        BuildCastleTower("Tower_NE", Vector3.@new(35, baseY, -35), 28);
        BuildCastleTower("Tower_NW", Vector3.@new(-35, baseY, -35), 28);
        BuildCastleTower("Tower_SE", Vector3.@new(35, baseY, 20), 24);
        BuildCastleTower("Tower_SW", Vector3.@new(-35, baseY, 20), 24);

        // Gatehouse
        BuildGatehouse(Vector3.@new(0, baseY, 20));

        // Keep (main tower in center)
        BuildKeep(Vector3.@new(0, baseY, -15));

        // Courtyard details
        Block("Courtyard", Vector3.@new(0, baseY - 0.4, -5), Vector3.@new(60, 0.2, 45), MossyStone);

        // Well in courtyard
        BuildWell("Castle_Well", Vector3.@new(-15, baseY, 0));

        // Banners on towers
        Block("Banner_NE", Vector3.@new(35, baseY + 30, -35), Vector3.@new(1, 8, 0.3), BannerRed);
        Block("Banner_NW", Vector3.@new(-35, baseY + 30, -35), Vector3.@new(1, 8, 0.3), BannerBlue);
    }

    static void BuildCastleWall(string name, Vector3 pos, double length, double height, bool alongX)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;
        double thick = 4;

        Vector3 size = alongX ? Vector3.@new(length, height, thick) : Vector3.@new(thick, height, length);

        // Main wall
        Block(name, Vector3.@new(x, y + height/2, z), size, MediumStone);

        // Wall top / walkway
        Vector3 topSize = alongX ? Vector3.@new(length, 1, thick + 2) : Vector3.@new(thick + 2, 1, length);
        Block(name + "_Top", Vector3.@new(x, y + height + 0.5, z), topSize, LightStone);

        // Battlements (merlons)
        int merlonCount = (int)(length / 6);
        for (int i = 0; i < merlonCount; i++)
        {
            double offset = -length/2 + 3 + i * 6;
            double mx = alongX ? x + offset : x;
            double mz = alongX ? z : z + offset;
            // Outer merlon
            double outerOffset = alongX ? thick/2 + 0.5 : 0;
            double outerOffsetZ = alongX ? 0 : thick/2 + 0.5;
            Block($"{name}_Merlon_{i}",
                Vector3.@new(mx + (alongX ? 0 : outerOffset), y + height + 2.5, mz + (alongX ? outerOffset : 0)),
                Vector3.@new(alongX ? 2 : 1.5, 4, alongX ? 1.5 : 2),
                DarkStone);
        }
    }

    static void BuildCastleTower(string name, Vector3 pos, double height)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;
        double size = 12;

        // Main tower body
        Block(name + "_Base", Vector3.@new(x, y + height/2, z), Vector3.@new(size, height, size), DarkStone);

        // Lighter middle section
        Block(name + "_Mid", Vector3.@new(x, y + height/2, z), Vector3.@new(size - 1, height - 4, size - 1), MediumStone);

        // Top platform
        Block(name + "_Platform", Vector3.@new(x, y + height + 0.5, z), Vector3.@new(size + 2, 1, size + 2), LightStone);

        // Conical roof (simplified as stacked blocks)
        Block(name + "_Roof1", Vector3.@new(x, y + height + 3, z), Vector3.@new(10, 4, 10), RoofSlate);
        Block(name + "_Roof2", Vector3.@new(x, y + height + 6, z), Vector3.@new(6, 4, 6), RoofSlate);
        Block(name + "_Roof3", Vector3.@new(x, y + height + 9, z), Vector3.@new(2, 3, 2), RoofSlate);

        // Windows
        Block(name + "_Win1", Vector3.@new(x, y + height * 0.7, z + size/2 + 0.1), Vector3.@new(2, 4, 0.3), WindowBlue);
        Block(name + "_Win2", Vector3.@new(x + size/2 + 0.1, y + height * 0.7, z), Vector3.@new(0.3, 4, 2), WindowBlue);
    }

    static void BuildGatehouse(Vector3 pos)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;
        double gateWidth = 10;
        double gateHeight = 12;

        // Two flanking towers
        Block("Gate_TowerL", Vector3.@new(x - 9, y + 10, z), Vector3.@new(8, 20, 10), DarkStone);
        Block("Gate_TowerR", Vector3.@new(x + 9, y + 10, z), Vector3.@new(8, 20, 10), DarkStone);

        // Tower tops
        Block("Gate_TopL", Vector3.@new(x - 9, y + 21, z), Vector3.@new(10, 2, 12), LightStone);
        Block("Gate_TopR", Vector3.@new(x + 9, y + 21, z), Vector3.@new(10, 2, 12), LightStone);

        // Pointed roofs
        Block("Gate_RoofL1", Vector3.@new(x - 9, y + 24, z), Vector3.@new(8, 4, 10), RoofSlate);
        Block("Gate_RoofL2", Vector3.@new(x - 9, y + 27, z), Vector3.@new(4, 3, 6), RoofSlate);
        Block("Gate_RoofR1", Vector3.@new(x + 9, y + 24, z), Vector3.@new(8, 4, 10), RoofSlate);
        Block("Gate_RoofR2", Vector3.@new(x + 9, y + 27, z), Vector3.@new(4, 3, 6), RoofSlate);

        // Arch over gate
        Block("Gate_Arch", Vector3.@new(x, y + gateHeight + 3, z), Vector3.@new(gateWidth + 2, 6, 6), MediumStone);
        Block("Gate_ArchTop", Vector3.@new(x, y + gateHeight + 7, z), Vector3.@new(gateWidth + 4, 2, 8), LightStone);

        // Decorative elements
        Block("Gate_Crest", Vector3.@new(x, y + gateHeight + 9, z + 4.5), Vector3.@new(6, 6, 1), GoldAccent);

        // Portcullis grooves (dark lines)
        Block("Gate_GrooveL", Vector3.@new(x - gateWidth/2 + 0.3, y + gateHeight/2, z + 3), Vector3.@new(0.5, gateHeight, 0.5), MetalDark);
        Block("Gate_GrooveR", Vector3.@new(x + gateWidth/2 - 0.3, y + gateHeight/2, z + 3), Vector3.@new(0.5, gateHeight, 0.5), MetalDark);
    }

    static void BuildKeep(Vector3 pos)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;

        // Main keep body
        Block("Keep_Base", Vector3.@new(x, y + 15, z), Vector3.@new(24, 30, 20), MediumStone);
        Block("Keep_Detail", Vector3.@new(x, y + 15, z), Vector3.@new(22, 28, 18), LightStone);

        // Top battlements
        Block("Keep_Top", Vector3.@new(x, y + 31, z), Vector3.@new(26, 2, 22), DarkStone);

        // Corner turrets on keep
        Block("Keep_Turret1", Vector3.@new(x - 10, y + 20, z - 8), Vector3.@new(6, 40, 6), DarkStone);
        Block("Keep_Turret2", Vector3.@new(x + 10, y + 20, z - 8), Vector3.@new(6, 40, 6), DarkStone);
        Block("Keep_Turret3", Vector3.@new(x - 10, y + 18, z + 8), Vector3.@new(6, 36, 6), DarkStone);
        Block("Keep_Turret4", Vector3.@new(x + 10, y + 18, z + 8), Vector3.@new(6, 36, 6), DarkStone);

        // Turret roofs
        Block("Keep_TRoof1", Vector3.@new(x - 10, y + 42, z - 8), Vector3.@new(4, 6, 4), RoofSlate);
        Block("Keep_TRoof2", Vector3.@new(x + 10, y + 42, z - 8), Vector3.@new(4, 6, 4), RoofSlate);
        Block("Keep_TRoof3", Vector3.@new(x - 10, y + 38, z + 8), Vector3.@new(4, 6, 4), RoofSlate);
        Block("Keep_TRoof4", Vector3.@new(x + 10, y + 38, z + 8), Vector3.@new(4, 6, 4), RoofSlate);

        // Windows
        for (int floor = 0; floor < 3; floor++)
        {
            double wy = y + 8 + floor * 10;
            Block($"Keep_WinF{floor}", Vector3.@new(x, wy, z + 10.1), Vector3.@new(3, 5, 0.3), WindowBlue);
            Block($"Keep_WinL{floor}", Vector3.@new(x - 12.1, wy, z), Vector3.@new(0.3, 5, 3), WindowBlue);
            Block($"Keep_WinR{floor}", Vector3.@new(x + 12.1, wy, z), Vector3.@new(0.3, 5, 3), WindowBlue);
        }

        // Grand entrance
        Block("Keep_Door", Vector3.@new(x, y + 5, z + 10.1), Vector3.@new(6, 10, 0.5), DarkTimber);
        Block("Keep_DoorFrame", Vector3.@new(x, y + 5, z + 10.3), Vector3.@new(8, 12, 0.3), MediumStone);
    }

    // ===========================================
    // VILLAGE LAYOUT
    // ===========================================

    static void BuildVillageLayout()
    {
        // Village is centered around z = 110 (the square)
        // Buildings arranged organically around the square
    }

    static void BuildHouses()
    {
        // Houses around the village square
        // Left side
        BuildTimberHouse("House_L1", Vector3.@new(-40, 0, 85), 14, 11, 12, PlasterWhite, RoofTerracotta, false);
        BuildTimberHouse("House_L2", Vector3.@new(-42, 0, 105), 12, 9, 14, PlasterCream, RoofBrown, true);
        BuildTimberHouse("House_L3", Vector3.@new(-38, 0, 125), 16, 12, 13, PlasterPink, RoofTerracotta, false);

        // Right side
        BuildTimberHouse("House_R1", Vector3.@new(40, 0, 85), 13, 10, 13, PlasterCream, RoofBrown, true);
        BuildTimberHouse("House_R2", Vector3.@new(42, 0, 108), 15, 11, 12, PlasterWhite, RoofTerracotta, false);
        BuildTimberHouse("House_R3", Vector3.@new(38, 0, 130), 11, 9, 11, PlasterPink, RoofBrown, true);

        // Back of square
        BuildTimberHouse("House_B1", Vector3.@new(-18, 0, 145), 14, 10, 14, PlasterWhite, RoofTerracotta, true);
        BuildTimberHouse("House_B2", Vector3.@new(18, 0, 148), 12, 11, 12, PlasterCream, RoofBrown, false);

        // Outer village
        BuildTimberHouse("House_O1", Vector3.@new(-60, 0, 95), 10, 8, 10, PlasterCream, ThatchYellow, false);
        BuildTimberHouse("House_O2", Vector3.@new(58, 0, 140), 11, 9, 11, PlasterWhite, ThatchYellow, true);
        BuildTimberHouse("House_O3", Vector3.@new(-55, 0, 145), 13, 10, 12, PlasterPink, ThatchYellow, false);
    }

    static void BuildTimberHouse(string name, Vector3 pos, double w, double h, double d,
                                  Color3 plaster, Color3 roof, bool hasJetty)
    {
        double x = pos.X, z = pos.Z;
        double jettyOverhang = hasJetty ? 1.5 : 0;
        double firstFloorH = h * 0.55;
        double secondFloorH = h * 0.45;

        // === FIRST FLOOR ===
        Block(name + "_Floor1", Vector3.@new(x, firstFloorH/2, z), Vector3.@new(w, firstFloorH, d), plaster);

        // Timber frame - first floor
        double beam = 0.6;
        // Corner posts
        Block(name + "_Post_FL", Vector3.@new(x - w/2 + beam/2, firstFloorH/2, z + d/2 - beam/2), Vector3.@new(beam, firstFloorH, beam), DarkTimber);
        Block(name + "_Post_FR", Vector3.@new(x + w/2 - beam/2, firstFloorH/2, z + d/2 - beam/2), Vector3.@new(beam, firstFloorH, beam), DarkTimber);
        Block(name + "_Post_BL", Vector3.@new(x - w/2 + beam/2, firstFloorH/2, z - d/2 + beam/2), Vector3.@new(beam, firstFloorH, beam), DarkTimber);
        Block(name + "_Post_BR", Vector3.@new(x + w/2 - beam/2, firstFloorH/2, z - d/2 + beam/2), Vector3.@new(beam, firstFloorH, beam), DarkTimber);

        // Horizontal beams
        Block(name + "_HBeam_Bot", Vector3.@new(x, beam/2, z + d/2), Vector3.@new(w + 0.2, beam, beam), DarkTimber);
        Block(name + "_HBeam_Mid", Vector3.@new(x, firstFloorH/2, z + d/2), Vector3.@new(w + 0.2, beam, beam), DarkTimber);
        Block(name + "_HBeam_Top", Vector3.@new(x, firstFloorH, z + d/2), Vector3.@new(w + 0.2, beam, beam), DarkTimber);

        // Cross braces (X pattern on front) - simplified
        Block(name + "_X1", Vector3.@new(x - w/4, firstFloorH * 0.25, z + d/2 + 0.05), Vector3.@new(beam * 0.8, firstFloorH * 0.4, beam * 0.5), DarkTimber);
        Block(name + "_X2", Vector3.@new(x + w/4, firstFloorH * 0.25, z + d/2 + 0.05), Vector3.@new(beam * 0.8, firstFloorH * 0.4, beam * 0.5), DarkTimber);
        Block(name + "_X3", Vector3.@new(x - w/4, firstFloorH * 0.75, z + d/2 + 0.05), Vector3.@new(beam * 0.8, firstFloorH * 0.4, beam * 0.5), DarkTimber);
        Block(name + "_X4", Vector3.@new(x + w/4, firstFloorH * 0.75, z + d/2 + 0.05), Vector3.@new(beam * 0.8, firstFloorH * 0.4, beam * 0.5), DarkTimber);

        // === SECOND FLOOR (with optional jetty) ===
        double floor2W = w + jettyOverhang * 2;
        double floor2D = d + jettyOverhang * 2;
        double floor2Y = firstFloorH + secondFloorH/2;

        // Jetty support beam
        if (hasJetty)
        {
            Block(name + "_Jetty", Vector3.@new(x, firstFloorH + 0.3, z + d/2 + jettyOverhang/2),
                  Vector3.@new(floor2W + 0.5, 0.6, jettyOverhang + 0.5), DarkTimber);
        }

        Block(name + "_Floor2", Vector3.@new(x, floor2Y, z), Vector3.@new(floor2W, secondFloorH, floor2D), plaster);

        // Second floor timber frame
        Block(name + "_Post2_FL", Vector3.@new(x - floor2W/2 + beam/2, floor2Y, z + floor2D/2 - beam/2), Vector3.@new(beam, secondFloorH, beam), DarkTimber);
        Block(name + "_Post2_FR", Vector3.@new(x + floor2W/2 - beam/2, floor2Y, z + floor2D/2 - beam/2), Vector3.@new(beam, secondFloorH, beam), DarkTimber);
        Block(name + "_HBeam2_Top", Vector3.@new(x, firstFloorH + secondFloorH, z + floor2D/2), Vector3.@new(floor2W + 0.2, beam, beam), DarkTimber);

        // === ROOF ===
        double roofH = h * 0.5;
        double roofOverhang = 1.5;

        // Two-sided roof
        Block(name + "_RoofL", Vector3.@new(x - floor2W/4, firstFloorH + secondFloorH + roofH/2, z),
              Vector3.@new(floor2W/2 + roofOverhang, roofH, floor2D + roofOverhang * 2), roof);
        Block(name + "_RoofR", Vector3.@new(x + floor2W/4, firstFloorH + secondFloorH + roofH/2, z),
              Vector3.@new(floor2W/2 + roofOverhang, roofH, floor2D + roofOverhang * 2), roof);

        // Ridge
        Block(name + "_Ridge", Vector3.@new(x, firstFloorH + secondFloorH + roofH - 0.3, z),
              Vector3.@new(1.5, 0.8, floor2D + roofOverhang * 2 + 1), DarkTimber);

        // === DETAILS ===
        // Door
        Block(name + "_Door", Vector3.@new(x, 2.5, z + d/2 + 0.15), Vector3.@new(2.5, 5, 0.4), DarkTimber);
        Block(name + "_DoorFrame", Vector3.@new(x, 2.5, z + d/2 + 0.2), Vector3.@new(3, 5.5, 0.2), MediumTimber);

        // Windows
        Block(name + "_Win1", Vector3.@new(x - w/3, firstFloorH * 0.6, z + d/2 + 0.15), Vector3.@new(1.8, 2.2, 0.2), WindowBlue);
        Block(name + "_Win2", Vector3.@new(x + w/3, firstFloorH * 0.6, z + d/2 + 0.15), Vector3.@new(1.8, 2.2, 0.2), WindowBlue);
        Block(name + "_Win3", Vector3.@new(x, floor2Y, z + floor2D/2 + 0.15), Vector3.@new(2.2, 2.5, 0.2), WindowBlue);

        // Window frames
        Block(name + "_WinFrame1", Vector3.@new(x - w/3, firstFloorH * 0.6, z + d/2 + 0.2), Vector3.@new(2.2, 2.6, 0.15), DarkTimber);
        Block(name + "_WinFrame2", Vector3.@new(x + w/3, firstFloorH * 0.6, z + d/2 + 0.2), Vector3.@new(2.2, 2.6, 0.15), DarkTimber);

        // Flower box under one window
        Block(name + "_FlowerBox", Vector3.@new(x - w/3, firstFloorH * 0.6 - 1.5, z + d/2 + 0.5), Vector3.@new(2.5, 0.8, 1), OldWood);
        Block(name + "_Flowers", Vector3.@new(x - w/3, firstFloorH * 0.6 - 1, z + d/2 + 0.5), Vector3.@new(2, 0.6, 0.6), FlowerRed);

        // Chimney
        Block(name + "_Chimney", Vector3.@new(x + w/3, firstFloorH + secondFloorH + roofH + 1, z - d/4),
              Vector3.@new(2, 4, 2), DarkStone);
    }

    // ===========================================
    // SPECIAL BUILDINGS
    // ===========================================

    static void BuildChurch()
    {
        double x = 0, z = 165;

        // Main nave
        Block("Church_Nave", Vector3.@new(x, 8, z), Vector3.@new(16, 16, 30), LightStone);

        // Bell tower
        Block("Church_Tower", Vector3.@new(x, 20, z + 12), Vector3.@new(10, 40, 10), MediumStone);
        Block("Church_TowerTop", Vector3.@new(x, 42, z + 12), Vector3.@new(12, 4, 12), LightStone);

        // Spire
        Block("Church_Spire1", Vector3.@new(x, 47, z + 12), Vector3.@new(8, 8, 8), RoofSlate);
        Block("Church_Spire2", Vector3.@new(x, 53, z + 12), Vector3.@new(4, 8, 4), RoofSlate);
        Block("Church_Spire3", Vector3.@new(x, 58, z + 12), Vector3.@new(1.5, 6, 1.5), RoofSlate);
        Block("Church_Cross", Vector3.@new(x, 62, z + 12), Vector3.@new(0.5, 3, 0.5), GoldAccent);
        Block("Church_CrossArm", Vector3.@new(x, 61, z + 12), Vector3.@new(2, 0.5, 0.5), GoldAccent);

        // Roof
        Block("Church_RoofL", Vector3.@new(x - 5, 18, z), Vector3.@new(8, 6, 32), RoofSlate);
        Block("Church_RoofR", Vector3.@new(x + 5, 18, z), Vector3.@new(8, 6, 32), RoofSlate);

        // Entrance
        Block("Church_Door", Vector3.@new(x, 4, z + 15.5), Vector3.@new(4, 8, 0.5), DarkTimber);
        Block("Church_Arch", Vector3.@new(x, 9, z + 15.5), Vector3.@new(6, 3, 1), LightStone);

        // Rose window
        Block("Church_Window", Vector3.@new(x, 12, z + 15.5), Vector3.@new(5, 5, 0.3), WindowBlue);
        Block("Church_WinFrame", Vector3.@new(x, 12, z + 15.6), Vector3.@new(6, 6, 0.2), DarkStone);

        // Side windows
        for (int i = 0; i < 3; i++)
        {
            double wz = z - 8 + i * 8;
            Block($"Church_SideWinL{i}", Vector3.@new(x - 8.2, 8, wz), Vector3.@new(0.3, 6, 2), WindowBlue);
            Block($"Church_SideWinR{i}", Vector3.@new(x + 8.2, 8, wz), Vector3.@new(0.3, 6, 2), WindowBlue);
        }
    }

    static void BuildMarketplace()
    {
        // Market stalls around the square
        BuildMarketStall("Stall_1", Vector3.@new(-12, 0, 100), BannerRed);
        BuildMarketStall("Stall_2", Vector3.@new(0, 0, 100), BannerBlue);
        BuildMarketStall("Stall_3", Vector3.@new(12, 0, 100), Color3.fromRGB(80, 160, 80));

        // Central fountain/well
        BuildFountain(Vector3.@new(0, 0, 115));

        // Crates and barrels
        Block("Crate_1", Vector3.@new(-8, 1, 108), Vector3.@new(2, 2, 2), LightWood);
        Block("Crate_2", Vector3.@new(-7, 1, 110), Vector3.@new(1.5, 1.5, 1.5), OldWood);
        Block("Barrel_1", Vector3.@new(10, 1.5, 107), Vector3.@new(2, 3, 2), MediumTimber);
        Block("Barrel_2", Vector3.@new(12, 1.5, 108), Vector3.@new(2, 3, 2), MediumTimber);

        // Hay bales
        Block("Hay_1", Vector3.@new(15, 1, 103), Vector3.@new(3, 2, 2), HayYellow);
        Block("Hay_2", Vector3.@new(16, 2.5, 104), Vector3.@new(2, 1.5, 2), HayYellow);
    }

    static void BuildMarketStall(string name, Vector3 pos, Color3 canopyColor)
    {
        double x = pos.X, z = pos.Z;

        // Counter
        Block(name + "_Counter", Vector3.@new(x, 2, z), Vector3.@new(8, 1.5, 4), LightWood);
        Block(name + "_Front", Vector3.@new(x, 1, z + 2), Vector3.@new(8, 2, 0.5), OldWood);

        // Support posts
        Block(name + "_PostL", Vector3.@new(x - 3.5, 3.5, z - 1.5), Vector3.@new(0.5, 7, 0.5), MediumTimber);
        Block(name + "_PostR", Vector3.@new(x + 3.5, 3.5, z - 1.5), Vector3.@new(0.5, 7, 0.5), MediumTimber);

        // Canopy
        Block(name + "_Canopy", Vector3.@new(x, 7.5, z), Vector3.@new(10, 0.4, 6), canopyColor);

        // Goods on display (colored blocks)
        Block(name + "_Goods1", Vector3.@new(x - 2, 3, z), Vector3.@new(1.5, 0.8, 1.5), FlowerRed);
        Block(name + "_Goods2", Vector3.@new(x, 3, z), Vector3.@new(1.5, 0.8, 1.5), FlowerYellow);
        Block(name + "_Goods3", Vector3.@new(x + 2, 3, z), Vector3.@new(1.5, 0.8, 1.5), FlowerPurple);
    }

    static void BuildFountain(Vector3 pos)
    {
        double x = pos.X, z = pos.Z;

        // Base pool
        Block("Fountain_Pool", Vector3.@new(x, 1, z), Vector3.@new(10, 2, 10), MediumStone);
        Block("Fountain_Water", Vector3.@new(x, 1.8, z), Vector3.@new(8, 1, 8), Water);

        // Central pillar
        Block("Fountain_Pillar", Vector3.@new(x, 4, z), Vector3.@new(2, 6, 2), LightStone);

        // Top basin
        Block("Fountain_Basin", Vector3.@new(x, 7.5, z), Vector3.@new(4, 1.5, 4), LightStone);
        Block("Fountain_TopWater", Vector3.@new(x, 8, z), Vector3.@new(3, 0.8, 3), Water);

        // Decorative top
        Block("Fountain_Top", Vector3.@new(x, 9, z), Vector3.@new(1, 2, 1), LightStone);
    }

    static void BuildBlacksmith()
    {
        double x = 55, z = 105;

        // Main building
        Block("Smith_Building", Vector3.@new(x, 5, z), Vector3.@new(14, 10, 12), MediumStone);
        Block("Smith_Roof", Vector3.@new(x, 11, z), Vector3.@new(16, 3, 14), RoofSlate);

        // Open forge area
        Block("Smith_ForgeFloor", Vector3.@new(x + 10, 0.5, z), Vector3.@new(8, 1, 10), Cobblestone);
        Block("Smith_ForgeRoof", Vector3.@new(x + 10, 8, z), Vector3.@new(10, 0.5, 12), RoofSlate);

        // Support posts
        Block("Smith_Post1", Vector3.@new(x + 6, 4, z + 5), Vector3.@new(1, 8, 1), DarkTimber);
        Block("Smith_Post2", Vector3.@new(x + 6, 4, z - 5), Vector3.@new(1, 8, 1), DarkTimber);
        Block("Smith_Post3", Vector3.@new(x + 14, 4, z + 5), Vector3.@new(1, 8, 1), DarkTimber);
        Block("Smith_Post4", Vector3.@new(x + 14, 4, z - 5), Vector3.@new(1, 8, 1), DarkTimber);

        // Forge
        Block("Smith_Forge", Vector3.@new(x + 12, 2, z), Vector3.@new(4, 4, 4), DarkStone);
        Block("Smith_Fire", Vector3.@new(x + 12, 3, z), Vector3.@new(2, 1.5, 2), Color3.fromRGB(255, 120, 30));

        // Chimney
        Block("Smith_Chimney", Vector3.@new(x + 12, 8, z), Vector3.@new(3, 10, 3), DarkStone);

        // Anvil
        Block("Smith_Anvil", Vector3.@new(x + 9, 1.5, z), Vector3.@new(1.5, 2, 1), MetalDark);

        // Water trough
        Block("Smith_Trough", Vector3.@new(x + 8, 1, z + 4), Vector3.@new(3, 2, 1.5), OldWood);
        Block("Smith_TroughWater", Vector3.@new(x + 8, 1.5, z + 4), Vector3.@new(2.5, 1, 1), WaterDeep);
    }

    static void BuildTavern()
    {
        double x = -55, z = 115;

        // Large main building
        Block("Tavern_Main", Vector3.@new(x, 7, z), Vector3.@new(20, 14, 18), PlasterCream);

        // Timber frame
        double b = 0.8;
        Block("Tavern_Frame1", Vector3.@new(x - 10, 7, z + 9), Vector3.@new(b, 14, b), DarkTimber);
        Block("Tavern_Frame2", Vector3.@new(x + 10, 7, z + 9), Vector3.@new(b, 14, b), DarkTimber);
        Block("Tavern_Frame3", Vector3.@new(x, 7, z + 9), Vector3.@new(b, 14, b), DarkTimber);
        Block("Tavern_HFrame1", Vector3.@new(x, 0.5, z + 9), Vector3.@new(20.5, b, b), DarkTimber);
        Block("Tavern_HFrame2", Vector3.@new(x, 7, z + 9), Vector3.@new(20.5, b, b), DarkTimber);
        Block("Tavern_HFrame3", Vector3.@new(x, 14, z + 9), Vector3.@new(20.5, b, b), DarkTimber);

        // Roof
        Block("Tavern_RoofL", Vector3.@new(x - 6, 17, z), Vector3.@new(12, 8, 20), RoofBrown);
        Block("Tavern_RoofR", Vector3.@new(x + 6, 17, z), Vector3.@new(12, 8, 20), RoofBrown);
        Block("Tavern_Ridge", Vector3.@new(x, 21.5, z), Vector3.@new(2, 1.5, 21), DarkTimber);

        // Chimney
        Block("Tavern_Chimney", Vector3.@new(x + 6, 18, z - 5), Vector3.@new(3, 10, 3), DarkStone);

        // Door
        Block("Tavern_Door", Vector3.@new(x, 3.5, z + 9.2), Vector3.@new(4, 7, 0.4), DarkTimber);

        // Sign
        Block("Tavern_SignPost", Vector3.@new(x + 8, 5, z + 10), Vector3.@new(0.5, 10, 0.5), DarkTimber);
        Block("Tavern_SignArm", Vector3.@new(x + 10, 9, z + 10), Vector3.@new(4, 0.3, 0.3), DarkTimber);
        Block("Tavern_Sign", Vector3.@new(x + 10, 7.5, z + 10), Vector3.@new(3, 2.5, 0.3), OldWood);

        // Windows with warm light
        Block("Tavern_Win1", Vector3.@new(x - 6, 5, z + 9.2), Vector3.@new(3, 4, 0.2), Color3.fromRGB(255, 220, 150));
        Block("Tavern_Win2", Vector3.@new(x + 6, 5, z + 9.2), Vector3.@new(3, 4, 0.2), Color3.fromRGB(255, 220, 150));
        Block("Tavern_Win3", Vector3.@new(x - 6, 11, z + 9.2), Vector3.@new(2.5, 3, 0.2), WindowBlue);
        Block("Tavern_Win4", Vector3.@new(x + 6, 11, z + 9.2), Vector3.@new(2.5, 3, 0.2), WindowBlue);

        // Outdoor seating area
        Block("Tavern_Table1", Vector3.@new(x - 5, 1.5, z + 15), Vector3.@new(4, 0.5, 3), OldWood);
        Block("Tavern_Bench1", Vector3.@new(x - 5, 1, z + 17), Vector3.@new(4, 1, 1), OldWood);
        Block("Tavern_Table2", Vector3.@new(x + 5, 1.5, z + 15), Vector3.@new(4, 0.5, 3), OldWood);
        Block("Tavern_Bench2", Vector3.@new(x + 5, 1, z + 17), Vector3.@new(4, 1, 1), OldWood);

        // Barrels outside
        Block("Tavern_Barrel1", Vector3.@new(x + 12, 1.5, z + 8), Vector3.@new(2.5, 3, 2.5), MediumTimber);
        Block("Tavern_Barrel2", Vector3.@new(x + 12, 1.5, z + 5), Vector3.@new(2.5, 3, 2.5), MediumTimber);
        Block("Tavern_Barrel3", Vector3.@new(x + 12, 4, z + 6.5), Vector3.@new(2.5, 3, 2.5), MediumTimber);
    }

    static void BuildMill()
    {
        double x = -70, z = 70;

        // Main building
        Block("Mill_Base", Vector3.@new(x, 8, z), Vector3.@new(14, 16, 14), LightStone);

        // Conical roof
        Block("Mill_Roof1", Vector3.@new(x, 18, z), Vector3.@new(16, 4, 16), ThatchYellow);
        Block("Mill_Roof2", Vector3.@new(x, 21, z), Vector3.@new(12, 4, 12), ThatchYellow);
        Block("Mill_Roof3", Vector3.@new(x, 24, z), Vector3.@new(8, 4, 8), ThatchYellow);
        Block("Mill_Roof4", Vector3.@new(x, 26.5, z), Vector3.@new(4, 3, 4), ThatchYellow);

        // Mill sails (simplified as flat rectangles)
        Block("Mill_Hub", Vector3.@new(x, 12, z + 7.5), Vector3.@new(3, 3, 1), DarkTimber);
        Block("Mill_Sail1", Vector3.@new(x, 20, z + 8), Vector3.@new(2, 14, 0.3), LightWood);
        Block("Mill_Sail2", Vector3.@new(x, 4, z + 8), Vector3.@new(2, 14, 0.3), LightWood);
        Block("Mill_Sail3", Vector3.@new(x - 8, 12, z + 8), Vector3.@new(14, 2, 0.3), LightWood);
        Block("Mill_Sail4", Vector3.@new(x + 8, 12, z + 8), Vector3.@new(14, 2, 0.3), LightWood);

        // Door
        Block("Mill_Door", Vector3.@new(x, 3, z + 7.2), Vector3.@new(3, 6, 0.4), DarkTimber);
    }

    // ===========================================
    // DECORATIONS & LANDSCAPING
    // ===========================================

    static void BuildWell(string name, Vector3 pos)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;

        Block(name + "_Base", Vector3.@new(x, y + 1.5, z), Vector3.@new(5, 3, 5), MediumStone);
        Block(name + "_Water", Vector3.@new(x, y + 2.2, z), Vector3.@new(3.5, 1, 3.5), WaterDeep);
        Block(name + "_PostL", Vector3.@new(x - 2, y + 5, z), Vector3.@new(0.6, 7, 0.6), DarkTimber);
        Block(name + "_PostR", Vector3.@new(x + 2, y + 5, z), Vector3.@new(0.6, 7, 0.6), DarkTimber);
        Block(name + "_Beam", Vector3.@new(x, y + 8.5, z), Vector3.@new(5, 0.6, 0.8), DarkTimber);
        Block(name + "_Roof", Vector3.@new(x, y + 9.5, z), Vector3.@new(6, 1.5, 4), RoofBrown);
    }

    static void BuildTrees()
    {
        // Trees scattered around
        BuildTree("Tree_1", Vector3.@new(-80, 0, 50));
        BuildTree("Tree_2", Vector3.@new(-85, 0, 80));
        BuildTree("Tree_3", Vector3.@new(-90, 0, 120));
        BuildTree("Tree_4", Vector3.@new(80, 0, 60));
        BuildTree("Tree_5", Vector3.@new(85, 0, 100));
        BuildTree("Tree_6", Vector3.@new(75, 0, 140));
        BuildTree("Tree_7", Vector3.@new(-75, 0, 170));
        BuildTree("Tree_8", Vector3.@new(70, 0, 175));

        // Trees behind castle
        BuildTree("Tree_B1", Vector3.@new(-50, 2, -60));
        BuildTree("Tree_B2", Vector3.@new(-30, 2, -70));
        BuildTree("Tree_B3", Vector3.@new(30, 2, -65));
        BuildTree("Tree_B4", Vector3.@new(55, 2, -55));

        // Small grove
        BuildTree("Tree_G1", Vector3.@new(90, 0, 80));
        BuildTree("Tree_G2", Vector3.@new(95, 0, 85));
        BuildTree("Tree_G3", Vector3.@new(88, 0, 92));
    }

    static void BuildTree(string name, Vector3 pos)
    {
        double x = pos.X, y = pos.Y, z = pos.Z;
        double height = 8 + (x + z) % 6; // Vary height
        double canopySize = 6 + (x + z) % 4;

        // Trunk
        Block(name + "_Trunk", Vector3.@new(x, y + height/2, z), Vector3.@new(1.5, height, 1.5), TrunkBrown);

        // Canopy (layered for fullness)
        Block(name + "_Canopy1", Vector3.@new(x, y + height + canopySize/2, z),
              Vector3.@new(canopySize, canopySize, canopySize), TreeGreen);
        Block(name + "_Canopy2", Vector3.@new(x + 1, y + height + canopySize/2 + 1, z + 1),
              Vector3.@new(canopySize * 0.7, canopySize * 0.8, canopySize * 0.7), TreeDark);
        Block(name + "_Canopy3", Vector3.@new(x - 0.5, y + height + canopySize/2 - 1, z - 0.5),
              Vector3.@new(canopySize * 0.8, canopySize * 0.6, canopySize * 0.8), TreeGreen);
    }

    static void BuildGardens()
    {
        // Garden plots near houses
        BuildGardenPlot("Garden_1", Vector3.@new(-50, 0, 90));
        BuildGardenPlot("Garden_2", Vector3.@new(50, 0, 125));
        BuildGardenPlot("Garden_3", Vector3.@new(-48, 0, 140));
    }

    static void BuildGardenPlot(string name, Vector3 pos)
    {
        double x = pos.X, z = pos.Z;

        // Soil
        Block(name + "_Soil", Vector3.@new(x, 0.2, z), Vector3.@new(8, 0.4, 6), Color3.fromRGB(90, 70, 50));

        // Fence
        Block(name + "_FenceF", Vector3.@new(x, 1, z + 3.2), Vector3.@new(8.5, 2, 0.3), LightWood);
        Block(name + "_FenceB", Vector3.@new(x, 1, z - 3.2), Vector3.@new(8.5, 2, 0.3), LightWood);
        Block(name + "_FenceL", Vector3.@new(x - 4.2, 1, z), Vector3.@new(0.3, 2, 6.5), LightWood);
        Block(name + "_FenceR", Vector3.@new(x + 4.2, 1, z), Vector3.@new(0.3, 2, 6.5), LightWood);

        // Crops (colored rows)
        Block(name + "_Crop1", Vector3.@new(x - 2, 0.7, z), Vector3.@new(1.5, 1, 5), Color3.fromRGB(60, 140, 50));
        Block(name + "_Crop2", Vector3.@new(x, 0.6, z), Vector3.@new(1.5, 0.8, 5), Color3.fromRGB(70, 150, 40));
        Block(name + "_Crop3", Vector3.@new(x + 2, 0.8, z), Vector3.@new(1.5, 1.2, 5), Color3.fromRGB(50, 130, 45));
    }

    static void BuildDecorations()
    {
        // Lamp posts along the main road
        BuildLampPost("Lamp_1", Vector3.@new(-6, 0, 40));
        BuildLampPost("Lamp_2", Vector3.@new(6, 0, 60));
        BuildLampPost("Lamp_3", Vector3.@new(-6, 0, 80));
        BuildLampPost("Lamp_4", Vector3.@new(6, 0, 95));

        // Carts
        BuildCart("Cart_1", Vector3.@new(-25, 0, 95));
        BuildCart("Cart_2", Vector3.@new(28, 0, 120));

        // Signpost at village entrance
        Block("Sign_Post", Vector3.@new(0, 3, 45), Vector3.@new(0.5, 6, 0.5), DarkTimber);
        Block("Sign_Board", Vector3.@new(2, 5, 45), Vector3.@new(4, 2, 0.3), OldWood);
    }

    static void BuildLampPost(string name, Vector3 pos)
    {
        double x = pos.X, z = pos.Z;

        Block(name + "_Post", Vector3.@new(x, 4, z), Vector3.@new(0.5, 8, 0.5), MetalDark);
        Block(name + "_Arm", Vector3.@new(x + 0.8, 7.5, z), Vector3.@new(1.5, 0.3, 0.3), MetalDark);
        Block(name + "_Lantern", Vector3.@new(x + 1.3, 7, z), Vector3.@new(1, 1.5, 1), Color3.fromRGB(255, 240, 180));
        Block(name + "_LanternTop", Vector3.@new(x + 1.3, 8, z), Vector3.@new(1.2, 0.5, 1.2), MetalDark);
    }

    static void BuildCart(string name, Vector3 pos)
    {
        double x = pos.X, z = pos.Z;

        // Cart body
        Block(name + "_Body", Vector3.@new(x, 2, z), Vector3.@new(5, 2, 3), OldWood);
        Block(name + "_Front", Vector3.@new(x + 2.7, 2.5, z), Vector3.@new(0.5, 3, 3), OldWood);

        // Wheels (simplified as dark blocks)
        Block(name + "_WheelL", Vector3.@new(x, 1.2, z + 2), Vector3.@new(0.5, 2.4, 2.4), DarkTimber);
        Block(name + "_WheelR", Vector3.@new(x, 1.2, z - 2), Vector3.@new(0.5, 2.4, 2.4), DarkTimber);

        // Handle
        Block(name + "_Handle", Vector3.@new(x - 4, 2, z), Vector3.@new(3, 0.3, 0.3), DarkTimber);

        // Hay in cart
        Block(name + "_Hay", Vector3.@new(x, 3.5, z), Vector3.@new(4, 1.5, 2.5), HayYellow);
    }

    static void BuildPond()
    {
        double x = 75, z = 160;

        // Pond shape (overlapping circles approximated with blocks)
        Block("Pond_Main", Vector3.@new(x, -0.3, z), Vector3.@new(20, 1, 15), Water);
        Block("Pond_Deep", Vector3.@new(x, -0.8, z), Vector3.@new(14, 1, 10), WaterDeep);
        Block("Pond_Edge1", Vector3.@new(x - 8, -0.2, z - 5), Vector3.@new(8, 0.8, 8), Water);
        Block("Pond_Edge2", Vector3.@new(x + 6, -0.2, z + 5), Vector3.@new(10, 0.8, 8), Water);

        // Rocks around edge
        Block("Pond_Rock1", Vector3.@new(x - 11, 0.5, z), Vector3.@new(2, 1.5, 2), MossyStone);
        Block("Pond_Rock2", Vector3.@new(x + 10, 0.3, z + 6), Vector3.@new(1.5, 1, 2), DarkStone);
        Block("Pond_Rock3", Vector3.@new(x - 5, 0.4, z - 8), Vector3.@new(2.5, 1.2, 1.5), MossyStone);

        // Reeds
        Block("Pond_Reeds1", Vector3.@new(x - 9, 1.5, z + 3), Vector3.@new(0.5, 3, 0.5), Color3.fromRGB(80, 120, 50));
        Block("Pond_Reeds2", Vector3.@new(x - 8, 1.3, z + 4), Vector3.@new(0.5, 2.6, 0.5), Color3.fromRGB(70, 110, 45));
        Block("Pond_Reeds3", Vector3.@new(x + 9, 1.2, z - 4), Vector3.@new(0.5, 2.4, 0.5), Color3.fromRGB(75, 115, 50));
    }

    static void BuildBridge()
    {
        // Small wooden bridge near the pond
        double x = 60, z = 155;

        Block("Bridge_Deck", Vector3.@new(x, 1, z), Vector3.@new(6, 0.5, 12), LightWood);
        Block("Bridge_RailL", Vector3.@new(x - 2.7, 2.5, z), Vector3.@new(0.4, 2.5, 12), DarkTimber);
        Block("Bridge_RailR", Vector3.@new(x + 2.7, 2.5, z), Vector3.@new(0.4, 2.5, 12), DarkTimber);

        // Support posts
        Block("Bridge_Post1", Vector3.@new(x - 2.7, 0.5, z - 5), Vector3.@new(0.6, 2, 0.6), DarkTimber);
        Block("Bridge_Post2", Vector3.@new(x + 2.7, 0.5, z - 5), Vector3.@new(0.6, 2, 0.6), DarkTimber);
        Block("Bridge_Post3", Vector3.@new(x - 2.7, 0.5, z + 5), Vector3.@new(0.6, 2, 0.6), DarkTimber);
        Block("Bridge_Post4", Vector3.@new(x + 2.7, 0.5, z + 5), Vector3.@new(0.6, 2, 0.6), DarkTimber);
    }
}
