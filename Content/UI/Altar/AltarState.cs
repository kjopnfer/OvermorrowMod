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

namespace OvermorrowMod.Content.UI.Altar
{
    public class AltarState : UIState
    {
        private int DrawCounter = 0;

        private AltarSlot SlotContainer = new AltarSlot();
        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.GetModPlayer<AltarPlayer>().NearAltar) return;

            this.RemoveAllChildren();

            Vector2 slotOffset = new Vector2(SlotContainer.Width.Pixels / 2f, SlotContainer.Height.Pixels) * Main.UIScale;
            Vector2 position = AltarWorld.AltarPosition + new Vector2(16, -48) - slotOffset - Main.screenPosition;
            ModUtils.AddElement(SlotContainer, (int)position.X, (int)position.Y, 52, 52, this);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.LocalPlayer.GetModPlayer<AltarPlayer>().NearAltar) return;

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
            spriteBatch.Draw(texture, GetDimensions().Center(), new Rectangle(0, 0, 52, 52), Color.White, 0, texture.Size() / 2f, 1, 0, 0);
        }

        /*public override bool CheckValid(Item item)
        {
            return item.type == ItemID.Bunny;
        }*/
    }
}