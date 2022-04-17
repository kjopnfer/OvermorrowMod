using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.GraniteMini
{
    public class GraniteMinibossMinion : ModNPC
    {
        private int moveSpeed;
        public bool kill = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight Squire");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 38;
            NPC.damage = 25;
            NPC.defense = 15;
            NPC.lifeMax = 60;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 0.4f;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (NPC.lifeMax > 60 || NPC.life > 60)
            {
                NPC.lifeMax = 60;
                NPC.life = 60;
            }

            switch (NPC.ai[0])
            {
                case 0:
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (NPC.ai[3] == 0)
                        {
                            moveSpeed = Main.rand.Next(5, 10);
                            NPC.ai[3]++;
                        }

                        Vector2 moveTo = player.Center;
                        var move = moveTo - NPC.Center;

                        float length = move.Length();
                        if (length > moveSpeed)
                        {
                            move *= moveSpeed / length;
                        }
                        var turnResistance = 45;
                        move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= moveSpeed / length;
                        }
                        NPC.velocity.X = move.X;
                        NPC.velocity.Y = move.Y * .98f;

                        if (++NPC.ai[1] > 600 + Main.rand.Next(100) && NPC.ai[2] == 1)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        if (!PlayerAlive(player)) { break; }

                        NPC.velocity = Vector2.Zero;

                        if (++NPC.ai[1] % 60 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(player.Center) * 7.5f, ProjectileType<GranLaser>(), NPC.damage, 10f, Main.myPlayer);
                            }
                            NPC.ai[2]++;
                        }

                        if (NPC.ai[2] == 1)
                        {
                            NPC.ai[2] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[0] = 0;
                        }
                    }
                    break;

            }
            if (kill == true)
            {
                NPC.active = false;
                NPC.life = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/GraniteMini/GraniteMinibossMinion_Glow").Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - Main.screenPosition.X + 2, NPC.Center.Y - Main.screenPosition.Y - 10), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            int num = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];

            NPC.rotation = NPC.velocity.X * 0.15f;
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += num;
            }
            if (NPC.frame.Y >= num * Main.npcFrameCount[NPC.type])
            {
                NPC.frame.Y = 0;
            }
        }

        bool PlayerAlive(Player player)
        {
            if (!player.active || player.dead)
            {
                player = Main.player[NPC.target];
                NPC.TargetClosest();
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 25)
                    {
                        NPC.timeLeft = 25;
                        NPC.velocity = Vector2.UnitY * -7;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
