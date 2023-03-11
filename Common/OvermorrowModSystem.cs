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

        internal UserInterface TitleInterface;
        internal UserInterface ScreenInterface;

        public TitleCard TitleCard;
        public ScreenColor ScreenColor;

        public override void PostUpdateEverything()
        {
            PrimitiveDrawing.UpdateTrails();
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TitleInterface = new UserInterface();
                TitleCard = new TitleCard();
                TitleInterface.SetState(TitleCard);

                ScreenInterface = new UserInterface();
                ScreenColor = new ScreenColor();
                ScreenInterface.SetState(ScreenColor);
            }
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
    }
}
