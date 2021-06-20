using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.PostRider.SandSorcerer
{
    public class SandBolt2 : ModProjectile
    {


        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);
        int movement = 0;

        public override void SetDefaults()
        {
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 200;
            projectile.tileCollide = false;
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandBallFalling;


        public override void AI()
        {

            float distanceFromTarget = 500f;
            Vector2 targetCenter = projectile.position;
            bool foundTarget = false;

            projectile.tileCollide = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Main.player[projectile.owner].Center);
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (npc.aiStyle == 97 && npc.lavaImmune && npc.width == 30)
                        {
                            NPCtargetX = npc.Center.X;
                            NPCtargetY = npc.Center.Y;
                            Vector2 Rot = npc.Center;
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            if (foundTarget)
            {
                movement++;
                if(movement == 1)
                {
                    mrand2 = Main.rand.Next(-60, 61);
                    mrand = Main.rand.Next(-60, 61);
                }

                if (movement > 0 && movement < 49)
                {
                    projectile.position.X = NPCtargetX + mrand2;
                    projectile.position.Y = NPCtargetY + mrand;
                }
                if (movement == 50)
                {
                    float positionX = projectile.Center.X;
                    float positionY = projectile.Center.Y;
                    float targetPositionX = Main.player[projectile.owner].Center.X;
                    float targetPositionY = Main.player[projectile.owner].Center.Y;
                    float directionX = targetPositionX - positionX;
                    float directionY = targetPositionY - positionY;
                    Vector2 directionRET = new Vector2(directionX, directionY);
                    directionRET.Normalize();
                    projectile.velocity = directionRET * 11;
                }
            }
        }
    }
}