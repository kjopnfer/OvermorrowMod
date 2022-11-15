using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using ReLogic.Utilities;
using System.Collections.Generic;
using OvermorrowMod.Content.Tiles.Altar;
using Terraria.ID;
using OvermorrowMod.Common;
using System;

namespace OvermorrowMod.Content.UI.Altar
{
    public class AltarState : UIState
    {
        public int DrawCounter = 0;
        public int RotationCounter = 0;

        private AltarSlot SlotContainer = new AltarSlot();
        private AltarButton AltarButton = new AltarButton();

        public bool HasCritter = false;
        public override void OnInitialize()
        {


            base.OnInitialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Main.NewText(DrawCounter);

            if (!Main.LocalPlayer.GetModPlayer<AltarPlayer>().NearAltar)
            {
                if (DrawCounter > 0 && !Main.gamePaused) DrawCounter--;
                //RotationCounter = 0;
                //if (RotationCounter > 0) RotationCounter--;
                //return;
            }
            else
            {
                if (!Main.gamePaused)
                {
                    if (DrawCounter < 60) DrawCounter++;
                    else if (DrawCounter >= 60 && RotationCounter < 180) RotationCounter++;

                }
            }

            this.RemoveAllChildren();

            float yOffset = MathHelper.Lerp(-64, -80, Utils.Clamp(DrawCounter, 0, 60) / 60f);
            Vector2 slotOffset = new Vector2(8, yOffset);
            Append(SlotContainer);
            Append(AltarButton);

            SlotContainer.SetCenter(AltarWorld.AltarPosition + slotOffset - Main.screenPosition);
            AltarButton.SetCenter(AltarWorld.AltarPosition + slotOffset + new Vector2(0, 41) - Main.screenPosition);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (!Main.LocalPlayer.GetModPlayer<AltarPlayer>().NearAltar) return;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer").Value;
            //spriteBatch.Draw(texture, AltarWorld.AltarPosition + new Vector2(16, -48) - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 1f);

            base.Draw(spriteBatch);
        }
    }

    public class AltarSlot : CustomItemSlot
    {
        /*public override void DrawTexture(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer_Active").Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), new Rectangle(0, 0, 52, 52), Color.White, 0, texture.Size() / 2f, 1, 0, 0);
        }*/

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer").Value;
            if (!Item.IsAir) texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer_Active").Value;

            if (Parent is AltarState parent)
            {
                parent.HasCritter = !Item.IsAir;

                canDraw = true;
                if (parent.DrawCounter == 0) canDraw = false;

                float progress = parent.DrawCounter / 60f;

                itemOpacity = MathHelper.Lerp(0, 1, progress);
                float opacity = MathHelper.Lerp(0, 1, progress);

                spriteBatch.Draw(texture, GetDimensions().Center(), new Rectangle(0, 0, 52, 52), Color.White * opacity, 0, texture.Size() / 2f, 1, 0, 0);

                Texture2D fillBar = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarBar_Filled").Value;
                float fillProgress = MathHelper.Lerp(0, 0.5f, Utils.Clamp(AltarWorld.SacrificePoints, 0, 100) / 100f);

                Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.RadialFade.Value;
                effect.Parameters["progress"].SetValue(fillProgress);
                effect.CurrentTechnique.Passes["RadialFade"].Apply();

                spriteBatch.Draw(fillBar, GetDimensions().Center(), null, Color.White * opacity, 0f, fillBar.Size() / 2f, 1, 0, 0);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);

                Texture2D progressBar = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarBar").Value;
                spriteBatch.Draw(progressBar, GetDimensions().Center(), null, Color.White * opacity, 0, progressBar.Size() / 2f, 1, 0, 0);

                Texture2D altarBottom = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarBottom").Value;
                spriteBatch.Draw(altarBottom, GetDimensions().Center(), null, Color.White * opacity, 0, altarBottom.Size() / 2f, 1, 0, 0);

                base.Draw(spriteBatch);
            }
        }

        public void SetCenter(Vector2 position)
        {
            Left.Set(position.X - Width.Pixels / 2, 0);
            Top.Set(position.Y - Height.Pixels / 2, 0);
        }

        public override bool CheckValid(Item item)
        {
            if (Parent is AltarState parent && parent.DrawCounter == 60)
            {
                return item.type == ItemID.Bunny;
            }

            return false;
        }

        /*public override bool CheckValid(Item item)
        {
            return item.type == ItemID.Bunny;
        }*/
    }

    public class AltarButton : UIElement
    {
        private int glowCounter = 0;
        public AltarButton()
        {
            Width.Set(32, 0);
            Height.Set(20, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (Parent is AltarState parent)
            {
                float progress = parent.DrawCounter / 60f;
                float opacity = MathHelper.Lerp(0, 1, progress);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarButton").Value;
                if (isHovering)
                {
                    texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarButton_Hover").Value;
                }

                spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White * opacity, 0, texture.Size() / 2f, 1, 0, 0);

                if (parent.HasCritter)
                {
                    float glowOpacity = MathHelper.Lerp(0, 1, (float)(Math.Sin(glowCounter++ / 20f) / 2 + 0.5f));
                    Texture2D glowTexture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarButton_Glow").Value;
                    spriteBatch.Draw(glowTexture, GetDimensions().Center(), null, Color.White * glowOpacity * opacity, 0, glowTexture.Size() / 2f, 1, 0, 0);
                }
            }

            //Utils.DrawBorderString(spriteBatch, "Next", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            if (Parent is AltarState parent)
            {
                foreach (UIElement element in parent.Children)
                {
                    if (element is AltarSlot itemSlot)
                    {
                        if (!itemSlot.Item.IsAir)
                        {
                            AltarWorld.SacrificePoints += (3 + AltarWorld.SacrificeBonus) * itemSlot.Item.stack;
                            itemSlot.Item.TurnToAir();

                            if (AltarWorld.SacrificePoints >= AltarWorld.MAX_SACRIFICE)
                            {
                                AltarWorld.SacrificePoints = 0;

                                float variant = Main.rand.Next(1, 3);
                                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/Altar/finish" + variant), Main.LocalPlayer.Center);
                            }
                            else
                            {
                                float variant = Main.rand.Next(1, 7);
                                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/Altar/sacrifice" + variant), Main.LocalPlayer.Center);
                            }

                            Main.NewText(AltarWorld.SacrificePoints + "/" + AltarWorld.MAX_SACRIFICE, Color.Red);
                        }
                    }
                }
            }
        }

        public void SetCenter(Vector2 position)
        {
            Left.Set(position.X - Width.Pixels / 2, 0);
            Top.Set(position.Y - Height.Pixels / 2, 0);
        }
    }
}