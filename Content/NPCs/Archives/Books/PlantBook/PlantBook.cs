using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class PlantBook : LivingGrimoire
    {
        protected override int CastTime => 360;
        protected override void DrawCastEffect(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.AlphaBlend);

            Vector2 drawOffset = new Vector2(2, -12);
            float rotation = NPC.localAI[0];

            float alpha = 1f;
            float size = 1f;
            if (AICounter < 20f)
            {
                alpha = MathHelper.Lerp(0, 1f, AICounter / 20f);
            }
            else if (AICounter > CastTime - 30)
            {
                alpha = MathHelper.Lerp(1f, 0f, (AICounter - (CastTime - 30)) / 30f);
            }

            //if (!Main.gamePaused) NPC.localAI[0] += 0.05f;
            NPC.localAI[0] += 0.05f;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "JungleRuneCircle").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "JungleCircle_Outer").Value;
            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, null, Color.White * alpha, rotation, texture.Size() / 2, size, SpriteEffects.None, 0);
            spriteBatch.Draw(texture2, NPC.Center + drawOffset - Main.screenPosition, null, Color.White * alpha, -rotation * 0.8f, texture2.Size() / 2, size, SpriteEffects.FlipVertically, 0);


            Texture2D texture3 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_02").Value;
            spriteBatch.Draw(texture3, NPC.Center + drawOffset - Main.screenPosition, null, Color.LimeGreen * alpha * 0.5f, rotation * 0.6f, texture3.Size() / 2, size * 0.3f, SpriteEffects.FlipVertically, 0);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }

        public override bool AttackCondition()
        {
            float xDistance = Math.Abs(NPC.Center.X - Player.Center.X);

            bool xDistanceCheck = xDistance <= tileAttackDistance * 18;
            bool yDistanceCheck = Math.Abs(NPC.Center.Y - Player.Center.Y) < 100;

            return xDistanceCheck && yDistanceCheck && Collision.CanHitLine(Player.Center, 1, 1, NPC.Center, 1, 1);
        }

        public override void CastSpell()
        {
            if (AICounter == 30)
            {
                float angle = MathHelper.ToRadians(75);
                Vector2 projectileVelocity = new Vector2(100 * NPC.direction, 0).RotatedByRandom(angle) * 50;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PlantAura>(), 1, 1f, Main.myPlayer);
            }
        }
    }
}