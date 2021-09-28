using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class DriplordAnim : ModProjectile
    {
        //public bool dripsdead = false;
        public float scale = 1;
        public int dripsdead = 0;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/DripplerBoss/DripplerBoss";//"OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoning Circles");
            Main.projFrames[projectile.type] = 12;
        }
        public override void SetDefaults()
        {
            //projectile.width = 12;
            //projectile.height = 12;
            projectile.width = 320;
            projectile.height = 482;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            projectile.scale = 1;
            //projectile.alpha = 255;
        }
        public override void AI()
        {
            projectile.scale = scale;

            if (projectile.ai[0] == 0)
            {
                scale = 0.01f;
                projectile.scale = 0.01f;
            }
            if (projectile.ai[0]++ % /*7*/ 4 == 0 && projectile.ai[1] < 33)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 1000;//500;
                    Projectile.NewProjectile(position, projectile.DirectionFrom(position) * /*12*/ 16, ModContent.ProjectileType<DriplordAnimDrips>(), 0, 0f, Main.myPlayer, 0, projectile.whoAmI);
                }
                projectile.ai[1]++;
            }
            if (dripsdead == 33)
            {
                Vector2 origin = new Vector2((int)projectile.Center.X, (int)projectile.Center.Y - /*50*/ projectile.height / 4);
                float radius = 333;//250;
                int numLocations = 200;
                for (int i = 0; i < 200; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.HeartCrystal, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
                Player player = Main.player[projectile.owner];
                player.GetModPlayer<OvermorrowModPlayer>().TitleID = 3;
                player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
                player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText("Dripplord, the Bloody Assimilator has awoken!", 175, 75, 255);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("Dripplord, the Bloody Assimilator has awoken!"), new Color(175, 75, 255));
                    }

                    NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y + 205, ModContent.NPCType<DripplerBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                    Main.PlaySound(SoundID.Roar, player.position, 0);
                    projectile.Kill();
                }
            }
        }
    }
}
