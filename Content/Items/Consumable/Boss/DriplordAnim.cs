using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.DripplerBoss;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class DriplordAnim : ModProjectile
    {
        //public bool dripsdead = false;
        public float scale = 1;
        public int dripsdead = 0;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/DripplerBoss/DripplerBoss";//"OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoning Circles");
            Main.projFrames[Projectile.type] = 12;
        }
        public override void SetDefaults()
        {
            //Projectile.width = 12;
            //Projectile.height = 12;
            Projectile.width = 320;
            Projectile.height = 482;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            Projectile.scale = 1;
            //Projectile.alpha = 255;
        }
        public override void AI()
        {
            Projectile.scale = scale;

            if (Projectile.ai[0] == 0)
            {
                scale = 0.01f;
                Projectile.scale = 0.01f;
            }
            if (Projectile.ai[0]++ % /*7*/ 4 == 0 && Projectile.ai[1] < 33)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 1000;//500;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), position, Projectile.DirectionFrom(position) * /*12*/ 16, ModContent.ProjectileType<DriplordAnimDrips>(), 0, 0f, Main.myPlayer, 0, Projectile.whoAmI);
                }
                Projectile.ai[1]++;
            }
            if (dripsdead == 33)
            {
                Vector2 origin = new Vector2((int)Projectile.Center.X, (int)Projectile.Center.Y - /*50*/ Projectile.height / 4);
                float radius = 333;//250;
                int numLocations = 200;
                for (int i = 0; i < 200; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.HeartCrystal, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
                Player player = Main.player[Projectile.owner];
                //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 3;
                //player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
                //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText("Dripplord, the Bloody Assimilator has awoken!", 175, 75, 255);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Dripplord, the Bloody Assimilator has awoken!"), new Color(175, 75, 255));
                    }

                    NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 205, ModContent.NPCType<DripplerBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                    SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
                    Projectile.Kill();
                }
            }
        }
    }
}
