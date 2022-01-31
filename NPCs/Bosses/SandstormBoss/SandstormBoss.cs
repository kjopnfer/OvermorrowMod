using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Projectiles.Boss;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    [AutoloadBossHead]
    public class SandstormBoss : ModNPC
    {
        private bool RunOnce = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dharuud, the Sandstorm");
            //Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 114;
            npc.height = 102;
            npc.aiStyle = -1;
            npc.damage = 21;
            npc.defense = 12;
            npc.lifeMax = 4100;
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.npcSlots = 10f;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SandstormBoss");
            bossBag = ModContent.ItemType<SandstormBag>();
        }
        public ref float AICase => ref npc.ai[0];
        public ref float GlobalCounter => ref npc.ai[1];
        public ref float MiscCounter => ref npc.ai[2];
        public ref float MiscCounter2 => ref npc.ai[3];

        public enum AIStates
        {
            Buffer = -2,
            Intro = -1,
            Selector = 0,
            Shards = 1,
            Vortex = 2,
            Runes = 3,
            Energy = 4,
            Death = 5
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

            if (RunOnce)
            {
                for (int i = 0; i < 3; i++)
                {
                    int RADIUS = 100;
                    Vector2 SpawnRotation = npc.Center + new Vector2(RADIUS, 0).RotatedBy(120 * i);

                    int NPCType = -1;
                    switch (i)
                    {
                        case 0:
                            NPCType = ModContent.NPCType<LaserMinion>();
                            break;
                        case 1:
                            NPCType = ModContent.NPCType<BeamMinion>();
                            break;
                        case 2:
                            NPCType = ModContent.NPCType<BlasterMinion>();
                            break;
                    }

                    NPC.NewNPC((int)SpawnRotation.X, (int)SpawnRotation.Y, NPCType, 0, npc.whoAmI, 0f, 120 * i);
                }

                RunOnce = false;
            }

            switch (AICase)
            {
                case (int)AIStates.Selector:
                    if (MiscCounter++ <= 180)
                    {
                        npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 2, 0.05f);
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (player.Center.Y) > npc.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }
                    else if (MiscCounter > 180 && MiscCounter < 300)
                    {
                        npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (player.Center.X > npc.Center.X ? 1 : -1) * 2, 0.05f);
                        npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, (player.Center.Y - 250) > npc.Center.Y ? 2.5f : -2.5f, 0.02f);
                    }

                    // Chooses a random active minion to perform their attack
                    if (MiscCounter == 300)
                    {
                        npc.velocity = Vector2.Zero;

                        AttackHandler();
                    }

                    if (MiscCounter == 540)
                    {
                        AICase = (int)AIStates.Vortex;
                        MiscCounter = 0;
                    }

                    break;
                case (int)AIStates.Shards:

                    if (++MiscCounter % 120 == 0)
                    {
                        for (int i = 0; i < Main.rand.Next(5, 8); i++)
                        {
                            Vector2 RandomPosition = new Vector2(Main.rand.Next(-10, 10) * 30, Main.rand.Next(-300, -250));
                            Projectile.NewProjectile(player.Center + RandomPosition, Vector2.UnitY * 8, ModContent.ProjectileType<Fragment>(), npc.damage, 3f, Main.myPlayer, 0f, Main.rand.Next(90, 120));
                        }
                    }

                    if (MiscCounter == 360)
                    {
                        AICase = (int)AIStates.Selector;
                        MiscCounter = 0;
                    }
                    break;
                case (int)AIStates.Vortex:
                    if (MiscCounter++ == 0)
                    {
                        Vector2 RandomPosition = new Vector2(Main.rand.Next(-8, 8) * 30, Main.rand.Next(-100, -50));
                        Projectile.NewProjectile(player.Center + RandomPosition, Vector2.Zero, ModContent.ProjectileType<SandVortex>(), npc.damage, 3f, Main.myPlayer);
                    }

                    if (MiscCounter == 60)
                    {
                        AICase = (int)AIStates.Shards;
                        MiscCounter = 0;
                    }
                    break;
            }
        }

        private void AttackHandler()
        {
            //int[] ValidNPC = { ModContent.NPCType<BlasterMinion>() };
            int[] ValidNPC = { ModContent.NPCType<LaserMinion>(), ModContent.NPCType<BeamMinion>(), ModContent.NPCType<BlasterMinion>() };
            ValidNPC.Shuffle();

            for (int index = 0; index < ValidNPC.Length; index++)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == ValidNPC[index])
                    {
                        if (!((DharuudMinion)npc.modNPC).IsDisabled && !((DharuudMinion)npc.modNPC).Grappled && !((DharuudMinion)npc.modNPC).ReturnIdle)
                        {
                            ((DharuudMinion)npc.modNPC).ExecuteAttack = true;
                            return;
                        }
                    }
                }
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item) => false;

        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        public override void FindFrame(int frameHeight)
        {
            /*npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }

            if (npc.frame.Y >= frameHeight * 4) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }*/
        }

        int frame = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/SandstormBoss_Arms");
            const int TextureHeight = 60;

            npc.frameCounter++;
            if (npc.frameCounter % 12f == 11f)
            {
                frame += 1;
            }

            if (frame >= 4)
            {
                frame = 0;
            }

            var DrawRectangle = new Rectangle(0, TextureHeight * frame, texture.Width, 60);
            Color color = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));
            Main.spriteBatch.Draw(texture, npc.Center + new Vector2(1, (npc.width / 2) + 64) - Main.screenPosition, DrawRectangle, color, npc.rotation, texture.Size() / 2f, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedDarude = true;

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(4);
                // Always drops one of:
                if (choice == 0) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandstormSpinner>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandStaff>());
                }
                else if (choice == 2) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandThrower>());
                }
                else if (choice == 3) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DustStaff>());
                }
            }

            if (Sandstorm.Happening)
            {
                Sandstorm.Happening = false;
                Sandstorm.TimeLeft = 18000;
                ModUtils.SandstormStuff();
            }

            for (int num785 = 0; num785 < 4; num785++)
            {
                Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Sand, 0f, 0f, 100, default(Color), 1.5f);
            }

            for (int num788 = 0; num788 < 40; num788++)
            {
                int num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Sand, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num797].noGravity = true;
                Dust dust24 = Main.dust[num797];
                dust24.velocity *= 3f;
                num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Sand, 0f, 0f, 100, default(Color), 1.5f);
                dust24 = Main.dust[num797];
                dust24.velocity *= 2f;
                Main.dust[num797].noGravity = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}