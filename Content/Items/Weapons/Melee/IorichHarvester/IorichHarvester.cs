using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester
{
    public class IorichHarvester : ModItem
    {
        private bool PlaySound = false;
        private int UsageCounter = 0;

        public override bool AltFunctionUse(Player player) => true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Iorich");
            Tooltip.SetDefault("Throws a scythe that returns to your hand\n" +
                "Hitting enemies gathers up energy, represented by the orbiting crystals\n" +
                "When ready, right click to summon a Nature Crystal\n" +
                "Breaking the crystal does massive damage in an area, and heals nearby allies\n" +
                "'During The War, it is said that Iorich alone repelled an army of a thousand troops'");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.damage = 17;
            item.useTime = 20;
            item.useAnimation = 20;
            item.width = 58;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<IorichHarvesterProjectile>();
            item.shootSpeed = 10f; //8f;
            item.knockBack = 3f;
            item.melee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(gold: 1);
            item.channel = true;
            item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (UsageCounter++ % 2 == 0)
            {
                Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ModContent.ProjectileType<IorichHarvesterProjectile>(), damage, 0f, player.whoAmI, 120f, 1f);
                return false;
            }

            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IorichHarvesterSpinner>()] <= 0)
            {
                int RADIUS = 65;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.Pi * i);
                    int proj = Projectile.NewProjectile(player.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<IorichHarvesterSpinner>(), 0, 0f, player.whoAmI, MathHelper.Pi * i, RADIUS);
                    ((IorichHarvesterSpinner)Main.projectile[proj].modProjectile).RotationCenter = player;
                }
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount >= 3 && !PlaySound)
            {
                Main.PlaySound(SoundID.Item4, player.Center);
                PlaySound = true;
            }
            else if (player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount <= 0)
            {
                PlaySound = false;
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount >= 3 && player.ownedProjectileCounts[ModContent.ProjectileType<IorichHarvesterCrystalProjectile>()] < 1)
                {
                    player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount = 0;

                    int npc = NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<IorichHarvesterCrystal>(), 0, 0f, player.whoAmI);
                    Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<IorichHarvesterCrystalProjectile>(), 0, 0f, player.whoAmI, npc);
                }

                return false;
            }

            return player.ownedProjectileCounts[item.shoot] < 1;
        }
    }
}