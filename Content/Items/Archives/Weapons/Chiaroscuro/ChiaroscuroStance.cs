using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class ChiaroscuroStance : ModProjectile, IDrawAdditive, IWeaponClassification
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "ChiaroscuroProjectile";

        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public Player Owner => Main.player[Projectile.owner];
        public WeaponType WeaponType => WeaponType.Rapier;
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
            Lighting.AddLight(Projectile.Center, new Color(108, 108, 224).ToVector3() * 0.5f);

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            //Projectile.timeLeft = 5;

            Projectile.Center = Owner.MountedCenter + new Vector2(10 * Owner.direction, 0);
            //Projectile.Center = Owner.MountedCenter + new Vector2(10 * Owner.direction, 0).RotatedBy(MathHelper.ToRadians(-90));

            AICounter++;
            float progress = MathHelper.Clamp(AICounter, 0, 15f) / 15f;
            if (AICounter == 15)
            {
                Owner.AddBuff(ModContent.BuffType<Buffs.ChiaroscuroStance>(), ModUtils.SecondsToTicks(10));
                ActivateAllShadows();
            }

            Projectile.rotation = MathHelper.Lerp(MathHelper.ToRadians(240) * Owner.direction, 0, EasingUtils.EaseOutBack(progress));
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
                    if (Main.projectile[i].ModProjectile is ChiaroscuroShadow shadow)
                    {
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
            if (AICounter < 15)
                return;

            const float totalLoopTicks = 30f;
            const float fadeOutStartTick = 900f;       // 15 seconds * 60 ticks per second
            const float fadeOutDurationTicks = 180f;   // Fade out duration (~3 seconds)

            if (AICounter >= fadeOutStartTick + fadeOutDurationTicks)
            {
                AICounter = (int)(fadeOutStartTick + fadeOutDurationTicks); // Clamp max fade out
            }

            float baseGlint = 0f;
            if (AICounter < fadeOutStartTick)
            {
                float loopTime = (AICounter % totalLoopTicks) / totalLoopTicks; // normalized 0 to 1 over 30 ticks
                baseGlint = EasingUtils.EaseInOutBounce(loopTime);
            }
            else
            {
                float fadeProgress = (AICounter - fadeOutStartTick) / fadeOutDurationTicks;
                fadeProgress = MathHelper.Clamp(fadeProgress, 0f, 1f);
                baseGlint = MathHelper.Lerp(1f, 0f, fadeProgress);
            }

            // For testing, directly assign glintScale and glintOpacity to baseGlint
            float glintScale = baseGlint * 0.6f;
            float glintOpacity = baseGlint;

            Color color = new Color(108, 108, 224);
            Texture2D texStar = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_08").Value;
            Texture2D texCircle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;

            float offsetY = MathHelper.SmoothStep(-90f, -120f, MathHelper.Clamp(AICounter - 15f, 0, 15f) / 15f);
            Vector2 offset = new Vector2(-5 * Owner.direction, offsetY);

            if (AICounter > 20) glintScale = 0;
            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, color * glintOpacity, -MathHelper.PiOver4, texStar.Size() / 2f, glintScale, 0, 0);
            spriteBatch.Draw(texCircle, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, color * glintOpacity, -MathHelper.PiOver4 * 1.3f, texCircle.Size() / 2f, glintScale * 0.8f, 0, 0);

            /*Color color = new Color(202, 188, 255);
                                                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_08").Value;
                                                Vector2 offset = new Vector2(-5, -90);
                                                Vector2 scale = Vector2.One * 0.5f;

                                                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, color, Projectile.rotation - MathHelper.PiOver2, texture.Size() / 2f, scale, 0, 0);

                                                Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
                                                Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY) + offset, null, color * 0.5f, Projectile.rotation - MathHelper.PiOver2, glow.Size() / 2f, scale, 0, 0);*/
        }
    }
}