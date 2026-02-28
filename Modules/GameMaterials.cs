using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DbsContentApi.Modules;

/// <summary>
/// Enum representing the original names of materials found in the game.
/// </summary>
public enum GameMaterial
{
    // UI_Procedural_UI_Image,
    // Zorro_UI_FadeIn,
    // Hidden_Universal_Render_Pipeline_Blit,
    // Hidden_SC_Post_Effects_Edge_Detection,
    // Hidden_Universal_Render_Pipeline_HBAO,
    // Default_UI_Material,
    // Hidden_Core_DebugOccluder,
    // Hidden_Core_DebugOcclusionTest,
    // Hidden_Universal_Render_Pipeline_Bloom,
    // Hidden_Universal_Render_Pipeline_LensFlareScreenSpace,
    // Hidden_Universal_Render_Pipeline_LensFlareDataDriven,
    // Hidden_Universal_Render_Pipeline_FinalPost,
    // Hidden_Universal_Render_Pipeline_UberPost,
    // Hidden_Universal_Render_Pipeline_Edge_Adaptive_Spatial_Upsampling,
    // Hidden_Universal_Render_Pipeline_Scaling_Setup,
    // Hidden_Universal_Render_Pipeline_TemporalAA,
    // Hidden_Universal_Render_Pipeline_PaniniProjection,
    // Hidden_Universal_Render_Pipeline_BokehDepthOfField,
    // Hidden_Universal_Render_Pipeline_GaussianDepthOfField,
    // Hidden_Universal_Render_Pipeline_SubpixelMorphologicalAntialiasing,
    // Hidden_Universal_Render_Pipeline_Stop_NaN,
    // Hidden_Universal_Render_Pipeline_LutBuilderHdr,
    // Hidden_Universal_Render_Pipeline_LutBuilderLdr,
    // Hidden_Universal_Render_Pipeline_CopyDepth,
    // Hidden_Universal_Render_Pipeline_CameraMotionVectors,
    // Hidden_Universal_Render_Pipeline_StencilDeferred,
    // Hidden_Universal_Render_Pipeline_Sampling,
    // Hidden_Universal_BlitHDROverlay,
    // Hidden_Universal_CoreBlit,
    // Hidden_Universal_Render_Pipeline_XR_XRMotionVector,
    // Hidden_SingleChannelPlanarYUV420Output,
    // Hidden_Shader_WarpEffect_RLPRO,
    // Hidden_Shader_VHSScanlinesEffect_RLPRO,
    // Hidden_Shader_VHSEffect_RLPRO,
    // Hidden_Shader_UltimateVignetteEffect_RLPRO,
    // Hidden_Shader_TV_RLPRO,
    // Hidden_Shader_PulsatingVignetteEffect_RLPRO,
    // Hidden_Shader_PictureCorrectionEffect_RLPRO,
    // Hidden_Shader_Phosphor_RLPRO,
    // Hidden_Shader_OldFilmEffect_RLPRO,
    // Hidden_Shader_NTSCEncode_RLPRO,
    // Hidden_Shader_NoiseEffects_RLPRO,
    // Hidden_Shader_NegativeEffect_RLPRO,
    // Hidden_Shader_LowResolution_RLPRO,
    // Hidden_Shader_VHS_Tape_Rewind,
    // Hidden_Shader_JitterEffect_RLPRO,
    // RetroLookPro_Glitch3,
    // Hidden_Shader_Glitch2,
    // Hidden_Shader_Glitch1,
    // Hidden_Shader_FisheyeEffect_RLPRO,
    // Hidden_Shader_EdgeStretchEffect_RLPRO,
    // Hidden_Shader_EdgeNoiseEffect_RLPRO,
    // Hidden_Shader_CustomTextureEffect_RLPRO,
    // Hidden_Shader_CRTAperture_RLPRO,
    // Hidden_Shader_ColormapPaletteEffect_RLPRO,
    // Hidden_Shader_CinematicBarsEffect_RLPRO,
    // Hidden_Shader_Bleed_RLPRO_HDRP,
    // Hidden_Shader_AnalogTVNoiseEffect_RLPRO,
    // Hidden_Universal_Render_Pipeline_XR_XRMirrorView,
    // Hidden_Universal_Render_Pipeline_XR_XROcclusionMesh,
    // Hidden_Universal_CoreBlitColorAndDepth,
    // Hidden_CoreSRP_CoreCopy,
    // Hidden_BlitCopy,
    // Hidden_ClearStencil,
    // Hidden_EPO_Fill_Utils_Fill_mask,
    // Hidden_EPO_Fill_Utils_Empty_fill,
    // Hidden_BasicBlit,
    // Hidden_FinalBlit,
    // Hidden_Blur,
    // Hidden_Dilate,
    // Hidden_OutlineMask,
    // Hidden_ZPrepass,
    // Hidden_TransparentBlit,
    // Hidden_Outline,
    // Hidden_Obstacle,
    // Hidden_PartialBlit,
    // Sprites_Mask,
    // Sprites_Default,
    // LiberationSans_SDF_Material,
    // Font_Material,
    M_Debug,
    M_Projector_1,
    M_BoomMic_2,
    M_Projector_4,
    M_Projector_3,
    M_GreenScreen_3,
    M_GreenScreen_1,
    M_GreenScreen_2,
    M_Projector_2,
    M_Projector_5,
    M_Container_2,
    M_Player,
    M_Winch_4,
    M_Winch_6,
    M_Winch_2,
    M_Winch_3,
    M_Winch_1,
    M_Winch_5,
    M_Shroom_1,
    M_ReporterMic_1,
    M_ReporterMic_3,
    M_ReporterMic_2,
    M_Monster_BlackLocal,
    M_HatShop_4,
    M_Book_2,
    M_BoomMic_3,
    M_Monster,
    M_World,
    M_Pool_6,
    M_Jester_1,
    M_Flashlight_2_2,
    M_Flashlight_1_1,
    M_Pool_4,
    M_Radio_4,
    M_Metal,
    M_Hotdog_1,
    M_Goo_3,
    M_Flare_1,
    M_Shroom_2,
    // NotoSansJP_Thin_Atlas_Material,
    // Lit,
    M_FredGull_2,
    M_Hotdog_3,
    M_Rugby_3,
    M_Rug_6,
    M_Hug,
    M_Radio_1,
    // TextMeshPro_Sprite,
    M_Borkin_2,
    M_World_AlphaClip,
    M_DarkBlack,
    M_Flashlight_Bright,
    // AkzidenzGroteskPro_Bold_SDF_Material,
    M_SoundPlayer_4,
    // NotoSansBengali_Bold_Atlas_Material,
    // AkzidenzGroteskPro_Cn_SDF_Material,
    M_Laser_Weak,
    M_BatteryLight_Off,
    M_BatteryLight_Green,
    M_BatteryLight_Yellow,
    M_BatteryLight_Red,
    M_Flashlight_1_2,
    M_Flashlight_Off,
    M_Player_1,
    M_PlayerVisor,
    M_OxygenDisplay,
    M_FlashBeam,
    M_World_Local,
    M_FlatUI,
    M_Hamburger_3,
    M_Norf_1,
    M_Norf_2,
    M_Jester_3,
    M_Splash,
    M_VideoCamera_Recording,
    M_Cinema_2,
    M_Hand,
    M_Cutbox_1,
    M_LongFlashlight_2_1,
    M_Cutbox_2,
    M_CD_Plastic,
    M_CD,
    M_VFX_ShockStick,
    M_Light_Bright,
    M_ShockStick_1,
    M_ShockStick_2,
    M_Pool_7,
    M_Defib_3,
    M_Flashlight_3_2,
    M_Pool_8,
    M_Chef_1,
    M_Helmet,
    M_Pirate_3,
    M_Pirate_1,
    M_PlayerHatColor,
    M_Pirate_2,
    M_Rug_3,
    M_VideoCamera_3,
    M_World_ShelfGrate_3,
    M_HatShop_6,
    M_Horn,
    M_VideoCamera_5,
    M_VideoCamera_1,
    M_VideoCamera_2,
    M_Eye,
    M_CameraScreen,
    M_Flare_2,
    M_FlareEYe,
    M_FlareBeam,
    M_Ushanka_2,
    M_Ushanka_1,
    M_Container_1,
    M_Container_3,
    // NotoSansKR_Thin_Atlas_Material,
    M_Cutbox_3,
    M_Halo,
    M_Beanie_1,
    M_Monster_Dark,
    // Gravity_Bold_SDF_Material,
    M_News2,
    M_World_BarbedWire,
    M_World_ShelfGrate_1,
    M_World_ShelfGrate_2,
    M_DivingBell_1,
    M_DivingBell_2,
    M_DivingBell_3,
    M_DivingBell_3_Local,
    M_DivingBell_3_Local_Actually,
    M_DivingBell_4,
    M_DivingBell_4_Local,
    M_DivingBell_4_Local_Actually,
    M_DivingBell_Lever_Local,
    M_Popper_2,
    M_Popper_1,
    M_Popper_4,
    M_Popper_3,
    M_BigSlapPainting_1,
    ParticlesUnlit,
    // NotoSansBengali_Thin_Atlas_Material,
    M_RescueHook_1,
    M_FredGull_1,
    M_Walkie_Talkie_2,
    M_Walkie_Talkie_1,
    M_Walkie_Talkie_3,
    M_Explosion,
    M_Milk1,
    M_Floppy_1,
    M_Hamburger_1,
    M_Hamburger_2,
    M_Hamburger_4,
    M_Jester_4,
    M_VideoDisplayScreen,
    M_Hair_1,
    M_Clown_2,
    M_Cowboy,
    M_Portrait_3,
    M_RescueHook_2,
    M_HookLight,
    M_Apple_3,
    // Rajdhani_Medium_SDF_Material,
    M_Child_5,
    M_Ears_1,
    M_Radio_2,
    M_Radio_3,
    M_Book_1,
    // NotoSansTC_Bold_Atlas_Material,
    M_Crane,
    M_Cinema_1,
    M_Screen,
    M_Cursor,
    M_Defib_4,
    // NotoSansJP_Bold_Atlas_Material,
    M_Defib_2,
    M_LongFlashlight_2_2,
    M_Laser,
    M_SoundPlayer_3,
    // NotoSans_Thin_Atlas_Material,
    M_Goo_1,
    M_TitleCardRender,
    M_TitleCard_1,
    M_TitleCard,
    M_TitleCard_2,
    M_GooBright,
    M_FredGull_4,
    M_FredGull_3,
    M_Flashlight_3_1,
    M_Crown,
    M_Copyright_1,
    M_Rugby_2,
    M_Interior_7,
    M_Clown_1,
    M_Hotdog_4,
    M_Top,
    M_Rug_5,
    M_Borkin_1,
    // NotoSans_Bold_Atlas_Material,
    M_House_Glas,
    M_Portrait_1,
    M_Copyright,
    M_HatShop_3,
    M_BoomMic_1,
    M_Foodora,
    M_Jester_2,
    M_Child_4,
    M_Balaclava,
    M_Child_2,
    M_ShopGlass,
    M_Podcast_4,
    M_Podcast_2,
    // NotoSans_VariableFont_wdth_wght_Atlas_Material,
    M_Portrait_2,
    M_SoundPlayer_2,
    M_Goop,
    M_SoundPlayer_1,
    M_HatShop_8,
    // AkzidenzGroteskPro_Light_SDF_Material,
    // NotoSansKR_VariableFont_wght_Atlas_Material,
    // NotoSansTC_Thin_Atlas_Material,
    M_News1,
    M_Defib_1,
    M_Apple_2,
    M_HatShop_5,
    M_Rug_1,
    // NotoSansKR_Bold_Atlas_Material,
    M_FrontScreen,
    M_SoundPlayer_5,
    M_Portrait_4,
    M_Child_3,
    M_TaserBeam,
    M_VFX_BombFuze,
    M_Rug_4,
    // NotoSansDevanagari_Thin_Atlas_Material,
    M_Flashlight_2_1,
    M_Apple_1,
    M_DefaultParticle,
    M_Knifo,
    M_Bucket_1,
    M_Bomb,
    M_Ears_2,
    M_Child_1,
    M_Ears_3,
    M_Cap_1,
    // NotoSansDevanagari_Bold_Atlas_Material,
    M_Goo_2,
    M_Party_2,
    // NotoSansSC_Bold_Atlas_Material,
    // NotoSansThaiLooped_Bold_Atlas_Material,
    M_Party_1,
    M_Milk2,
    M_HatShop_7,
    // NotoSansSC_Thin_Atlas_Material,
    M_Brain_1,
    M_HatShop_2,
    M_Rugby_1,
    M_Witch,
    M_Hotdog_2,
    M_HatShop_1,
    M_Bowler,
    M_Brain_2,
    // NotoSansThaiLooped_Thin_Atlas_Material,
    Jello_PostShader,
    // AkzidenzGroteskPro_Regular_SDF_Material,
    // monoflow_regular_Atlas_Material,
    M_DivingScreen_Nice,
    M_Light_Warm,
    M_Asphalt,
    M_Barrel_1,
    M_Barrel_2,
    M_Bed_1,
    M_Bed_2,
    M_Bed_3,
    M_Bed_4,
    M_Bed_5,
    M_Bed_6,
    M_Bed_7,
    M_Bed_Default_1,
    M_House_1,
    M_House_10,
    M_House_11,
    M_House_12,
    M_House_2,
    M_House_3,
    M_House_4,
    M_House_5,
    M_House_6,
    M_House_8,
    M_House_9,
    M_Interior_1,
    M_Interior_2,
    M_Interior_3,
    M_Interior_4,
    M_Interior_5,
    M_Interior_6,
    M_Interior_8,
    M_Rock,
    M_Rug_2,
    M_Rug_7,
    M_Rug_8,
    M_Rug_9,
    M_Solar_1,
    M_Solar_2,
    M_Sunflower_1,
    M_Sunflower_2,
    M_Sunflower_3,
    M_ThePlan_1,
    M_ThePlan_2,
    M_Trunk,
    M_Surf_Bush,
    M_Surf_Ground,
    M_Surf_GroundGrass,
    M_DivingBell1Nice,
    M_DivingBell2Nice,
    M_DivingBell3Nice,
    M_DivingBell4Nice,
    M_DivingBell5Nice,
    M_DivingBell6Nice,
    M_CloudSphere_2,
    M_CloudSphere,
    M_CloudSphere_Evening_2,
    M_CloudSphere_Evening,
    M_Sky,
    M_Sky_Evening,
    // Rajdhani_Bold_SDF_Material,
    // Rajdhani_Light_SDF_Material,
    // Gravity_Book_SDF_Material,
    M_Intro,
    M_House_7,
    // AkzidenzGroteskPro_LightCn_SDF_Material,
    M_Scraper,
    M_Invis,
    M_Mirror,
    M_ArrowIcon,
    M_CamConverter_1,
    M_CamConverter_2,
    M_CamConverter_3,
    M_CamConverter_4,
    M_CamConverter_5,
    M_CameraIcon,
    M_Charge_1,
    M_Charge_2,
    M_Charge_3,
    M_Charge_4,
    M_Charge_5,
    M_Drone_1,
    M_Drone_2,
    M_Drone_3,
    M_Drone_4,
    M_Drone_5,
    M_FaceMachine,
    M_House_13,
    M_House_Door,
    M_Landing,
    M_Laptop,
    M_Machine,
    M_NetworkDeal_1,
    M_NetworkDeal_2,
    M_NetworkDeal_3,
    M_NetworkDeal_4,
    M_Podcast_1,
    M_Podcast_10,
    M_Podcast_11,
    M_Podcast_3,
    M_Podcast_5,
    M_Podcast_6,
    M_Podcast_7,
    M_Podcast_8,
    M_Podcast_9,
    M_Pool_1,
    M_Pool_11,
    M_Pool_2,
    M_Pool_3,
    M_Pool_5,
    M_Shop_1,
    M_Shop_2,
    M_Shop_3,
    M_TV_1,
    M_TV_2,
    M_TV,
    M_Teleport_1,
    M_Teleport_2,
    M_Trampoline_1,
    M_Trampoline_2,
    M_Trampoline_3,
    M_Trampoline_4,
    M_UpgradeSign,
    M_Water,
    M_Wood,
    M_Surf_Bush_NoWind,
    M_Surf_Ground_Dirt,
    M_Surf_Ground_Side,
    M_Surf_Ground_Side_Dirt,
    M_Charge,
    M_Dust,
    M_Heal
}

/// <summary>
/// Enum representing descriptive names for materials.
/// </summary>
public enum DescriptiveMaterial
{
    DEFAULT_SPRITE,
    DEBUG,
    PROJECTOR_BRIGHT,
    GREEN_SCREEN_LUNAR,
    PLAYER_BASE,
    MONSTER_BASE,
    MONSTER_RED,
    MONSTER_BLACK,
    GHOST_BASE,
    SKY_NICE,
    METAL_DARK,
    GOLD_HALO,
    DEEP_BLUE, // Example for M_EARS_3
    // Add more descriptive names as needed
}

public static class GameMaterials
{
    public static readonly Dictionary<GameMaterial, Material> _materials = new();
    public static readonly Dictionary<string, GameMaterial> _nameToEnum = new();
    public static readonly Dictionary<DescriptiveMaterial, GameMaterial> _descriptiveToOriginal = new()
    {
        // { DescriptiveMaterial.DEFAULT_SPRITE, GameMaterial.Sprites_Default },
        { DescriptiveMaterial.DEBUG, GameMaterial.M_Debug },
        { DescriptiveMaterial.PROJECTOR_BRIGHT, GameMaterial.M_Projector_5 },
        { DescriptiveMaterial.GREEN_SCREEN_LUNAR, GameMaterial.M_GreenScreen_3 },
        { DescriptiveMaterial.PLAYER_BASE, GameMaterial.M_Player },
        { DescriptiveMaterial.MONSTER_BASE, GameMaterial.M_Monster },
        { DescriptiveMaterial.MONSTER_BLACK, GameMaterial.M_Monster_BlackLocal },
        { DescriptiveMaterial.METAL_DARK, GameMaterial.M_Metal },
        { DescriptiveMaterial.GOLD_HALO, GameMaterial.M_Halo },
        { DescriptiveMaterial.DEEP_BLUE, GameMaterial.M_Ears_3 },
    };

    static GameMaterials()
    {
        // Initialize mapping from material name to GameMaterial enum
        _nameToEnum["M_Debug"] = GameMaterial.M_Debug;
        _nameToEnum["M_Projector 1"] = GameMaterial.M_Projector_1;
        _nameToEnum["M_BoomMic 2"] = GameMaterial.M_BoomMic_2;
        _nameToEnum["M_Projector 4"] = GameMaterial.M_Projector_4;
        _nameToEnum["M_Projector 3"] = GameMaterial.M_Projector_3;
        _nameToEnum["M_GreenScreen 3"] = GameMaterial.M_GreenScreen_3;
        _nameToEnum["M_GreenScreen 1"] = GameMaterial.M_GreenScreen_1;
        _nameToEnum["M_GreenScreen 2"] = GameMaterial.M_GreenScreen_2;
        _nameToEnum["M_Projector 2"] = GameMaterial.M_Projector_2;
        _nameToEnum["M_Projector 5"] = GameMaterial.M_Projector_5;
        _nameToEnum["M_Container 2"] = GameMaterial.M_Container_2;
        _nameToEnum["M_Player"] = GameMaterial.M_Player;
        _nameToEnum["M_Winch 4"] = GameMaterial.M_Winch_4;
        _nameToEnum["M_Winch 6"] = GameMaterial.M_Winch_6;
        _nameToEnum["M_Winch 2"] = GameMaterial.M_Winch_2;
        _nameToEnum["M_Winch 3"] = GameMaterial.M_Winch_3;
        _nameToEnum["M_Winch 1"] = GameMaterial.M_Winch_1;
        _nameToEnum["M_Winch 5"] = GameMaterial.M_Winch_5;
        _nameToEnum["M_Shroom 1"] = GameMaterial.M_Shroom_1;
        _nameToEnum["M_ReporterMic 1"] = GameMaterial.M_ReporterMic_1;
        _nameToEnum["M_ReporterMic 3"] = GameMaterial.M_ReporterMic_3;
        _nameToEnum["M_ReporterMic 2"] = GameMaterial.M_ReporterMic_2;
        _nameToEnum["M_Monster_BlackLocal"] = GameMaterial.M_Monster_BlackLocal;
        _nameToEnum["M_HatShop 4"] = GameMaterial.M_HatShop_4;
        _nameToEnum["M_Book 2"] = GameMaterial.M_Book_2;
        _nameToEnum["M_BoomMic 3"] = GameMaterial.M_BoomMic_3;
        _nameToEnum["M_Monster"] = GameMaterial.M_Monster;
        _nameToEnum["M_World"] = GameMaterial.M_World;
        _nameToEnum["M_Pool 6"] = GameMaterial.M_Pool_6;
        _nameToEnum["M_Jester 1"] = GameMaterial.M_Jester_1;
        _nameToEnum["M_Flashlight 2 2"] = GameMaterial.M_Flashlight_2_2;
        _nameToEnum["M_Flashlight 1 1"] = GameMaterial.M_Flashlight_1_1;
        _nameToEnum["M_Pool 4"] = GameMaterial.M_Pool_4;
        _nameToEnum["M_Radio 4"] = GameMaterial.M_Radio_4;
        _nameToEnum["M_Metal"] = GameMaterial.M_Metal;
        _nameToEnum["M_Hotdog 1"] = GameMaterial.M_Hotdog_1;
        _nameToEnum["M_Goo 3"] = GameMaterial.M_Goo_3;
        _nameToEnum["M_Flare 1"] = GameMaterial.M_Flare_1;
        _nameToEnum["M_Shroom 2"] = GameMaterial.M_Shroom_2;
        _nameToEnum["M_FredGull 2"] = GameMaterial.M_FredGull_2;
        _nameToEnum["M_Hotdog 3"] = GameMaterial.M_Hotdog_3;
        _nameToEnum["M_Rugby 3"] = GameMaterial.M_Rugby_3;
        _nameToEnum["M_Rug 6"] = GameMaterial.M_Rug_6;
        _nameToEnum["M_Hug"] = GameMaterial.M_Hug;
        _nameToEnum["M_Radio 1"] = GameMaterial.M_Radio_1;
        _nameToEnum["M_Borkin 2"] = GameMaterial.M_Borkin_2;
        _nameToEnum["M_World_AlphaClip"] = GameMaterial.M_World_AlphaClip;
        _nameToEnum["M_DarkBlack"] = GameMaterial.M_DarkBlack;
        _nameToEnum["M_Flashlight Bright"] = GameMaterial.M_Flashlight_Bright;
        _nameToEnum["M_SoundPlayer 4"] = GameMaterial.M_SoundPlayer_4;
        _nameToEnum["M_Laser Weak"] = GameMaterial.M_Laser_Weak;
        _nameToEnum["M_BatteryLight_Off"] = GameMaterial.M_BatteryLight_Off;
        _nameToEnum["M_BatteryLight_Green"] = GameMaterial.M_BatteryLight_Green;
        _nameToEnum["M_BatteryLight_Yellow"] = GameMaterial.M_BatteryLight_Yellow;
        _nameToEnum["M_BatteryLight_Red"] = GameMaterial.M_BatteryLight_Red;
        _nameToEnum["M_Flashlight 1 2"] = GameMaterial.M_Flashlight_1_2;
        _nameToEnum["M_Flashlight Off"] = GameMaterial.M_Flashlight_Off;
        _nameToEnum["M_Player 1"] = GameMaterial.M_Player_1;
        _nameToEnum["M_PlayerVisor"] = GameMaterial.M_PlayerVisor;
        _nameToEnum["M_OxygenDisplay"] = GameMaterial.M_OxygenDisplay;
        _nameToEnum["M_FlashBeam"] = GameMaterial.M_FlashBeam;
        _nameToEnum["M_World_Local"] = GameMaterial.M_World_Local;
        _nameToEnum["M_FlatUI"] = GameMaterial.M_FlatUI;
        _nameToEnum["M_Hamburger 3"] = GameMaterial.M_Hamburger_3;
        _nameToEnum["M_Norf 1"] = GameMaterial.M_Norf_1;
        _nameToEnum["M_Norf 2"] = GameMaterial.M_Norf_2;
        _nameToEnum["M_Jester 3"] = GameMaterial.M_Jester_3;
        _nameToEnum["M_Splash"] = GameMaterial.M_Splash;
        _nameToEnum["M_VideoCamera_Recording"] = GameMaterial.M_VideoCamera_Recording;
        _nameToEnum["M_Cinema 2"] = GameMaterial.M_Cinema_2;
        _nameToEnum["M_Hand"] = GameMaterial.M_Hand;
        _nameToEnum["M_Cutbox 1"] = GameMaterial.M_Cutbox_1;
        _nameToEnum["M_LongFlashlight 2 1"] = GameMaterial.M_LongFlashlight_2_1;
        _nameToEnum["M_Cutbox 2"] = GameMaterial.M_Cutbox_2;
        _nameToEnum["M_CD_Plastic"] = GameMaterial.M_CD_Plastic;
        _nameToEnum["M_CD"] = GameMaterial.M_CD;
        _nameToEnum["M_VFX_ShockStick"] = GameMaterial.M_VFX_ShockStick;
        _nameToEnum["M_Light_Bright"] = GameMaterial.M_Light_Bright;
        _nameToEnum["M_ShockStick 1"] = GameMaterial.M_ShockStick_1;
        _nameToEnum["M_ShockStick 2"] = GameMaterial.M_ShockStick_2;
        _nameToEnum["M_Pool 7"] = GameMaterial.M_Pool_7;
        _nameToEnum["M_Defib 3"] = GameMaterial.M_Defib_3;
        _nameToEnum["M_Flashlight 3 2"] = GameMaterial.M_Flashlight_3_2;
        _nameToEnum["M_Pool 8"] = GameMaterial.M_Pool_8;
        _nameToEnum["M_Chef 1"] = GameMaterial.M_Chef_1;
        _nameToEnum["M_Helmet"] = GameMaterial.M_Helmet;
        _nameToEnum["M_Pirate 3"] = GameMaterial.M_Pirate_3;
        _nameToEnum["M_Pirate 1"] = GameMaterial.M_Pirate_1;
        _nameToEnum["M_PlayerHatColor"] = GameMaterial.M_PlayerHatColor;
        _nameToEnum["M_Pirate 2"] = GameMaterial.M_Pirate_2;
        _nameToEnum["M_Rug 3"] = GameMaterial.M_Rug_3;
        _nameToEnum["M_VideoCamera 3"] = GameMaterial.M_VideoCamera_3;
        _nameToEnum["M_World_ShelfGrate 3"] = GameMaterial.M_World_ShelfGrate_3;
        _nameToEnum["M_HatShop 6"] = GameMaterial.M_HatShop_6;
        _nameToEnum["M_Horn"] = GameMaterial.M_Horn;
        _nameToEnum["M_VideoCamera 5"] = GameMaterial.M_VideoCamera_5;
        _nameToEnum["M_VideoCamera 1"] = GameMaterial.M_VideoCamera_1;
        _nameToEnum["M_VideoCamera 2"] = GameMaterial.M_VideoCamera_2;
        _nameToEnum["M_Eye"] = GameMaterial.M_Eye;
        _nameToEnum["M_CameraScreen"] = GameMaterial.M_CameraScreen;
        _nameToEnum["M_Flare 2"] = GameMaterial.M_Flare_2;
        _nameToEnum["M_FlareEYe"] = GameMaterial.M_FlareEYe;
        _nameToEnum["M_FlareBeam"] = GameMaterial.M_FlareBeam;
        _nameToEnum["M_Ushanka 2"] = GameMaterial.M_Ushanka_2;
        _nameToEnum["M_Ushanka 1"] = GameMaterial.M_Ushanka_1;
        _nameToEnum["M_Container 1"] = GameMaterial.M_Container_1;
        _nameToEnum["M_Container 3"] = GameMaterial.M_Container_3;
        _nameToEnum["M_Cutbox 3"] = GameMaterial.M_Cutbox_3;
        _nameToEnum["M_Halo"] = GameMaterial.M_Halo;
        _nameToEnum["M_Beanie 1"] = GameMaterial.M_Beanie_1;
        _nameToEnum["M_Monster_Dark"] = GameMaterial.M_Monster_Dark;
        // _nameToEnum["Gravity-Bold SDF Material"] = GameMaterial.Gravity_Bold_SDF_Material;
        _nameToEnum["M_News2"] = GameMaterial.M_News2;
        _nameToEnum["M_World_BarbedWire"] = GameMaterial.M_World_BarbedWire;
        _nameToEnum["M_World_ShelfGrate 1"] = GameMaterial.M_World_ShelfGrate_1;
        _nameToEnum["M_World_ShelfGrate 2"] = GameMaterial.M_World_ShelfGrate_2;
        _nameToEnum["M_DivingBell 1"] = GameMaterial.M_DivingBell_1;
        _nameToEnum["M_DivingBell 2"] = GameMaterial.M_DivingBell_2;
        _nameToEnum["M_DivingBell 3"] = GameMaterial.M_DivingBell_3;
        _nameToEnum["M_DivingBell 3_Local"] = GameMaterial.M_DivingBell_3_Local;
        _nameToEnum["M_DivingBell 3_Local Actually"] = GameMaterial.M_DivingBell_3_Local_Actually;
        _nameToEnum["M_DivingBell 4"] = GameMaterial.M_DivingBell_4;
        _nameToEnum["M_DivingBell 4_Local"] = GameMaterial.M_DivingBell_4_Local;
        _nameToEnum["M_DivingBell 4_Local Actually"] = GameMaterial.M_DivingBell_4_Local_Actually;
        _nameToEnum["M_DivingBell Lever_Local"] = GameMaterial.M_DivingBell_Lever_Local;
        _nameToEnum["M_Popper 2"] = GameMaterial.M_Popper_2;
        _nameToEnum["M_Popper 1"] = GameMaterial.M_Popper_1;
        _nameToEnum["M_Popper 4"] = GameMaterial.M_Popper_4;
        _nameToEnum["M_Popper 3"] = GameMaterial.M_Popper_3;
        _nameToEnum["M_BigSlapPainting 1"] = GameMaterial.M_BigSlapPainting_1;
        _nameToEnum["ParticlesUnlit"] = GameMaterial.ParticlesUnlit;
        // _nameToEnum["NotoSansBengali-Thin Atlas Material"] = GameMaterial.NotoSansBengali_Thin_Atlas_Material;
        _nameToEnum["M_RescueHook 1"] = GameMaterial.M_RescueHook_1;
        _nameToEnum["M_FredGull 1"] = GameMaterial.M_FredGull_1;
        _nameToEnum["M_Walkie Talkie 2"] = GameMaterial.M_Walkie_Talkie_2;
        _nameToEnum["M_Walkie Talkie 1"] = GameMaterial.M_Walkie_Talkie_1;
        _nameToEnum["M_Walkie Talkie 3"] = GameMaterial.M_Walkie_Talkie_3;
        _nameToEnum["M_Explosion"] = GameMaterial.M_Explosion;
        _nameToEnum["M_Milk1"] = GameMaterial.M_Milk1;
        _nameToEnum["M_Floppy 1"] = GameMaterial.M_Floppy_1;
        _nameToEnum["M_Hamburger 1"] = GameMaterial.M_Hamburger_1;
        _nameToEnum["M_Hamburger 2"] = GameMaterial.M_Hamburger_2;
        _nameToEnum["M_Hamburger 4"] = GameMaterial.M_Hamburger_4;
        _nameToEnum["M_Jester 4"] = GameMaterial.M_Jester_4;
        _nameToEnum["M_VideoDisplayScreen"] = GameMaterial.M_VideoDisplayScreen;
        _nameToEnum["M_Hair 1"] = GameMaterial.M_Hair_1;
        _nameToEnum["M_Clown 2"] = GameMaterial.M_Clown_2;
        _nameToEnum["M_Cowboy"] = GameMaterial.M_Cowboy;
        _nameToEnum["M_Portrait 3"] = GameMaterial.M_Portrait_3;
        _nameToEnum["M_RescueHook 2"] = GameMaterial.M_RescueHook_2;
        _nameToEnum["M_HookLight"] = GameMaterial.M_HookLight;
        _nameToEnum["M_Apple 3"] = GameMaterial.M_Apple_3;
        // _nameToEnum["Rajdhani-Medium SDF Material"] = GameMaterial.Rajdhani_Medium_SDF_Material;
        _nameToEnum["M_Child 5"] = GameMaterial.M_Child_5;
        _nameToEnum["M_Ears 1"] = GameMaterial.M_Ears_1;
        _nameToEnum["M_Radio 2"] = GameMaterial.M_Radio_2;
        _nameToEnum["M_Radio 3"] = GameMaterial.M_Radio_3;
        _nameToEnum["M_Book 1"] = GameMaterial.M_Book_1;
        // _nameToEnum["NotoSansTC-Bold Atlas Material"] = GameMaterial.NotoSansTC_Bold_Atlas_Material;
        _nameToEnum["M_Crane"] = GameMaterial.M_Crane;
        _nameToEnum["M_Cinema 1"] = GameMaterial.M_Cinema_1;
        _nameToEnum["M_Screen"] = GameMaterial.M_Screen;
        _nameToEnum["M_Cursor"] = GameMaterial.M_Cursor;
        _nameToEnum["M_Defib 4"] = GameMaterial.M_Defib_4;
        // _nameToEnum["NotoSansJP-Bold Atlas Material"] = GameMaterial.NotoSansJP_Bold_Atlas_Material;
        _nameToEnum["M_Defib 2"] = GameMaterial.M_Defib_2;
        _nameToEnum["M_LongFlashlight 2 2"] = GameMaterial.M_LongFlashlight_2_2;
        _nameToEnum["M_Laser"] = GameMaterial.M_Laser;
        _nameToEnum["M_SoundPlayer 3"] = GameMaterial.M_SoundPlayer_3;
        // _nameToEnum["NotoSans-Thin Atlas Material"] = GameMaterial.NotoSans_Thin_Atlas_Material;
        _nameToEnum["M_Goo 1"] = GameMaterial.M_Goo_1;
        _nameToEnum["M_TitleCardRender"] = GameMaterial.M_TitleCardRender;
        _nameToEnum["M_TitleCard 1"] = GameMaterial.M_TitleCard_1;
        _nameToEnum["M_TitleCard"] = GameMaterial.M_TitleCard;
        _nameToEnum["M_TitleCard 2"] = GameMaterial.M_TitleCard_2;
        _nameToEnum["M_GooBright"] = GameMaterial.M_GooBright;
        _nameToEnum["M_FredGull 4"] = GameMaterial.M_FredGull_4;
        _nameToEnum["M_FredGull 3"] = GameMaterial.M_FredGull_3;
        _nameToEnum["M_Flashlight 3 1"] = GameMaterial.M_Flashlight_3_1;
        _nameToEnum["M_Crown"] = GameMaterial.M_Crown;
        _nameToEnum["M_Copyright 1"] = GameMaterial.M_Copyright_1;
        _nameToEnum["M_Rugby 2"] = GameMaterial.M_Rugby_2;
        _nameToEnum["M_Interior 7"] = GameMaterial.M_Interior_7;
        _nameToEnum["M_Clown 1"] = GameMaterial.M_Clown_1;
        _nameToEnum["M_Hotdog 4"] = GameMaterial.M_Hotdog_4;
        _nameToEnum["M_Top"] = GameMaterial.M_Top;
        _nameToEnum["M_Rug 5"] = GameMaterial.M_Rug_5;
        _nameToEnum["M_Borkin 1"] = GameMaterial.M_Borkin_1;
        // _nameToEnum["NotoSans-Bold Atlas Material"] = GameMaterial.NotoSans_Bold_Atlas_Material;
        _nameToEnum["M_House Glas"] = GameMaterial.M_House_Glas;
        _nameToEnum["M_Portrait 1"] = GameMaterial.M_Portrait_1;
        _nameToEnum["M_Copyright"] = GameMaterial.M_Copyright;
        _nameToEnum["M_HatShop 3"] = GameMaterial.M_HatShop_3;
        _nameToEnum["M_BoomMic 1"] = GameMaterial.M_BoomMic_1;
        _nameToEnum["M_Foodora"] = GameMaterial.M_Foodora;
        _nameToEnum["M_Jester 2"] = GameMaterial.M_Jester_2;
        _nameToEnum["M_Child 4"] = GameMaterial.M_Child_4;
        _nameToEnum["M_Balaclava"] = GameMaterial.M_Balaclava;
        _nameToEnum["M_Child 2"] = GameMaterial.M_Child_2;
        _nameToEnum["M_ShopGlass"] = GameMaterial.M_ShopGlass;
        _nameToEnum["M_Podcast 4"] = GameMaterial.M_Podcast_4;
        _nameToEnum["M_Podcast 2"] = GameMaterial.M_Podcast_2;
        // _nameToEnum["NotoSans-VariableFont_wdth,wght Atlas Material"] = GameMaterial.NotoSans_VariableFont_wdth_wght_Atlas_Material;
        _nameToEnum["M_Portrait 2"] = GameMaterial.M_Portrait_2;
        _nameToEnum["M_SoundPlayer 2"] = GameMaterial.M_SoundPlayer_2;
        _nameToEnum["M_Goop"] = GameMaterial.M_Goop;
        _nameToEnum["M_SoundPlayer 1"] = GameMaterial.M_SoundPlayer_1;
        _nameToEnum["M_HatShop 8"] = GameMaterial.M_HatShop_8;
        // _nameToEnum["AkzidenzGroteskPro-Light SDF Material"] = GameMaterial.AkzidenzGroteskPro_Light_SDF_Material;
        // _nameToEnum["NotoSansKR-VariableFont_wght Atlas Material"] = GameMaterial.NotoSansKR_VariableFont_wght_Atlas_Material;
        // _nameToEnum["NotoSansTC-Thin Atlas Material"] = GameMaterial.NotoSansTC_Thin_Atlas_Material;
        _nameToEnum["M_News1"] = GameMaterial.M_News1;
        _nameToEnum["M_Defib 1"] = GameMaterial.M_Defib_1;
        _nameToEnum["M_Apple 2"] = GameMaterial.M_Apple_2;
        _nameToEnum["M_HatShop 5"] = GameMaterial.M_HatShop_5;
        _nameToEnum["M_Rug 1"] = GameMaterial.M_Rug_1;
        // _nameToEnum["NotoSansKR-Bold Atlas Material"] = GameMaterial.NotoSansKR_Bold_Atlas_Material;
        _nameToEnum["M_FrontScreen"] = GameMaterial.M_FrontScreen;
        _nameToEnum["M_SoundPlayer 5"] = GameMaterial.M_SoundPlayer_5;
        _nameToEnum["M_Portrait 4"] = GameMaterial.M_Portrait_4;
        _nameToEnum["M_Child 3"] = GameMaterial.M_Child_3;
        _nameToEnum["M_TaserBeam"] = GameMaterial.M_TaserBeam;
        _nameToEnum["M_VFX_BombFuze"] = GameMaterial.M_VFX_BombFuze;
        _nameToEnum["M_Rug 4"] = GameMaterial.M_Rug_4;
        // _nameToEnum["NotoSansDevanagari-Thin Atlas Material"] = GameMaterial.NotoSansDevanagari_Thin_Atlas_Material;
        _nameToEnum["M_Flashlight 2 1"] = GameMaterial.M_Flashlight_2_1;
        _nameToEnum["M_Apple 1"] = GameMaterial.M_Apple_1;
        _nameToEnum["M_DefaultParticle"] = GameMaterial.M_DefaultParticle;
        _nameToEnum["M_Knifo"] = GameMaterial.M_Knifo;
        _nameToEnum["M_Bucket 1"] = GameMaterial.M_Bucket_1;
        _nameToEnum["M_Bomb"] = GameMaterial.M_Bomb;
        _nameToEnum["M_Ears 2"] = GameMaterial.M_Ears_2;
        _nameToEnum["M_Child 1"] = GameMaterial.M_Child_1;
        _nameToEnum["M_Ears 3"] = GameMaterial.M_Ears_3;
        _nameToEnum["M_Cap 1"] = GameMaterial.M_Cap_1;
        // _nameToEnum["NotoSansDevanagari-Bold Atlas Material"] = GameMaterial.NotoSansDevanagari_Bold_Atlas_Material;
        _nameToEnum["M_Goo 2"] = GameMaterial.M_Goo_2;
        _nameToEnum["M_Party 2"] = GameMaterial.M_Party_2;
        // _nameToEnum["NotoSansSC-Bold Atlas Material"] = GameMaterial.NotoSansSC_Bold_Atlas_Material;
        // _nameToEnum["NotoSansThaiLooped-Bold Atlas Material"] = GameMaterial.NotoSansThaiLooped_Bold_Atlas_Material;
        _nameToEnum["M_Party 1"] = GameMaterial.M_Party_1;
        _nameToEnum["M_Milk2"] = GameMaterial.M_Milk2;
        _nameToEnum["M_HatShop 7"] = GameMaterial.M_HatShop_7;
        // _nameToEnum["NotoSansSC-Thin Atlas Material"] = GameMaterial.NotoSansSC_Thin_Atlas_Material;
        _nameToEnum["M_Brain 1"] = GameMaterial.M_Brain_1;
        _nameToEnum["M_HatShop 2"] = GameMaterial.M_HatShop_2;
        _nameToEnum["M_Rugby 1"] = GameMaterial.M_Rugby_1;
        _nameToEnum["M_Witch"] = GameMaterial.M_Witch;
        _nameToEnum["M_Hotdog 2"] = GameMaterial.M_Hotdog_2;
        _nameToEnum["M_HatShop 1"] = GameMaterial.M_HatShop_1;
        _nameToEnum["M_Bowler"] = GameMaterial.M_Bowler;
        _nameToEnum["M_Brain 2"] = GameMaterial.M_Brain_2;
        // _nameToEnum["NotoSansThaiLooped-Thin Atlas Material"] = GameMaterial.NotoSansThaiLooped_Thin_Atlas_Material;
        _nameToEnum["Jello_PostShader"] = GameMaterial.Jello_PostShader;
        // _nameToEnum["AkzidenzGroteskPro-Regular SDF Material"] = GameMaterial.AkzidenzGroteskPro_Regular_SDF_Material;
        // _nameToEnum["monoflow-regular Atlas Material"] = GameMaterial.monoflow_regular_Atlas_Material;
        _nameToEnum["M_DivingScreen_Nice"] = GameMaterial.M_DivingScreen_Nice;
        _nameToEnum["M_Light_Warm"] = GameMaterial.M_Light_Warm;
        _nameToEnum["M_Asphalt"] = GameMaterial.M_Asphalt;
        _nameToEnum["M_Barrel 1"] = GameMaterial.M_Barrel_1;
        _nameToEnum["M_Barrel 2"] = GameMaterial.M_Barrel_2;
        _nameToEnum["M_Bed 1"] = GameMaterial.M_Bed_1;
        _nameToEnum["M_Bed 2"] = GameMaterial.M_Bed_2;
        _nameToEnum["M_Bed 3"] = GameMaterial.M_Bed_3;
        _nameToEnum["M_Bed 4"] = GameMaterial.M_Bed_4;
        _nameToEnum["M_Bed 5"] = GameMaterial.M_Bed_5;
        _nameToEnum["M_Bed 6"] = GameMaterial.M_Bed_6;
        _nameToEnum["M_Bed 7"] = GameMaterial.M_Bed_7;
        _nameToEnum["M_Bed Default 1"] = GameMaterial.M_Bed_Default_1;
        _nameToEnum["M_House 1"] = GameMaterial.M_House_1;
        _nameToEnum["M_House 10"] = GameMaterial.M_House_10;
        _nameToEnum["M_House 11"] = GameMaterial.M_House_11;
        _nameToEnum["M_House 12"] = GameMaterial.M_House_12;
        _nameToEnum["M_House 2"] = GameMaterial.M_House_2;
        _nameToEnum["M_House 3"] = GameMaterial.M_House_3;
        _nameToEnum["M_House 4"] = GameMaterial.M_House_4;
        _nameToEnum["M_House 5"] = GameMaterial.M_House_5;
        _nameToEnum["M_House 6"] = GameMaterial.M_House_6;
        _nameToEnum["M_House 8"] = GameMaterial.M_House_8;
        _nameToEnum["M_House 9"] = GameMaterial.M_House_9;
        _nameToEnum["M_Interior 1"] = GameMaterial.M_Interior_1;
        _nameToEnum["M_Interior 2"] = GameMaterial.M_Interior_2;
        _nameToEnum["M_Interior 3"] = GameMaterial.M_Interior_3;
        _nameToEnum["M_Interior 4"] = GameMaterial.M_Interior_4;
        _nameToEnum["M_Interior 5"] = GameMaterial.M_Interior_5;
        _nameToEnum["M_Interior 6"] = GameMaterial.M_Interior_6;
        _nameToEnum["M_Interior 8"] = GameMaterial.M_Interior_8;
        _nameToEnum["M_Rock"] = GameMaterial.M_Rock;
        _nameToEnum["M_Rug 2"] = GameMaterial.M_Rug_2;
        _nameToEnum["M_Rug 7"] = GameMaterial.M_Rug_7;
        _nameToEnum["M_Rug 8"] = GameMaterial.M_Rug_8;
        _nameToEnum["M_Rug 9"] = GameMaterial.M_Rug_9;
        _nameToEnum["M_Solar 1"] = GameMaterial.M_Solar_1;
        _nameToEnum["M_Solar 2"] = GameMaterial.M_Solar_2;
        _nameToEnum["M_Sunflower 1"] = GameMaterial.M_Sunflower_1;
        _nameToEnum["M_Sunflower 2"] = GameMaterial.M_Sunflower_2;
        _nameToEnum["M_Sunflower 3"] = GameMaterial.M_Sunflower_3;
        _nameToEnum["M_ThePlan 1"] = GameMaterial.M_ThePlan_1;
        _nameToEnum["M_ThePlan 2"] = GameMaterial.M_ThePlan_2;
        _nameToEnum["M_Trunk"] = GameMaterial.M_Trunk;
        _nameToEnum["M_Surf_Bush"] = GameMaterial.M_Surf_Bush;
        _nameToEnum["M_Surf_Ground"] = GameMaterial.M_Surf_Ground;
        _nameToEnum["M_Surf_GroundGrass"] = GameMaterial.M_Surf_GroundGrass;
        _nameToEnum["M_DivingBell1Nice"] = GameMaterial.M_DivingBell1Nice;
        _nameToEnum["M_DivingBell2Nice"] = GameMaterial.M_DivingBell2Nice;
        _nameToEnum["M_DivingBell3Nice"] = GameMaterial.M_DivingBell3Nice;
        _nameToEnum["M_DivingBell4Nice"] = GameMaterial.M_DivingBell4Nice;
        _nameToEnum["M_DivingBell5Nice"] = GameMaterial.M_DivingBell5Nice;
        _nameToEnum["M_DivingBell6Nice"] = GameMaterial.M_DivingBell6Nice;
        _nameToEnum["M_CloudSphere 2"] = GameMaterial.M_CloudSphere_2;
        _nameToEnum["M_CloudSphere"] = GameMaterial.M_CloudSphere;
        _nameToEnum["M_CloudSphere_Evening 2"] = GameMaterial.M_CloudSphere_Evening_2;
        _nameToEnum["M_CloudSphere_Evening"] = GameMaterial.M_CloudSphere_Evening;
        _nameToEnum["M_Sky"] = GameMaterial.M_Sky;
        _nameToEnum["M_Sky_Evening"] = GameMaterial.M_Sky_Evening;
        // _nameToEnum["Rajdhani-Bold SDF Material"] = GameMaterial.Rajdhani_Bold_SDF_Material;
        // _nameToEnum["Rajdhani-Light SDF Material"] = GameMaterial.Rajdhani_Light_SDF_Material;
        // _nameToEnum["Gravity-Book SDF Material"] = GameMaterial.Gravity_Book_SDF_Material;
        _nameToEnum["M_Intro"] = GameMaterial.M_Intro;
        _nameToEnum["M_House 7"] = GameMaterial.M_House_7;
        // _nameToEnum["AkzidenzGroteskPro-LightCn SDF Material"] = GameMaterial.AkzidenzGroteskPro_LightCn_SDF_Material;
        _nameToEnum["M_Scraper"] = GameMaterial.M_Scraper;
        _nameToEnum["M_Invis"] = GameMaterial.M_Invis;
        _nameToEnum["M_Mirror"] = GameMaterial.M_Mirror;
        _nameToEnum["M_ArrowIcon"] = GameMaterial.M_ArrowIcon;
        _nameToEnum["M_CamConverter 1"] = GameMaterial.M_CamConverter_1;
        _nameToEnum["M_CamConverter 2"] = GameMaterial.M_CamConverter_2;
        _nameToEnum["M_CamConverter 3"] = GameMaterial.M_CamConverter_3;
        _nameToEnum["M_CamConverter 4"] = GameMaterial.M_CamConverter_4;
        _nameToEnum["M_CamConverter 5"] = GameMaterial.M_CamConverter_5;
        _nameToEnum["M_CameraIcon"] = GameMaterial.M_CameraIcon;
        _nameToEnum["M_Charge 1"] = GameMaterial.M_Charge_1;
        _nameToEnum["M_Charge 2"] = GameMaterial.M_Charge_2;
        _nameToEnum["M_Charge 3"] = GameMaterial.M_Charge_3;
        _nameToEnum["M_Charge 4"] = GameMaterial.M_Charge_4;
        _nameToEnum["M_Charge 5"] = GameMaterial.M_Charge_5;
        _nameToEnum["M_Drone 1"] = GameMaterial.M_Drone_1;
        _nameToEnum["M_Drone 2"] = GameMaterial.M_Drone_2;
        _nameToEnum["M_Drone 3"] = GameMaterial.M_Drone_3;
        _nameToEnum["M_Drone 4"] = GameMaterial.M_Drone_4;
        _nameToEnum["M_Drone 5"] = GameMaterial.M_Drone_5;
        _nameToEnum["M_FaceMachine"] = GameMaterial.M_FaceMachine;
        _nameToEnum["M_House 13"] = GameMaterial.M_House_13;
        _nameToEnum["M_House Door"] = GameMaterial.M_House_Door;
        _nameToEnum["M_Landing"] = GameMaterial.M_Landing;
        _nameToEnum["M_Laptop"] = GameMaterial.M_Laptop;
        _nameToEnum["M_Machine"] = GameMaterial.M_Machine;
        _nameToEnum["M_NetworkDeal 1"] = GameMaterial.M_NetworkDeal_1;
        _nameToEnum["M_NetworkDeal 2"] = GameMaterial.M_NetworkDeal_2;
        _nameToEnum["M_NetworkDeal 3"] = GameMaterial.M_NetworkDeal_3;
        _nameToEnum["M_NetworkDeal 4"] = GameMaterial.M_NetworkDeal_4;
        _nameToEnum["M_Podcast 1"] = GameMaterial.M_Podcast_1;
        _nameToEnum["M_Podcast 10"] = GameMaterial.M_Podcast_10;
        _nameToEnum["M_Podcast 11"] = GameMaterial.M_Podcast_11;
        _nameToEnum["M_Podcast 3"] = GameMaterial.M_Podcast_3;
        _nameToEnum["M_Podcast 5"] = GameMaterial.M_Podcast_5;
        _nameToEnum["M_Podcast 6"] = GameMaterial.M_Podcast_6;
        _nameToEnum["M_Podcast 7"] = GameMaterial.M_Podcast_7;
        _nameToEnum["M_Podcast 8"] = GameMaterial.M_Podcast_8;
        _nameToEnum["M_Podcast 9"] = GameMaterial.M_Podcast_9;
        _nameToEnum["M_Pool 1"] = GameMaterial.M_Pool_1;
        _nameToEnum["M_Pool 11"] = GameMaterial.M_Pool_11;
        _nameToEnum["M_Pool 2"] = GameMaterial.M_Pool_2;
        _nameToEnum["M_Pool 3"] = GameMaterial.M_Pool_3;
        _nameToEnum["M_Pool 5"] = GameMaterial.M_Pool_5;
        _nameToEnum["M_Shop 1"] = GameMaterial.M_Shop_1;
        _nameToEnum["M_Shop 2"] = GameMaterial.M_Shop_2;
        _nameToEnum["M_Shop 3"] = GameMaterial.M_Shop_3;
        _nameToEnum["M_TV 1"] = GameMaterial.M_TV_1;
        _nameToEnum["M_TV 2"] = GameMaterial.M_TV_2;
        _nameToEnum["M_TV"] = GameMaterial.M_TV;
        _nameToEnum["M_Teleport 1"] = GameMaterial.M_Teleport_1;
        _nameToEnum["M_Teleport 2"] = GameMaterial.M_Teleport_2;
        _nameToEnum["M_Trampoline 1"] = GameMaterial.M_Trampoline_1;
        _nameToEnum["M_Trampoline 2"] = GameMaterial.M_Trampoline_2;
        _nameToEnum["M_Trampoline 3"] = GameMaterial.M_Trampoline_3;
        _nameToEnum["M_Trampoline 4"] = GameMaterial.M_Trampoline_4;
        _nameToEnum["M_UpgradeSign"] = GameMaterial.M_UpgradeSign;
        _nameToEnum["M_Water"] = GameMaterial.M_Water;
        _nameToEnum["M_Wood"] = GameMaterial.M_Wood;
        _nameToEnum["M_Surf_Bush_NoWind"] = GameMaterial.M_Surf_Bush_NoWind;
        _nameToEnum["M_Surf_Ground_Dirt"] = GameMaterial.M_Surf_Ground_Dirt;
        _nameToEnum["M_Surf_Ground_Side"] = GameMaterial.M_Surf_Ground_Side;
        _nameToEnum["M_Surf_Ground_Side_Dirt"] = GameMaterial.M_Surf_Ground_Side_Dirt;
        _nameToEnum["M_Charge"] = GameMaterial.M_Charge;
        _nameToEnum["M_Dust"] = GameMaterial.M_Dust;
        _nameToEnum["M_Heal"] = GameMaterial.M_Heal;
    }

    /// <summary>
    /// Loads all materials from the game resources.
    /// </summary>
    public static void InitMaterials()
    {
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        // lots of duplicates so lets remove them
        var uniqueMaterialNames = new HashSet<string>();
        var uniqueMaterials = new List<Material>();
        foreach (Material mat in allMaterials)
        {
            if (uniqueMaterialNames.Add(mat.name))
            {
                uniqueMaterials.Add(mat);
            }
        }

        Logger.Log($"Found {uniqueMaterials.Count} unique materials in game resources");
        Logger.Log($"Unique material names: {string.Join(", ", uniqueMaterialNames.ToList())}");

        foreach (Material mat in uniqueMaterials)
        {
            if (_nameToEnum.TryGetValue(mat.name, out GameMaterial type))
            {
                if (!_materials.ContainsKey(type))
                {
                    _materials[type] = mat;
                }
            }
            else
            {
                Logger.LogError($"Could not find material: {mat.name} in _nameToEnum, skipping");
            }
        }
        Logger.Log($"Loaded {_materials.Count} materials");
    }

    public static Material GetMaterial(GameMaterial type) => _materials.TryGetValue(type, out var mat) ? mat : null!;

    public static Material GetMaterial(DescriptiveMaterial type)
    {
        if (_descriptiveToOriginal.TryGetValue(type, out var original))
        {
            return GetMaterial(original);
        }
        return null!;
    }

    public static void ApplyMaterial(GameObject target, GameMaterial type, bool deepApply = true)
    {
        Material mat = GetMaterial(type);
        if (mat != null) ApplyToTarget(target, mat, deepApply);
    }

    public static void ApplyMaterial(GameObject target, DescriptiveMaterial type, bool deepApply = true)
    {
        Material mat = GetMaterial(type);
        if (mat != null) ApplyToTarget(target, mat, deepApply);
    }

    private static void ApplyToTarget(GameObject target, Material mat, bool deepApply)
    {
        if (deepApply)
        {
            foreach (Renderer r in target.GetComponentsInChildren<Renderer>(true))
            {
                ApplyToRenderer(r, mat);
            }
        }
        else
        {
            var r = target.GetComponent<Renderer>();
            if (r != null) ApplyToRenderer(r, mat);
        }
    }

    private static void ApplyToRenderer(Renderer r, Material mat)
    {
        Material[] mats = r.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = mat;
        }
        r.materials = mats;
    }
}
