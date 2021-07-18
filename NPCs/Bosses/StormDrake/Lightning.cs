using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    public class LightningSegment
    {
        public Vector2 Position;
        public float Size;
        public float DefSize;
        public float Alpha;
        public LightningSegment(Vector2 pos, float size, float defsize = 0f, float alpha = 1f)
        {
            Position = pos;
            Size = size;
            if (defsize == 0) DefSize = size;
            Alpha = alpha;
        }
    }
    public abstract class Lightning : ModProjectile
    {
        public delegate float Size(float progress);
        public static (List<LightningSegment>, List<float>) CreateLightning(Vector2 from, Vector2 to, Size size, float lengthDiv = 8, float Sway = 80f)
        {
            if (lengthDiv < 1) lengthDiv = 1;
            List<LightningSegment> segments = new List<LightningSegment>();
            Vector2 direction = from - to;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);

            List<float> lengths = new List<float>();
            lengths.Add(0f);
            for (int i = 0; i < direction.Length() / lengthDiv; i++)
            {
                lengths.Add(Main.rand.NextFloat());
            }
            lengths.Add(1f);
            lengths.Sort();

            float jaggedness = 1 / Sway;
            float prevPlacement = 0f;
            segments.Add(new LightningSegment(from, size(0f), 0f, 1));
            for (int i = 1; i < lengths.Count; i++)
            {
                float len = lengths[i];

                float scale = (direction.Length() * jaggedness) * (len - lengths[i - 1]);
                float envelope = MathHelper.Clamp((float)Math.Sin(len * Math.PI) * 10, 0, 1);

                float displace = Main.rand.NextFloat(-Sway, Sway);
                displace -= (displace - prevPlacement) * (1 - scale);
                displace *= envelope;
                Vector2 position = from + len * direction + displace * dir;
                segments.Add(new LightningSegment(position, size(len)));
                prevPlacement = displace;
            }
            return (segments, lengths);
        }
        public static List<LightningSegment> CreateLightning(Vector2 from, Vector2 to, float thickness, float Sway = 80f, float lengthDivider = 8f, bool sine = false)
        {
            var positions = new List<LightningSegment>();
            Vector2 direction = to - from;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);
            float length = direction.Length();

            List<float> Lengths = new List<float>();
            Lengths.Add(0);
            for (float i = 0; i < length / lengthDivider; i++)
                Lengths.Add(Main.rand.NextFloat());
            Lengths.Add(1f);
            Lengths.Sort();

            float Jaggedness = 1 / Sway;

            float prevDisplacement = 0;
            for (int i = 1; i < Lengths.Count; i++)
            {
                float current = thickness;
                float prog = (float)Math.Sin(((float)i / (float)Lengths.Count) * MathHelper.Pi);
                if (sine) current *= prog;
                float len = Lengths[i];

                float scale = (length * Jaggedness) * (len - Lengths[i - 1]);
                float envelope = len > 0.95f ? 20 * (1 - len) : 1;
                float val = Sway;
                float displace = Main.rand.NextFloat(-val, val);
                displace -= (displace - prevDisplacement) * (1 - scale);
                displace *= envelope;
                Vector2 offset = len * direction;
                Vector2 point = from + offset + displace * dir;
                positions.Add(new LightningSegment(point, current, 0f, prog));
                prevDisplacement = displace;
            }

            return positions;
        }
        
        public static List<LightningSegment> CreateLightning(Vector2 from, Vector2 to, float thickness, bool sine)
        {
            var positions = new List<LightningSegment>();
            Vector2 direction = to - from;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);
            float length = direction.Length();

            List<float> Lengths = new List<float>();
            Lengths.Add(0);
            for (float i = 0; i < length / 8; i++)
                Lengths.Add(Main.rand.NextFloat());
            Lengths.Add(1f);
            Lengths.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            float prevDisplacement = 0;
            for (int i = 1; i < Lengths.Count; i++)
            {
                float current = thickness;
                float prog = (float)Math.Sin(((float)i / (float)Lengths.Count) * MathHelper.Pi);
                if (sine) current *= prog;
                float len = Lengths[i];

                float scale = (length * Jaggedness) * (len - Lengths[i - 1]);
                float envelope = len > 0.95f ? 20 * (1 - len) : 1;
                float val = Sway;
                if (Main.rand.NextBool(5)) val *= 3;
                float displace = Main.rand.NextFloat(-val, val);
                displace -= (displace - prevDisplacement) * (1 - scale);
                displace *= envelope;
                Vector2 offset = len * direction;
                Vector2 point = from + offset + displace * dir;
                positions.Add(new LightningSegment(point, current, 0f, prog));
                prevDisplacement = displace;
            }

            return positions;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;
        public List<LightningSegment> Positions = new List<LightningSegment>();
        public float Length;
        public bool Sine;
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }
        private bool Startup;
        public override bool PreAI()
        {
            if (!Startup)
            {
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                Startup = true;
                Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 122, 0.5f, -0.5f);
            }
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Positions[i].Position, Positions[i + 1].Position, Positions[i].Size, ref a))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //if (Positions == default || Positions == null) return false;
            Texture2D texture = Main.projectileTexture[ProjectileID.StardustTowerMark];
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                var seg1 = Positions[i];
                var seg2 = Positions[i + 1];
                int length = (int)(seg2.Position - seg1.Position).Length();
                for (int j = 0; j < length; j++)
                {
                    float progress = (float)j / (float)length;
                    Vector2 pos = Vector2.Lerp(seg1.Position, seg2.Position, progress);
                    float alpha = MathHelper.Lerp(seg1.Alpha, seg2.Alpha, progress);
                    float scale = MathHelper.Lerp(seg1.Size, seg2.Size, progress) / texture.Width;
                    spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.Lerp(Color.LightBlue, Color.Cyan, alpha) * alpha , 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
    }
    public class TestLightning : Lightning
    {
        public float maxTime = 30;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Bolt");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.hostile = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, /*2000f*/ /*1500*/ /*1250f*/ 1500f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
    public class TestLightning2 : Lightning
    {
        public int direction = 1;
        public float rotateby;
        public float maxTime = 360;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Breath");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.hostile = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2500f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width, 240, 4f);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
            NPC projectileowner = Main.npc[(int)projectile.ai[1]];
            //projectile.position = projectileowner.Center + new Vector2(187 * direction, -110);//49);
            projectile.velocity = (Vector2.UnitX * direction).RotatedBy(MathHelper.ToRadians((direction == 1) ? 315 + rotateby : 45 + -rotateby));
        }
    }
    public class TestLightning3 : Lightning
    {
        public float maxTime = 300;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Spark");
        }
        public override void SafeSetDefaults()
        {
            projectile.damage = 25;
            projectile.width = 5;
            projectile.hostile = true;
            projectile.timeLeft = (int)maxTime;
            Length = 3f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 10f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width, 160, /*2f*/ 1f/*, true*/);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
    public class TestLightning4 : Lightning
    {
        public float maxTime = 600;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Beam");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.hostile = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = 2500;
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width, /*320*/ 640, /*16*/ 8, true);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
    public class StormBolt2 : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Bolt");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.friendly = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }

    public abstract class GoldLightning : ModProjectile
    {
        public delegate float Size(float progress);
        public static (List<LightningSegment>, List<float>) CreateLightning(Vector2 from, Vector2 to, Size size, float lengthDiv = 8, float Sway = 80f)
        {
            if (lengthDiv < 1) lengthDiv = 1;
            List<LightningSegment> segments = new List<LightningSegment>();
            Vector2 direction = from - to;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);

            List<float> lengths = new List<float>();
            lengths.Add(0f);
            for (int i = 0; i < direction.Length() / lengthDiv; i++)
            {
                lengths.Add(Main.rand.NextFloat());
            }
            lengths.Add(1f);
            lengths.Sort();

            float jaggedness = 1 / Sway;
            float prevPlacement = 0f;
            segments.Add(new LightningSegment(from, size(0f), 0f, 1));
            for (int i = 1; i < lengths.Count; i++)
            {
                float len = lengths[i];

                float scale = (direction.Length() * jaggedness) * (len - lengths[i - 1]);
                float envelope = MathHelper.Clamp((float)Math.Sin(len * Math.PI) * 10, 0, 1);

                float displace = Main.rand.NextFloat(-Sway, Sway);
                displace -= (displace - prevPlacement) * (1 - scale);
                displace *= envelope;
                Vector2 position = from + len * direction + displace * dir;
                segments.Add(new LightningSegment(position, size(len)));
                prevPlacement = displace;
            }
            return (segments, lengths);
        }
        public static List<LightningSegment> CreateLightning(Vector2 from, Vector2 to, float thickness, float Sway = 80f, float lengthDivider = 8f, bool sine = false)
        {
            var positions = new List<LightningSegment>();
            Vector2 direction = to - from;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);
            float length = direction.Length();

            List<float> Lengths = new List<float>();
            Lengths.Add(0);
            for (float i = 0; i < length / lengthDivider; i++)
                Lengths.Add(Main.rand.NextFloat());
            Lengths.Add(1f);
            Lengths.Sort();

            float Jaggedness = 1 / Sway;

            float prevDisplacement = 0;
            for (int i = 1; i < Lengths.Count; i++)
            {
                float current = thickness;
                float prog = (float)Math.Sin(((float)i / (float)Lengths.Count) * MathHelper.Pi);
                if (sine) current *= prog;
                float len = Lengths[i];

                float scale = (length * Jaggedness) * (len - Lengths[i - 1]);
                float envelope = len > 0.95f ? 20 * (1 - len) : 1;
                float val = Sway;
                float displace = Main.rand.NextFloat(-val, val);
                displace -= (displace - prevDisplacement) * (1 - scale);
                displace *= envelope;
                Vector2 offset = len * direction;
                Vector2 point = from + offset + displace * dir;
                positions.Add(new LightningSegment(point, current, 0f, prog));
                prevDisplacement = displace;
            }

            return positions;
        }

        public static List<LightningSegment> CreateLightning(Vector2 from, Vector2 to, float thickness, bool sine)
        {
            var positions = new List<LightningSegment>();
            Vector2 direction = to - from;
            Vector2 dir = direction.SafeNormalize(-Vector2.UnitY).RotatedBy(Math.PI / 2);
            float length = direction.Length();

            List<float> Lengths = new List<float>();
            Lengths.Add(0);
            for (float i = 0; i < length / 8; i++)
                Lengths.Add(Main.rand.NextFloat());
            Lengths.Add(1f);
            Lengths.Sort();

            const float Sway = 80;
            const float Jaggedness = 1 / Sway;

            float prevDisplacement = 0;
            for (int i = 1; i < Lengths.Count; i++)
            {
                float current = thickness;
                float prog = (float)Math.Sin(((float)i / (float)Lengths.Count) * MathHelper.Pi);
                if (sine) current *= prog;
                float len = Lengths[i];

                float scale = (length * Jaggedness) * (len - Lengths[i - 1]);
                float envelope = len > 0.95f ? 20 * (1 - len) : 1;
                float val = Sway;
                if (Main.rand.NextBool(5)) val *= 3;
                float displace = Main.rand.NextFloat(-val, val);
                displace -= (displace - prevDisplacement) * (1 - scale);
                displace *= envelope;
                Vector2 offset = len * direction;
                Vector2 point = from + offset + displace * dir;
                positions.Add(new LightningSegment(point, current, 0f, prog));
                prevDisplacement = displace;
            }

            return positions;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulHostile;
        public List<LightningSegment> Positions = new List<LightningSegment>();
        public float Length;
        public bool Sine;
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
        }
        private bool Startup;
        public override bool PreAI()
        {
            if (!Startup)
            {
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                Startup = true;
                Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
                Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 122, 0.5f, -0.5f);
            }
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Positions[i].Position, Positions[i + 1].Position, Positions[i].Size, ref a))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //if (Positions == default || Positions == null) return false;
            Texture2D texture = Main.projectileTexture[ProjectileID.StardustTowerMark];
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                var seg1 = Positions[i];
                var seg2 = Positions[i + 1];
                int length = (int)(seg2.Position - seg1.Position).Length();
                for (int j = 0; j < length; j++)
                {
                    float progress = (float)j / (float)length;
                    Vector2 pos = Vector2.Lerp(seg1.Position, seg2.Position, progress);
                    float alpha = MathHelper.Lerp(seg1.Alpha, seg2.Alpha, progress);
                    float scale = MathHelper.Lerp(seg1.Size, seg2.Size, progress) / texture.Width;
                    spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.Lerp(Color.LightYellow, Color.Gold, alpha) * alpha, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
    }

    public class DivineLightning : GoldLightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divine Lightning");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.friendly = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}