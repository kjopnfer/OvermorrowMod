using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class ChiaroscuroStance : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "ChiaroscuroProjectile";

        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.timeLeft = ModUtils.SecondsToTicks(0.5f);
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
        }

        public Vector2 anchorOffset;
        public override void OnSpawn(IEntitySource source)
        {

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            //Projectile.timeLeft = 5;

            float floatSpeed = 0.025f;
            float floatRange = 10f;
            Projectile.Center = Owner.MountedCenter + new Vector2(10 * Owner.direction, 0);
            //Projectile.Center = Owner.MountedCenter + new Vector2(10 * Owner.direction, 0).RotatedBy(MathHelper.ToRadians(-90));

            AICounter++;
            float progress = MathHelper.Clamp(AICounter, 0, 15f) / 15f;
            if (AICounter == 15)
            {
                ActivateAllShadows();
            }

            Projectile.rotation = MathHelper.Lerp(MathHelper.ToRadians(240) * Owner.direction, 0, progress);
            //Projectile.rotation += 0.4f;
            Owner.SetCompositeArmFront(true, stretch, -MathHelper.PiOver2 * Owner.direction);
            //Owner.SetCompositeArmFront(true, stretch, MathHelper.Lerp(MathHelper.ToRadians(15), -MathHelper.PiOver2, progress) * Owner.direction);
            Owner.ChangeDir(Projectile.direction);
        }

        private void ActivateAllShadows()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<ChiaroscuroShadow>() &&
                    Main.projectile[i].owner == Projectile.owner)
                {
                    var shadow = Main.projectile[i].ModProjectile as ChiaroscuroShadow;
                    if (shadow != null)
                    {
                        Main.NewText("activated");
                        shadow.SetActive();
                    }
                }
            }
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            //Vector2 offset = new Vector2(Owner.direction == -1 ? 4 : -6, -40);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, Color.White, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale, 0, 0);

            Vector2 offset = new Vector2(Owner.direction == -1 ? 4 : -6, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, Color.White, Projectile.rotation - MathHelper.PiOver2, new Vector2(12, texture.Height / 2f), Projectile.scale, 0, 0);

            /*Main.spriteBatch.Reload(SpriteSortMode.Immediate);
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.Black.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(1f);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, Color.White, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale, 0, 0);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);
            */
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Color color = new Color(108, 108, 224);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "ChiaroscuroProjectileGlow").Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, color, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, Projectile.scale * 0.75f, 0, 0);
        }
    }
}