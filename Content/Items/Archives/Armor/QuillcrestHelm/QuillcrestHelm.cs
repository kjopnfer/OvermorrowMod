using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Bows;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Bows;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Loot.Pools;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Armor
{
    [AutoloadEquip(EquipType.Head)]
    [Loot<ArchivePool>(ItemType.Ranged, Rarity.Rare)]
    public class QuillcrestHelm : ModItem, IBowModifier, ITooltipEntities
    {
        private const float ProcChance = 0.25f;
        private const float QuillSpeedMultiplier = 1.4f;
        private const float QuillSpreadDegrees = 6f;
        private const float QuillSpawnOffset = 10f;

        public override string Texture => AssetDirectory.ArchiveItems + Name;

        public override void SetStaticDefaults()
        {
            int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
        }

        public List<TooltipEntity> TooltipObjects()
        {
            var title = Language.GetTextValue(LocalizationPath.TooltipEntities + "QuillArrow.DisplayName");
            var line0 = Language.GetTextValue(LocalizationPath.TooltipEntities + "QuillArrow.Description.Line0");
            var line1 = Language.GetTextValue(LocalizationPath.TooltipEntities + "QuillArrow.Description.Line1");
            var line2 = Language.GetTextValue(LocalizationPath.TooltipEntities + "QuillArrow.Description.Line2");

            return [
                new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.Tooltips + "QuillArrow").Value,
                    title,
                    [line0, line1, line2],
                    0f,
                    ProjectileTooltipType.Projectile,
                    DamageClass.Ranged),
            ];
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 38;
            Item.defense = 6;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.05f;
            player.GetModPlayer<BowPlayer>().AddBowModifier(this);
        }

        public void ModifyBowStats(BowStats stats, Player player) { }

        public void OnPowerShot(HeldBow bow, Player player) { }

        public void OnArrowFired(HeldBow bow, Player player, Projectile arrow)
        {
            if (player.whoAmI != Main.myPlayer) return;
            if ((float)player.statLife / player.statLifeMax2 < 0.8f) return;
            if (Main.rand.NextFloat() >= ProcChance) return;

            float spread = MathHelper.ToRadians(Main.rand.NextFloat(-QuillSpreadDegrees, QuillSpreadDegrees));
            Vector2 direction = arrow.velocity.SafeNormalize(Vector2.UnitX * player.direction).RotatedBy(spread);
            Vector2 velocity = direction * (arrow.velocity.Length() * QuillSpeedMultiplier);

            Vector2 perpendicular = direction.RotatedBy(MathHelper.PiOver2);
            Vector2 spawnPosition = arrow.Center + perpendicular * Main.rand.NextFloat(-QuillSpawnOffset, QuillSpawnOffset);

            Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), spawnPosition, velocity,
                ModContent.ProjectileType<QuillArrow>(), arrow.damage, arrow.knockBack, player.whoAmI);
        }
    }
}
