using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Buffs
{
    public class ShadowBrand : ModBuff
    {
        public static readonly int TagDamage = 5;

        public override string Texture => AssetDirectory.Buffs + Name;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.gamePaused) return;

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/spotlight").Value;
            int widthRange = npc.width + 8;
            int heightRange = npc.height - 4;
            int stepSize = 2;
            int maxXSteps = widthRange / stepSize;
            int maxYSteps = heightRange / stepSize;

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(
                    Main.rand.Next(-maxXSteps, maxXSteps + 1) * stepSize,
                    Main.rand.Next(-maxYSteps, 1) * stepSize
                );

                var aura = new Spark(texture, ModUtils.SecondsToTicks(Main.rand.NextFloat(0.7f, 1f)), false)
                {
                    endColor = Color.Red,
                    slowModifier = 0.98f,
                    squashHeight = false
                };

                float scale = Main.rand.NextFloat(0.1f, 0.2f);
                Vector2 velocity = -Vector2.UnitY * Main.rand.Next(3, 4);

                ParticleManager.CreateParticleDirect(aura, npc.Bottom + offset, velocity, Color.Black, 1f, scale, 0f, useAdditiveBlending: false);
            }
        }

        public static void OnKill(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<ShadowBrand>())) return;

            ShadowGrasp.SpawnSmoke(npc.Center);
            var modNPC = npc.GetGlobalNPC<GlobalNPCs>();
            var owner = modNPC.ShadowBrandOwner;

            owner.AddBuff(ModContent.BuffType<BlackEchoes>(), ModUtils.SecondsToTicks(10));
            for (int i = 0; i < 2; i++)
            {
                float xDirection = (i == 0) ? -3f : 3f;
                Vector2 velocity = new Vector2(xDirection, -Main.rand.Next(2, 3));

                Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, velocity, ModContent.ProjectileType<BlackEcho>(), 16, 1f, owner.whoAmI, ai0: (int)BlackEcho.AIStates.Spawned);
            }
        }
    }
}