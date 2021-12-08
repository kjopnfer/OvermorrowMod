using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs;
using OvermorrowMod.Buffs.Hexes;
using OvermorrowMod.Particles;
using OvermorrowMod.Projectiles.Artifact;
using OvermorrowMod.Projectiles.Artifact.DarkPortal;
using OvermorrowMod.Projectiles.Misc;
using OvermorrowMod.Projectiles.Piercing;
using System;
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
                if (item.type != ModContent.ItemType<HoneyPot>())
                {
                    ConsumeSouls(soulResourceCost, player);
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
            var modPlayer = WardenDamagePlayer.ModPlayer(player);
            if (modPlayer.soulResourceCurrent >= soulResourceCost)
            {
                if (item.type == ModContent.ItemType<HoneyPot>())
                {
                    int consumedSouls = modPlayer.soulResourceCurrent;

                    if (consumedSouls > 0)
                    {

                        //Main.NewText(modPlayer.soulResourceCurrent);

                        player.statLife += 10 * consumedSouls;
                        player.HealEffect(10 * consumedSouls);

                        // Loop through all players and check if they are on the same team
                        if (Main.netMode == NetmodeID.MultiplayerClient && player.team != 0)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                Player client = Main.player[i];

                                if (client.active && !client.dead && client.team == player.team && client.whoAmI != player.whoAmI)
                                {
                                    client.AddBuff(BuffID.Honey, 3600);

                                    client.statLife += 10 * consumedSouls;
                                    client.HealEffect(10 * consumedSouls);

                                    //Main.NewText(client.name);
                                }
                            }
                        }

                        ConsumeSouls(consumedSouls, player);
                    }
                }
            }

            if (item.type == ModContent.ItemType<CorruptedMirror>())
            {
                player.AddBuff(ModContent.BuffType<MirrorBuff>(), 10800);

                // Loop through all players and check if they are on the same team
                if (Main.netMode == NetmodeID.MultiplayerClient)
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
                player.AddBuff(BuffID.Honey, 3600);
            }
            else if (item.type == ModContent.ItemType<SlimeArtifact>())
            {
                player.AddBuff(ModContent.BuffType<SlimeBuff>(), 10800);
            }

            // Apply additional buffs while a rune is active
            #region Runes
            if (item.type == ModContent.ItemType<CorruptedMirror>() || item.type == ModContent.ItemType<HoneyPot>() || item.type == ModContent.ItemType<SlimeArtifact>())
            {
                switch (player.GetModPlayer<WardenRunePlayer>().RuneID)
                {
                    case WardenRunePlayer.Runes.SkyRune:
                        player.AddBuff(ModContent.BuffType<LightningCloud>(), defBuffDuration);

                        if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldCloud>()] < 1)
                        {
                            Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<GoldCloud>(), 20, 6f, player.whoAmI, 0f, 0f);
                        }

                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (Main.player[i].team == player.team && player.team != 0)
                            {
                                Main.player[i].AddBuff(ModContent.BuffType<LightningCloud>(), defBuffDuration);
                                if (Main.player[i].ownedProjectileCounts[ModContent.ProjectileType<GoldCloud>()] < 1)
                                {
                                    Projectile.NewProjectile(player.Center + new Vector2(0, -50), Vector2.Zero, ModContent.ProjectileType<GoldCloud>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                }
                            }
                        }
                        break;
                    case WardenRunePlayer.Runes.HellRune:
                        player.AddBuff(ModContent.BuffType<ExplosionBuff>(), defBuffDuration);

                        if (Main.netMode == NetmodeID.MultiplayerClient)
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
                        // Remove mushroom summons if they are active, also removes from friendly teammates
                        #region Mushroom Replacement
                        for (int i = 0; i < player.CountBuffs(); i++)
                        {
                            if (player.buffType[i] == ModContent.BuffType<ShroomBuff>())
                            {
                                player.DelBuff(i);
                            }
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    for (int ii = 0; ii < player.CountBuffs(); ii++)
                                    {
                                        if (Main.player[i].buffType[ii] == ModContent.BuffType<ShroomBuff>())
                                        {
                                            Main.player[i].DelBuff(i);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Effect Application
                        player.AddBuff(ModContent.BuffType<VineBuff>(), defBuffDuration);

                        if (player.ownedProjectileCounts[ModContent.ProjectileType<StabberVine>()] < 3)
                        {
                            Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, -1f);
                            Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, player.whoAmI, 0f, 1f);
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    if (Main.player[i].ownedProjectileCounts[ModContent.ProjectileType<StabberVine>()] < 3)
                                    {

                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, -1f);
                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<StabberVine>(), 20, 6f, Main.player[i].whoAmI, 0f, 1f);
                                    }

                                    Main.player[i].AddBuff(ModContent.BuffType<VineBuff>(), defBuffDuration);
                                }
                            }
                        }
                        #endregion
                        break;
                    case WardenRunePlayer.Runes.MushroomRune:
                        // Remove vine summons if they are active, also removes from friendly teammates
                        #region Vine Replacement
                        for (int i = 0; i < player.CountBuffs(); i++)
                        {
                            if (player.buffType[i] == ModContent.BuffType<VineBuff>())
                            {
                                player.DelBuff(i);
                            }
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    for (int ii = 0; ii < player.CountBuffs(); ii++)
                                    {
                                        if (Main.player[i].buffType[ii] == ModContent.BuffType<VineBuff>())
                                        {
                                            Main.player[i].DelBuff(i);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Effect Application
                        player.AddBuff(ModContent.BuffType<ShroomBuff>(), defBuffDuration);

                        if (player.ownedProjectileCounts[ModContent.ProjectileType<FungiHead>()] < 3)
                        {
                            Projectile.NewProjectile(player.Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(player.Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, -1f);
                            Projectile.NewProjectile(player.Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, player.whoAmI, 0f, 1f);
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.player[i].team == player.team && player.team != 0)
                                {
                                    if (Main.player[i].ownedProjectileCounts[ModContent.ProjectileType<FungiHead>()] < 3)
                                    {

                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(0, -100), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, 0f);
                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(-25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, -1f);
                                        Projectile.NewProjectile(Main.player[i].Center + new Vector2(25, -50), Vector2.Zero, ModContent.ProjectileType<FungiHead>(), 20, 6f, Main.player[i].whoAmI, 0f, 1f);
                                    }

                                    Main.player[i].AddBuff(ModContent.BuffType<ShroomBuff>(), defBuffDuration);
                                }
                            }
                        }
                        #endregion
                        break;
                    case WardenRunePlayer.Runes.BoneRune:
                        // This doesn't get called in the shoot hook for Support, so I put it in here again
                        int projectiles = 6;
                        int randRotation = Main.rand.Next(24) * 15; // Uhhh, random degrees in increments of 15
                        for (int i = 0; i < projectiles; i++)
                        {
                            Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + randRotation)), ModContent.ProjectileType<Skulls>(), 16, 2, player.whoAmI);
                        }

                        break;
                    case WardenRunePlayer.Runes.DefaultRune:
                        // Rotation for the hour hand
                        double deg = Math.Abs(player.GetModPlayer<WardenRunePlayer>().rotateCounter * 0.8 * MathHelper.Lerp(1, 4, (float)(!player.GetModPlayer<WardenRunePlayer>().runeDeactivate ? player.GetModPlayer<WardenRunePlayer>().runeCounter / 300.0 : 1)) % 720) * 0.5f;

                        // The 90th degree is when both hands cross
                        if (deg >= 80 && deg <= 95) // Error margin
                        {
                            //Main.NewText("hit: " + deg);
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), player.Center, Vector2.Zero, Color.Goldenrod, 1, 2f, 0, 1f);
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                float distance = Vector2.Distance(Main.projectile[i].Center, Main.LocalPlayer.Center);
                                if (distance <= 390)
                                {
                                    Main.projectile[i].GetGlobalProjectile<OvermorrowGlobalProjectile>().slowedTime = true;
                                }
                            }

                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                float distance = Vector2.Distance(Main.npc[i].Center, Main.LocalPlayer.Center);
                                if (distance <= 390 && !Main.npc[i].boss)
                                {
                                    Main.npc[i].AddHex(Hex.HexType<TimeSlow>(), 60 * 8);
                                }
                            }
                        }
                        else
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), player.Center, Vector2.Zero, Color.Gray, 1, 2f, 0, 1f);
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                float distance = Vector2.Distance(Main.projectile[i].Center, Main.LocalPlayer.Center);
                                if (distance <= 390)
                                {
                                    Main.projectile[i].GetGlobalProjectile<OvermorrowGlobalProjectile>().slowedTime = true;
                                }
                            }
                        }
                        break;
                }
                return true;
            }
            #endregion
            return false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            List<int> ProjectileList = new List<int>();

            #region Attack/Power
            if (item.type == ModContent.ItemType<TorchGod>())
            {
                int projectile = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0);
                /*int numberProjectiles = 2 + Main.rand.Next(2); 
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(30));
                    float scale = 1f - (Main.rand.NextFloat() * .3f);
                    perturbedSpeed = perturbedSpeed * scale;

                    ProjectileList.Add(Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<SmallBlueFire>(), damage / numberProjectiles, knockBack, player.whoAmI));
                }*/

                ProjectileList.Add(projectile);
            }

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
                for (int i = 0; i < projectiles; i++)
                {
                    int projectile = Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<DemonEye>(), item.damage, 2, player.whoAmI);
                    ProjectileList.Add(projectile);
                }
            }

            if (item.type == ModContent.ItemType<DarkPearl>())
            {
                // Kill projectile instance to allow only one at a time
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DarkPortal>()] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                            (Main.projectile[i].type == ModContent.ProjectileType<DarkPortal>()))
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                }

                position = Main.MouseWorld;

                int projectile = Projectile.NewProjectile(position, Vector2.Zero, type, 0, 0, player.whoAmI);
                Main.projectile[projectile].spriteDirection = player.direction;

                ProjectileList.Add(projectile);
            }
            #endregion

            #region Support/Wisdom
            if (item.type == ModContent.ItemType<EarthCrystal>() || item.type == ModContent.ItemType<BloodyAntikythera>() || item.type == ModContent.ItemType<PillarArtifact>())
            {
                // Allow only one instance of the projectile
                if (player.ownedProjectileCounts[ModContent.ProjectileType<RedCloud>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<WorldTree>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<Pillar>()] > 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI &&
                            (Main.projectile[i].type == ModContent.ProjectileType<RedCloud>() || Main.projectile[i].type == ModContent.ProjectileType<WorldTree>() || Main.projectile[i].type == ModContent.ProjectileType<Pillar>()))
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
            #endregion

            #region Properties
            // After spawning in the projectiles, apply the special properties
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

                    if (item.type == ModContent.ItemType<PillarArtifact>())
                    {
                        ((ArtifactProjectile)Main.projectile[projectile].modProjectile).AuraRadius = 330;
                    }
                }
            }

            // This thing is for when you use Attack/Aura Artifacts because the Shoot hook is called, I guess
            switch (player.GetModPlayer<WardenRunePlayer>().RuneID)
            {
                case WardenRunePlayer.Runes.BoneRune:
                    int projectiles = 6;
                    int randRotation = Main.rand.Next(24) * 15; // Uhhh, random degrees in increments of 15
                    for (int i = 0; i < projectiles; i++)
                    {
                        Projectile.NewProjectile(player.Center, new Vector2(4).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + randRotation)), ModContent.ProjectileType<Skulls>(), 16, 2, player.whoAmI);
                    }

                    break;
                case WardenRunePlayer.Runes.DefaultRune:
                    // Rotation for the hour hand
                    double deg = Math.Abs(player.GetModPlayer<WardenRunePlayer>().rotateCounter * 0.8 * MathHelper.Lerp(1, 4, (float)(!player.GetModPlayer<WardenRunePlayer>().runeDeactivate ? player.GetModPlayer<WardenRunePlayer>().runeCounter / 300.0 : 1)) % 720) * 0.5f;

                    // The 90th degree is when both hands cross
                    if (deg >= 80 && deg <= 95) // Error margin
                    {
                        //Main.NewText("hit: " + deg);
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), player.Center, Vector2.Zero, Color.Goldenrod, 1, 2f, 0, 1f);

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            float distance = Vector2.Distance(Main.projectile[i].Center, Main.LocalPlayer.Center);
                            if (distance <= 390)
                            {
                                Main.projectile[i].GetGlobalProjectile<OvermorrowGlobalProjectile>().slowedTime = true;
                            }
                        }

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            float distance = Vector2.Distance(Main.npc[i].Center, Main.LocalPlayer.Center);
                            if (distance <= 390 && !Main.npc[i].boss)
                            {
                                Main.npc[i].AddHex(Hex.HexType<TimeSlow>(), 60 * 8);
                            }
                        }
                    }
                    else
                    {
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), player.Center, Vector2.Zero, Color.Gray, 1, 2f, 0, 1f);
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            float distance = Vector2.Distance(Main.projectile[i].Center, Main.LocalPlayer.Center);
                            if (distance <= 390)
                            {
                                Main.projectile[i].GetGlobalProjectile<OvermorrowGlobalProjectile>().slowedTime = true;
                            }
                        }
                    }
                    break;
            }

            #endregion
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