using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public class PlayerGlowmasks : ModPlayer
    {
        private static readonly Dictionary<int, Texture2D> ItemGlowMask = new Dictionary<int, Texture2D>();

        internal static void Unload()
        {
            ItemGlowMask.Clear();
        }

        /*public static readonly PlayerLayer HemoHelmetGlowmask = new PlayerLayer("Overmorrow", "HemoHelmetGlowmask", PlayerLayer.Head, delegate (PlayerDrawInfo drawInfo)
        {

            // This glowmask has almost the same code as ExampleBreastplateBodyGlowmask but draws the arm texture
            if (drawInfo.shadow != 0f || drawInfo.drawPlayer.dead)
            {
                return;
            }

            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("OvermorrowMod");

            if (drawPlayer.head != mod.GetEquipSlot("HemoHelmet", EquipType.Head))
            {
                return;
            }

            Texture2D texture = mod.GetTexture("WardenClass/Armor/HemoHelmet_Head_Glowmask");

            float drawX = (int)drawInfo.position.X + drawPlayer.width / 2;
            float drawY = (int)drawInfo.position.Y + drawPlayer.height - drawPlayer.headFrame.Height / 2 + 4f;

            Vector2 origin = drawInfo.headOrigin;
            Vector2 position = new Vector2(drawX, drawY) + drawPlayer.headPosition - Main.screenPosition;
            float alpha = (255 - drawPlayer.immuneAlpha) / 255f;
            Color color = Color.White;
            Rectangle frame = drawPlayer.headFrame;
            float rotation = drawPlayer.headRotation;
            SpriteEffects spriteEffects = drawInfo.spriteEffects;

            DrawData drawData = new DrawData(texture, position, frame, color * alpha, rotation, origin, 1f, spriteEffects, 0);

            drawData.shader = drawInfo.headArmorShader;

            Main.playerDrawData.Add(drawData);
        });*/

        /*public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int headLayer = layers.FindIndex(l => l == PlayerLayer.Head);
            // We have to find the layer that we want our glowmask to insert to, which is PlayerLayer.Body in this example. For a full list of vanilla layers, check the docs
            //int bodyLayer = layers.FindIndex(l => l == PlayerLayer.Body);

            /*if (bodyLayer > -1)
            {
                // We add the glowmask ontop of the existing armor body texture, hence + 1
                layers.Insert(bodyLayer + 1, XonixArmorGlowmask);

                // Because arms are a separate texture and layer, we have to write another glowmask that draws over that texture aswell
                // Remove this if your intent is to just draw something over the body armor instead of making a full glowmask
                int armsLayer = layers.FindIndex(l => l == PlayerLayer.Arms);

                if (armsLayer > -1)
                {
                    layers.Insert(armsLayer + 1, XonixArmsGlowmask);
                }
            }

            if (headLayer > -1)
            {
                layers.Insert(headLayer + 1, HemoHelmetGlowmask);
            }
        }*/
    }
}