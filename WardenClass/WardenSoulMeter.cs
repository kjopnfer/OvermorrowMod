using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass
{
    public class WardenSoulMeter : ModPlayer
    {
        /*public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (RuneID != 0)
            {
                int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsBack);
                if (body < 0)
                {
                    return;
                }

                layers.Insert(body - 1, Rune);
            }
        }*/

        public float chargeProgress;
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsFront);
            if (body < 0)
                return;

            layers.Insert(body - 1, new PlayerLayer(mod.Name, "Body",
                delegate (PlayerDrawInfo drawInfo)
                {
                    Player drawPlayer = drawInfo.drawPlayer;
                    Mod mod = ModLoader.GetMod("OvermorrowMod");
                    WardenDamagePlayer modPlayer = drawPlayer.GetModPlayer<WardenDamagePlayer>();

                    if (drawInfo.shadow != 0f)
                    {
                        return;
                    }

                    if (modPlayer.UIToggled)
                    {
                        /*Player drawPlayer = drawInfo.drawPlayer;
                        Mod mod = ModLoader.GetMod("SpiritMod");
                        ChargeMeterPlayer modPlayer = drawPlayer.GetModPlayer<ChargeMeterPlayer>();*/
                        Vector2 Position = drawPlayer.position;
                        DrawData drawData = new DrawData();
                        
                        Texture2D ChargeMeter = ModContent.GetTexture("OvermorrowMod/WardenClass/ChargeMeter");
                        Texture2D ChargeBar = ModContent.GetTexture("OvermorrowMod/WardenClass/ChargeBar");
                      
                        Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 50) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
                        drawData = new DrawData(ChargeMeter, position, new Microsoft.Xna.Framework.Rectangle?(), Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                        Main.playerDrawData.Add(drawData);

                        Rectangle rect = new Rectangle(0, 0, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), ChargeBar.Height);
                        drawData = new DrawData(ChargeBar, position + new Vector2(48, 0), rect, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                        Main.playerDrawData.Add(drawData);
                    }
                }));
        }
    }
}