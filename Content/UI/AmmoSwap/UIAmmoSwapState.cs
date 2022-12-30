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
using Terraria.ID;
using OvermorrowMod.Common;
using System;
using System.Xml;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Input;

namespace OvermorrowMod.Content.UI.AmmoSwap
{
    public class UIAmmoSwapState : UIState
    {
        private Test testPanel = new Test();
        public int drawCounter = 0;
        public int scaleCounter = 0;
        public int closeCounter = 0;

        public float MAX_TIME = 5;
        public float SCALE_TIME = 5;
        public float CLOSE_TIME = 5;

        private Vector2 anchorPosition;
        private bool hasAnchorPosition = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.keyState.IsKeyDown(Keys.LeftShift) && hasAnchorPosition)
            {
                //Vector2 position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(26, 26);
                //Utils.DrawLine(spriteBatch, position - Main.screenPosition, Main.MouseWorld, Color.Red);
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            if (player.HeldItem.DamageType != DamageClass.Ranged) return;

            MAX_TIME = 5;
            SCALE_TIME = 5;
            //Main.NewText("maxtime: " + MAX_TIME);

            if (Main.keyState.IsKeyDown(Keys.Q))
            {
                if (drawCounter < MAX_TIME) drawCounter++;
                if (drawCounter >= MAX_TIME && scaleCounter < SCALE_TIME) scaleCounter++;

                if (!hasAnchorPosition)
                {
                    anchorPosition = new Vector2(Main.MouseWorld.X - Main.screenPosition.X - 60, Main.MouseWorld.Y - Main.screenPosition.Y - 60);
                    hasAnchorPosition = true;
                }
                else
                {
                    this.RemoveAllChildren();
                    ModUtils.AddElement(testPanel, (int)anchorPosition.X, (int)anchorPosition.Y, 120, 120, this);
                    testPanel.RemoveAllChildren();


                    //Utils.DrawLine(spriteBatch, anchorPosition, Main.MouseWorld, Color.Red);

                    PlaceAmmoSlots();

                }

                closeCounter = (int)CLOSE_TIME;
            }
            else
            {
                if (closeCounter > 0) closeCounter--;

                if (closeCounter == 0)
                {
                    this.RemoveAllChildren();

                    hasAnchorPosition = false;
                    drawCounter = 0;
                    scaleCounter = 0;

                    rotateCounter = 0;
                }
            }

            base.Update(gameTime);
        }

        private float rotateCounter = 0;
        /// <summary>
        /// this method is made by stupid people
        /// </summary>
        private void PlaceAmmoSlots()
        {
            List<Item> ammoList = new List<Item>();

            for (int i = 0; i <= 3; i++)
            {
                if (Main.LocalPlayer.inventory[54 + i].ammo == AmmoID.Arrow)
                {
                    ammoList.Add(Main.LocalPlayer.inventory[54 + i]);
                }
            }

            //Vector2 position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(26, 26);
            //int offset = (int)MathHelper.Lerp(0, 40, Utils.Clamp(drawCounter, 0, MAX_TIME) / MAX_TIME);

            int offset = (int)MathHelper.Lerp(0, 40, Utils.Clamp(drawCounter, 0, MAX_TIME) / MAX_TIME);
            Vector2 rotationOffset = new Vector2(0, offset).RotatedBy(MathHelper.ToRadians(rotateCounter += 0.5f));
            Vector2 position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(28, 26) - rotationOffset;

            switch (ammoList.Count)
            {
                case 4:
                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y, 40, 40, testPanel);

                    rotationOffset = new Vector2(offset, 0).RotatedBy(MathHelper.ToRadians(rotateCounter++));
                    position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(28, 26) - rotationOffset;

                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y, 40, 40, testPanel);

                    rotationOffset = new Vector2(-offset, 0).RotatedBy(MathHelper.ToRadians(rotateCounter++));
                    position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(28, 26) - rotationOffset;

                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y, 40, 40, testPanel);

                    rotationOffset = new Vector2(0, -offset).RotatedBy(MathHelper.ToRadians(rotateCounter++));
                    position = new Vector2(testPanel.Width.Pixels, testPanel.Height.Pixels) / 2 - new Vector2(28, 26) - rotationOffset;

                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y, 40, 40, testPanel);

                    //ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y - offset, 40, 40, testPanel);
                    //ModUtils.AddElement(new AmmoSlot(ammoList[1].shoot), (int)position.X - offset, (int)position.Y, 40, 40, testPanel);
                    //ModUtils.AddElement(new AmmoSlot(ammoList[2].shoot), (int)position.X + offset, (int)position.Y, 40, 40, testPanel);
                    //ModUtils.AddElement(new AmmoSlot(ammoList[3].shoot), (int)position.X, (int)position.Y + offset, 40, 40, testPanel);
                    break;
                case 3:
                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y - offset, 40, 40, testPanel);
                    ModUtils.AddElement(new AmmoSlot(ammoList[1].shoot), (int)position.X - offset, (int)position.Y + offset - 8, 40, 40, testPanel);
                    ModUtils.AddElement(new AmmoSlot(ammoList[2].shoot), (int)position.X + offset, (int)position.Y + offset - 8, 40, 40, testPanel);
                    break;
                case 2:
                    ModUtils.AddElement(new AmmoSlot(ammoList[0].shoot), (int)position.X, (int)position.Y - offset, 40, 40, testPanel);
                    ModUtils.AddElement(new AmmoSlot(ammoList[1].shoot), (int)position.X, (int)position.Y + offset, 40, 40, testPanel);
                    break;
            }
        }
    }

    public class Test : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "MonkeyStones").Value;
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1, 0, 0);
        }
    }

    public class AmmoSlot : UIElement
    {
        private int itemID;

        public AmmoSlot(int itemID)
        {
            this.itemID = itemID;
        }

        bool isHovering => ContainsPoint(Main.MouseScreen);
        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.LocalPlayer.mouseInterface = true;

            if (Parent.Parent is UIAmmoSwapState swapState)
            {
                float progress = Utils.Clamp(swapState.scaleCounter, 0.5f, swapState.SCALE_TIME) / swapState.SCALE_TIME;
                if (swapState.closeCounter < swapState.CLOSE_TIME) progress = Utils.Clamp(swapState.closeCounter, 0, swapState.CLOSE_TIME) / swapState.CLOSE_TIME;

                Vector2 containerScale = Vector2.Lerp(new Vector2(1f, 0), new Vector2(1f, 1f), progress);
                Vector2 itemScale = Vector2.Lerp(new Vector2(0.75f, 0), new Vector2(0.75f, 0.75f), progress);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "AmmoSlot").Value;
                if (isHovering) texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "AmmoSlot_Hover").Value;
                spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, containerScale, 0, 0);

                Texture2D arrow = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + itemID).Value;
                spriteBatch.Draw(arrow, GetDimensions().Center(), null, Color.White, MathHelper.PiOver4, arrow.Size() / 2f, itemScale, 0, 0);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isHovering && !Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                //Main.NewText("Selected ammo type: " + itemID);
            }

            base.Update(gameTime);
        }
    }
}