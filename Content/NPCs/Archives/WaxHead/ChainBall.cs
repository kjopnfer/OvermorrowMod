using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class ChainBall : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "WaxheadFlail";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(300);
            Projectile.penetrate = -1;
        }

        public int ParentID
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public ref float AICounter => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
            {
                Projectile.Kill();
            }
        }

        public override void AI()
        {
            NPC npc = Main.npc[ParentID];
            if (!npc.active)
                Projectile.Kill();
            Projectile.timeLeft = 5;

            if (npc.ModNPC is ChainArm arm)
            {
                float offsetDistance = MathHelper.Lerp(8f, 0f, Math.Abs(arm.BendOffset / 40f));
                Vector2 forearmDirection = new Vector2((float)Math.Cos(arm.GetForearmAngle()), (float)Math.Sin(arm.GetForearmAngle()));
                Projectile.Center = arm.GetHandPosition() + forearmDirection * offsetDistance;
                Projectile.rotation = arm.GetForearmAngle();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ballTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxheadFlail").Value;
            Vector2 ballScreenPos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(ballTexture, ballScreenPos, null, Color.White, Projectile.rotation + MathHelper.PiOver2, ballTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}