using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.Apollus;
using OvermorrowMod.Content.NPCs.Bosses.GraniteMini;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Chat;
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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            switch (Projectile.knockBack)
            {
                case 0:
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 dustPos = Projectile.Center + new Vector2(Projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + Projectile.ai[0]));
                            Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, DustID.UnusedWhiteBluePurple, 0f, 0f, 0, default, 2.04f)];
                            dust.noGravity = true;
                        }
                        Projectile.ai[1] -= 15;

                        if (Projectile.ai[1] <= 0)
                        {
                            NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)Projectile.position.X, (int)(Projectile.position.Y), ModContent.NPCType<AngryStone>(), 0, -3f, 0f, 0f, 0f, 255);
                            Player Projectileowner = Main.player[Projectile.owner];
                            //Projectileowner.GetModPlayer<OvermorrowModPlayer>().TitleID = 5;
                            //Projectileowner.GetModPlayer<OvermorrowModPlayer>().FocusBoss = false;
                            //Projectileowner.GetModPlayer<OvermorrowModPlayer>().ShowText = true;



                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    Main.NewText(/*"Gra-knight and Apollus have awoken!"*//*"The Super Biome Bros have awoken!"*//*"The Super Stoner Bros have awoken!"*/ "The Super Stoner Buds have awoken!", 175, 75, 255);
                                }
                                else if (Main.netMode == NetmodeID.Server)
                                {
                                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The Super Stoner Buds have awoken!"), new Color(175, 75, 255));
                                }
                            }
                            Projectile.Kill();
                        }
                    }
                    break;
                case 1:
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            Vector2 dustPos = Projectile.Center + new Vector2(Projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + Projectile.ai[0]));
                            Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, DustID.Enchanted_Gold, 0f, 0f, 0, default, 2.04f)];
                            dust.noGravity = true;
                        }
                        Projectile.ai[1] -= 15;

                        if (Projectile.ai[1] <= 0)
                        {
                            NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)Projectile.position.X, (int)(Projectile.position.Y), ModContent.NPCType<ApollusBoss>(), 0, -1f, 0f, 0f, 0f, 255);
                            Projectile.Kill();
                        }
                    }
                    break;
            }
            Projectile.ai[0]++;
        }
    }
}
