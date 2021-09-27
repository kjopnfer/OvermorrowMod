using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Items.Tools;

namespace OvermorrowMod.NPCs.Sky
{
    public class SunPlatePickaxe : ModNPC
    {
        private int AttTimer = 0;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 50;
            npc.height = 50;
            npc.damage = 13;
            npc.defense = 2;
            npc.lifeMax = 125;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.value = 5f;
            npc.knockBackResist = 0.5f;
            npc.aiStyle = 23;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
        }

        public override void AI()
        {
            frameTimer++;
            if (frameTimer > 4)
            {
                frameTimer = 0;
                frame++;
            }
            if(frame > 3)
            {
                frame = 0;
            }

            if(npc.velocity.X > 5)
            {
                npc.velocity.X = 5;
            }

            if(npc.velocity.X < -5)
            {
                npc.velocity.X = -5;
            }


            if(npc.velocity.Y > 5)
            {
                npc.velocity.Y = 5;
            }

            if(npc.velocity.Y < -5)
            {
                npc.velocity.Y = -5;
            }

            if(npc.velocity.X > 1 || npc.velocity.X < -1)
            {
                AttTimer = 0;
            }


            if(npc.velocity.X < 0.5f && npc.velocity.X > -0.5f)
            {
                AttTimer++;
                if(AttTimer == 30)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    int proj = Projectile.NewProjectile(npc.Center, direction * 2, ModContent.ProjectileType<SkyScythe>(), npc.damage, 3f, Main.player[npc.target].whoAmI, 0f);
                    Main.projectile[proj].tileCollide = false;
                    Main.PlaySound(SoundID.Item, npc.position, 8);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneSkyHeight ? 0.4f : 0f;
        }



        public override void NPCLoot()
        {
            if (Main.rand.Next(100) == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SunPlatePickaxeItem>());
            }
        }
    }
}

