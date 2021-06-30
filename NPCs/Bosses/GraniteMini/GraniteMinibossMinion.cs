using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class GraniteMinibossMinion : ModNPC
    {
        private int moveSpeed;
        public bool kill = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight Squire");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 38;
            npc.damage = 25;
            npc.defense = 15;
            npc.lifeMax = 60;
            npc.HitSound = SoundID.NPCHit19;
            npc.knockBackResist = 0.4f;
            npc.DeathSound = SoundID.NPCDeath22;
            npc.noGravity = true;
            npc.noTileCollide = true;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];

            if (npc.lifeMax > 60 || npc.life > 60)
            {
                npc.lifeMax = 60;
                npc.life = 60;
            }

            switch (npc.ai[0])
            {
                case 0:
                    {
                        if (!AliveCheck(player)) { break; }

                        /*if (!AliveCheck(player))
                        {
                            npc.velocity = Vector2.UnitY * -15;
                        }*/
                        
                        if (npc.ai[3] == 0)
                        {
                            moveSpeed = Main.rand.Next(10, 15);
                            npc.ai[3]++;
                        }

                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;

                        float length = move.Length();
                        if (length > moveSpeed)
                        {
                            move *= moveSpeed / length;
                        }
                        var turnResistance = 45;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= moveSpeed / length;
                        }
                        npc.velocity.X = move.X;
                        npc.velocity.Y = move.Y * .98f;

                        if (++npc.ai[1] > 600 + Main.rand.Next(100) && npc.ai[2] == 1)
                        {
                            npc.ai[1] = 0;
                            npc.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        if (!AliveCheck(player)) { break; }

                        npc.velocity = Vector2.Zero;

                        if (++npc.ai[1] % 60 == 0)
                        {
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 7.5f, ProjectileType<GranLaser>(), 2, 10f, Main.myPlayer);
                            npc.ai[2]++;
                        }

                        if (npc.ai[2] == /*3*/ 1)
                        {
                            npc.ai[2] = 0;
                            npc.ai[1] = 0;
                            npc.ai[0] = 0;
                        }
                    }
                    break;
                    
            }
            if (kill == true)
            {
                npc.active = false;
                npc.life = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/GraniteMini/GraniteMinibossMinion_Glow");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X + 2, npc.Center.Y - Main.screenPosition.Y - 10), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            int num = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];

            npc.rotation = npc.velocity.X * 0.15f;
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y += num;
            }
            if (npc.frame.Y >= num * Main.npcFrameCount[npc.type])
            {
                npc.frame.Y = 0;
            }
        }

        private bool AliveCheck(Player player)
        {
            if (!player.active || player.dead)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 25)
                    {
                        npc.timeLeft = 25;
                        npc.velocity = Vector2.UnitY * -7;
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
