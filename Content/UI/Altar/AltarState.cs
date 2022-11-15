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

namespace OvermorrowMod.Content.UI.Altar
{
    public class AltarState : UIState
    {
        public int DrawCounter = 0;
        public int RotationCounter = 0;

        private AltarSlot SlotContainer = new AltarSlot();

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

                RotationCounter = 0;
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

            SlotContainer.SetCenter(AltarWorld.AltarPosition + slotOffset - Main.screenPosition);

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
            base.Draw(spriteBatch);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer").Value;
            if (!Item.IsAir) texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarContainer_Active").Value;

            if (Parent is AltarState parent)
            {
                canDraw = true;
                if (parent.DrawCounter == 0) canDraw = false;

                float progress = parent.DrawCounter / 60f;

                itemOpacity = MathHelper.Lerp(0, 1, progress);
                float opacity = MathHelper.Lerp(0, 1, progress);

                spriteBatch.Draw(texture, GetDimensions().Center(), new Rectangle(0, 0, 52, 52), Color.White * opacity, 0, texture.Size() / 2f, 1, 0, 0);

                Texture2D fillBar = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarBar_Filled").Value;
                //spriteBatch.Draw(fillBar, GetDimensions().Center(), null, Color.White * opacity, MathHelper.ToRadians(0), new Vector2(fillBar.Width / 2f, fillBar.Height), 1, 0, 0);

                float fillProgress = MathHelper.Lerp(0, 0.5f, Utils.Clamp(parent.RotationCounter, 0, 180) / 180f);

                Main.spriteBatch.Reload(SpriteSortMode.Immediate);

                Effect effect = OvermorrowModFile.Instance.RadialFade.Value;
                effect.Parameters["progress"].SetValue(fillProgress);
                effect.CurrentTechnique.Passes["RadialFade"].Apply();

                spriteBatch.Draw(fillBar, GetDimensions().Center(), new Rectangle(0, 0, 104, 52), Color.White * opacity, 0f, fillBar.Size() / 2f, 1, 0, 0);

                Main.spriteBatch.Reload(SpriteSortMode.Deferred);

                Texture2D progressBar = ModContent.Request<Texture2D>(AssetDirectory.UI + "Altar/AltarBar").Value;
                spriteBatch.Draw(progressBar, GetDimensions().Center(), null, Color.White * opacity, 0, progressBar.Size() / 2f, 1, 0, 0);
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
}