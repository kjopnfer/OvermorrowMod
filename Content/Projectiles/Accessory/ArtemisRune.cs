using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class ArtemisRune : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/Apollus/ArrowRuneCircle";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Circle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
        }

        public override void AI()
        {

            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] == 0)
                {
                    Projectile.scale = 0.01f;
                }
                if (Projectile.ai[0] > 2 && Projectile.ai[0] < 45)
                {
                    Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.05f);
                    Projectile.localAI[0] = MathHelper.Lerp(0.001f, 5f, 0.05f);
                    Projectile.rotation += Projectile.localAI[0];
                }
                if (Projectile.ai[0] == 45)
                {
                    Projectile.ai[1] = 1;
                }
            }
            else
            {
                Projectile.localAI[0] = MathHelper.Lerp(0.001f, 5f, 0.05f);

                if (Projectile.ai[0] % 45 == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<ArtemisArrow>(), 75, 3f, Main.myPlayer);
                        }
                    }
                }
            }

            Projectile.ai[0]++;
            Projectile.rotation += Projectile.localAI[0];
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}