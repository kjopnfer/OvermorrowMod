using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Projectiles.DiceProj
{
    public class NpcDice : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.alpha = 0;
            projectile.width = 32;
            projectile.height = 31;
            projectile.CloneDefaults(ProjectileID.SpikyBall);
            projectile.timeLeft = 150;
        }
        public override void AI()
        {
            projectile.velocity.Y = projectile.velocity.Y + 0.00001f;
            if (++projectile.frameCounter >= 7)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6)
                {
                    projectile.frame = 0;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            Vector2 value1 = new Vector2(0f, 0f);
            NPC.NewNPC((int)projectile.Center.X + 0, (int)projectile.Center.Y + 0, mod.NPCType("DoubleDice"), 55, projectile.whoAmI, projectile.Center.X, projectile.Center.Y, 0.0f, byte.MaxValue);
            Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X = 0, value1.Y = 0, mod.ProjectileType("TwoTextNicecopy"), projectile.damage - 10, 1f, projectile.owner, 0f);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dice");
            Main.projFrames[projectile.type] = 6;
        }
    }
}
