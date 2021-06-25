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
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 26;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.aiStyle = 0;
            npc.damage = 10;
            npc.defense = 9;
            npc.lifeMax = 50;
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
        }


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Mini Boss");
            Main.npcFrameCount[npc.type] = 3;
        }
    }
}

