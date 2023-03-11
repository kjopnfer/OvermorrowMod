using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Spark : Particle
    {
        float maxTime = 70;
        public override void OnSpawn()
        {
            float ff = MathHelper.ToRadians(30);
            Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-ff, ff));
        }
        public override void Update()
        {
            Rotation = Velocity.ToRotation();
            Alpha = (maxTime - ActiveTime) / maxTime;
            Scale = MathHelper.Lerp(0, Scale, Alpha);
            Velocity += Vector2.UnitY * 0.5f;
            if (ActiveTime > maxTime) Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spark").Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, new Vector2(texture.Width, texture.Height), new Vector2(Scale, 0.05f), SpriteEffects.None, 0f);
        }
    }
}