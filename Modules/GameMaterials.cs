using System.Collections.Generic;
using UnityEngine;

namespace DbsContentApi.Modules;

public enum GameMaterialType
{
    /// <summary>Standard monster material featuring a unique parallax effect.</summary>
    M_World,
    /// <summary>Dark/Black material with prominent dark strokes.</summary>
    M_World_Local,
    /// <summary>Neutral white lit material.</summary>
    Lit,
    /// <summary>Gray material with subtle dark spotting.</summary>
    GRAY,
    /// <summary>A variation of the standard player material (similar to M_Player).</summary>
    M_Player_1,
    /// <summary>The base material used for monster entities.</summary>
    M_Monster,
    /// <summary>A red-tinted variation of the base monster material.</summary>
    M_Monster_Red,
    /// <summary>A significantly darker variation of the monster material.</summary>
    M_Monster_Black,
    /// <summary>A material used for harpoon-related entities, darker than M_Monster but lighter than M_Monster_Black.</summary>
    M_Monster_Harpoon,
    /// <summary>A unique material that is transparent when unlit, but glows white when exposed to light.</summary>
    M_Ghost,
    /// <summary>A fully transparent material used for ghostly dental features.</summary>
    M_Ghost_Teeth,
    /// <summary>A transparent, slightly darkened material.</summary>
    M_Slurp,
    /// <summary>A white material with a 50% distribution of black spots.</summary>
    M_Snatcho,
    /// <summary>A gray material with small white spots.</summary>
    M_Witch,
    /// <summary>An animated material featuring spinning dark strokes.</summary>
    M_Propeller,
    /// <summary>A significantly glowing white material used for eyes.</summary>
    M_Eye,
    /// <summary>An eye-like texture based on M_Monster with a central hole.</summary>
    M_MonsterEye,
    /// <summary>An animated red blinking material similar to emergency vehicle lighting.</summary>
    M_MonsterSkinnyEye,
    /// <summary>A blue/white material with an icy, visor-like appearance.</summary>
    M_PlayerVisor,
    /// <summary>A wobbling white fire effect material.</summary>
    M_VFX_Fire,
    /// <summary>A material used for red arrow visual effects.</summary>
    M_VFX_BombFuze,
    /// <summary>A dark, transparent material featuring a parallax effect.</summary>
    M_VFX_Tentacle,
    /// <summary>A material used for blue arrow visual effects.</summary>
    M_VFX_ShockStick,
    /// <summary>A fully transparent material used for distortion particles.</summary>
    M_DistrorionParticle,
    /// <summary>A white, slightly blurred material for default particles.</summary>
    M_DefaultParticle,
    /// <summary>A transparent material with a distinct white spot.</summary>
    ParticlesUnlit,
    /// <summary>A fully transparent material used for splash effects.</summary>
    M_Splash,
    /// <summary>A transparent material with small dark spots.</summary>
    M_Gas,
    /// <summary>A transparent material with reflective properties.</summary>
    M_Goop,
    /// <summary>A glowing red material used for laser effects.</summary>
    M_Laser,
    /// <summary>A fully transparent material for flashlight beams.</summary>
    M_FlashBeam,
    /// <summary>A fully transparent red material for red flashlight beams.</summary>
    M_FlashBeamRed,
    /// <summary>A bright, glowing white light material.</summary>
    M_Light_Bright,
    /// <summary>A plain gray material without spots, used for deactivated battery lights.</summary>
    M_BatteryLight_Off,
    /// <summary>A bright, glowing white material for flashlights.</summary>
    BRIGHT_WHITE,
    /// <summary>A light brown material with dark spots.</summary>
    M_Flashlight_1_1,
    /// <summary>A standard brown material with dark spots.</summary>
    M_Flashlight_1_2,
    /// <summary>A dark material with small dark spots for deactivated flashlights.</summary>
    M_Flashlight_Off,
    /// <summary>A blinking white material with dark borders for oxygen displays.</summary>
    M_OxygenDisplay,
    /// <summary>The texture atlas for the Liberation Sans font.</summary>
    LiberationSans_SDF,
    /// <summary>The texture atlas for the Rajdhani Regular font.</summary>
    Rajdhani_Regular_SDF,
    /// <summary>The texture atlas for the Akzidenz Grotesk Pro Bold font.</summary>
    AkzidenzGroteskPro_Bold_SDF,
    /// <summary>The texture atlas for the Akzidenz Grotesk Pro Light font.</summary>
    AkzidenzGroteskPro_Light_SDF,
    /// <summary>A transparent material with a cursor texture.</summary>
    M_Cursor,
    /// <summary>A gray material with dark spots and a parallax effect for screens.</summary>
    M_Screen,
    /// <summary>A material for video displays featuring gradient strips and text.</summary>
    M_VideoDisplayScreen,
    /// <summary>A plain white cinema screen material.</summary>
    M_Cinema_1,
    /// <summary>A variation of the white cinema screen material.</summary>
    M_Cinema_2,
    /// <summary>A standard gray green screen material.</summary>
    FLAT_GRAY,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_GreenScreen_2,
    /// <summary>A material featuring a lunar surface texture.</summary>
    M_GreenScreen_3,
    /// <summary>A dark gray material with lighter gray spots used for balaclavas.</summary>
    VERY_DARK_GRAY,
    /// <summary>A soft violet/magenta material with discrete white strips for beanies.</summary>
    M_Beanie_1,
    /// <summary>An opaque blue-gray material with a water-like appearance.</summary>
    GRAY_BLUE,
    /// <summary>A white material with lighter white spots for caps.</summary>
    M_Cap_1,
    /// <summary>A white material with lighter white spots for chef hats.</summary>
    M_Chef_1,
    /// <summary>A sky blue material with small white spots and an icy look.</summary>
    SKYBLUE,
    /// <summary>A green material with small white spots.</summary>
    GREEN,
    /// <summary>A yellow material with small white spots.</summary>
    YELLOW,
    /// <summary>A red material with small white spots.</summary>
    RED,
    /// <summary>A white material with small white spots.</summary>
    WHITE,
    /// <summary>A red material with small white spots for clown hats.</summary>
    RED2,
    /// <summary>A beige material with small white spots for clown hats.</summary>
    BEIGE,
    /// <summary>A brown material with small white spots for cowboy hats.</summary>
    BROWN,
    /// <summary>A gold material with small white spots for crowns.</summary>
    GOLD,
    /// <summary>A dark gray material with small white spots.</summary>
    DARKGRAY2,
    /// <summary>A pink/magenta material with small white spots.</summary>
    MAGENTA,
    /// <summary>A variation of dark gray material with small white spots.</summary>
    DARKGRAY3,
    /// <summary>A simple beige material.</summary>
    BEIGE2,
    /// <summary>A brown/gray material with small white spots.</summary>
    BROWN2,
    /// <summary>A brown material with small white spots for hair.</summary>
    BROWN3,
    /// <summary>A gold material used for halos.</summary>
    GOLD2,
    /// <summary>A white material with small white spots used for horns.</summary>
    M_Horn,
    /// <summary>An ivory white material with small white spots.</summary>
    WHITE_IVORY,
    /// <summary>An orange material with small white spots.</summary>
    ORANGE_SAUSAGE,
    /// <summary>A red material with small white spots.</summary>
    RED3,
    /// <summary>A yellow-beige material.</summary>
    YELLOW_BEIGE,
    /// <summary>A yellow material with small white spots for jester hats.</summary>
    YELLOW2,
    /// <summary>A violet/magenta material with small white spots for jester hats.</summary>
    VIOLET,
    /// <summary>A green material with small white spots for jester hats.</summary>
    GREEN2,
    /// <summary>A yellow/orange material with small white spots for jester hats.</summary>
    YELLOW_ORANGE,
    /// <summary>A bright white drape-like material with whiter spotting.</summary>
    M_Knifo,
    /// <summary>A dark gray material with small white spots.</summary>
    M_DARKGRAY,
    /// <summary>A white material with small white spots.</summary>
    M_Milk2,
    /// <summary>A gray material with small white spots.</summary>
    M_PAPER,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_News2,
    /// <summary>A red material decorated with yellow dots.</summary>
    M_Party_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Party_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Pirate_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Pirate_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Pirate_3,
    /// <summary>A cyan material with small white spots for player hats.</summary>
    M_PlayerHatColor,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rugby_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rugby_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rugby_3,
    /// <summary>A red material decorated with white dots.</summary>
    M_Shroom_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Shroom_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Top,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Ushanka_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Ushanka_2,
    /// <summary>A gray-blue material with white spots for bowler hats.</summary>
    M_Bowler,
    /// <summary>A dark gray material with small dark spots.</summary>
    M_Metal,
    /// <summary>A gray material with small white spots for books.</summary>
    M_Book_1,
    /// <summary>A white paper material for books.</summary>
    M_Book_2,
    /// <summary>A red/orange clay-like material for interior surfaces.</summary>
    M_Interior_7,
    /// <summary>A gray material with small white strokes and a parallax effect.</summary>
    M_Crane,
    /// <summary>A clay-like material used for pool areas.</summary>
    M_Pool_4,
    /// <summary>An orange material used for pool areas.</summary>
    M_Pool_6,
    /// <summary>A cyan material used for pool areas.</summary>
    M_Pool_7,
    /// <summary>A yellow material used for pool areas.</summary>
    M_Pool_8,
    /// <summary>A bright, glowing red material.</summary>
    M_BrightRed,
    /// <summary>A variant of the glowing red material with a softer appearance.</summary>
    M_BrightRed_Chill,
    /// <summary>A solid black material.</summary>
    M_DarkBlack,
    /// <summary>A clay-like material for hat shop surfaces.</summary>
    M_HatShop_1,
    /// <summary>A grayish blue material for hat shop surfaces.</summary>
    M_HatShop_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_HatShop_3,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_HatShop_4,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_HatShop_5,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_HatShop_6,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_HatShop_7,
    /// <summary>A bright yellow material with a parallax effect.</summary>
    M_HatShop_8,
    /// <summary>A wobbly, glass-like material.</summary>
    M_ShopGlass,
    /// <summary>A transparent glass material with small, darker spotting.</summary>
    M_House_Glas,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Copyright,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Copyright_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rug_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rug_3,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rug_4,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rug_5,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Rug_6,
    /// <summary>A transparent sky material.</summary>
    M_SkyNice,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Podcast_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Podcast_4,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Projector_1,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Projector_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Projector_3,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Projector_4,
    /// <summary>A bright, glowing white projector light material.</summary>
    M_Projector_5,
    /// <summary>A gray material for shock stick items.</summary>
    M_ShockStick_1,
    /// <summary>A bright orange material for shock stick items.</summary>
    M_ShockStick_2,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_StreamerCamera,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    Material_004,
    /// <summary>A shiny red/magenta material.</summary>
    Material_005,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    Material_010,
    /// <summary>A shiny pink material.</summary>
    Material_011,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    Material_012,
    /// <summary>Unremarkable and/or undocumented material.</summary>
    M_Debug
}

public class GameMaterials
{
    public static Dictionary<GameMaterialType, Material> Materials = new Dictionary<GameMaterialType, Material>();

    private static readonly Dictionary<string, Dictionary<string, GameMaterialType>> PrefabExtractionMap = new Dictionary<string, Dictionary<string, GameMaterialType>>
    {
        { "Hatshop", new Dictionary<string, GameMaterialType> {
            { "M_HatShop 1", GameMaterialType.M_HatShop_1 },
            { "M_HatShop 2", GameMaterialType.M_HatShop_2 },
            { "M_HatShop 3", GameMaterialType.M_HatShop_3 },
            { "M_HatShop 4", GameMaterialType.M_HatShop_4 },
            { "M_HatShop 5", GameMaterialType.M_HatShop_5 },
            { "M_HatShop 6", GameMaterialType.M_HatShop_6 },
            { "M_HatShop 7", GameMaterialType.M_HatShop_7 },
            { "M_HatShop 8", GameMaterialType.M_HatShop_8 },
            { "M_ShopGlass", GameMaterialType.M_ShopGlass },
            { "M_House Glas", GameMaterialType.M_House_Glas },
            { "M_Balaclava", GameMaterialType.VERY_DARK_GRAY },
            { "M_Beanie 1", GameMaterialType.M_Beanie_1 },
            { "M_Bucket 1", GameMaterialType.GRAY_BLUE },
            { "M_Chef 1", GameMaterialType.M_Chef_1 },
            { "M_Floppy 1", GameMaterialType.BEIGE2 },
            { "M_PlayerHatColor", GameMaterialType.M_PlayerHatColor },
            { "M_Foodora", GameMaterialType.BROWN2 },
            { "M_Hair 1", GameMaterialType.BROWN3 },
            { "M_Party 1", GameMaterialType.M_Party_1 },
            { "M_Party 2", GameMaterialType.M_Party_2 },
            { "M_Shroom 1", GameMaterialType.M_Shroom_1 },
            { "M_Shroom 2", GameMaterialType.M_Shroom_2 },
            { "M_Ushanka 1", GameMaterialType.M_Ushanka_1 },
            { "M_Ushanka 2", GameMaterialType.M_Ushanka_2 },
            { "M_Witch", GameMaterialType.M_Witch },
            { "M_Bowler", GameMaterialType.M_Bowler },
            { "M_Cowboy", GameMaterialType.BROWN },
            { "M_Crown", GameMaterialType.GOLD },
            { "M_Knifo", GameMaterialType.M_Knifo },
            { "M_Milk1", GameMaterialType.M_DARKGRAY },
            { "M_Milk2", GameMaterialType.M_Milk2 },
            { "M_News1", GameMaterialType.M_PAPER },
            { "M_News2", GameMaterialType.M_News2 },
            { "M_Top", GameMaterialType.M_Top },
            { "M_Halo", GameMaterialType.GOLD2 },
            { "M_Horn", GameMaterialType.M_Horn },
            { "M_Laser", GameMaterialType.M_Laser },
            { "M_Ears 1", GameMaterialType.DARKGRAY2 },
            { "M_Ears 2", GameMaterialType.MAGENTA },
            { "M_Ears 3", GameMaterialType.DARKGRAY3 },
            { "M_Rug 1", GameMaterialType.M_Rug_1 },
            { "M_Rug 3", GameMaterialType.M_Rug_3 },
            { "M_Rug 4", GameMaterialType.M_Rug_4 },
            { "M_Rug 5", GameMaterialType.M_Rug_5 },
            { "M_Rug 6", GameMaterialType.M_Rug_6 },
            { "AkzidenzGroteskPro-Bold SDF Material", GameMaterialType.AkzidenzGroteskPro_Bold_SDF },
            { "AkzidenzGroteskPro-Light SDF Material", GameMaterialType.AkzidenzGroteskPro_Light_SDF },
            { "M_Child 1", GameMaterialType.SKYBLUE }, { "M_Child 2", GameMaterialType.GREEN }, { "M_Child 3", GameMaterialType.YELLOW }, { "M_Child 4", GameMaterialType.RED }, { "M_Child 5", GameMaterialType.WHITE },
            { "M_Pirate 1", GameMaterialType.M_Pirate_1 }, { "M_Pirate 2", GameMaterialType.M_Pirate_2 }, { "M_Pirate 3", GameMaterialType.M_Pirate_3 },
            { "M_Jester 1", GameMaterialType.YELLOW2 }, { "M_Jester 2", GameMaterialType.VIOLET }, { "M_Jester 3", GameMaterialType.GREEN2 }, { "M_Jester 4", GameMaterialType.YELLOW_ORANGE },
            { "M_Clown 1", GameMaterialType.RED2 }, { "M_Clown 2", GameMaterialType.BEIGE },
            { "M_Hotdog 1", GameMaterialType.WHITE_IVORY }, { "M_Hotdog 2", GameMaterialType.ORANGE_SAUSAGE }, { "M_Hotdog 3", GameMaterialType.RED3 }, { "M_Hotdog 4", GameMaterialType.YELLOW_BEIGE },
            { "M_Rugby 1", GameMaterialType.M_Rugby_1 }, { "M_Rugby 2", GameMaterialType.M_Rugby_2 }, { "M_Rugby 3", GameMaterialType.M_Rugby_3 },
            { "M_Copyright", GameMaterialType.M_Copyright }, { "M_Copyright 1", GameMaterialType.M_Copyright_1 },
            { "M_Cap 1", GameMaterialType.M_Cap_1 },
            { "Material.004", GameMaterialType.Material_004 },
            { "Material.011", GameMaterialType.Material_011 },
            { "Material.012", GameMaterialType.Material_012 }
        }},
        { "AnglerMimic", new Dictionary<string, GameMaterialType> {
            { "M_Player", GameMaterialType.GRAY },
            { "M_Player 1", GameMaterialType.M_Player_1 },
            { "M_PlayerVisor", GameMaterialType.M_PlayerVisor },
            { "M_FlashBeam", GameMaterialType.M_FlashBeam },
            { "M_Metal", GameMaterialType.M_Metal },
            { "M_Eye", GameMaterialType.M_Eye },
            { "LiberationSans SDF Material", GameMaterialType.LiberationSans_SDF }
        }},
        { "Projector", new Dictionary<string, GameMaterialType> {
            { "M_Projector 1", GameMaterialType.M_Projector_1 },
            { "M_Projector 2", GameMaterialType.M_Projector_2 },
            { "M_Projector 3", GameMaterialType.M_Projector_3 },
            { "M_Projector 4", GameMaterialType.M_Projector_4 },
            { "M_Projector 5", GameMaterialType.M_Projector_5 },
            { "M_GreenScreen 1", GameMaterialType.FLAT_GRAY },
            { "M_GreenScreen 2", GameMaterialType.M_GreenScreen_2 },
            { "M_GreenScreen 3", GameMaterialType.M_GreenScreen_3 }
        }},
        { "CinemaScreen", new Dictionary<string, GameMaterialType> {
            { "M_Cinema 1", GameMaterialType.M_Cinema_1 },
            { "M_Cinema 2", GameMaterialType.M_Cinema_2 },
            { "M_Crane", GameMaterialType.M_Crane },
            { "M_Screen", GameMaterialType.M_Screen },
            { "M_VideoDisplayScreen", GameMaterialType.M_VideoDisplayScreen },
            { "M_Cursor", GameMaterialType.M_Cursor }
        }},
        { "Streamer", new Dictionary<string, GameMaterialType> {
            { "M_ShockStick 1", GameMaterialType.M_ShockStick_1 },
            { "M_ShockStick 2", GameMaterialType.M_ShockStick_2 },
            { "M_VFX_ShockStick", GameMaterialType.M_VFX_ShockStick },
            { "M_StreamerCamera", GameMaterialType.M_StreamerCamera },
            { "M_Light_Bright", GameMaterialType.M_Light_Bright },
            { "Material.010", GameMaterialType.Material_010 }
        }},
        { "AnglerMimic2", new Dictionary<string, GameMaterialType> {
            { "M_Flashlight 1 1", GameMaterialType.M_Flashlight_1_1 },
            { "M_Flashlight 1 2", GameMaterialType.M_Flashlight_1_2 },
            { "M_Flashlight Off", GameMaterialType.M_Flashlight_Off },
            { "M_Flashlight Bright", GameMaterialType.BRIGHT_WHITE },
            { "M_BatteryLight_Off", GameMaterialType.M_BatteryLight_Off }
        }},
        { "Fire", new Dictionary<string, GameMaterialType> {
            { "M_Monster", GameMaterialType.M_Monster },
            { "M_VFX_Fire", GameMaterialType.M_VFX_Fire },
            { "M_Debug", GameMaterialType.M_Debug }
        }},
        { "Harpooner", new Dictionary<string, GameMaterialType> {
            { "M_Monster_Harpoon", GameMaterialType.M_Monster_Harpoon },
            { "M_DarkBlack", GameMaterialType.M_DarkBlack },
            { "Material.005", GameMaterialType.Material_005 }
        }},
        { "Ghost", new Dictionary<string, GameMaterialType> {
            { "M_Ghost", GameMaterialType.M_Ghost },
            { "M_Ghost_Teeth", GameMaterialType.M_Ghost_Teeth }
        }},
        { "Flicker", new Dictionary<string, GameMaterialType> {
            { "M_VFX_Tentacle", GameMaterialType.M_VFX_Tentacle },
            { "M_MonsterSkinnyEye", GameMaterialType.M_MonsterSkinnyEye }
        }},
        { "EyeGuy", new Dictionary<string, GameMaterialType> { { "M_MonsterEye", GameMaterialType.M_MonsterEye } }},
        { "Snatcho", new Dictionary<string, GameMaterialType> { { "M_Snatcho", GameMaterialType.M_Snatcho } }},
        { "Slurper", new Dictionary<string, GameMaterialType> {
            { "M_Slurp", GameMaterialType.M_Slurp },
            { "M_World_Local", GameMaterialType.M_World_Local }
        }},
        { "Spider", new Dictionary<string, GameMaterialType> { { "M_Monster_BlackLocal", GameMaterialType.M_Monster_Black } }},
        { "ButtonRobot", new Dictionary<string, GameMaterialType> { { "M_Monster_Red", GameMaterialType.M_Monster_Red } }},
        { "BlackHoleBot", new Dictionary<string, GameMaterialType> {
            { "M_Propeller", GameMaterialType.M_Propeller },
            { "Rajdhani-Regular SDF Material", GameMaterialType.Rajdhani_Regular_SDF }
        }},
        { "Dog", new Dictionary<string, GameMaterialType> {
            { "M_BrightRed", GameMaterialType.M_BrightRed },
            { "M_BrightRed_Chill", GameMaterialType.M_BrightRed_Chill },
            { "M_FlashBeamRed", GameMaterialType.M_FlashBeamRed }
        }},
        { "Angler", new Dictionary<string, GameMaterialType> {
            { "M_World", GameMaterialType.M_World },
            { "Lit", GameMaterialType.Lit },
            { "M_DistrorionParticle", GameMaterialType.M_DistrorionParticle }
        }},
        { "PoolBall", new Dictionary<string, GameMaterialType> {
            { "M_Pool 6", GameMaterialType.M_Pool_6 }, { "M_Pool 7", GameMaterialType.M_Pool_7 }, { "M_Pool 8", GameMaterialType.M_Pool_8 }
        }},
        { "DeckChair", new Dictionary<string, GameMaterialType> { { "M_Pool 4", GameMaterialType.M_Pool_4 } }},
        { "PodcastChair", new Dictionary<string, GameMaterialType> {
            { "M_Podcast 2", GameMaterialType.M_Podcast_2 }, { "M_Podcast 4", GameMaterialType.M_Podcast_4 }
        }},
        { "BoookDissolve", new Dictionary<string, GameMaterialType> {
            { "M_Book 1", GameMaterialType.M_Book_1 }, { "M_Book 2", GameMaterialType.M_Book_2 }
        }},
        { "Chair_Inside", new Dictionary<string, GameMaterialType> { { "M_Interior 7", GameMaterialType.M_Interior_7 } }},
        { "Jello", new Dictionary<string, GameMaterialType> { { "M_SkyNice", GameMaterialType.M_SkyNice } }},
        { "BarnacleBall", new Dictionary<string, GameMaterialType> { { "M_Gas", GameMaterialType.M_Gas } }},
        { "Bombs", new Dictionary<string, GameMaterialType> { { "M_VFX_BombFuze", GameMaterialType.M_VFX_BombFuze } }},
        { "Infiltrator2", new Dictionary<string, GameMaterialType> { { "M_OxygenDisplay", GameMaterialType.M_OxygenDisplay } }},
        { "ExplodedGoop", new Dictionary<string, GameMaterialType> { { "M_Goop", GameMaterialType.M_Goop } }},
        { "UnlockExplosion", new Dictionary<string, GameMaterialType> { { "M_Splash", GameMaterialType.M_Splash } }},
        { "PartyPopperConfetti", new Dictionary<string, GameMaterialType> {
            { "ParticlesUnlit", GameMaterialType.ParticlesUnlit },
            { "M_DefaultParticle", GameMaterialType.M_DefaultParticle }
        }}
    };

    /// <summary>
    /// Scans internal game resources and prefabs to extract and cache materials.
    /// </summary>
    public static void InitMaterials()
    {
        foreach (var entry in PrefabExtractionMap)
        {
            string prefabName = entry.Key;
            Dictionary<string, GameMaterialType> targetMaterials = entry.Value;

            bool allFound = true;
            foreach (var target in targetMaterials.Values)
            {
                if (!Materials.ContainsKey(target))
                {
                    allFound = false;
                    break;
                }
            }
            if (allFound) continue;

            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if (prefab == null) continue;

            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat == null) continue;

                    if (targetMaterials.TryGetValue(mat.name, out GameMaterialType type))
                    {
                        if (!Materials.ContainsKey(type)) Materials.Add(type, mat);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fetches a cached material by its type.
    /// </summary>
    /// <param name="type">The material type to retrieve.</param>
    /// <returns>The Material instance if found; otherwise null.</returns>
    public static Material GetMaterial(GameMaterialType type)
    {
        return Materials.TryGetValue(type, out Material mat) ? mat : null!;
    }

    /// <summary>
    /// Applies a specific GameMaterialType to all renderers of a target GameObject.
    /// </summary>
    /// <param name="target">The GameObject to apply the material to.</param>
    /// <param name="type">The material type to apply.</param>
    /// <param name="deepApply">Whether to apply to children renderers.</param>
    public static void ApplyMaterial(GameObject target, GameMaterialType type, bool deepApply = true)
    {
        if (Materials.TryGetValue(type, out Material mat) && mat != null)
        {
            void editArray(Material[] materials)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = mat;
                }
            }
            if (deepApply)
            {
                foreach (Renderer r in target.GetComponentsInChildren<Renderer>(true))
                {
                    editArray(r.materials);
                }
            }
            else
            {
                var r = target.GetComponent<Renderer>();
                if (r != null)
                {
                    editArray(r.materials);
                }
            }
        }
    }
}
