using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.IorichWand
{
    public class IorichWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Renewal Wand of Iorich");
            Tooltip.SetDefault("Summons a Guardian of Iorich\n" +
                "Hold down left click to fire stars in the cursor's direction\n" +
                "Right click to summon a shield that heals the player and reduces incoming damage\n" +
                "While the Sentinel is summoned and this weapon is held, gather nature energy towards the player\n" +
                "Sentinel abilities consume nature energy that has been stored by the player\n" +
                "Takes 2 minion slots, only one can be active at a time\n" +
                "'Perhaps in time, the Ents shall return'");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.mana = 0;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 25;
            item.useTurn = false;
            item.useAnimation = 19;
            item.useTime = 19;
            item.width = 48;
            item.height = 48;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<IorichSummon>();
            item.buffType = ModContent.BuffType<IorichGuardian>();
            item.shootSpeed = 10.5f;
            item.knockBack = 4.5f;
            item.summon = true;
            item.channel = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.ownedProjectileCounts[item.shoot] < 1)
            {
                // This literally won't apply from item.buffType?????
                player.AddBuff(ModContent.BuffType<IorichGuardian>(), 1);
                Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }

            return false;
        }
    }
}