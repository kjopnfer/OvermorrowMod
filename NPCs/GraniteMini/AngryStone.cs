using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace OvermorrowMod.NPCs.GraniteMini
{
    public class AngryStone : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int timer = 0;
        int spritetimer = 0;
        int frame = 1;

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 26;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.aiStyle = 0;
            npc.damage = 15;
            npc.defense = 4;
            npc.lifeMax = 600;
            npc.HitSound = SoundID.NPCHit4;
            npc.value = 12f;
            animationType = NPCID.Zombie;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(36, MethodHelper.SecondsToTicks(60));
        }


        public override void AI()
        {
            timer++;
            if (timer == 2)
            {
                Vector2 pos = npc.Center;
                int damage = 30;
                if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<GraniteRay>()] < 1)
                {
                    Projectile.NewProjectile(pos.X, pos.Y, 0f, 0f, ProjectileType<GraniteRay>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                }
            }

            spritetimer++;
            if (spritetimer > 6)
            {
                frame++;
                spritetimer = 0;
            }
            if (frame > 7)
            {
                frame = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if (Main.player[npc.target].position.X > npc.position.X)
            {
                npc.spriteDirection = -1;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Mini Boss");
            Main.npcFrameCount[npc.type] = 8;
        }
    }
}

