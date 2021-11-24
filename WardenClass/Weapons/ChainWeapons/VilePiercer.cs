using OvermorrowMod.Buffs.RuneBuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class VilePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Guillotine");
            Tooltip.SetDefault("[c/09DBB8:{ Imbuement }]\n" +
                "[c/800080:Right Click] to empower your Warden Artifacts on use\n" +
                "[c/DE3A28:{ Power }] Your Artifact summons inflict 'Emerald Hex,' dealing damage over time\n" +
                "Additionally, spawns Cursed Fireballs on enemy hits that chase down nearby enemies\n" +
                "[c/00FF00:{ Wisdom }] Enemies within range are inflicted with Cursed Flames\n" +
                "Consumes 1 Soul Essence\n" +
                "'The Corruption is the birthplace for many wicked creatures: Like politicians'");
        }

        public override void SafeSetDefaults()
        {
            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 14;
            item.useTime = 14;
            item.knockBack = 0f;
            item.width = 42;
            item.height = 46;
            item.damage = 3;
            item.shootSpeed = 14f;
            item.shoot = mod.ProjectileType("VilePiercerProjectile");
            item.rare = ItemRarityID.Blue;
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
                item.knockBack = 0f;
                item.damage = 3;
                item.shootSpeed = 0f;
                item.shoot = ProjectileID.None;
                item.UseSound = SoundID.DD2_WitherBeastAuraPulse;

                ConsumeSouls(1, player);
                player.GetModPlayer<WardenRunePlayer>().ActiveRune = true;
                player.AddBuff(ModContent.BuffType<VileRune>(), 600);
            }
            else
            {
                item.useStyle = ItemUseStyleID.HoldingOut;
                item.useTurn = true;
                item.useAnimation = 14;
                item.useTime = 14;
                item.UseSound = SoundID.Item71;
                item.damage = 3;
                item.shootSpeed = 14f + modPlayer.modifyShootSpeed();
                item.shoot = mod.ProjectileType("VilePiercerProjectile");
            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ChainKnife);
            recipe.AddIngredient(ItemID.DemoniteBar, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}