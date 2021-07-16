using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.NPCs.Bosses.Apollus;
using OvermorrowMod.NPCs.Bosses.GraniteMini;
using OvermorrowMod.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.Projectiles.Artifact;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class DharuudAnim : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandnadoHostile;
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
            projectile.damage = 0;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (projectile.ai[1] == 0)
            {
                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SandnadoFriendly, projectile.damage / 2, 0f, projectile.owner);
                Main.projectile[proj].timeLeft = 160;
                Main.projectile[proj].damage = 0;
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = false;
            }
            if (projectile.ai[1] > 160)
            {
                Player player = Main.player[projectile.owner];
                player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(projectile.Center, 90, 120f, 60f);
                player.GetModPlayer<OvermorrowModPlayer>().TitleID = 1;
                player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;
                int npc = NPC.NewNPC((int)projectile.Center.X, (int)(projectile.Center.Y), ModContent.NPCType<SandstormBoss>(), 0, 0f, 0f, 0f, 0f, 255);
                Projectile.NewProjectile(projectile.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-200, 200)), Vector2.Zero, ModContent.ProjectileType<SafetyZone>(), 0, npc, Main.myPlayer);
                Projectile.NewProjectile(projectile.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-200, 200)), Vector2.Zero, ModContent.ProjectileType<SafetyZone>(), 0, npc, Main.myPlayer);
                Projectile.NewProjectile(projectile.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-200, 200)), Vector2.Zero, ModContent.ProjectileType<SafetyZone>(), 0, npc, Main.myPlayer);
                Projectile.NewProjectile(projectile.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), Main.rand.NextFloat(-200, 200)), Vector2.Zero, ModContent.ProjectileType<SafetyZone>(), 0, npc, Main.myPlayer);
                Main.PlaySound(SoundID.Roar, player.position, 0);
                Vector2 origin = new Vector2((int)projectile.Center.X, (int)(projectile.Center.Y));
                float radius = 100;
                int numLocations = 200;
                for (int i = 0; i < 200; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, 32, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
                projectile.Kill();
            }
            projectile.ai[1]++;
        }
    }
}
