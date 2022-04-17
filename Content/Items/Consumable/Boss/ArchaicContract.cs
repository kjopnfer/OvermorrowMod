using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs.Bosses.Apollus;
using OvermorrowMod.Content.NPCs.Bosses.GraniteMini;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
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
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.maxStack = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 playerPos = new Vector2(player.position.X / 16, player.position.Y / 16);
            Tile tile = Framing.GetTileSafely((int)playerPos.X, (int)playerPos.Y);
            if (tile.WallType == WallID.GraniteUnsafe || tile.WallType == WallID.MarbleUnsafe)
            {
                return !NPC.AnyNPCs(ModContent.NPCType<ApollusBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<AngryStone>());
            }
            else
            {
                return false;
            }
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                graknightSummonIdentity = Projectile.NewProjectile(null, new Vector2((int)player.position.X + 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 0, Main.myPlayer, 0, 900);
                apollusSummonIdentity = Projectile.NewProjectile(null, new Vector2((int)player.position.X - 250, (int)(player.position.Y - 250f)), Vector2.Zero, ModContent.ProjectileType<SSBAnim>(), 0, 1, Main.myPlayer, 0, 900);

                ((SSBAnim)Main.projectile[graknightSummonIdentity].ModProjectile).graknightSummonIdentity = graknightSummonIdentity;
                ((SSBAnim)Main.projectile[apollusSummonIdentity].ModProjectile).apollusSummonIdentity = apollusSummonIdentity;

                SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
                return true;
            }
            return false;
        }
    }
}
