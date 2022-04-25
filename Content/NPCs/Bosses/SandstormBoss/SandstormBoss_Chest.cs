using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Core;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class SandstormBoss_Chest : PullableNPC
    {
        protected NPC ParentNPC;
        public override string Texture => "OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/SandstormBoss_Chest";
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chestplate");
        }

        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 48;
            NPC.aiStyle = -1;
            NPC.lifeMax = 1200;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.friendly = false;
        }

        public override void AI()
        {
            ParentNPC = Main.npc[(int)NPC.ai[0]];

            NPC.spriteDirection = ParentNPC.spriteDirection;
            NPC.Center = ParentNPC.Center;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            int[] Whitelist = { ModContent.ProjectileType<ForbiddenBeamFriendly>(), ModContent.ProjectileType<GiantBeam>(), ModContent.ProjectileType<ForbiddenBurst>() };
            if (projectile.friendly)
            {
                if (Whitelist.Contains(projectile.type))
                {
                    return true;
                }
            }

            return false;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/SandstormBoss_Chest").Value;
            Color color = Lighting.GetColor((int)NPC.Center.X / 16, (int)(NPC.Center.Y / 16f));

            float DirectionOffset = NPC.spriteDirection == 1 ? 0 : 2;
            spriteBatch.Draw(texture, NPC.Center + new Vector2(DirectionOffset, 2) - screenPos, null, color, NPC.rotation, texture.Size() / 2f, 1f, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }

        public override void OnKill()
        {
            if (Sandstorm.Happening)
            {
                Sandstorm.Happening = false;
                Sandstorm.TimeLeft = 120;
                ModUtils.SandstormStuff();
            }
        }
    }
}