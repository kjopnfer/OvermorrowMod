using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Tiles.TrapOre
{
    public class FakeGold : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fakeite");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 46;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 0; //The amount of time the projectile is alive for
        }


        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 350)
                {
                    Main.player[i].AddBuff(BuffID.Obstructed, 240);
                }
            }

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 randPos = new Vector2(projectile.Center.X + Main.rand.Next(-10, 10) * 50, projectile.Center.Y + Main.rand.Next(-10, 10) * 50);
                NPC.NewNPC((int)randPos.X, (int)randPos.Y, NPCID.Wraith);
            }
        }
    }
}