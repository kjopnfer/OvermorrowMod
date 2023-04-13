using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.CapturedMirage
{
    public class CapturedMirage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Captured Mirage");
            Tooltip.SetDefault("On arrow critical strike, your next bow shot fires a Mirage Arrow\n" +
                "Mirage Arrows copy the effect of another arrow in your inventory");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().CapturedMirage = true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Texture2D texture = TextureAssets.Item[Item.type].Value;

            DrawData data = new DrawData(texture, position, frame, drawColor, 0f, origin, scale, 0, 0);
            int shaderID = GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye);
            GameShaders.Armor.Apply(shaderID, Item, data);
            //GameShaders.Armor.ApplySecondary(shaderID, Item);

            data.Draw(spriteBatch);
            //spriteBatch.Draw(texture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 1);

            spriteBatch.Reload(SpriteSortMode.Deferred);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            spriteBatch.Reload(SpriteSortMode.Immediate);

            DrawData data = new DrawData(texture, Item.Center - Main.screenPosition, null, lightColor, 0f, texture.Size() / 2f, scale, 0, 0);
            int shaderID = GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye);
            GameShaders.Armor.Apply(shaderID, Item, data);
            data.Draw(spriteBatch);

            spriteBatch.Reload(SpriteSortMode.Deferred);

            return false;
        }
    }
}