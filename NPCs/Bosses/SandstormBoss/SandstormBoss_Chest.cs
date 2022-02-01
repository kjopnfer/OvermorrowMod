using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class SandstormBoss_Chest : PullableNPC
    {
        protected NPC ParentNPC;
        public override string Texture => "OvermorrowMod/NPCs/Bosses/SandstormBoss/SandstormBoss_Chest";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chestplate");
        }

        public override void SetDefaults()
        {
            npc.width = 44;
            npc.height = 48;
            npc.aiStyle = -1;
            npc.lifeMax = 1200;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit4;
            npc.friendly = false;
        }

        public override void AI()
        {
            ParentNPC = Main.npc[(int)npc.ai[0]];

            npc.spriteDirection = ParentNPC.spriteDirection;
            npc.Center = ParentNPC.Center;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/SandstormBoss_Chest");
            Color color = Lighting.GetColor((int)npc.Center.X / 16, (int)(npc.Center.Y / 16f));

            float DirectionOffset = npc.spriteDirection == 1 ? 0 : 2;
            Main.spriteBatch.Draw(texture, npc.Center + new Vector2(DirectionOffset, 2) - Main.screenPosition, null, color, npc.rotation, texture.Size() / 2f, 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }
    }
}