using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.NPCs.Bosses.StormDrake;

namespace OvermorrowMod.Particles
{
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
                catch(Exception e)
                {
                    Main.NewText(e.Message + " FUUUUUUUUUUUUUUUCK");
                }
            }
        }
        public void Recreate()
        {
            // previousSegments = segments;
            var seg = Lightning.CreateLightning(from, to, delegate (float progress) {return baseSize * (1f - progress);}, segDiv, sway);
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
            var seg = Lightning.CreateLightning(from, to, delegate (float progress) {return baseSize * (1f - progress);}, segDiv, sway);
            segments = seg.Item1;
            previousSegments = seg.Item1;
            lengths = seg.Item2;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw lightnings, draw alpha circle, draw extra small electric particles
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_" + ProjectileID.StardustTowerMark);
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
    public class LightningSpark : CustomParticle
    {
        public override string Texture => "Textures/Empty";

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
            foreach(Lightning2 lightning in lightnings)
            {
                lightning.Draw(spriteBatch);
            }
        }
    }
}