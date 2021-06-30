using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Effects
{
    public class LightningBolt : ModProjectile
    {
        public float widthdecrease = 0.01f;
        public Vector2 start;
        public Vector2 end;
        public Color color = Color.White;
        public float scale = 1;
        public double jagamount = 1.3;
        public int segmentamount = 20;
        Vector2[] positions;
        public virtual void CustomDrawPerPixel(Vector2 vec, SpriteBatch sprite) { }
        public virtual void CustomDraw(Vector2[] positions, SpriteBatch spriteBatch) { }
        public override string Texture => "OvermorrowMod/Effects/Pixel";
        public void DrawLine(Vector2 start, Vector2 end, Color color, SpriteBatch spriteBatch, float scale)
        {
            Vector2 unit = end - start;
            float length = unit.Length();
            unit.Normalize();
            for (int i = 0; i < length; i++)
            {
                Vector2 drawpos = start + unit * i - Main.screenPosition;
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawpos, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                CustomDrawPerPixel(drawpos, spriteBatch);
            }
        }
        public void DrawLine(Vector2 start, Vector2 end, Color color, SpriteBatch spriteBatch, Vector2 scale)
        {
            Vector2 unit = end - start;
            float length = unit.Length();
            unit.Normalize();
            for (int i = 0; i < length; i++)
            {
                Vector2 drawpos = start + unit * i - Main.screenPosition;
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawpos, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
                CustomDrawPerPixel(drawpos, spriteBatch);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            start = Main.player[(int)projectile.ai[0]].Center;
            end = Main.MouseWorld;
            Vector2 unit = end - start;
            float length = unit.Length();
            unit.Normalize();
            if (projectile.ai[1] == 0)
            {
                positions = new Vector2[segmentamount];
                projectile.ai[1]++;
                positions[0] = start;
                for (int i = 1; i < positions.Length; i++)
                {
                    positions[i] = (positions[i - 1] + unit.RotatedByRandom(jagamount) * ((int)length / positions.Length));
                }
            }
            if (!Main.gamePaused)
            {
                for (int i = 0; i < positions.Length - 1; i++)
                {
                    scale -= widthdecrease;
                    DrawLine(positions[i], positions[i + 1], color, spriteBatch, scale);
                }
                CustomDraw(positions, spriteBatch);
            }
            return false;
        }
        public int Clamp(int i, int min, int max)
        {
            if (i < 0)
                if (i < -max)
                    i = -max;
                else if (i > -min)
                    i = -min;
            if (i > 0)
                if (i > max)
                    i = max;
                else if (i < min)
                    i = min;
            return i;
        }
    }
    //the following is an example
    class GiantBolt : LightningBolt
    {

        public override void SetDefaults()
        {
            projectile.damage = 100;
            projectile.friendly = true;
        }
        public Vector2 FindBelowTile(Vector2 vec)
        {
            vec.Y /= 16;
            while (!Framing.GetTileSafely((int)vec.X / 16, (int)vec.Y).active() && Framing.GetTileSafely((int)vec.X / 16, (int)vec.Y).type != TileID.Tombstones && Framing.GetTileSafely((int)vec.X / 16, (int)vec.Y).type != TileID.Trees)
                vec.Y++;
            vec.Y *= 16;
            return vec;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.ai[0] = reader.ReadInt32();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2[] positions = new Vector2[segmentamount];
            start = Main.player[(int)projectile.ai[0]].Center - Vector2.UnitY * 600;
            end = FindBelowTile(Main.player[(int)projectile.ai[0]].Center);
            Vector2 unit = end - start;
            float length = unit.Length();
            unit.Normalize();
            for (int i = 0; i < Main.rand.Next(6) * projectile.localAI[0]; i++)
            {
                positions = new Vector2[segmentamount];
                positions[0] = start;
                for (int k = 1; k < positions.Length; k++)
                {
                    positions[k] = (positions[k - 1] + unit.RotatedByRandom(jagamount) * ((int)length / positions.Length));
                }
                if (!Main.gamePaused)
                {
                    for (int k = 0; k < positions.Length - 1; k++)
                    {
                        DrawLine(positions[k], positions[k + 1], color, spriteBatch, scale);
                        scale -= widthdecrease;
                    }
                    DrawLine(positions[positions.Length - 1], end, color, spriteBatch, scale);
                }
            }
            return false;
        }
        public override void CustomDrawPerPixel(Vector2 vec, SpriteBatch sprite)
        {
            Lighting.AddLight(vec, color.ToVector3());
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                segmentamount = 20;
                scale = 10;
            }
            if (++projectile.localAI[0] > 10)
                color = Color.Aqua;
            if (projectile.localAI[0] > 20)
            {
                projectile.localAI[0] = 0;
                color = Color.White;
            }
        }
    }
}
