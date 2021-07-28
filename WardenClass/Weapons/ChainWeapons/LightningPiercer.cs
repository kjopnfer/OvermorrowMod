using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.RuneBuffs;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class LightningPiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heaven's Chain");
            Tooltip.SetDefault("[c/09DBB8:{ Imbuement }]\n" +
                            "[c/800080:Right Click] to empower your Warden Artifacts on use\n" +
                            "[c/EBDE34:{ Courage }] Summons a Storm Cloud to fight for each friendly teammate\n" +
                            "[c/00FF00:{ Wisdom }] Grants 'Golden Wind', increasing speed while within range\n" +
                            "Consumes 1 Soul Essence\n" +
                            "'And then along came Zeus'");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 38;
            item.height = 48;
            item.damage = 6;
            item.shootSpeed = 18f;
            item.shoot = mod.ProjectileType("LightningPiercerProjectile");
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item71;
            item.noUseGraphic = true;

            soulGainChance = 4;
        }

        public override bool CanUseItem(Player player)
        {
            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (player.altFunctionUse == 2 && modPlayer.soulResourceCurrent >= 1 && player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.None)
            {
                item.useStyle = ItemUseStyleID.HoldingUp;
                item.useAnimation = 45;
                item.useTime = 45;
                item.damage = 6;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<LightningRune>(), 600);
            }
            else
            {
                item.autoReuse = true;
                item.useTurn = true;
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.SkyRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
                {
                    item.useStyle = ItemUseStyleID.HoldingUp;
                    item.useAnimation = 35;
                    item.useTime = 35;
                    item.UseSound = null;
                }
                else
                {
                    item.useStyle = ItemUseStyleID.SwingThrow;
                    item.useAnimation = 14;
                    item.useTime = 14;
                    item.UseSound = SoundID.Item71;
                }

                item.damage = 6;
                item.shootSpeed = 18f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("LightningPiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.SkyRune && !player.GetModPlayer<WardenRunePlayer>().runeDeactivate)
            {
                position = Main.MouseWorld;
                type = ModContent.ProjectileType<GoldCloud>();
            }

            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}