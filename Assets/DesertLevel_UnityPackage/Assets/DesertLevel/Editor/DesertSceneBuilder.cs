using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class DesertSceneBuilder
{
    private const string Root = "Assets/DesertLevel";
    private static Material sand, track, rock, darkRock, cactus, cactusDark, gold, ruin, cloth, playerMat, skyMat;

    [MenuItem("Game Design/Build Desert Level")]
    public static void BuildDesertLevel()
    {
        EnsureFolders();
        EnsureTag("Coin");
        EnsureTag("Hazard");
        EnsureTag("Finish");
        CreateMaterials();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "DesertLevel";

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.88f, 0.61f, 0.34f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 75;
        RenderSettings.fogEndDistance = 210;
        RenderSettings.ambientLight = new Color(0.72f, 0.59f, 0.43f);
        RenderSettings.skybox = skyMat;

        var lightGo = new GameObject("Desert Sun");
        var sun = lightGo.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.35f;
        sun.color = new Color(1f, 0.82f, 0.62f);
        lightGo.transform.rotation = Quaternion.Euler(42, -32, 0);

        // Wide desert floor and a raised, readable three-lane running route.
        MakePrimitive("Endless Sand", PrimitiveType.Plane, new Vector3(0, -0.35f, 85), new Vector3(22, 1, 22), sand);
        MakePrimitive("Ancient Trail", PrimitiveType.Plane, new Vector3(0, -0.02f, 92), new Vector3(1.25f, 1, 18.5f), track);
        for (int z = 8; z < 184; z += 12)
        {
            MakePrimitive("Trail marker left", PrimitiveType.Cube, new Vector3(-4.15f, 0.015f, z), new Vector3(0.12f, 0.035f, 4.2f), gold);
            MakePrimitive("Trail marker right", PrimitiveType.Cube, new Vector3(4.15f, 0.015f, z), new Vector3(0.12f, 0.035f, 4.2f), gold);
        }

        // Dune ridges and distant mesa silhouettes.
        for (int i = 0; i < 18; i++)
        {
            float z = 8 + i * 10.5f;
            float side = (i % 2 == 0) ? -1 : 1;
            MakePrimitive("Dune ridge", PrimitiveType.Sphere,
                new Vector3(side * (12 + (i % 3) * 2), -1.4f, z),
                new Vector3(22, 4.5f + (i % 3), 17), sand);
        }
        for (int i = 0; i < 9; i++)
        {
            float x = (i % 2 == 0 ? -1 : 1) * (24 + (i % 3) * 5);
            float z = 20 + i * 19;
            var mesa = MakePrimitive("Far sandstone mesa", PrimitiveType.Cube,
                new Vector3(x, 5 + (i % 3) * 2, z),
                new Vector3(15 + (i % 3) * 3, 12 + (i % 4) * 3, 13), rock);
            mesa.transform.rotation = Quaternion.Euler(0, (i * 17) % 35, 0);
        }

        // Rock clusters, cacti and dry palms on both sides of the route.
        Random.InitState(2411);
        for (int i = 0; i < 46; i++)
        {
            int side = (i % 2 == 0) ? -1 : 1;
            float x = side * Random.Range(6.5f, 17f);
            float z = Random.Range(5f, 178f);
            if (i % 3 == 0) CreateCactus(new Vector3(x, 0, z), Random.Range(0.75f, 1.35f));
            else if (i % 3 == 1) CreateRock(new Vector3(x, 0, z), Random.Range(0.65f, 1.8f));
            else CreateDryPalm(new Vector3(x, 0, z), Random.Range(0.8f, 1.25f));
        }

        // Ancient desert ruins and a distant pyramid landmark.
        CreatePyramid(new Vector3(-20, 0, 74), 13);
        CreatePyramid(new Vector3(23, 0, 128), 10);
        CreateTemple(new Vector3(15, 0, 48));
        CreateTemple(new Vector3(-16, 0, 151));

        // Obstacles and coin lines, deliberately distributed across the three lanes.
        for (int z = 22; z < 165; z += 13)
        {
            int hazardLane = Random.Range(0, 3);
            float x = (hazardLane - 1) * 3f;
            CreateHazard(new Vector3(x, 0.75f, z), (z % 26 == 0) ? "Ancient stone block" : "Fallen desert log");
            int coinLane = (hazardLane + 1) % 3;
            for (int j = 0; j < 4; j++)
                CreateCoin(new Vector3((coinLane - 1) * 3f, 1.25f, z + 2.3f + j * 1.7f));
        }

        // Player and camera are intentionally keyboard/mouse friendly for Editor Play Mode.
        var player = new GameObject("Desert Runner");
        player.tag = "Player";
        player.transform.position = new Vector3(0, 1, 4);
        var controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.42f;
        controller.center = Vector3.zero;
        var body = MakePrimitive("Runner Visual", PrimitiveType.Capsule, new Vector3(0, 1, 4), Vector3.one, playerMat);
        body.transform.SetParent(player.transform, true);
        Object.DestroyImmediate(body.GetComponent<Collider>());
        body.transform.localPosition = new Vector3(0, 0, 0);
        player.AddComponent<DesertRunnerController>();

        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.fieldOfView = 62;
        camGo.AddComponent<AudioListener>();
        camGo.transform.position = new Vector3(0, 7.5f, -7);
        camGo.transform.LookAt(new Vector3(0, 1.5f, 15));

        // Simple horizon landmark and finish gate.
        CreateFinishGate(new Vector3(0, 0, 180));
        var info = new GameObject("DESERT LEVEL INFO");
        var note = info.AddComponent<DesertCameraMarker>();
        Selection.activeGameObject = player;

        string scenePath = Root + "/Scenes/DesertLevel.unity";
        Directory.CreateDirectory(Root + "/Scenes");
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Desert Level Created",
            "Created " + scenePath + "\\n\\nOpen it and press Play. Controls: A/D or arrows, Space to jump, S to slide, R to restart.",
            "Great!");
    }


    private static void EnsureTag(string tagName)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0) return;
        var manager = new SerializedObject(assets[0]);
        var tags = manager.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
            if (tags.GetArrayElementAtIndex(i).stringValue == tagName) return;
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tagName;
        manager.ApplyModifiedProperties();
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/DesertLevel")) AssetDatabase.CreateFolder("Assets", "DesertLevel");
        if (!AssetDatabase.IsValidFolder(Root + "/Materials")) AssetDatabase.CreateFolder(Root, "Materials");
        if (!AssetDatabase.IsValidFolder(Root + "/Scenes")) AssetDatabase.CreateFolder(Root, "Scenes");
    }

    private static void CreateMaterials()
    {
        sand = Mat("WarmDuneSand", new Color(0.76f, 0.48f, 0.23f));
        track = Mat("PackedDesertTrail", new Color(0.48f, 0.27f, 0.13f));
        rock = Mat("SunbakedSandstone", new Color(0.55f, 0.30f, 0.17f));
        darkRock = Mat("ShadowedRock", new Color(0.30f, 0.19f, 0.14f));
        cactus = Mat("CactusGreen", new Color(0.13f, 0.32f, 0.16f));
        cactusDark = Mat("CactusShadow", new Color(0.08f, 0.22f, 0.12f));
        gold = Mat("DesertGold", new Color(1f, 0.67f, 0.08f), 0.45f, 0.55f);
        ruin = Mat("AncientRuins", new Color(0.72f, 0.56f, 0.37f));
        cloth = Mat("NomadCloth", new Color(0.48f, 0.12f, 0.07f));
        playerMat = Mat("RunnerTurquoise", new Color(0.04f, 0.45f, 0.48f));
        skyMat = new Material(Shader.Find("Skybox/Procedural"));
        skyMat.SetColor("_SkyTint", new Color(0.9f, 0.62f, 0.38f));
        skyMat.SetFloat("_AtmosphereThickness", 0.8f);
        SaveMat(skyMat, "DesertSky");
    }

    private static Material Mat(string name, Color color, float metallic = 0, float smooth = 0.2f)
    {
        string path = Root + "/Materials/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;
        Shader shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
        var m = new Material(shader);
        m.name = name;
        m.color = color;
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
        if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smooth);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    private static void SaveMat(Material m, string name)
    {
        string path = Root + "/Materials/" + name + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) == null)
            AssetDatabase.CreateAsset(m, path);
    }

    private static GameObject MakePrimitive(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        var renderer = go.GetComponent<Renderer>();
        if (renderer != null) renderer.sharedMaterial = mat;
        return go;
    }

    private static void CreateRock(Vector3 p, float s)
    {
        var r = MakePrimitive("Weathered boulder", PrimitiveType.Sphere, p + Vector3.up * (s * 0.35f),
            new Vector3(s * 1.5f, s * 0.8f, s), (Random.value > 0.5f ? rock : darkRock));
        r.transform.rotation = Quaternion.Euler(Random.Range(-15, 15), Random.Range(0, 180), Random.Range(-15, 15));
    }

    private static void CreateCactus(Vector3 p, float s)
    {
        MakePrimitive("Saguaro cactus trunk", PrimitiveType.Capsule, p + Vector3.up * (1.1f * s),
            new Vector3(0.38f * s, 1.1f * s, 0.38f * s), cactus);
        var arm1 = MakePrimitive("Cactus arm", PrimitiveType.Capsule, p + new Vector3(0.48f*s, 1.1f*s, 0),
            new Vector3(0.22f*s, 0.55f*s, 0.22f*s), cactusDark);
        arm1.transform.rotation = Quaternion.Euler(0, 0, -90);
        var arm2 = MakePrimitive("Cactus arm", PrimitiveType.Capsule, p + new Vector3(-0.42f*s, 1.45f*s, 0),
            new Vector3(0.2f*s, 0.48f*s, 0.2f*s), cactus);
        arm2.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private static void CreateDryPalm(Vector3 p, float s)
    {
        var trunk = MakePrimitive("Dry palm trunk", PrimitiveType.Cylinder, p + Vector3.up * (2*s),
            new Vector3(0.32f*s, 2*s, 0.32f*s), ruin);
        trunk.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-8, 8));
        for (int i = 0; i < 6; i++)
        {
            var leaf = MakePrimitive("Dry palm frond", PrimitiveType.Capsule,
                p + new Vector3(Mathf.Cos(i*Mathf.PI/3)*1.1f*s, 4*s, Mathf.Sin(i*Mathf.PI/3)*1.1f*s),
                new Vector3(0.16f*s, 1.15f*s, 0.28f*s), cactusDark);
            leaf.transform.rotation = Quaternion.Euler(0, i*60, 48);
        }
    }

    private static void CreatePyramid(Vector3 p, float size)
    {
        var baseBlock = MakePrimitive("Ancient pyramid base", PrimitiveType.Cube,
            p + new Vector3(0, size*0.28f, 0), new Vector3(size, size*0.56f, size), ruin);
        baseBlock.transform.rotation = Quaternion.Euler(0, 45, 0);
        var top = MakePrimitive("Pyramid upper stone", PrimitiveType.Cube,
            p + new Vector3(0, size*0.72f, 0), new Vector3(size*0.58f, size*0.4f, size*0.58f), rock);
        top.transform.rotation = Quaternion.Euler(0, 45, 0);
    }

    private static void CreateTemple(Vector3 p)
    {
        for (int i = 0; i < 4; i++)
        {
            float x = (i < 2 ? -1 : 1) * 3.2f;
            float z = (i % 2 == 0 ? -2.4f : 2.4f);
            MakePrimitive("Ruined temple pillar", PrimitiveType.Cylinder, p + new Vector3(x, 2, z),
                new Vector3(0.65f, 2, 0.65f), ruin);
        }
        MakePrimitive("Temple lintel", PrimitiveType.Cube, p + new Vector3(0, 4.1f, 0),
            new Vector3(8, 0.65f, 6), rock);
        MakePrimitive("Temple steps", PrimitiveType.Cube, p + new Vector3(0, 0.2f, -4),
            new Vector3(10, 0.4f, 2), ruin);
    }

    private static void CreateHazard(Vector3 p, string name)
    {
        GameObject h;
        if (name.Contains("block"))
            h = MakePrimitive(name, PrimitiveType.Cube, p + Vector3.up * 0.8f, new Vector3(1.8f, 1.6f, 1.4f), rock);
        else
        {
            h = MakePrimitive(name, PrimitiveType.Cylinder, p + Vector3.up * 0.35f, new Vector3(0.7f, 0.35f, 1.8f), darkRock);
            h.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        h.tag = "Hazard";
    }

    private static void CreateCoin(Vector3 p)
    {
        var c = MakePrimitive("Collectible gold coin", PrimitiveType.Cylinder, p, new Vector3(0.72f, 0.12f, 0.72f), gold);
        c.transform.rotation = Quaternion.Euler(90, 0, 0);
        c.tag = "Coin";
        var col = c.GetComponent<Collider>();
        col.isTrigger = true;
        c.AddComponent<DesertCoinSpin>();
    }

    private static void CreateFinishGate(Vector3 p)
    {
        MakePrimitive("Finish gate left", PrimitiveType.Cube, p + new Vector3(-4, 2.5f, 0),
            new Vector3(0.7f, 5, 0.7f), ruin);
        MakePrimitive("Finish gate right", PrimitiveType.Cube, p + new Vector3(4, 2.5f, 0),
            new Vector3(0.7f, 5, 0.7f), ruin);
        var beam = MakePrimitive("Finish gate top", PrimitiveType.Cube, p + new Vector3(0, 5, 0),
            new Vector3(8.7f, 0.8f, 0.9f), gold);
        var trigger = new GameObject("Finish Trigger");
        trigger.tag = "Finish";
        trigger.transform.position = p + new Vector3(0, 1, 0);
        var box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(10, 4, 2);
    }
}
