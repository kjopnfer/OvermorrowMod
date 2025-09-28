using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class DisplayItem : ModProjectile
    {
        public int ItemID => (int)Projectile.ai[0];

        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (ItemID <= 0)
            {
                Projectile.Kill();
            }
        }

        public override void AI()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && player.Hitbox.Intersects(Projectile.Hitbox))
                {
                    Item.NewItem(null, player.Center, ItemID);
                    //player.QuickSpawnItem(Projectile.GetSource_FromThis(), ItemID);
                    Projectile.Kill();
                    return;
                }
            }

            if (!Main.gamePaused)
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] % 20 == 0)
                {
                    Texture2D lightTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "ray").Value;
                    Color color = Color.Green;

                    for (int i = 0; i < Main.rand.Next(2, 5); i++)
                    {
                        var lightRay = new Light(lightTexture, ModUtils.SecondsToTicks(4), Projectile, Vector2.Zero);
                        float randomRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                        float randomSize = Main.rand.NextFloat(0.05f, 0.085f);
                        ParticleManager.CreateParticleDirect(lightRay, Projectile.Center, Vector2.Zero, color, 1f, randomSize, randomRotation, ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
                    }
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (ItemID <= 0)
                return false;

            Texture2D texture = TextureAssets.Item[ItemID].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return true;
        }
    }
}