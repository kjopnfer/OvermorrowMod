using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Bows;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class PatronusBow_Held : HeldBow
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + "PatronusBow";
        public override int ParentItem => ModContent.ItemType<PatronusBow>();
        public override BowStats GetBaseBowStats()
        {
            return new BowStats
            {
                ChargeSpeed = 1.2f,
                MaxChargeTime = 45f,
                ShootDelay = 20f,
                MaxSpeed = 14f,
                DamageMultiplier = 1.1f,
                StringColor = Color.White,
                PositionOffset = new Vector2(12, 0),
                StringPositions = (new Vector2(-4, 18), new Vector2(-4, -16))
            };
        }

        private BowPlayer BowPlayer => player.GetModPlayer<BowPlayer>();
        protected override Texture2D GetCustomArrowTexture(Texture2D defaultTexture, bool isPowerShot)
        {
            if (BowPlayer.PatronusBowDamage >= 200)
            {
                return ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "PatronusArrow").Value;
            }

            return ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "PatronusArrow2").Value;
        }

        float animationCounter = 0;
        public override void PreDrawBowEffects(SpriteBatch spriteBatch, Vector2 bowPosition, float chargeProgress)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveProjectiles + "PatronusBowGlow").Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            float progress = MathHelper.Clamp(animationCounter / 60f, 0, 1f);
            float alpha = MathHelper.Lerp(0, 1f, progress);

            //float alpha = BowPlayer.PatronusBowDamage >= 200 ? 1f : 0f;
            Color color = new Color(202, 188, 255);

            if (BowPlayer.PatronusBowDamage >= 200)
            {
                if (animationCounter < 60f) animationCounter++;
                Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.4f, 0.7f));
            }
            else
            {
                animationCounter = 0;
            }

            spriteBatch.Draw(texture, bowPosition - Main.screenPosition, null, color * alpha, Projectile.rotation, texture.Size() / 2f, 1f, spriteEffects, 0f);
        }

        protected override int GetArrowTypeForShot(int defaultArrowType, bool isPowerShot)
        {
            if (isPowerShot && BowPlayer.PatronusBowDamage >= 200)
            {
                //return ModContent.ProjectileType<Patronus>();
            }

            return ModContent.ProjectileType<WispArrow>();
        }

        protected override void OnArrowFired(Projectile arrow, bool isPowerShot)
        {
            if (isPowerShot && BowPlayer.PatronusBowDamage >= 200)
            {
                BowPlayer.PatronusBowDamage = 0;
                arrow.ai[1] = 1;
                //Vector2 direction = Vector2.Normalize(arrow.velocity);
                //arrow.velocity = direction * 8f;
            }

            base.OnArrowFired(arrow, isPowerShot);
        }
    }

    public class PatronusBow : ModBow<PatronusBow_Held>, ITooltipEntities, IWeaponClassification
    {
        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "StarboundStag" + ".DisplayName");
            var line = Language.GetTextValue(LocalizationPath.TooltipEntities + "StarboundStag" + ".Description.Line0");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "StarboundStag" + ".Description.Line1");
            var line3 = Language.GetTextValue(LocalizationPath.TooltipEntities + "StarboundStag" + ".Description.Line2");

            return new List<TooltipEntity>() {
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "StarboundStag").Value,
                    title,
                    [line, line2, line3],
                    30f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Ranged),
            };
        }

        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public WeaponType WeaponType => WeaponType.Bow;

        public override void SafeSetDefaults()
        {
            //Item.damage = 1000;
            Item.damage = 32;
            Item.width = 24;
            Item.height = 72;
            Item.autoReuse = true;
            Item.shootSpeed = 10f;
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 25;
            Item.useAnimation = 25;
        }
    }
}