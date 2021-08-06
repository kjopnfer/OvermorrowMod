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
        public int defBuffDuration;

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
                        int randRotation = Main.rand.Next(24) * 15; // Uhhh, random degrees in increments of 15
                        for (int i = 0; i < projectiles; i++)
                        {
                            Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + randRotation)), ModContent.ProjectileType<Skulls>(), 16, 2, player.whoAmI);
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
            }
            else if (item.type == ModContent.ItemType<HoneyPot>())
            {
                int consumedSouls = 0;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    var modPlayer = WardenDamagePlayer.ModPlayer(player);
                    int soulCount = modPlayer.soulResourceCurrent;
                    for (int i = 0; i < soulCount; i++)
                    {
                        consumedSouls++;
                    }

                    player.statLife += 10 * consumedSouls;
                    player.HealEffect(10 * consumedSouls);

                    ConsumeSouls(consumedSouls, player);
                }

                player.AddBuff(BuffID.Honey, 3600);

                // Loop through all players and check if they are on the same team
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].team == player.team && player.team != 0)
                        {
                            Main.player[i].AddBuff(BuffID.Honey, 3600);
                            Main.player[i].statLife += 10 * consumedSouls;
                            Main.player[i].HealEffect(10 * consumedSouls);
                        }
                    }
                }
            }

            // Apply additional buffs while a rune is active
            if (item.type == ModContent.ItemType<CorruptedMirror>() || item.type == ModContent.ItemType<HoneyPot>())
            {
                switch (player.GetModPlayer<WardenRunePlayer>().RuneID)
                {
                    case WardenRunePlayer.Runes.SkyRune:
                        player.AddBuff(ModContent.BuffType<LightningCloud>(), defBuffDuration);
                        Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<GoldCloud>(), 20, 6f, player.whoAmI, 0f, 0f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    Main.player[i].AddBuff(ModContent.BuffType<LightningCloud>(), defBuffDuration);
                                    Projectile.NewProjectile(player.Center + new Vector2(0, -50), Vector2.Zero, ModContent.ProjectileType<GoldCloud>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                }
                            }
                        }
                        break;
                    case WardenRunePlayer.Runes.HellRune:
                        player.AddBuff(ModContent.BuffType<ExplosionBuff>(), defBuffDuration);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    Main.player[i].AddBuff(ModContent.BuffType<ExplosionBuff>(), defBuffDuration);
                                }
                            }
                        }
                        break;
                    case WardenRunePlayer.Runes.JungleRune:
                        player.AddBuff(ModContent.BuffType<VineBuff>(), defBuffDuration);
                        Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, 0f);
                        Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, -1f);
                        Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, 1f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                    Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, -1f);
                                    Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, 1f);
                                    Main.player[i].AddBuff(ModContent.BuffType<VineBuff>(), defBuffDuration);
                                }
                            }
                        }
                        break;
                    case WardenRunePlayer.Runes.MushroomRune:
                        player.AddBuff(ModContent.BuffType<ShroomBuff>(), defBuffDuration);
                        Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, 0f);
                        Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, -1f);
                        Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, 1f);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                    Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, -1f);
                                    Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, 1f);
                                    Main.player[i].AddBuff(ModContent.BuffType<ShroomBuff>(), defBuffDuration);
                                }
                            }
                        }
                        break;
                }
                return true;
            }

            return false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> ProjectileList = new List<int>();

            ConsumeSouls(soulResourceCost, player);

            // Attack
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

            // Support
            if (item.type == ModContent.ItemType<EarthCrystal>() || item.type == ModContent.ItemType<BloodyAntikythera>())
            {
                // Allow only one instance of the projectile
                if (player.ownedProjectileCounts[ModContent.ProjectileType<RedCloud>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<WorldTree>()] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                            (Main.projectile[i].type == ModContent.ProjectileType<RedCloud>() || Main.projectile[i].type == ModContent.ProjectileType<WorldTree>()))
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                    position = Main.MouseWorld;

                    int projectile = Projectile.NewProjectile(position, Vector2.Zero, type, 0, 0, player.whoAmI);
                    ProjectileList.Add(projectile);
                }
                else
                {
                    position = Main.MouseWorld;
                    int projectile = Projectile.NewProjectile(position, Vector2.Zero, type, 0, 0, player.whoAmI);
                    ProjectileList.Add(projectile);
                }
            }

            // After spawning in the projectiles, apply the special properties
            if (player.GetModPlayer<WardenRunePlayer>().RuneID != WardenRunePlayer.Runes.None)
            {
                foreach (int projectile in ProjectileList)
                {
                    if (Main.projectile[projectile].active)
                    {
                        // Pass Rune ID to the projectile
                        ((ArtifactProjectile)Main.projectile[projectile].modProjectile).RuneID = player.GetModPlayer<WardenRunePlayer>().RuneID;

                        // Set radius if it is a Support Artifact
                        if (item.type == ModContent.ItemType<EarthCrystal>())
                        {
                            ((ArtifactProjectile)Main.projectile[projectile].modProjectile).AuraRadius = 330;
                        }

                        if (item.type == ModContent.ItemType<BloodyAntikythera>())
                        {
                            ((ArtifactProjectile)Main.projectile[projectile].modProjectile).AuraRadius = 390;
                        }
                    }
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