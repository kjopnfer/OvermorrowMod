using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs;
using OvermorrowMod.Projectiles.Artifact;
using OvermorrowMod.Projectiles.Misc;
using OvermorrowMod.Projectiles.Piercing;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass.Weapons.Artifacts
{
    public abstract class Artifact : ModItem
    {
        public override bool CloneNewInstances => true;
        public int soulResourceCost = 0;

        // Toggles the UI if holding an Artifact
        public override void HoldItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            modPlayer.UIToggled = true;
        }

        public virtual void SafeSetDefaults()
        {

        }

        // By making the override sealed, we prevent derived classes from further overriding the method and enforcing the use of SafeSetDefaults()
        // We do this to ensure that the vanilla damage types are always set to false, which makes the custom damage type work
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            // all vanilla damage types must be false for custom damage types to work
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            item.thrown = false;
            item.summon = false;
            item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= soulResourceCost)
            {
                // Putting this in UseItem doesn't do anything for some apparent reason
                if (player.GetModPlayer<WardenRunePlayer>().RuneID == WardenRunePlayer.Runes.BoneRune)
                {
                    int projectiles = 6;
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                    {
                        for (int i = 0; i < projectiles; i++)
                        {
                            Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<Skulls>(), 16, 2, player.whoAmI);
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool UseItem(Player player)
        {
            if (item.type == ModContent.ItemType<CorruptedMirror>())
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    ConsumeSouls(1, player);
                }
                player.AddBuff(ModContent.BuffType<MirrorBuff>(), 10800);

                // Loop through all players and check if they are on the same team
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].team == player.team && player.team != 0)
                        {
                            Main.player[i].AddBuff(ModContent.BuffType<MirrorBuff>(), 10800);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> ProjectileList = new List<int>();
            
            ConsumeSouls(soulResourceCost, player);

            if (item.type == ModContent.ItemType<EaterArtifact>())
            {


                type = ModContent.ProjectileType<WormHead>();
                float numberProjectiles = 3;
                float rotation = MathHelper.ToRadians(30);
                position += Vector2.Normalize(new Vector2(speedX, speedY)) * 30f;

                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                    int projectile = Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
                    ProjectileList.Add(projectile);
                }
            }

            if (item.type == ModContent.ItemType<DemonMonocle>())
            {
                int projectiles = 6;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.myPlayer == player.whoAmI)
                {
                    for (int i = 0; i < projectiles; i++)
                    {
                        int projectile = Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<DemonEye>(), item.damage, 2, player.whoAmI);
                        ProjectileList.Add(projectile);
                    }
                }
            }

            // After spawning in the projectiles, apply the special properties
            if (player.GetModPlayer<WardenRunePlayer>().RuneID != WardenRunePlayer.Runes.None)
            {
                foreach (int projectile in ProjectileList)
                {
                    ((ArtifactProjectile)Main.projectile[projectile].modProjectile).RuneID = player.GetModPlayer<WardenRunePlayer>().RuneID;
                }
            }

            return false;
        }

        // Method used to remove souls from the list and reduce the resource
        protected void ConsumeSouls(int numSouls, Player player)
        {
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (modPlayer.soulResourceCurrent >= numSouls)
            {
                for (int i = 0; i < numSouls; i++)
                {
                    // Get the instance of the first projectile in the list
                    int removeProjectile = modPlayer.soulList[0];

                    // Remove the projectile from the list
                    modPlayer.soulList.RemoveAt(0);
                    modPlayer.soulResourceCurrent--;

                    // Call the projectile's method to kill itself
                    for (int j = 0; j < Main.maxProjectiles; j++) // Loop through the projectile array
                    {
                        // Check that the projectile is the same as the removed projectile and it is active
                        if (Main.projectile[j] == Main.projectile[removeProjectile] && Main.projectile[j].active)
                        {
                            // Kill the projectile
                            Main.projectile[j].Kill();
                            break;
                        }
                    }
                }
            }

            // Sullen Binder set bonus
            if (modPlayer.WaterArmor)
            {
                Vector2 randPos = new Vector2(player.Center.X + Main.rand.Next(-9, 9) * 10, player.Center.Y + Main.rand.Next(-9, 9) * 10);
                Projectile.NewProjectile(randPos, Vector2.Zero, ModContent.ProjectileType<WaterOrbSpawner>(), 0, 0f);
            }
        }
    }
}