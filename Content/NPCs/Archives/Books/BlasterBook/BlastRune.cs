using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class BlastRune : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public ref float ParentID => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];
        public ref float MiscCounter2 => ref Projectile.ai[2];
        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            Projectile.Center = npc.Center + new Vector2(32, 0).RotatedBy(Projectile.velocity.ToRotation());

            AICounter++;
            if (AICounter > 30 && AICounter <= 60 && AICounter % 10 == 0)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/SpellShot")
                {
                    MaxInstances = 5,
                    PitchVariance = 0.5f,
                    Volume = 0.75f,
                    Pitch = 0.75f,
                }, Projectile.Center);

                float angleSpread = MathHelper.ToRadians(25); // Spread angle for randomness
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(angleSpread) * 8; // Randomized rotation towards the player

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<FireBolt>(), Projectile.damage, 1f, Main.myPlayer);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.rotation);

            float progress = Utils.Clamp(AICounter, 0, 25f) / 25f;
            if (AICounter > 70)
            {
                progress = 1 - (Utils.Clamp(AICounter - 70, 0, 30) / 30f);
            }

            //DrawRing(AssetDirectory.Textures + "Crosshair", Main.spriteBatch, Projectile.Center + offset * (Projectile.height / 2 - 20), 1, 1, Main.GameUpdateCount / 40f, progress2, new Color(244, 188, 91));
            //DrawRing(AssetDirectory.Textures + "MagicCircle", spriteBatch, npc.Center, 2f, 2f, Main.GameUpdateCount / 40f, progress3, new Color(244, 188, 91));
            Color color = Color.Cyan;
            DrawRing(AssetDirectory.Textures + "magic_01", Main.spriteBatch, Projectile.Center, 3f, 3f, Main.GameUpdateCount / 20f, progress, color);
            DrawRing(AssetDirectory.Textures + "magic_circle_02", Main.spriteBatch, Projectile.Center, 1f, 1f, Main.GameUpdateCount / 20f, progress, color);

            return base.PreDraw(ref lightColor);
        }

        private void DrawRing(string texture, SpriteBatch spriteBatch, Vector2 position, float width, float height, float rotation, float prog, Color color)
        {
            var texRing = ModContent.Request<Texture2D>(texture).Value;
            Effect effect = OvermorrowModFile.Instance.Ring.Value;

            effect.Parameters["uProgress"].SetValue(rotation);
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uImageSize1"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uOpacity"].SetValue(prog);
            effect.CurrentTechnique.Passes["BowRingPass"].Apply();

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

            var target = new Rectangle((int)(position.X - Main.screenPosition.X), (int)(position.Y - Main.screenPosition.Y), (int)(16 * (width + prog)), (int)(40 * (height + prog)));
            spriteBatch.Draw(texRing, target, null, color * prog, Projectile.velocity.ToRotation(), texRing.Size() / 2, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }
    }
}