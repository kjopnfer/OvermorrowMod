using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items
{
    /// <summary>
    /// Placeholder that fills an inventory slot locked for the current run.
    /// </summary>
    public class LockedSlot : ModItem
    {
        public override string Texture => AssetDirectory.Misc + "DoorLock";

        private static readonly Vector2[] Diagonals = { Vector2.Normalize(new Vector2(1f, 1f)), Vector2.Normalize(new Vector2(1f, -1f)) };

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Gray;
        }

        public override bool CanPickup(Player player) => false;

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Item.active = false;
            Item.TurnToAir();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D chain = ModContent.Request<Texture2D>(AssetDirectory.Misc + "LockChain").Value;
            Texture2D padlock = TextureAssets.Item[Type].Value;

            float slotSize = TextureAssets.InventoryBack.Width() * Main.inventoryScale;
            float chainScale = Main.inventoryScale * 0.5f;
            float linkLength = chain.Height * chainScale;
            int links = (int)(slotSize * 0.8f * MathF.Sqrt(2f) / linkLength);
            float halfSpan = links * linkLength / 2f;

            foreach (Vector2 direction in Diagonals)
            {
                float rotation = direction.ToRotation() + MathHelper.PiOver2;
                for (int i = 0; i < links; i++)
                {
                    Vector2 linkPosition = position + direction * (linkLength * (i + 0.5f) - halfSpan);
                    spriteBatch.Draw(chain, linkPosition, null, drawColor, rotation, chain.Size() / 2f, chainScale, SpriteEffects.None, 0f);
                }
            }

            float padlockScale = slotSize * 0.45f / padlock.Height;
            spriteBatch.Draw(padlock, position, null, drawColor, 0f, padlock.Size() / 2f, padlockScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
