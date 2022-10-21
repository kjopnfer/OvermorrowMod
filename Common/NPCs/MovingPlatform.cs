using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Players;
using System;
using Terraria;

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
            NPC.lifeMax = 10;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }

        Vector2 prevPos;
        public override void AI()
        {
            base.AI();

            float yDistTraveled = NPC.position.Y - prevPos.Y;
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead || player.GoingDownWithGrapple || player.GetModPlayer<OvermorrowModPlayer>().PlatformTimer > 0)
                    continue;

                Rectangle playerRect = new Rectangle((int)player.position.X, (int)player.position.Y + (player.height), player.width, 1);
                Rectangle npcRect = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, 8 + (player.velocity.Y > 0 ? (int)player.velocity.Y : 0) + (int)Math.Abs(yDistTraveled));

                if (playerRect.Intersects(npcRect) && player.position.Y <= NPC.position.Y)
                {
                    if (!player.justJumped && player.velocity.Y >= 0)
                    {
                        player.velocity.Y = 0;
                        //player.position.X = NPC.position.X;
                        player.position.Y = NPC.position.Y - player.height + 4;
                        player.position += NPC.velocity;
                    }
                }
            }

            prevPos = NPC.position;
        }
    }
}
