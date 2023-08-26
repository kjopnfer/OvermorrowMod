using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.AmethystSlicer
{
    public class AmethystSlicer_Shards : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 480;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public ref float AICounter => ref Projectile.ai[2];
        public override void AI()
        {
            // If its the small crystal variant, make the hitbox smaller so it doesn't float above the ground
            if (Projectile.ai[0] == 0) Projectile.width = Projectile.height = 8;

            Projectile.velocity.Y += 0.25f;
            Projectile.velocity.X *= 0.97f;

            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X * 3f);

            AICounter++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Melee + "AmethystSlicer/AmethystShard_" + Projectile.ai[0]).Value;
            float alpha = 1f;

            if (Projectile.timeLeft < 60f) alpha = MathHelper.Lerp(1f, 0f, 1 - Projectile.timeLeft / 60f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 1);

            if (AICounter == 120 && Projectile.ai[1] == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 positionOffset = Vector2.UnitX * Main.rand.NextFloat(-1.5f, 1.5f) * 16;
                    Projectile.NewProjectile(null, Projectile.Center + positionOffset, Vector2.Zero, ModContent.ProjectileType<AmethystCrystal>(), Projectile.damage, 0f, Projectile.owner);
                }
            }

            return base.PreDraw(ref lightColor);
        }
    }

    public class AmethystCrystal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.timeLeft = 480;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        float rotation = 0f;
        float maxLength = 0f;
        float delayTime = 60f;
        public override void OnSpawn(IEntitySource source)
        {
            rotation = MathHelper.ToRadians(Main.rand.Next(-9, 9) * 5);
            maxLength = Main.rand.NextFloat(2f, 3.5f);
            delayTime = Main.rand.Next(8, 15) * 10;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;
            Tile tile = Framing.GetTileSafely((int)(Projectile.Center.X * 16f), (int)(Projectile.Center.Y * 16f));
            if (AICounter < delayTime && !tile.HasTile) Projectile.velocity.X += 0.25f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = 1f;
            if (Projectile.timeLeft < 60f) alpha = MathHelper.Lerp(1f, 0f, 1 - Projectile.timeLeft / 60f);

            if (AICounter > delayTime)
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 scale = new Vector2(0.6f, MathHelper.Lerp(0f, maxLength, Utils.Clamp(AICounter - delayTime, 0f, 20f) / 20f));
                Vector2 offset = Vector2.UnitY * MathHelper.Lerp(0, -4f, Utils.Clamp(AICounter - delayTime, 0f, 20f) / 20f);
                Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, lightColor * alpha, rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            return false;
        }
    }
}