using System.Reflection;
using DbsContentApi;
using UnityEngine;

namespace DocsSnippets;

public static class TutorialSnippets
{
    #region ShopItem

    public static ShopItemCategory WeaponsCategory;

    public static void RegisterShopItem(AssetBundle bundle)
    {
        WeaponsCategory = Items.RegisterCustomCategory("Weapons");

        Items.DeferRegistration(() =>
        {
            GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyGun.prefab");
            prefab.AddComponent<MyGunBehaviour>();

            GameMaterials.ApplyMaterial(prefab, GameMaterial.M_Metal, deepApply: true);

            Items.RegisterItem(prefab, new ItemConfig
            {
                displayName = "My Gun",
                price = 150,
                category = WeaponsCategory,
                icon = bundle.LoadAsset<Sprite>("gun_icon"),
                holdPos = new Vector3(0.3f, -0.3f, 0.7f),
                useAlternativeHoldPos = true,
                alternativeHoldPos = new Vector3(0.2f, -0.22f, 0.7f),
                impactSoundTypes = new[] { ImpactSoundType.PlasticBounce1 },
            });
        });
    }

    private class MyGunBehaviour : MonoBehaviour { }

    #endregion

    #region Monster

    public static void RegisterMonster(AssetBundle bundle)
    {
        GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyMonster.prefab");

        var config = new MobSetupConfig
        {
            budget = new BudgetConfig { budgetCost = 1, rarity = 1f },
            controller = new ControllerConfig(),
            player = new PlayerConfig(),
            ragdoll = new RagdollConfig(),
            photonView = new PhotonViewConfig(),
            bot = new BotConfig(),
            navMesh = new NavMeshAgentConfig { height = 2f, radius = 1f, speed = 3.5f },
            addMonsterSyncer = true,
            addAnimRefHandler = true,
            addMonsterAnimationHandler = true,
            addHeadFollower = true,
            addGroundPos = true,
        };

        Mobs.RegisterMonster(prefab, "MyMonster", config,
            material: GameMaterial.M_Monster,
            postSetup: go =>
            {
                GameObject bot = Mobs.GetBotChildObject(go);
                Mobs.AddBotChaserComponent(bot, new BotChaserConfig { targetDistance = 1.5f });
            });
    }

    #endregion

    #region Map

    public static void RegisterMap(AssetBundle mapBundle)
    {
        string? scenePath = Maps.FindScenePath(mapBundle, "MyScene");
        if (scenePath == null) return;

        Maps.RegisterMap(
            mapBundle,
            scenePath,
            "My Custom Map",
            mapId: "mymod.my_scene");
    }

    #endregion

    #region Materials

    public static void FixMaterials(GameObject prefab)
    {
        GameMaterials.ApplyMaterial(prefab, GameMaterial.M_Metal, deepApply: true);

        GameMaterials.Batch(prefab)
            .At("Item/Mesh", DescriptiveMaterial.WHITE_1, deep: true)
            .At("Item/Handle", GameMaterial.M_Metal);
    }

    #endregion

    #region ContentEvent

    public static void RegisterFilmingEvents()
    {
        // Register your mod's ContentEvent subclasses here.
        // IDs = 2000 + registration index. Never reorder after release.
        ContentEvents.RegisterEvent(new MyExplosionContentEvent());
    }

    // Define in your mod — see UnlistedEntities/CustomContent/ContentEvents/
    private class MyExplosionContentEvent : ContentEvent { /* ... */ }

    #endregion
}
