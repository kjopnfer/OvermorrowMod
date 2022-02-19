
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class CreeperShooter : ModNPC
    {
        public override void SetDefaults()
        {
            npc.lifeMax = 850;
            npc.damage = 30;
            npc.defense = 10;
            npc.knockBackResist = 0f;
            npc.width = 34;
            npc.height = 32;
            animationType = 0;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.aiStyle = 0;
            npc.HitSound = SoundID.DD2_SkeletonHurt;
            npc.value = Item.buyPrice(0, 0, 14, 7);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Boss");
        }
        public override void AI()
        {
            npc.ai[0]++;
            if (npc.ai[0] == 2)
            {
                Vector2 pos = npc.Center;
                int damage = 100;
                if (Main.LocalPlayer.ownedProjectileCounts[ProjectileType<GraniteRay>()] < 1)
                {
                    Projectile.NewProjectile(pos.X, pos.Y, 0f, 0f, ProjectileType<GraniteRay>(), damage, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.target;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (npc.lifeMax * 1);
            npc.damage = (npc.damage * 1);
        }
    }
}