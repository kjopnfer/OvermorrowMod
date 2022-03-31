using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.Apollus;
using OvermorrowMod.Content.NPCs.Bosses.GraniteMini;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class SSBAnim : ModProjectile
    {
        public int graknightSummonIdentity;
        public int apollusSummonIdentity;

        public override string Texture => AssetDirectory.Empty;
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
            switch (projectile.knockBack)
            {
                case 0:
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + projectile.ai[0]));
                            Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, DustID.UnusedWhiteBluePurple, 0f, 0f, 0, default, 2.04f)];
                            dust.noGravity = true;
                        }
                        projectile.ai[1] -= 15;

                        if (projectile.ai[1] <= 0)
                        {
                            NPC.NewNPC((int)projectile.position.X, (int)(projectile.position.Y), ModContent.NPCType<AngryStone>(), 0, -3f, 0f, 0f, 0f, 255);
                            Player projectileowner = Main.player[projectile.owner];
                            //projectileowner.GetModPlayer<OvermorrowModPlayer>().TitleID = 5;
                            //projectileowner.GetModPlayer<OvermorrowModPlayer>().FocusBoss = false;
                            //projectileowner.GetModPlayer<OvermorrowModPlayer>().ShowText = true;



                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    Main.NewText(/*"Gra-knight and Apollus have awoken!"*//*"The Super Biome Bros have awoken!"*//*"The Super Stoner Bros have awoken!"*/ "The Super Stoner Buds have awoken!", 175, 75, 255);
                                }
                                else if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The Super Stoner Buds have awoken!"), new Color(175, 75, 255));
                                }
                            }
                            projectile.Kill();
                        }
                    }
                    break;
                case 1:
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + projectile.ai[0]));
                            Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, DustID.Enchanted_Gold, 0f, 0f, 0, default, 2.04f)];
                            dust.noGravity = true;
                        }
                        projectile.ai[1] -= 15;

                        if (projectile.ai[1] <= 0)
                        {
                            NPC.NewNPC((int)projectile.position.X, (int)(projectile.position.Y), ModContent.NPCType<ApollusBoss>(), 0, -1f, 0f, 0f, 0f, 255);
                            projectile.Kill();
                        }
                    }
                    break;
            }
            projectile.ai[0]++;
        }
    }
}
