using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.BearTrap
{
    public class BearTrapIcon : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanCutTiles() => false;
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 70;
        }

        public ref float AICounter => ref Projectile.ai[0];

        private Vector2 initialPosition;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = 70;

            initialPosition = Projectile.Center;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            AICounter++;

            if (AICounter < 60)       
                Projectile.Center = Vector2.Lerp(initialPosition, initialPosition - Vector2.UnitY * 32, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);           
            else        
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, (AICounter - 60) / 10f);   
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ModContent.ItemType<BearTrap>()].Value;

            Vector2 scale = new Vector2(MathHelper.Lerp(0.5f, 1f, Utils.Clamp(AICounter, 0, 10) / 10f), 1f);
            if (AICounter > 60) scale = new Vector2(MathHelper.Lerp(1f, 0f, (AICounter - 60) / 10f), MathHelper.Lerp(1f, 0f, (AICounter - 60) / 10f));
            //float alpha = MathHelper.Lerp(0, 0.75f, (float)(Math.Sin(AICounter / 15f)) / 2 + 0.5f);
            float alpha = 0.75f;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, texture.Size() / 2f, scale * 0.9f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
            Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, outline.Size() / 2f, scale * 0.1f, SpriteEffects.None, 1);

            outline = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            Main.spriteBatch.Draw(outline, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, outline.Size() / 2f, scale * 0.5f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}