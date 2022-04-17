using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs.Debuffs
{
    public class SoulFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            DisplayName.SetDefault("Flame Test");
        }

        public override string Texture => "Terraria/Buff_" + BuffID.OnFire;

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 100;
            Vector2 size = npc.Size;
            for (int i = 0; i < 3; i++)
                Particle.CreateParticle(Particle.ParticleType<Glow1>(), npc.position + new Vector2(size.X * Main.rand.NextFloat(), size.Y * Main.rand.NextFloat()), Vector2.Zero, new Color(125, 228, 240 /*0.4f, 0.2f, 0.07f*/), 0.25f, Main.rand.NextFloat(0.7f, 1.3f), 0, Main.rand.Next(20, 30));
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 100;
            Vector2 size = player.Size;
            for (int i = 0; i < 3; i++)
                Particle.CreateParticle(Particle.ParticleType<Glow1>(), player.position + new Vector2(size.X * Main.rand.NextFloat(), size.Y * Main.rand.NextFloat()), Vector2.Zero, new Color(0.4f, 0.2f, 0.02f), 1f, Main.rand.NextFloat(0.7f, 1.3f), 0, Main.rand.Next(20, 30));
        }
    }
}