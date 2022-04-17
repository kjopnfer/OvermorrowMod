using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.Items.Materials;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using OvermorrowMod.Core;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Common.Particles;
using Terraria.Graphics.Effects;
using OvermorrowMod.Content.WorldGeneration;
using System.IO;
using OvermorrowMod.Common.Netcode;
using Terraria.GameContent;

namespace OvermorrowMod.Common
{
    public class OvermorrowModFile : Mod
    {
        // UI

        // Hotkeys
        public static ModKeybind SandModeKey;
        public static ModKeybind ToggleUI;
        public static ModKeybind AmuletKey;

        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;


        public Effect BeamShader;
        public Effect Ring;
        public Effect Shockwave;
        public Effect Shockwave2;
        public Effect TrailShader;
        public Effect TextShader;
        public Effect Whiteout;

        public static List<Texture2D> TrailTextures;

        /* TODO: Replace this with ModSceneEffect logic.
         * public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer != -1 && !Main.gameMenu)
            {
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave)
                {
                    music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/WaterBiomeMusic");
                    priority = MusicPriority.BiomeHigh;
                }
            }
        } */

        public override void Load()
        {
            //Terraria.ModLoader.IO.TagSerializer.AddSerializer(new VectorSerializer());

            // Keys
            SandModeKey = KeybindLoader.RegisterKeybind(this, "Swap Sand Mode", "Q");
            AmuletKey = KeybindLoader.RegisterKeybind(this, "Artemis Amulet Attack", "C");
            ToggleUI = KeybindLoader.RegisterKeybind(this, "Toggle UI", "R"); // This is for debugging

            if (!Main.dedServ)
            {
                // TODO: These are gone, need to figure out what to do.
                // Main.logoTexture = Instance.GetTexture("logo");
                // Main.logo2Texture = Instance.GetTexture("logo");

                // Effects
                BeamShader = ModContent.Request<Effect>("OvermorrowMod/Effects/Beam").Value;
                Ring = ModContent.Request<Effect>("OvermorrowMod/Effects/Ring").Value;
                Shockwave = ModContent.Request<Effect>("OvermorrowMod/Effects/Shockwave1").Value;
                Shockwave2 = ModContent.Request<Effect>("OvermorrowMod/Effects/ShockwaveEffect").Value;
                TextShader = ModContent.Request<Effect>("OvermorrowMod/Effects/TextShader").Value;
                TrailShader = ModContent.Request<Effect>("OvermorrowMod/Effects/Trail").Value;
                Whiteout = ModContent.Request<Effect>("OvermorrowMod/Effects/Whiteout").Value;

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave);
                Ref<Effect> ref2 = new Ref<Effect>(Shockwave2);
                
                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "ForceField");

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(ref2, "Shockwave"), EffectPriority.VeryHigh);


                TrailTextures = new List<Texture2D>();
                for (int i = 0; i < 7; i++)
                {
                    TrailTextures.Add(ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail" + i).Value);
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

            if (!Main.dedServ)
            {
                // Main.logoTexture = ModContent.GetTexture("Terraria/Logo");
                // Main.logo2Texture = ModContent.GetTexture("Terraria/Logo2");
            }
            

            SandModeKey = null;
            AmuletKey = null;
            ToggleUI = null;

        }

        public override void PostSetupContent()
        {
            if (Main.gameMenu && Main.menuMode >= 0 && !Main.dedServ)
            {
                if (Main.LogoB <= 255)
                {
                    // Main.logoTexture = Instance.GetTexture("logo");
                }

                if (Main.LogoB < 10 || (!Main.dayTime && Main.LogoA <= 255))
                {
                    // Main.logo2Texture = Instance.GetTexture("logo");
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(ItemID.ChainKnife)
                .AddIngredient(ItemID.Chain, 6)
                .AddRecipeGroup("IronBar", 1)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.HermesBoots)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            CreateRecipe(ItemID.WaterWalkingBoots)
                .AddIngredient(ItemID.HermesBoots)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            CreateRecipe(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.Cloud, 10)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.ClimbingClaws)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.ShoeSpikes)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(ItemID.JellyfishNecklace)
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