using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Particles;
using OvermorrowMod.UI;
using ReLogic.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod
{
    public class OvermorrowModFile : Mod
	{
        // UI
        internal UserInterface MyInterface;
        internal UserInterface AltarUI;

        internal SoulUI Souls;
        internal AltarUI Altar;
        private GameTime _lastUpdateUiGameTime;

        // Hotkeys
        public static ModHotKey SandModeKey;
        public static ModHotKey ToggleUI;

        public static OvermorrowModFile Mod { get; set; }
        public Effect Sword;
        public Effect Shockwave;

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

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            Message msg = (Message)reader.ReadByte();
            switch (msg)
            {
                case Message.AddSoul:
                    XPPacket.Read(reader);
                    break;
            }
        }

        public override void Load()
        {
            // Keys
            SandModeKey = RegisterHotKey("Swap Sand Mode", "Q");
            ToggleUI = RegisterHotKey("Toggle UI", "R"); // This is for debugging

            if (!Main.dedServ)
            {            // Effects
                Sword = GetEffect("Effects/Trailshader");
                Shockwave = GetEffect("Effects/Shockwave1");

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave);
                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "Shockwave");

                Particle.Load();
                TestDetours.Load();

                AltarUI = new UserInterface();

                MyInterface = new UserInterface();

                Altar = new AltarUI();
                Altar.Activate();

                Souls = new SoulUI();
                Souls.Activate();

                Main.itemTexture[ItemID.ChainKnife] = ModContent.GetTexture("OvermorrowMod/Items/Weapons/PreHardmode/Vanilla/ChainKnife");

                if (Main.hardMode)
                {
                    //Main.itemTexture[ModContent.ItemType<HerosBlade>()] = ModContent.GetTexture("OvermorrowMod/Items/Weapons/PreHardmode/Melee/HerosBlade_Tier_2");
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
                    BossName = "Gra-knight and Apollus";
                    BossTitle = "The Super Stoner Bros"; /*The Super Biome Brothers*/
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

        internal void ShowMyUI()
        {
            MyInterface?.SetState(Souls);
        }

        internal void HideMyUI()
        {
            MyInterface?.SetState(null);
        }

        public override void Unload()
        {
            Mod = null;
            PlayerGlowmasks.Unload();

            Sword = null;
            Shockwave = null;
            Particle.Unload();
            TestDetours.Unload();

            Souls = null;
            Altar = null;
            SandModeKey = null;
            ToggleUI = null;
        }

        public override void PostUpdateEverything()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                Particle.UpdateParticles();
            }
        }
    }
}