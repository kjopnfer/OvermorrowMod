using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Electricity : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        public List<LightningSegment> Positions = new List<LightningSegment>();


        float width = 8f;
        float length = 8f;
        int maxTime = 20;
        float sway = 960f;
        float divider = 16f;
        public override void OnSpawn()
        {
            Positions = Lightning.CreateLightning(particle.position, particle.position + particle.velocity * length, width, sway, divider, true);
        }

        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Circle").Value;

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
                    spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.Lerp(Color.Cyan, Color.Cyan, alpha) * 0.5f, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale * 3, SpriteEffects.None, 0);
                    spriteBatch.Draw(texture, pos - Main.screenPosition, null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), scale * 0.5f, SpriteEffects.None, 0);
                }
            }

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}