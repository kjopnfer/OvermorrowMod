using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.DataStructures;
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
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.mana = 0;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 25;
            Item.useTurn = false;
            Item.useAnimation = 19;
            Item.useTime = 19;
            Item.width = 48;
            Item.height = 48;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IorichSummon>();
            Item.buffType = ModContent.BuffType<IorichGuardian>();
            Item.shootSpeed = 10.5f;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Summon;
            Item.channel = true;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                // This literally won't apply from Item.buffType?????
                player.AddBuff(ModContent.BuffType<IorichGuardian>(), 1);
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }

            return false;
        }
    }
}