using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{

    public class InfernoRoller : ModNPC
    {

        public override void SetDefaults()
        {
            npc.width = 18;
            npc.height = 18;
            npc.damage = 25;
            npc.aiStyle = 25;
            npc.dontTakeDamage = true;
            npc.defense = 0;
            npc.lifeMax = 60;
            npc.alpha = 20;
            npc.value = 25f;
            npc.noGravity = false;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        int direction = 0;

        public override void AI()
        {

            direction++;
            if (direction == 360)
            {
                npc.dontTakeDamage = false;
                npc.life -= 1000;
            }

            npc.rotation = npc.rotation + 1;
            {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, 0.0f, 0.0f, 400, new Color(), 1.2f);
                Main.dust[dust].noGravity = true;
                Vector2 velocity = npc.velocity;
                Main.dust[dust].velocity = npc.velocity;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Hopper");
        }
    }
}

