using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Common.VanillaOverrides;
using OvermorrowMod.Core;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModFile : Mod
    {
        // Hotkeys
        public static ModKeybind SandModeKey;
        public static ModKeybind ToggleUI;
        public static ModKeybind AmuletKey;

        public static List<int[]> ModBowsToOverride = new List<int[]>();
        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;

        public Asset<Effect> BeamShader;
        public Asset<Effect> Ring;
        public Asset<Effect> Shockwave;
        public Asset<Effect> Shockwave2;
        public Asset<Effect> TrailShader;
        public Asset<Effect> TextShader;
        public Asset<Effect> Whiteout;
        public Asset<Effect> RadialBlur;
        public Asset<Texture2D> BlurTestTexture;

        public static List<Asset<Texture2D>> TrailTextures;

        public override void Load()
        {
            //Terraria.ModLoader.IO.TagSerializer.AddSerializer(new VectorSerializer());

            // Keys
            SandModeKey = KeybindLoader.RegisterKeybind(this, "Swap Sand Mode", "Q");
            AmuletKey = KeybindLoader.RegisterKeybind(this, "Artemis Amulet Attack", "C");
            ToggleUI = KeybindLoader.RegisterKeybind(this, "Toggle UI", "R"); // This is for debugging

            ShaderLoader.Load();

            if (!Main.dedServ)
            {
                // TODO: These are gone, need to figure out what to do.
                // Main.logoTexture = Instance.GetTexture("logo");
                // Main.logo2Texture = Instance.GetTexture("logo");

                // Effects
                BeamShader = Assets.Request<Effect>("Effects/Beam");
                Ring = Assets.Request<Effect>("Effects/Ring");
                Shockwave = Assets.Request<Effect>("Effects/Shockwave1");
                Shockwave2 = Assets.Request<Effect>("Effects/ShockwaveEffect");
                TextShader = Assets.Request<Effect>("Effects/TextShader");
                TrailShader = Assets.Request<Effect>("Effects/Trail");
                Whiteout = Assets.Request<Effect>("Effects/Whiteout");
                RadialBlur = Assets.Request<Effect>("Effects/CenterBlurShader");
                BlurTestTexture = Assets.Request<Texture2D>("Effects/testpattern");

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave.Value);
                Ref<Effect> ref2 = new Ref<Effect>(Shockwave2.Value);

                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "ForceField");

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(ref2, "Shockwave"), EffectPriority.VeryHigh);


                TrailTextures = new List<Asset<Texture2D>>();
                for (int i = 0; i < 7; i++)
                {
                    TrailTextures.Add(ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail" + i));
                }

                Terraria.GameContent.TextureAssets.Item[ItemID.ChainKnife] = ModContent.Request<Texture2D>(AssetDirectory.Textures + "ChainKnife");
                if (Main.hardMode)
                {
                    //Main.itemTexture[ModContent.ItemType<HerosBlade>()] = ModContent.GetTexture("OvermorrowMod/Items/Weapons/PreHardmode/Melee/HerosBlade_Tier_2");
                }
            }
            ModDetours.Load();
            ModUtils.Load(false);
            HexLoader.Load(false);
            ILEdits.Load();
            Particle.Load();
            Trail.Load();
            Quests.Quests.Load(this);

            GlobalBow.LoadBows();
            GlobalSword.LoadSwords();

            foreach (Type type in Code.GetTypes())
            {
                HexLoader.TryRegisteringHex(type);
                Particle.TryRegisteringParticle(type);
            }
        }

        public override void Unload()
        {
            Instance = null;

            BeamShader = null;
            Ring = null;
            Shockwave = null;
            TrailShader = null;
            TextShader = null;
            Whiteout = null;

            TrailTextures = null;
            ModDetours.Unload();
            ModUtils.Load(true);
            HexLoader.Load(true);
            Quests.Quests.Unload();
            ILEdits.Unload();
            Particle.Unload();
            Trail.Unload();

            GlobalBow.UnloadBows();
            GlobalSword.UnloadSwords();

            SandModeKey = null;
            AmuletKey = null;
            ToggleUI = null;

            ModBowsToOverride.Clear();
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.ChainKnife)
                .AddIngredient(ItemID.Chain, 6)
                .AddRecipeGroup("IronBar", 1)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.HermesBoots)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.WaterWalkingBoots)
                .AddIngredient(ItemID.HermesBoots)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.Cloud, 10)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ClimbingClaws)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ShoeSpikes)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.JellyfishNecklace)
                .AddIngredient(ItemID.Coral, 6)
                .AddIngredient(ItemID.Starfish, 4)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkMessageHandler.HandlePacket(reader, whoAmI);
        }
    }
}
