using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs
{
    public abstract class MovingPlatform : CollideableNPC
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("");
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool CheckActive() => false;
        public override void SetDefaults()
        {
            npc.lifeMax = 10;
            npc.immortal = true;
            npc.dontTakeDamage = true;
            npc.noGravity = true;
            npc.knockBackResist = 0;
            npc.aiStyle = -1;
            npc.netAlways = true;
        }

        Vector2 prevPos;
        public override void AI()
        {
            base.AI();

            float yDistTraveled = npc.position.Y - prevPos.Y;
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead || player.GoingDownWithGrapple || player.GetModPlayer<OvermorrowModPlayer>().PlatformTimer > 0)
                    continue;

                Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y + (player.height), player.width, 1);
                Rectangle npcRect = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, 8 + (player.velocity.Y > 0 ? (int)player.velocity.Y : 0) + (int)Math.Abs(yDistTraveled));

                if (playerRect.Intersects(npcRect) && player.position.Y <= npc.position.Y)
                {
                    if (!player.justJumped && player.velocity.Y >= 0)
                    {
                        player.velocity.Y = 0;
                        //player.position.X = npc.position.X;
                        player.position.Y = npc.position.Y - player.height + 4;
                        player.position += npc.velocity;
                    }
                }
            }

            prevPos = npc.position;
        }
    }
}
