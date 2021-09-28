using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.Apollus;
using OvermorrowMod.NPCs.Bosses.GraniteMini;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Consumable.Boss
{
    public class ArchaicContract : ModItem
    {
        private int graknightSummonIdentity;
        private int apollusSummonIdentity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archaic Contract");
            Tooltip.SetDefault("Summons the Spirits of Marble and Granite\n'The words 'simp' seem to be inscribed within the scroll'");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.maxStack = 20;
            item.noMelee = true;
            item.consumable = true;
            item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 playerPos = new Vector2(player.position.X / 16, player.position.Y / 16);
            Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
            if (tile.wall == WallID.GraniteUnsafe || tile.wall == WallID.MarbleUnsafe)
            {
                return !NPC.AnyNPCs(ModContent.NPCType<ApollusBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<AngryStone>());
            }
            else
            {
                return false;
            }
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                graknightSummonIdentity = Projectile.NewProjectile(new Vector2((int)player.position.X + 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 0, Main.myPlayer, 0, 900);
                apollusSummonIdentity = Projectile.NewProjectile(new Vector2((int)player.position.X - 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 1, Main.myPlayer, 0, 900);

                ((SSBAnim)Main.projectile[graknightSummonIdentity].modProjectile).graknightSummonIdentity = graknightSummonIdentity;
                ((SSBAnim)Main.projectile[apollusSummonIdentity].modProjectile).apollusSummonIdentity = apollusSummonIdentity;

                Main.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }
    }
}
