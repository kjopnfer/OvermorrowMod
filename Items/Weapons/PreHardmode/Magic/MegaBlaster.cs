using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class MegaBlaster : ModItem
    {


        int Charge = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mega Blaster");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 21;
            item.magic = true;
            item.mana = 5;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useTime = 7;
            item.useAnimation = 7;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.5f;
            item.shoot = mod.ProjectileType("SporeMagic");
            item.shootSpeed = 7f;
        }
        public override void HoldItem(Player player)
        {
            Charge++;

            if(Charge > 10 && Charge < 70)
            {
                item.shoot = 590;
            }
            if(Charge > 69)
            {
                item.shoot = 1;
            }

            if(item.shoot == 1 && Charge < 70)
            {
                item.shoot = 590;
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Charge = 0;
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
