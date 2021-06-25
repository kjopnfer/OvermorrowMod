using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
{
    public class Granite_Staff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Geomancer Staff");
            Tooltip.SetDefault("Summons Granite Elementals to dive-bomb your enemies");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.damage = 18;
            item.mana = 20;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Green;
            item.noMelee = true;
            item.summon = true;
            item.buffType = ModContent.BuffType<GraniteEleBuff>();
            item.shoot = ModContent.ProjectileType<GraniteSummon>();
            item.UseSound = SoundID.Item82;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
            return true;
        }
    }
}