using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass
{
    public class WardenSoulMeter : ModPlayer
    {

        public float chargeProgress;
        public int frameCounter;
        public int frame = 0;

        private void DrawPlayer(PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            WardenDamagePlayer modPlayer = drawPlayer.GetModPlayer<WardenDamagePlayer>();
            chargeProgress = modPlayer.soulPercentage;

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
                    Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 164 + 201) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

                    Texture2D ChargeMeter = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulContainerL");
                    Texture2D ChargeBar = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterFBar");

                    // Soul Meter is completely full
                    if (modPlayer.soulResourceCurrent == modPlayer.soulResourceMax2)
                    {
                        // Sets the looping frame properly
                        if (frame < 10)
                        {
                            frame = 10;
                        }

                        frameCounter++;

                        if (frameCounter % 3f == 2f) // Ticks per frame
                        {
                            frame += 1;
                        }

                        if (frame >= 15) // 6 is max # of frames
                        {
                            frame = 10; // Reset back to default
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

                            if (frame >= 9) // 9 is max # of frames
                            {
                                frame = 11; // Reset back to default
                                modPlayer.soulPercentage = 0;
                                modPlayer.soulMeterMax = false;
                            }

                            var drawRectangleMeter = new Rectangle(0, 42 * frame, /*GainAnimation*/ChargeMeter.Width, 42);

                            drawData = new DrawData(/*GainAnimation*/ChargeMeter, position /*+ new Vector2(0, 84)*/, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, /*GainAnimation*/ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);
                        }
                        else
                        {
                            var drawRectangleMeter = new Rectangle(0, 42 * 11, ChargeMeter.Width, 42);

                            drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);

                            Rectangle drawRectangleBar = new Rectangle(0, 42 * 11, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), 42);
                            drawData = new DrawData(ChargeBar, position + new Vector2(47, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                }
                else // Smol bar for other players
                {
                    // Note to self: The small bar and large bar have different frame counts, might cause issues in the future
                    // In the event the other players are suddenly shitting out Souls because of different frame counts, just add
                    // One more frame in the small bar frame, and then add that offset to the positioning

                    Vector2 position = new Vector2((int)(Position.X - (double)Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0) + 153 + 60) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);

                    Texture2D ChargeMeter = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulContainerS");
                    Texture2D ChargeBar = ModContent.GetTexture("OvermorrowMod/WardenClass/SoulMeterSFBar");

                    const int textureHeight = 24;
                    // Soul Meter is completely full
                    if (modPlayer.soulResourceCurrent == modPlayer.soulResourceMax2)
                    {
                        // Sets the looping frame properly
                        if (frame < 6)
                        {
                            frame = 6;
                        }

                        frameCounter++;

                        if (frameCounter % 3f == 2f) // Ticks per frame
                        {
                            frame += 1;
                        }

                        if (frame >= 15) // 15 is max # of frames
                        {
                            frame = 6; // Reset back to default
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

                            if (frame >= 5) // 5 is max # of frames
                            {
                                frame = 6; // Reset back to default
                                modPlayer.soulMeterMax = false;
                                modPlayer.soulPercentage = 0;
                            }

                            var drawRectangleMeter = new Rectangle(0, textureHeight * frame, ChargeMeter.Width, textureHeight);

                            drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);
                        }
                        else
                        {
                            var drawRectangleMeter = new Rectangle(0, textureHeight * 6, ChargeMeter.Width, textureHeight);

                            drawData = new DrawData(ChargeMeter, position, drawRectangleMeter, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);

                            Rectangle drawRectangleBar = new Rectangle(0, textureHeight * 5, (int)(ChargeBar.Width * MathHelper.Lerp(0, 1, chargeProgress / 100f)), textureHeight);
                            drawData = new DrawData(ChargeBar, position + new Vector2(19, 0), drawRectangleBar, Color.White, drawPlayer.bodyRotation, ChargeMeter.Size() / 2f, 1f, SpriteEffects.None, 0);
                            Main.playerDrawData.Add(drawData);
                        }
                    }
                }
            }
        }


        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsFront);
            if (body < 0)
                return;

            layers.Insert(body - 1, new PlayerLayer(mod.Name, "Body", DrawPlayer));
        }
    }
}