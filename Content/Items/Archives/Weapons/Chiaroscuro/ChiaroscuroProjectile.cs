using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class ChiaroscuroProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)(Projectile.timeLeft * (1f / Owner.GetTotalAttackSpeed(DamageClass.Melee)));
            maxTimeLeft = Projectile.timeLeft;
            //maxTimeLeft = 5;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        public int maxTimeLeft;
        public Vector2 offset;
        public Vector2 stabVec;

        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public Player Owner => Main.player[Projectile.owner];
        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);
            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            //Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.35f);
            Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.2f);

            int stabTimer = maxTimeLeft - Projectile.timeLeft;
            float totalTime = maxTimeLeft * 0.2f; //total amount of frames the stab lasts

            /*if (stabTimer < totalTime)
            {
                if (stabTimer == 1) //things are initialized here each stab, such as the random rotation, and the randomness of the length of the stab
                {
                    Projectile.velocity = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.35f);
                    stabVec = new Vector2(Main.rand.NextFloat(90, 150), 0);
                }

                if (stabTimer < totalTime * 0.5f)
                {
                    float lerper = stabTimer / (totalTime * 0.5f);

                    offset = Vector2.Lerp(new Vector2(75, 0), stabVec, EasingUtils.EaseOutCirc(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                }
                else
                {

                    float lerper = (stabTimer - totalTime * 0.5f) / (float)(totalTime * 0.5f);

                    offset = Vector2.Lerp(stabVec, new Vector2(75, 0), EasingUtils.EaseOutCirc(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                }
            }
            else
            {
                stretch = (Player.CompositeArmStretchAmount)Main.rand.Next(4);
            }*/
            float lerper = Projectile.timeLeft / (float)maxTimeLeft;
            //offset = Vector2.Lerp(Vector2.Zero, new Vector2(8, 0), EasingUtils.EaseOutCirc(lerper)).RotatedBy(Projectile.rotation - MathHelper.PiOver2);


            /*if (!Owner.channel && Projectile.timeLeft == maxTimeLeft)
            {
                Projectile.timeLeft = 5;
                //fading = true;
            }*/
            stretch = (Player.CompositeArmStretchAmount)Main.rand.Next(4);

            Projectile.Center = Owner.MountedCenter;

            Owner.SetCompositeArmFront(true, stretch, Projectile.rotation - MathHelper.Pi);
            Owner.ChangeDir(Projectile.direction);

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            Vector2 off = new Vector2(10, -20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                if (Main.rand.NextBool(4))
                {
                    randomScale = Main.rand.NextFloat(1f, 1.75f);
                    //randomScale = Main.rand.NextFloat(0.5f, 1f);

                    //float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                    Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(60)) * Main.rand.Next(10, 12);
                    //Color color = new Color(202, 188, 255);
                    Color color = new Color(108, 108, 224);

                    //Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale, 0f, 0f, 1f);
                    var lightSpark = new Circle(sparkTexture, 45, true, false);
                    lightSpark.endColor = new Color(108, 108, 224);
                    lightSpark.floatUp = false;
                    lightSpark.AnchorEntity = Owner;
                    lightSpark.AnchorOffset = new Vector2(-10, -90).RotatedBy(Projectile.rotation);
                    ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity, color, 0.5f, randomScale, Projectile.rotation, 0f, useAdditiveBlending: true);
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.friendly && !target.friendly;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox)) return true;

            var hitboxHeight = 140;
            var hitboxWidth = 38;
            float _ = float.NaN;
            Vector2 endPosition = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPosition, hitboxWidth * Projectile.scale, ref _);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            float progress = Projectile.timeLeft / (float)maxTimeLeft;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            float swordOffset = MathHelper.Lerp(10, 0, progress);
            Vector2 off = new Vector2(swordOffset, -20).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + off, null, Color.White, Projectile.rotation - MathHelper.PiOver2, Vector2.Zero, Projectile.scale, 0, 0);

            Color color = new Color(108, 108, 224);
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D effect = ModContent.Request<Texture2D>(AssetDirectory.Textures + "slash_01").Value;

            float stabOffset = MathHelper.Lerp(40, -50, progress);
            Main.NewText(stabOffset + " " + progress + " " + Projectile.timeLeft);
            off = new Vector2(-200 + stabOffset, 12).RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Main.spriteBatch.Draw(effect, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + off, null, color * 1f, Projectile.rotation + MathHelper.Pi, Vector2.Zero, new Vector2(0.05f, 0.8f), 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, Main.Rasterizer, default, Main.GameViewMatrix.TransformationMatrix); //also dont know if this spritebatch reset is needed

            return false;
        }
    }
}