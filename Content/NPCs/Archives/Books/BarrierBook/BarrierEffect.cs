using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class BarrierEffect : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            //Projectile.hostile = true;
            Projectile.Opacity = 0.75f;
            Projectile.timeLeft = 120;
        }

        public ref float ParentID => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            float scale = 0.1f;

            if (AICounter++ >= 60)
            {

                
            }
            else
            {
                if (Main.rand.NextBool(3))
                {
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
                    var lightOrb = new Circle(0f, scale * 0.5f);
                    ParticleManager.CreateParticleDirect(lightOrb, npc.Bottom + offset, -Vector2.UnitY, Color.White, 1f, scale, 0f);
                }

                if (Main.rand.NextBool(3))
                {
                    float radius = npc.width / 2f; // Adjust this for the size of the circle
                    float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                    Vector2 spawnPosition = npc.Center + offset;
                    Vector2 velocity = (npc.Center - spawnPosition).SafeNormalize(Vector2.Zero); // Adjust speed as needed (2f is the speed here)

                    //Particle.CreateParticleDirect(Particle.ParticleType<LightOrb>(), spawnPosition, -velocity, Color.White, 1f, scale, 0f, 0, scale * 0.5f);
                }
            }

            Projectile.Center = npc.Center;
            Projectile.rotation += 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            /*Texture2D magic = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_02").Value;
            float magicSize = MathHelper.Lerp(0.5f, 0f, Math.Clamp(AICounter / 60f, 0, 1f));
            Main.spriteBatch.Draw(magic, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, magic.Size() / 2f, magicSize, SpriteEffects.None, 0);*/

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + Name).Value;
            Color textureColor = Color.White;
            float size = MathHelper.Lerp(0f, 2.5f, 1 - Math.Clamp(Projectile.timeLeft / 60f, 0, 1f));
            float alpha = 0f;

            if (Projectile.timeLeft <= 60) alpha = Math.Clamp(MathHelper.Lerp(0, 1f, Projectile.timeLeft / 60f), 0, 1f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, textureColor * alpha, 0f, texture.Size() / 2f, size, SpriteEffects.None, 0);

            return false;
        }
    }
}