using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 17;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 58;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<IorichHarvesterProjectile>();
            Item.shootSpeed = 10f; //8f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (UsageCounter++ % 2 == 0)
            {
                Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<IorichHarvesterProjectile>(), damage, 0f, player.whoAmI, 120f, 1f);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IorichHarvesterSpinner>()] <= 0)
            {
                int RADIUS = 65;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 SpawnLocation = new Vector2(RADIUS, 0).RotatedBy(MathHelper.Pi * i);
                    int proj = Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center + SpawnLocation, Vector2.Zero, ModContent.ProjectileType<IorichHarvesterSpinner>(), 0, 0f, player.whoAmI, MathHelper.Pi * i, RADIUS);
                    ((IorichHarvesterSpinner)Main.projectile[proj].ModProjectile).RotationCenter = player;
                }
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount >= 3 && !PlaySound)
            {
                SoundEngine.PlaySound(SoundID.Item4, player.Center);
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

                    int npc = NPC.NewNPC(null, (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<IorichHarvesterCrystal>(), 0, 0f, player.whoAmI);
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<IorichHarvesterCrystalProjectile>(), 0, 0f, player.whoAmI, npc);
                }

                return false;
            }

            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}