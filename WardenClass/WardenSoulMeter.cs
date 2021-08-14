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
        public int frameCounter;
        public int frame = 0;

        public override void clientClone(ModPlayer clientClone)
        {
            WardenSoulMeter clone = clientClone as WardenSoulMeter;
            clone.chargeProgress = chargeProgress;
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            packet.Write(chargeProgress);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            packet.Write(chargeProgress);
            packet.Send();
        }

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

                    Vector2 Position = drawPlayer.position;

                    if (modPlayer.UIToggled)
                    {
                        DrawData drawData = new DrawData();

                        // Draw big bar for local player
                        if (player.whoAmI == Main.myPlayer)
                        {
                            Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 164) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

                            Texture2D ChargeMeter = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterF");
                            Texture2D ChargeBar = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterFBar");

                            // Soul Meter is completely full
                            if (modPlayer.soulResourceCurrent == modPlayer.soulResourceMax2)
                            {
                                frameCounter++;

                                if (frameCounter % 3f == 2f) // Ticks per frame
                                {
                                    frame += 1;
                                }

                                if (frame >= 6) // 6 is max # of frames
                                {
                                    frame = 0; // Reset back to default
                                }

                                var drawRectangleMeter = new Rectangle(0, 42 * frame, ChargeMeter.Width, 42);

                                drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                Main.playerDrawData.Add(drawData);

                                Rectangle drawRectangleBar = new Rectangle(0, 42 * frame, ChargeBar.Width, 42);
                                drawData = new DrawData(ChargeBar, position + new Vector2(47, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                Main.playerDrawData.Add(drawData);
                            }
                            else
                            {

                                // Fill animation
                                if (modPlayer.soulMeterMax)
                                {
                                    frameCounter++;

                                    if (frameCounter % 3f == 2f) // Ticks per frame
                                    {
                                        frame += 1;
                                    }

                                    if (frame >= 6) // 6 is max # of frames
                                    {
                                        frame = 0; // Reset back to default
                                        modPlayer.soulMeterMax = false;
                                        modPlayer.soulPercentage = 0;
                                        modPlayer.AddSoul(1);
                                    }

                                    var drawRectangleMeter = new Rectangle(0, 42 * frame, ChargeMeter.Width, 42);

                                    drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);

                                    Rectangle drawRectangleBar = new Rectangle(0, 24 * frame, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), 42);
                                    drawData = new DrawData(ChargeBar, position + new Vector2(47, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);
                                }
                                else
                                {
                                    var drawRectangleMeter = new Rectangle(0, 42 * 1, ChargeMeter.Width, 42);

                                    drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);

                                    Rectangle drawRectangleBar = new Rectangle(0, 42 * 0, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), 42);
                                    drawData = new DrawData(ChargeBar, position + new Vector2(47, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);
                                }
                            }
                        }
                        else // Smol bar for other players
                        {
                            Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 153) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

                            Texture2D ChargeMeter = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterSF");
                            Texture2D ChargeBar = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterSFBar");

                            const int textureHeight = 24;
                            // Soul Meter is completely full
                            if (modPlayer.soulResourceCurrent == modPlayer.soulResourceMax2)
                            {
                                frameCounter++;

                                if (frameCounter % 3f == 2f) // Ticks per frame
                                {
                                    frame += 1;
                                }

                                if (frame >= 10) // 10 is max # of frames
                                {
                                    frame = 0; // Reset back to default
                                }

                                var drawRectangleMeter = new Rectangle(0, textureHeight * frame, ChargeMeter.Width, textureHeight);

                                drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                Main.playerDrawData.Add(drawData);

                                Rectangle drawRectangleBar = new Rectangle(0, textureHeight * frame, ChargeBar.Width, textureHeight);
                                drawData = new DrawData(ChargeBar, position + new Vector2(19, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                Main.playerDrawData.Add(drawData);
                            }
                            else
                            {

                                // Fill animation
                                if (modPlayer.soulMeterMax)
                                {
                                    frameCounter++;

                                    if (frameCounter % 3f == 2f) // Ticks per frame
                                    {
                                        frame += 1;
                                    }

                                    if (frame >= 10) // 10 is max # of frames
                                    {
                                        frame = 0; // Reset back to default
                                        modPlayer.soulMeterMax = false;
                                        modPlayer.soulPercentage = 0;
                                        modPlayer.AddSoul(1);
                                    }

                                    var drawRectangleMeter = new Rectangle(0, textureHeight * frame, ChargeMeter.Width, textureHeight);

                                    drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);

                                    Rectangle drawRectangleBar = new Rectangle(0, textureHeight * frame, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), textureHeight);
                                    drawData = new DrawData(ChargeBar, position + new Vector2(19, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);
                                }
                                else
                                {
                                    var drawRectangleMeter = new Rectangle(0, textureHeight * 0, ChargeMeter.Width, textureHeight);

                                    drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);

                                    Rectangle drawRectangleBar = new Rectangle(0, textureHeight * 0, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), textureHeight);
                                    drawData = new DrawData(ChargeBar, position + new Vector2(19, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                                    Main.playerDrawData.Add(drawData);
                                }
                            }
                        }
                    }
                }));
        }
    }
}