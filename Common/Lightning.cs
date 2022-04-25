using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
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
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulHostile;
        public List<LightningSegment> Positions = new List<LightningSegment>();
        public float Length;
        public bool Sine;
        public Color Color1 = Color.LightBlue;
        public Color Color2 = Color.Cyan;
        public virtual void SafeSetDefaults() { }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width/*, Sine*/);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        private bool Startup;
        public override bool PreAI()
        {
            if (!Startup)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY);
                Startup = true;
                Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width/*, Sine*/);
                SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 122, 0.5f, -0.5f);
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

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            //if (Positions == default || Positions == null) return false;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Circle").Value;
            //Texture2D texture2 = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);

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
                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, Color.Lerp(Color1, Color2, alpha) * 0.5f, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale * 3, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale * 0.5f, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

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
            Projectile.width = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 3000f);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width/*, Sine*/);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
    public class TestLightning2 : Lightning
    {
        public int Direction = 1;
        public float RotateBy;
        public float maxTime = 360;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Breath");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 3000f);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width, 240, 4f);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
            NPC projectileowner = Main.npc[(int)Projectile.ai[1]];
            //Projectile.position = projectileowner.Center + new Vector2(187 * Direction, -110);//49);
            Projectile.velocity = (Vector2.UnitX * Direction).RotatedBy(MathHelper.ToRadians((Direction == 1) ? 315 + RotateBy : 45 + -RotateBy));
        }
    }
    public class LightningBurst : Lightning
    {
        public float maxTime = 90;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Spark");
        }
        public override void SafeSetDefaults()
        {
            Projectile.damage = 25;
            Projectile.width = 5;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 3f;
            Sine = true;
        }
        public override void AI()
        {
            if (!Main.npc[(int)Projectile.ai[0]].active || !Main.projectile[(int)Projectile.ai[1]].active)
            {
                Projectile.Kill();
            }

            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 10f);
            Positions = Lightning.CreateLightning(Main.projectile[(int)Projectile.ai[1]].Center/*Projectile.Center*/, Main.npc[(int)Projectile.ai[0]].Center/*Projectile.Center + Projectile.velocity * Length*/, Projectile.width, 160, /*2f*/ 1f/*, true*/);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
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
            Projectile.width = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 3000f);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width, /*320*/ 640, /*16*/ 8, true);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
    public class TestLightning5 : Lightning
    {
        public float RotateBy;
        public float maxTime = 180;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Breath");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 3000f);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width, 240, 4f);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(RotateBy));
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
            Projectile.width = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 2000f);
            Positions = Lightning.CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width/*, Sine*/);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}