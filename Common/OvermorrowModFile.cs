using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.Items.Materials;
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
using System.IO;
using OvermorrowMod.Common.Netcode;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModFile : Mod
    {
        // Hotkeys
        public static ModHotKey SandModeKey;
        public static ModHotKey ToggleUI;
        public static ModHotKey AmuletKey;

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

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer != -1 && !Main.gameMenu)
            {
                if (Main.player[Main.myPlayer].active && Main.player[Main.myPlayer].GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave)
                {
                    music = this.GetSoundSlot(SoundType.Music, "Sounds/Music/WaterBiomeMusic");
                    priority = MusicPriority.BiomeHigh;
                }
            }
        }

        public override void Load()
        {
            //Terraria.ModLoader.IO.TagSerializer.AddSerializer(new VectorSerializer());

            // Keys
            SandModeKey = RegisterHotKey("Swap Sand Mode", "Q");
            AmuletKey = RegisterHotKey("Artemis Amulet Attack", "C");
            ToggleUI = RegisterHotKey("Toggle UI", "R"); // This is for debugging

            if (!Main.dedServ)
            {
                Main.logoTexture = Instance.GetTexture("logo");
                Main.logo2Texture = Instance.GetTexture("logo");

                // Effects
                BeamShader = GetEffect("Effects/Beam");
                Ring = GetEffect("Effects/Ring");
                Shockwave = GetEffect("Effects/Shockwave1");
                Shockwave2 = GetEffect("Effects/ShockwaveEffect");
                TextShader = GetEffect("Effects/TextShader");
                TrailShader = GetEffect("Effects/Trail");
                Whiteout = GetEffect("Effects/Whiteout");

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave);
                Ref<Effect> ref2 = new Ref<Effect>(Shockwave2);

                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "ForceField");

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(ref2, "Shockwave"), EffectPriority.VeryHigh);


                TrailTextures = new List<Texture2D>();
                for (int i = 0; i < 7; i++)
                {
                    TrailTextures.Add(GetTexture(AssetDirectory.Trails + "Trail" + i));
                }

                AltarUI = new UserInterface();

                MyInterface = new UserInterface();

                Altar = new AltarUI();
                Altar.Activate();

                Main.itemTexture[ItemID.ChainKnife] = ModContent.GetTexture(AssetDirectory.Textures + "ChainKnife");
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
                Main.logoTexture = ModContent.GetTexture("Terraria/Logo");
                Main.logo2Texture = ModContent.GetTexture("Terraria/Logo2");
            }


            Altar = null;
            SandModeKey = null;
            AmuletKey = null;
            ToggleUI = null;

        }

        public override void PostUpdateEverything()
        {
            Trail.UpdateTrails();
        }

        public override void PostSetupContent()
        {
            if (Main.gameMenu && Main.menuMode >= 0 && !Main.dedServ)
            {
                if (Main.LogoB <= 255)
                {
                    Main.logoTexture = Instance.GetTexture("logo");
                }

                if (Main.LogoB < 10 || (!Main.dayTime && Main.LogoA <= 255))
                {
                    Main.logo2Texture = Instance.GetTexture("logo");
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Chain, 6);
            recipe.AddRecipeGroup("IronBar", 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.ChainKnife);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Silk, 6);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 3);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(ItemID.HermesBoots);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.HermesBoots);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 3);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(ItemID.WaterWalkingBoots);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.CloudinaBottle);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.GoldBar, 10);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.LuckyHorseshoe);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.PlatinumBar, 10);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.LuckyHorseshoe);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddRecipeGroup("IronBar", 8);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.ClimbingClaws);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddRecipeGroup("IronBar", 8);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.ShoeSpikes);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Coral, 6);
            recipe.AddIngredient(ItemID.Starfish, 4);
            recipe.AddIngredient(ModContent.ItemType<EruditeOrb>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.JellyfishNecklace);
            recipe.AddRecipe();
        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                Particle.UpdateParticles();
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkMessageHandler.HandlePacket(reader, whoAmI);
        }
    }
}