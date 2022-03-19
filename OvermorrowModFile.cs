using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Hexes;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Particles;
using OvermorrowMod.UI;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using OvermorrowMod.Core;

namespace OvermorrowMod
{
    internal enum Message : byte
    {
        syncPlayer,
        soulAdded,
        soulsChanged,
        meterMaxed,
    }
    public class OvermorrowModFile : Mod
    {
        // UI
        internal UserInterface MyInterface;
        internal UserInterface AltarUI;

        internal AltarUI Altar;
        private GameTime _lastUpdateUiGameTime;

        // Hotkeys
        public static ModHotKey SandModeKey;
        public static ModHotKey ToggleUI;
        public static ModHotKey AmuletKey;

        public static OvermorrowModFile Mod { get; set; }
        public Effect Shockwave;
        public Effect TrailShader;
        public Effect TextShader;

        public static List<Texture2D> TrailTextures;
        public OvermorrowModFile()
        {
            Mod = this;
        }

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
            // Keys
            SandModeKey = RegisterHotKey("Swap Sand Mode", "Q");
            AmuletKey = RegisterHotKey("Artemis Amulet Attack", "C");
            ToggleUI = RegisterHotKey("Toggle UI", "R"); // This is for debugging

            if (!Main.dedServ)
            {
                Main.logoTexture = Mod.GetTexture("logo");
                Main.logo2Texture = Mod.GetTexture("logo");

                // Effects
                Shockwave = GetEffect("Effects/Shockwave1");
                TrailShader = GetEffect("Effects/Trail");
                TextShader = GetEffect("Effects/TextShader");

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave);
                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "ForceField");
                TrailTextures = new List<Texture2D>();
                for (int i = 0; i < 7; i++)
                {
                    TrailTextures.Add(GetTexture("Effects/TrailTextures/Trail" + i));
                }
                ModUtils.Load(false);
                HexLoader.Load(false);
                Particle.Load();
                TestDetours.Load();
                Trail.Load();

                foreach (Type type in Code.GetTypes())
                {
                    HexLoader.TryRegisteringHex(type);
                    Particle.TryRegisteringParticle(type);
                }

                AltarUI = new UserInterface();

                MyInterface = new UserInterface();

                Altar = new AltarUI();
                Altar.Activate();

                Main.itemTexture[ItemID.ChainKnife] = ModContent.GetTexture("OvermorrowMod/Items/Weapons/PreHardmode/Vanilla/ChainKnife");
                if (Main.hardMode)
                {
                    //Main.itemTexture[ModContent.ItemType<HerosBlade>()] = ModContent.GetTexture("OvermorrowMod/Items/Weapons/PreHardmode/Melee/HerosBlade_Tier_2");
                }
            }
        }

        public override void PostSetupContent()
        {
            if (Main.gameMenu && Main.menuMode >= 0)
            {
                if (Main.LogoB <= 255)
                {
                    Main.logoTexture = Mod.GetTexture("logo");
                }

                if (Main.LogoB < 10 || (!Main.dayTime && Main.LogoA <= 255))
                {
                    Main.logo2Texture = Mod.GetTexture("logo");
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

        internal void BossTitle(int BossID)
        {
            string BossName = "";
            string BossTitle = "";
            Color titleColor = Color.White;
            Color nameColor = Color.White;
            switch (BossID)
            {
                case 1:
                    BossName = "Dharuud";
                    BossTitle = "The Sandstorm";
                    nameColor = Color.LightGoldenrodYellow;
                    titleColor = Color.Yellow;
                    break;
                case 2:
                    BossName = "The Storm Drake";
                    BossTitle = "Apex Predator";
                    nameColor = Color.Cyan;
                    titleColor = Color.DarkCyan;
                    break;
                case 3:
                    BossName = "Dripplord";
                    BossTitle = "Bloody Assimilator";
                    nameColor = Color.Red;
                    titleColor = Color.DarkRed;
                    break;
                case 4:
                    BossName = "Iorich";
                    BossTitle = "The Guardian";
                    nameColor = Color.LimeGreen;
                    titleColor = Color.Green;
                    break;
                case 5:
                    BossName = "Gra-knight and Lady Apollo";//"Gra-knight and Apollus";
                    BossTitle = "The Super Stoner Buds";//"The Super Stoner Bros"; /*The Super Biome Brothers*/
                    nameColor = new Color(230, 228, 216);
                    titleColor = new Color(64, 80, 89);
                    break;
                default:
                    BossName = "snoop dogg";
                    BossTitle = "high king";
                    nameColor = Color.LimeGreen;
                    titleColor = Color.Green;
                    break;

            }
            Vector2 textSize = Main.fontDeathText.MeasureString(BossName);
            Vector2 textSize2 = Main.fontDeathText.MeasureString(BossTitle) * 0.5f;
            float textPositionLeft = (Main.screenWidth / 2) - textSize.X / 2f;
            float text2PositionLeft = (Main.screenWidth / 2) - textSize2.X / 2f;
            /*float alpha = 255;
			float alpha2 = 255;*/

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossTitle, new Vector2(text2PositionLeft, (Main.screenHeight / 2 - 250)), titleColor, 0f, Vector2.Zero, 0.6f, 0, 0f);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, BossName, new Vector2(textPositionLeft, (Main.screenHeight / 2 - 300)), nameColor, 0f, Vector2.Zero, 1f, 0, 0f);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (MyInterface?.CurrentState != null && !Main.gameMenu)
            {
                MyInterface.Update(gameTime);
            }

            if (AltarUI?.CurrentState != null && !Main.gameMenu)
            {
                AltarUI.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                       "OvermorrowMod: AltarUI",
                       delegate
                       {
                           if (_lastUpdateUiGameTime != null && AltarUI?.CurrentState != null)
                           {
                               AltarUI.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                           }
                           return true;
                       },
                          InterfaceScaleType.UI));

                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: MyInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && MyInterface?.CurrentState != null)
                        {
                            MyInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                       InterfaceScaleType.UI));


                OvermorrowModPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<OvermorrowModPlayer>();
                if (modPlayer.ShowText)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Title",
                    delegate
                    {
                        BossTitle(modPlayer.TitleID);
                        return true;
                    },
                    InterfaceScaleType.UI));
                }
            }
        }

        internal void ShowAltar()
        {
            AltarUI?.SetState(Altar);
        }

        internal void HideAltar()
        {
            AltarUI?.SetState(null);
        }

        internal void HideMyUI()
        {
            MyInterface?.SetState(null);
        }

        public override void Unload()
        {
            Mod = null;
            Shockwave = null;
            TrailShader = null;
            TextShader = null;

            TrailTextures = null;
            ModUtils.Load(true);
            HexLoader.Load(true);
            Particle.Unload();
            TestDetours.Unload();
            Trail.Unload();

            Main.logoTexture = ModContent.GetTexture("Terraria/Logo");
            Main.logo2Texture = ModContent.GetTexture("Terraria/Logo2");

            Altar = null;
            SandModeKey = null;
            AmuletKey = null;
            ToggleUI = null;

        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                Particle.UpdateParticles();
            }
        }
    }
}