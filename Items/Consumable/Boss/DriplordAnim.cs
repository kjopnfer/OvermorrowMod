using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.Apollus;
using OvermorrowMod.NPCs.Bosses.DripplerBoss;
using OvermorrowMod.NPCs.Bosses.GraniteMini;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class DriplordAnim : ModProjectile
    {
        public bool dripsdead = false;
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoning Circles");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (projectile.ai[0] <= 0)
            {
                projectile.ai[0]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 position = projectile.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / 50 * i)) * 200;
                        Projectile.NewProjectile(position, projectile.DirectionFrom(position) * 5/*10*/, ModContent.ProjectileType<DriplordAnimDrips>(), 0, 0f, Main.myPlayer, 0, projectile.whoAmI);
                    }
                }
            }
            if (dripsdead)
            {
                if (projectile.ai[0] == /*30*/ 1)
                {
                    Vector2 origin = new Vector2((int)projectile.position.X, (int)projectile.position.Y - 125);
                    float radius = 333;//250;
                    int numLocations = 200;
                    for (int i = 0; i < 200; i++)
                    {
                        Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                        Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                        int dust = Dust.NewDust(position, 2, 2, 12, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                        Main.dust[dust].noGravity = true;
                    }
                }
                if (projectile.ai[0]++ >= /*60*/2)
                {
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

                        NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, ModContent.NPCType<DripplerBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                        Main.PlaySound(SoundID.Roar, player.position, 0);
                        projectile.Kill();
                    }
                }
            }
        }
    }
}
