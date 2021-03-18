using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public class DemonMonocle : Artifact
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demonic Monocle");
            Tooltip.SetDefault("[c/00FF00:{ Artifact }]\nConsume 2 Soul Essences to summon Demon Eyes that home in on enemies\n" +
                "'Why? I don't know either'");
        }

        public override void SafeSetDefaults()
        {
            item.width = 32;
            item.height = 30;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.UseSound = SoundID.Item103;
            item.consumable = false;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                ConsumeSouls(2, player);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for(int i = 0; i < 2; i++)
                {
                    Vector2 randPosition = new Vector2(Main.rand.Next(-2, 2) * 20, Main.rand.Next(-2, 2) * 20);
                    Projectile.NewProjectile(player.Center + randPosition, Vector2.Zero, ModContent.ProjectileType<BloodyBallFriendly>(), 16, 4f, player.whoAmI, 0f, 0f);
                }
            }
            return true;
        }
    }
}