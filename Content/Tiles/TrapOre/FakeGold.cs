using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Content.Tiles.TrapOre
{
    public class FakeGold : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fakeite");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 0; //The amount of time the projectile is alive for
        }


        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(Projectile.Center, Main.player[i].Center);
                if (distance <= 350)
                {
                    Main.player[i].AddBuff(BuffID.Obstructed, 240);
                }
            }

            for (int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                Vector2 randPos = new Vector2(Projectile.Center.X + Main.rand.Next(-10, 10) * 50, Projectile.Center.Y + Main.rand.Next(-10, 10) * 50);
                NPC.NewNPC(Projectile.GetSource_Death(), (int)randPos.X, (int)randPos.Y, NPCID.Wraith);
            }
        }
    }
}