// using System.Collections.Generic;
// using UnityEngine;

// namespace DbsContentApi.Modules;

// /// <summary>
// /// Utility component to scan all game prefabs for their materials and cache them.
// /// </summary>
// public class MobMaterialScanner
// {
//     private static readonly string[] PrefabPaths = new string[]
//     {
//         "0_Balaclava", "10_Hat_Child", "11_Hat_Clown", "12_Hat_Cowboy", "13_Hat_Crown",
//         "14_Hat_Halo", "15_Hat_Horns", "16_Hat_Hotdog", "17_Hat_Jester", "18_Hat_Knifo",
//         "19_Hat_Milk", "1_Beanie", "20_Hat_News", "21_Hat_Pirate", "22_Hat_Rugby",
//         "23_Hat_Savannah", "24_Hat_Tooop", "25_Hat_Top", "26_Party", "27_Shroom",
//         "28_Ushanka", "29_Witch", "2_BucketHat", "30_Hat_Hard", "3_CatEars",
//         "4_Chef", "5_Floppy", "6_Homburg", "7_Hair 1", "8_Hat_Bowler",
//         "9_Hat_Cap", "AppleArtifact", "ArtifactSpawner", "BarbedFence", "BigSlapPaintingArtifact",
//         "BlackHole", "Bomb", "Bone2Artifact", "BoneArtifact", "BonesArtifact",
//         "BOOL INPUT", "BoomMic", "Box", "BoxDestroy", "BrokenCamera",
//         "Burger", "Bus Tire", "BusDoor", "Camera1", "Camera_Zoom_1",
//         "Camera_Zoom_2", "Camera_Zoom_3", "Capsule", "ChorbyAtifact", "Clapper",
//         "ColorSelector", "COMMENT", "Console", "ContainerArtifact", "Deal",
//         "DebugUIBitField", "DebugUIButton", "DebugUIColor", "DebugUIEnumField", "DebugUIEnumHistory",
//         "DebugUIFloatField", "DebugUIFoldout", "DebugUIGroup", "DebugUIHBox", "DebugUIIntField",
//         "DebugUIMessageBox", "DebugUIObject", "DebugUIObjectList", "DebugUIObjectPopupField", "DebugUIPanel",
//         "DebugUIProgressBar", "DebugUIRow", "DebugUIToggle", "DebugUIToggleHistory", "DebugUIUIntField",
//         "DebugUIValue", "DebugUIValuePersistent", "DebugUIValueTuple", "DebugUIVBox", "DebugUIVector2",
//         "DebugUIVector3", "DebugUIVector4", "Defib", "Disc", "DogBullet",
//         "DogBulletHit", "Drone", "EmoteBook", "ENUM INPUT", "EscapePlayerCell",
//         "Exhaust Pipe", "Explosion", "Fire", "FireShot", "Flare",
//         "Flashlight 1", "Flashlight 2", "Flashlight 3", "Flashlight Long 2", "Flashlight Long 3",
//         "Flashlight Wide 2", "Flashlight Wide 3", "FLOAT INPUT", "FredGull", "GasTrigger",
//         "GooBall", "GrabberArm", "GravityFlipArtifactOff", "GravityFlipArtifactOn", "HandBag",
//         "HatInstance", "HeadBoneArtifact", "Hugger", "INT INPUT", "Iron Beam",
//         "KEYCODE INPUT", "LostDisc", "ModalButton", "ModlistEntry", "ModlistEntryHeader",
//         "MoneyCell", "NorfGun", "NorfShot", "OilDrum 1", "OilDrum 2",
//         "OilDrum 3", "PartyPopper", "Plank Broken 1", "Plank Broken 2", "Plank Broken 3",
//         "Plank Broken 4", "Plank Broken 5", "PluginCell", "Radio", "Rails",
//         "Realm_BigSlap", "Realm_Dog", "Realm_Jumps", "Realm_Knifo", "ReporterMic",
//         "RescueHook", "RESOLUTION SETTING", "Rubble Metal 1", "Rubble Metal 2", "Rubble Metal 3",
//         "Rubble Metal 4", "Rubble Metal 5", "Rubble Metal 6", "Rubble Metal 7", "Rubble Metal 8",
//         "R_Pipe 1", "R_Pipe 2", "R_Pipe 3", "R_Pipe 5", "R_Pipe 6",
//         "R_Scaffold", "R_Scaffold_AnyDir", "R_Scaffold_Down", "Scaffold", "SettingCell",
//         "SFX Collection Attack Swing 0", "SFX Collection Attack Swing 1", "SFX Collection Eat Off", "SFX Collection Eat On", "SFX Collection Ghost Voice Catch",
//         "SFX Collection Ghost Voice Cry", "SFX Collection Ghost Voice Laugh", "SFX Collection Jello Move", "SFX Collection Jello Start 1", "SFX Collection Jello Start",
//         "SFX Collection Land Basic", "SFX Collection Step Cloth 1", "SFX Collection Step Dirt 1", "SFX Collection Step Ear 1", "SFX Collection Step Flicker 1",
//         "SFX Collection Step Grass 1", "SFX Collection Step Gravel 1", "SFX Collection Step Metal 1", "SFX Collection Step Metal 2", "SFX Collection Step Metal 3",
//         "SFX Collection Step Metal 4", "SFX Collection Step Metal 5", "SFX Collection Step Sand 1", "SFX Collection Step Screamo", "SFX Collection Step Skin 1",
//         "SFX Collection Step Snatcho", "SFX Collection Step Spider 1", "SFX Collection Step Stone 1", "SFX Collection Step Stone 2", "SFX Collection Step Toolkit 1",
//         "SFX Collection Step Toolkit 2", "SFX Collection Step Wood 1", "SFX Collection Step Wood 2", "SFX Collection Step Wood 3", "ShipDoor",
//         "ShockStick", "ShockWave", "ShroomAfrtifact", "SnailBurst", "SnailDeleter",
//         "SnailShot", "SongRadio", "SoundPlayer", "Sphere", "STRING INPUT",
//         "Striped Metal Panel", "Tire", "TitleCardItemInstance", "TitleCardItemInstanceBGWhite", "VOICE INPUT CELL",
//         "WalkieTalkie", "WebBurst", "WebShot", "WebString", "Winch",
//         "WoodCrate", "WoodPallette", "ZombieArtifact", "Angler", "AnglerMimic",
//         "AnglerMimic2", "Arms", "BarnacleBall", "BigSlap", "BigSlap_Small",
//         "BlackHoleBot", "Bombs", "BoookDissolve", "ButtonRobot", "CalibrationScreen",
//         "CamCreep", "Chair_Inside", "CharacterBot_Base", "CinemaScreen", "CurseOfDeath",
//         "CurseOfGravityFlip", "CurseOfShroom", "CurseOfZombie", "DebugUICanvas", "DebugUIPersistentCanvas",
//         "DeckChair", "Dog", "DummyMonster", "Dummy_Alive", "Dummy_Dead",
//         "Ear", "EmoteBookEffectTest", "ExplodedGoop", "EyeGuy", "Fire",
//         "Flicker", "Ghost", "Harpooner", "Hatshop", "Infiltrator2",
//         "InfiltratorSpawner", "InputHandler", "ItemDataSyncer", "Jello", "Knifo",
//         "KnifoGroup 2", "KnifoGroup 5", "Larva", "Mime", "MimicInfiltrator",
//         "MimicMan", "Modal", "Mouthe", "Mouthe5", "PartyPopperConfetti",
//         "PickupHolder", "Player", "PlayerData", "PodcastChair", "PoolBall",
//         "PoolRing", "Projector", "Puffo", "RecordingHandler", "Slurper",
//         "SlurperGroup 5", "SnailSpawn", "SnailSpawner", "Snatcho", "SnatchoGroup 5",
//         "Spider", "Streamer", "Toolkit_Fan", "Toolkit_Hammer", "Toolkit_Iron",
//         "Toolkit_Vaccuum", "Toolkit_Wisk", "Toolkit_WiskGroup 3", "TransitionHandler", "UltraKnifo",
//         "UltraKnifoGroup", "UnlockExplosion", "Wallo", "WalloArm", "Web",
//         "Weeping", "WeepingGroup2", "Worm", "Zombe", "ZombieGroup3",
//         "ZombieGroup5"
//     };

//     public static Dictionary<string, Material> FoundMaterials = new Dictionary<string, Material>();

//     /// <summary>
//     /// Performs an exhaustive scan of game resources to find and cache unique materials.
//     /// </summary>
//     public static void ScanAndMapMaterials()
//     {
//         Logger.Log("Starting exhaustive material scan...");

//         foreach (string path in PrefabPaths)
//         {
//             GameObject prefab = Resources.Load<GameObject>(path);
//             if (prefab == null)
//             {
//                 Logger.LogWarning($"Prefab {path} not found during scan.");
//                 continue;
//             }

//             foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
//             {
//                 foreach (var mat in renderer.sharedMaterials)
//                 {
//                     if (mat != null && !FoundMaterials.ContainsKey(mat.name))
//                     {
//                         FoundMaterials.Add(mat.name, mat);
//                     }
//                 }
//             }
//         }

//         Logger.Log($"Scan complete: {FoundMaterials.Count} unique materials cached.");
//     }

//     /// <summary>
//     /// Applies a material found during scanning to a target GameObject.
//     /// </summary>
//     /// <param name="target">The GameObject to modify.</param>
//     /// <param name="materialName">The name of the material to apply.</param>
//     public static void ApplyFoundMaterial(GameObject target, string materialName)
//     {
//         if (FoundMaterials.TryGetValue(materialName, out Material mat))
//         {
//             foreach (var renderer in target.GetComponentsInChildren<Renderer>())
//             {
//                 renderer.material = mat;
//             }
//         }
//         else
//         {
//             Logger.LogWarning($"Material '{materialName}' not in cache. Ensure the source prefab is loaded.");
//         }
//     }
// }
