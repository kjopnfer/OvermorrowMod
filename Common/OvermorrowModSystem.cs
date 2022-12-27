using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using OvermorrowMod.Content.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.TilePiles;

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
        internal UserInterface ScreenInterface;

        internal AltarUI Altar;
        public TitleCard TitleCard;
        public ScreenColor ScreenColor;

        internal TrajectoryDraw trajectoryDraw;
        private UserInterface trajDraw;

        internal bowChargeDraw BowChargeDraw;
        private UserInterface bowCargDraw;

        public static bool shid;
        public static int[] bow2Send;
        public string faef = "foof";

        public override void PostUpdateEverything()
        {
            PrimitiveDrawing.UpdateTrails();
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

                ScreenInterface = new UserInterface();
                ScreenColor = new ScreenColor();
                ScreenInterface.SetState(ScreenColor);
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

            int cursorIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Cursor"));
            if (cursorIndex != -1)
            {
                AddInterfaceLayer(layers, ScreenInterface, ScreenColor, cursorIndex, ScreenColor.visible, "Screen Color");
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
