using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    // This is the file where all the particles are stored currently
    public class Lightning2
    {
        public Color color;
        public List<LightningSegment> segments = new List<LightningSegment>();
        public List<LightningSegment> previousSegments = new List<LightningSegment>();
        public List<float> lengths = new List<float>();
        public float baseSize;
        public Vector2 from;
        public Vector2 to;
        public float sway;
        public float segDiv;
        public void UpdateLightning(float progress)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                try
                {
                    previousSegments[i].Position = Vector2.Lerp(previousSegments[i].Position, segments[i].Position, progress);
                    previousSegments[i].Size = MathHelper.Lerp(previousSegments[i].Size, segments[i].Size, progress);
                    previousSegments[i].DefSize = MathHelper.Lerp(previousSegments[i].DefSize, segments[i].DefSize, progress);
                    previousSegments[i].Alpha = MathHelper.Lerp(previousSegments[i].Alpha, segments[i].Alpha, progress);
                }
                catch (Exception e)
                {
                    Main.NewText(e.Message + " FUUUUUUUUUUUUUUUCK");
                }
            }
        }
        public void Recreate()
        {
            // previousSegments = segments;
            var seg = Lightning.CreateLightning(from, to, delegate (float progress) { return baseSize * (1f - progress); }, segDiv, sway);
            segments = seg.Item1;
            lengths = seg.Item2;
        }
        public Lightning2(Vector2 from, Vector2 to, float baseSize, Color color, float segDiv = 8, float sway = 80)
        {
            this.color = color;
            this.segDiv = segDiv;
            this.sway = sway;
            this.baseSize = baseSize;
            this.from = from;
            this.to = to;
            var seg = Lightning.CreateLightning(from, to, delegate (float progress) { return baseSize * (1f - progress); }, segDiv, sway);
            segments = seg.Item1;
            previousSegments = seg.Item1;
            lengths = seg.Item2;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw lightnings, draw alpha circle, draw extra small electric particles
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            for (int i = 0; i < previousSegments.Count - 1; i++)
            {
                var seg1 = previousSegments[i];
                var seg2 = previousSegments[i + 1];
                int length = (int)(seg2.Position - seg1.Position).Length();
                for (int j = 0; j < length; j++)
                {
                    float progress = (float)j / (float)length;
                    Vector2 pos = Vector2.Lerp(seg1.Position, seg2.Position, progress);
                    float alpha = MathHelper.Lerp(seg1.Alpha, seg2.Alpha, progress);
                    float scale = MathHelper.Lerp(seg1.Size, seg2.Size, progress) / texture.Width;
                    spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.Lerp(Color.White, color, alpha) * alpha, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
                }
            }
        }
    }

    public class Smoke : CustomParticle
    {
        float maxTime = 420;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Purple, Color.Violet, particle.scale);
            particle.customData[0] = Main.rand.Next(3, 6);
            if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.scale = 0;
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            /*if (particle.activeTime < 10)
            {
                float progress = (float)particle.activeTime / 10f;
                particle.scale = MathHelper.Lerp(0, particle.customData[0], progress);
                particle.alpha = progress;
            }
            if (particle.activeTime > 35)
            {
                float progress = (float)(particle.activeTime - 35) / 10f;
                particle.scale = MathHelper.Lerp(particle.customData[0], 0f, progress);
                particle.alpha = 1f - progress;
            }*/

            float progress = (float)(particle.activeTime) / maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            particle.scale += 0.05f;
            particle.alpha = 1f - progress;

            particle.rotation += 0.06f;
            particle.velocity.Y -= 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
    public class Smoke2 : CustomParticle
    {
        float maxTime = 120;
        public override void OnSpawn()
        {
            //particle.color = new Color(19, 20, 20);
            particle.customData[0] = Main.rand.Next(3, 6);
            maxTime = particle.customData[1];
            if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.scale = 0;
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            float progress = (float)(particle.activeTime) / maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            particle.scale += 0.025f;
            particle.alpha = 1f - progress;

            particle.rotation += 0.06f;

            if (particle.velocity.X > 0)
            {
                particle.velocity.X -= 0.05f;
            }

            if (particle.velocity.X < 0)
            {
                particle.velocity.X += 0.05f;
            }

            particle.velocity.Y -= 0.05f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);

            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class Slash : CustomParticle
    {
        float maxTime = 30;
        public override void OnSpawn()
        {
            //particle.color = Color.Lerp(Color.Yellow, Color.Orange, particle.scale);

            //particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            particle.rotation = particle.velocity.ToRotation() + MathHelper.Pi;
            //maxTime = particle.customData[0];
            //particle.scale = 0.5f;
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            float progress = (float)(particle.activeTime) / maxTime;
            //particle.scale = MathHelper.Lerp(0f, particle.customData[0], progress);
            //particle.scale += 0.025f;

            particle.alpha = 1f - progress;
            //particle.rotation += 0.06f;
            particle.velocity *= 0.98f;
            particle.rotation = particle.velocity.ToRotation() + MathHelper.Pi;

            /*if (particle.velocity.X > 0)
            {
                particle.velocity.X -= 0.05f;
            }
            if (particle.velocity.X < 0)
            {
                particle.velocity.X += 0.05f;
            }
            particle.velocity.Y -= 0.05f;*/
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale / 2, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class Orb : CustomParticle
    {
        float maxTime = 120;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Yellow, Color.Orange, particle.scale);

            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            maxTime = particle.customData[0];
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            float progress = (float)(particle.activeTime) / maxTime;

            particle.alpha = 1f - progress;
            particle.rotation += 0.06f;
            particle.velocity *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height) / 2, particle.scale / 2, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class Glow1 : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Spotlight";
        public override void Update()
        {
            particle.velocity.X += Main.windSpeedCurrent;
            particle.velocity.Y -= 0.4f;
            float progress = Utils.GetLerpValue(0, particle.customData[0], particle.activeTime);
            if (progress > 0.8f)
                particle.alpha = (progress - 0.5f) * 2;
            if (particle.activeTime > particle.customData[0])
                particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.ParticleTextures[particle.type];
            //Texture2D texture2 = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
            spriteBatch.Reload(BlendState.Additive);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale / 4, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.8f, 0f, texture.Size() / 2, particle.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.5f, 0f, texture.Size() / 2, particle.scale * 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.2f, 0f, texture.Size() / 2, particle.scale * 3, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.01f, 0f, texture.Size() / 2, particle.scale * 7, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture2, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.5f, 0f, texture2.Size() / 2, particle.scale / 4, SpriteEffects.None, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class LightningSpark : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        public List<Lightning2> lightnings = new List<Lightning2>();
        public override void OnSpawn()
        {
            int small = 16;
            int large = 8;
            for (int i = 0; i < small; i++)
            {
                float div = MathHelper.Pi / small;
                float rot = Main.rand.NextFloat(-div, div) + MathHelper.TwoPi / small * i;
                lightnings.Add(new Lightning2(particle.position, particle.position + rot.ToRotationVector2() * 40f * particle.scale, 5f, Color.LightBlue, 4, 1280));
            }
            for (int i = 0; i < large; i++)
            {
                float div = MathHelper.Pi / large;
                float rot = Main.rand.NextFloat(-div, div) + MathHelper.TwoPi / large * i;
                lightnings.Add(new Lightning2(particle.position, particle.position + rot.ToRotationVector2() * 80f * particle.scale, 10f, Color.LightBlue, 8, 1280));
            }
        }
        public override void Update()
        {
            float time = 5;
            float progress = (float)(particle.activeTime % time) / time;
            bool regen = false;
            if (particle.activeTime % time == 0) regen = true;
            foreach (Lightning2 lightning in lightnings)
            {
                if (regen) lightning.Recreate();
                lightning.UpdateLightning(progress);
            }
            if (particle.activeTime > 200) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Lightning2 lightning in lightnings)
            {
                lightning.Draw(spriteBatch);
            }
        }
    }
    public class Spark : CustomParticle
    {
        float maxTime = 70;
        public override void OnSpawn()
        {
            float ff = MathHelper.ToRadians(30);
            particle.velocity = particle.velocity.RotatedBy(Main.rand.NextFloat(-ff, ff));
        }
        public override void Update()
        {
            particle.rotation = particle.velocity.ToRotation();
            particle.alpha = (maxTime - particle.activeTime) / maxTime;
            particle.scale = MathHelper.Lerp(0, particle.scale, particle.alpha);
            particle.velocity += Vector2.UnitY * 0.5f;
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width, texture.Height), new Vector2(particle.scale, 0.05f), SpriteEffects.None, 0f);
        }
    }
    public class BlackFlame : CustomParticle
    {
        float maxTime = 45;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Purple, Color.Violet, particle.scale);
            if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            if (particle.activeTime < 10)
            {
                float progress = (float)particle.activeTime / 10f;
                particle.scale = MathHelper.Lerp(0, particle.customData[0], progress);
                particle.alpha = progress;
            }
            if (particle.activeTime > 35)
            {
                float progress = (float)(particle.activeTime - 35) / 10f;
                particle.scale = MathHelper.Lerp(particle.customData[0], 0f, progress);
                particle.alpha = 1f - progress;
            }
            particle.rotation += 0.1f;
            particle.velocity *= 0f;
        }
    }
    public class Diamond : CustomParticle
    {
        public float maxTime = 90f;
        public override void OnSpawn()
        {
            if (particle.scale != 0f)
            {
                particle.customData[0] = particle.scale;
            }
            else
                particle.customData[0] = Main.rand.NextFloat(1f, 3f);
            particle.scale = 0;
        }
        public override void Update()
        {
            float progress = ((float)particle.activeTime / maxTime);
            float p = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0f, 1f);
            particle.scale = MathHelper.Lerp(0f, particle.customData[0], p);
            particle.alpha = p;
            particle.velocity *= 0.99f;
            particle.rotation = MathHelper.Pi / 2 + particle.velocity.X / 10f;
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos2 = (particle.rotation + MathHelper.Pi / 2).ToRotationVector2() * 5f * particle.scale;
            PrimitivePacket packet = new PrimitivePacket(
                new[]
                {
                    PrimitiveHelper.AsVertex(
                        particle.position + particle.rotation.ToRotationVector2() * 10f * particle.scale,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position + pos2,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position - pos2,
                        particle.color,
                        Vector2.Zero),
                    PrimitiveHelper.AsVertex(
                        particle.position - particle.rotation.ToRotationVector2() * 10f * particle.scale,
                        particle.color,
                        Vector2.Zero)
                },
                PrimitiveType.TriangleStrip,
                4);

            packet.Send();
        }
    }
    public class Explosion : CustomParticle
    {
        float maxTime = 45f;
        public int frame;
        public int maxFrames = 7;
        public override void OnSpawn()
        {
        }
        public override void Update()
        {
            frame = (int)(maxFrames / maxTime * particle.activeTime);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.ParticleTextures[particle.type];
            int frameHeight = texture.Height / maxFrames;
            int curFrame = frameHeight * frame;
            Rectangle source = new Rectangle(0, curFrame, texture.Width, frameHeight);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, source, particle.color, particle.rotation, source.Size() / 2, particle.scale, SpriteEffects.None, 0f);
        }
    }
    public class Glow : CustomParticle
    {
        public float maxTime = 60f;
        public override void OnSpawn()
        {
            particle.customData[0] = particle.scale;
            particle.rotation += MathHelper.Pi / 2;
            particle.scale = 0f;
        }
        public override void Update()
        {
            // 0.05 == 20
            particle.velocity *= 0.98f;
            particle.alpha = Utils.GetLerpValue(0f, 0.05f, particle.activeTime / 60f, clamped: true) * Utils.GetLerpValue(1f, 0.9f, particle.activeTime / 60f, clamped: true);
            particle.scale = Utils.GetLerpValue(0f, 20f, particle.activeTime, clamped: true) * Utils.GetLerpValue(45f, 30f, particle.activeTime, clamped: true);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_644").Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color col = Color.White * particle.alpha * 0.9f;
            col.A /= 2;
            Color col1 = particle.color * particle.alpha * 0.5f; // used
            col1.A = 0;
            Color col2 = col * 0.5f; // used
            col1 *= particle.scale;
            col2 *= particle.scale;
            Vector2 scale1 = new Vector2(0.3f, 2f) * particle.scale * particle.customData[0];
            Vector2 scale2 = new Vector2(0.3f, 1f) * particle.scale * particle.customData[0];
            Vector2 pos = particle.position - Main.screenPosition;
            SpriteEffects effects = SpriteEffects.None;
            spriteBatch.Draw(texture, pos, null, col1, (float)Math.PI / 2f + particle.rotation, origin, scale1, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col1, particle.rotation, origin, scale2, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, (float)Math.PI / 2f + particle.rotation, origin, scale1 * 0.6f, effects, 0f);
            spriteBatch.Draw(texture, pos, null, col2, particle.rotation, origin, scale2 * 0.6f, effects, 0f);
        }
    }

    public class Pulse : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "PulseCircle";
        public float maxSize { get { return particle.customData[0]; } set { particle.customData[0] = value; } }
        public float maxTime { get { return particle.customData[1]; } set { particle.customData[1] = value; } }
        public override void OnSpawn()
        {
            if (particle.customData[1] == 0) particle.customData[1] = 60;
            if (particle.customData[2] == 0) particle.customData[2] = 1;
            if (particle.customData[3] == 0) particle.customData[3] = 1;

            maxTime = particle.customData[1] == 0 ? 60 : particle.customData[1];
            maxSize = particle.scale;
            particle.scale = 0f;
        }

        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            //float progress = particle.activeTime / maxTime;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            particle.scale = MathHelper.SmoothStep(particle.scale, maxSize, progress);
            particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class Shockwave : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Perlin";
        public float maxSize { get { return particle.customData[0]; } set { particle.customData[0] = value; } }
        float maxTime = 60f;
        public override void OnSpawn()
        {
            if (particle.customData[1] == 0) particle.customData[1] = 1;
            if (particle.customData[2] == 0) particle.customData[2] = 1;
            if (particle.customData[3] == 0) particle.customData[3] = 1;
            maxSize = particle.scale;
            particle.scale = 0f;
        }
        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            float progress = (float)particle.activeTime / maxTime;
            particle.scale = MathHelper.Lerp(particle.scale, maxSize, progress);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, progress);
            Color col = new Color(particle.customData[1], particle.customData[2], particle.customData[3]);
            particle.color = Color.Lerp(particle.color, col, progress);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // 1.5 X scale cuz the vanilla shader halves X size
            Vector2 scale = new Vector2(particle.scale * 1.5f, particle.scale);
            // restart spritebatch
            spriteBatch.Reload(SpriteSortMode.Immediate);
            Texture2D texture = Particle.ParticleTextures[particle.type];
            // make a new drawdata(spritebatch draw but saved inside a class)
            DrawData data = new DrawData(texture,
                particle.position - Main.screenPosition,
                new Rectangle(0, 0, texture.Width, texture.Height),
                particle.color * particle.alpha,
                particle.rotation,
                new Vector2(texture.Width, texture.Height) / 2,
                scale,
                SpriteEffects.None,
            0);
            // vanilla effect used in pillar shield
            var effect = GameShaders.Misc["ForceField"];
            effect.UseColor(particle.color);
            effect.Apply(data);
            // make it actually draw
            data.Draw(spriteBatch);
            // restart spritebatch again so effect doesnt continue to be applied
            spriteBatch.Reload(SpriteSortMode.Deferred);
        }
    }
}