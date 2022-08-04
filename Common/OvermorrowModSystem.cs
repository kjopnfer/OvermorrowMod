using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.NPCs.Bosses.Eye;
using OvermorrowMod.Common.VanillaOverrides;
using OvermorrowMod.Content.UI;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Common
{
    public class OvermorrowModSystem : ModSystem
    {
        public static OvermorrowModSystem Instance { get; set; }
        public OvermorrowModSystem()
        {
            Instance = this;
        }

        private GameTime _lastUpdateUiGameTime;
        internal UserInterface MyInterface;
        internal UserInterface AltarUI;
        internal UserInterface TitleInterface;

        internal AltarUI Altar;
        internal TitleCard TitleCard;

        internal TrajectoryDraw trajectoryDraw;
        private UserInterface trajDraw;

        internal bowChargeDraw BowChargeDraw;
        private UserInterface bowCargDraw;

        public static bool shid;
        public static int[] bow2Send;
        public string faef = "foof";

        public override void PostUpdateEverything()
        {
            Trail.UpdateTrails();
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                AltarUI = new UserInterface();

                MyInterface = new UserInterface();

                Altar = new AltarUI();
                Altar.Activate();

                trajectoryDraw = new TrajectoryDraw();
                trajectoryDraw.Activate();
                trajDraw = new UserInterface();
                trajDraw.SetState(trajectoryDraw);

                BowChargeDraw = new bowChargeDraw();
                BowChargeDraw.Activate();
                bowCargDraw = new UserInterface();
                bowCargDraw.SetState(BowChargeDraw);

                TitleInterface = new UserInterface();
                TitleCard = new TitleCard();
                TitleInterface.SetState(TitleCard);
            }
        }

        public override void Unload()
        {
            shid = false;
            bow2Send = new int[] { };
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

            trajDraw?.Update(gameTime);

            bowCargDraw?.Update(gameTime);
        }

        internal void BossTitle(int BossID)
        {
            string BossName;
            string BossTitle;
            Color titleColor = Color.White;
            Color nameColor = Color.White;

            switch (BossID)
            {
                case 1:
                    BossName = "eye of cthulhu";
                    BossTitle = "the gamer";
                    break;
                default:
                    BossName = "snoop dogg";
                    BossTitle = "high king";
                    nameColor = Color.LimeGreen;
                    titleColor = Color.Green;
                    break;

            }
            Vector2 nameSize = FontAssets.DeathText.Value.MeasureString(BossName);
            Vector2 titleSize = FontAssets.DeathText.Value.MeasureString(BossTitle);
            float nameOffset = (Main.screenWidth / 2) - nameSize.X / 2f;
            float titleOffset = (Main.screenWidth / 2) - titleSize.X / 2f;

            //Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag").Value;
            float backOffset = (Main.screenWidth / 2);
            Main.spriteBatch.Draw(backDrop, new Vector2(backOffset, 0), null, Color.White, 0f, backDrop.Size() / 2, 1f, SpriteEffects.None, 1f);
            //Main.spriteBatch.Reload(BlendState.AlphaBlend);

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, BossTitle, new Vector2(titleOffset, 50), titleColor, 0f, Vector2.Zero, 0.6f, 0, 0f);
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, BossName, new Vector2(nameOffset, 100), nameColor, 0f, Vector2.Zero, 1f, 0, 0f);
        }

        public override void PostDrawTiles()
        {
            SpriteBatch sb = Main.spriteBatch;

            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * proj.ai[1]);
            //sb.End();

            foreach (NPC npc in Main.npc)
            {
                if (npc.type == NPCID.EyeofCthulhu && npc.active)
                {
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * npc.ai[3]);
                    sb.End();

                    //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    //Player player = Main.player[Main.myPlayer];
                    //Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, 0, Vector2.Zero);
                    //sb.End();
                }
            }

            base.PostDrawTiles();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, TitleInterface, TitleCard, mouseTextIndex, TitleCard.visible, "Title Card");
            }

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

            mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Bow Trajectory",
                    delegate
                    {
                        trajDraw.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
            mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "OvermorrowMod: Bow Charge",
                    delegate
                    {
                        bowCargDraw.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null)
        {
            string name = customName == null ? state.ToString() : customName;
            layers.Insert(index, new LegacyGameInterfaceLayer("OvermorrowMod: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
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

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                Particle.UpdateParticles();
            }
        }
    }
}
