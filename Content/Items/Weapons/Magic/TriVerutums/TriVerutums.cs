using System;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Audio;

namespace OvermorrowMod.Content.Items.Weapons.Magic.TriVerutums
{
    public class TriVerutums : ModItem
    {
        public const int MAX_PROJECTILES = 3;

        double distance = 0;
        double rotationCounter = 0.0;
        double RotAdd = 0.0;
        float ProjRotationAdd = 0.0f;
        int RotDirection = 1;
        Vector2 PositionOffset = Vector2.Zero;
        public static List<int> StoredProjectiles = new List<int>();
        public static List<int> ReadyProjectiles = new List<int>();
        List<int> CloseEnoughProjectiles = new List<int>();
        int ItemHoldTime = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tri-Veruta");
            Tooltip.SetDefault("Summons three magical Verutums to aid you in combat");
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.damage = 15;
            Item.mana = 14;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 10;
        }

        public override bool? UseItem(Player player)
        {
            // Get the instance of the closest projectile from the list
            Projectile ClosestCoolProjectile = new Projectile();
            for (int i = 0; ReadyProjectiles.Count > i; i++)
            {
                if (CloseEnoughProjectiles.Contains(ReadyProjectiles[i]))
                {
                    ClosestCoolProjectile = Main.projectile[ReadyProjectiles[i]];
                    break;
                }
            }

            for (int i = 0; StoredProjectiles.Count > i; i++)
            {
                if (Main.projectile[StoredProjectiles[i]] == ClosestCoolProjectile)
                {
                    ReadyProjectiles.Remove(StoredProjectiles[i]);
                    Main.projectile[StoredProjectiles[i]].tileCollide = true;
                    Main.projectile[StoredProjectiles[i]].friendly = true;
                    Main.projectile[StoredProjectiles[i]].velocity = Main.projectile[StoredProjectiles[i]].DirectionTo(Main.MouseWorld) * 20;
                    Main.projectile[StoredProjectiles[i]].ai[1] = 1;
                }
            }

            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/Woosh{Main.rand.Next(2, 4)}")
            {
                Volume = 1.25f,
                PitchVariance = 1.1f,
                MaxInstances = 3,
            }, player.Center);

            return true;
        }

        // Allow the player to fire the projectile if there are active projectiles to throw
        public override bool CanUseItem(Player player) => CloseEnoughProjectiles.Count != 0 && ReadyProjectiles.Count != 0;
        public override void HoldItem(Player player)
        {
            // Clear the lists when the player has started holding the item
            if (ItemHoldTime == 0)
            {
                ReadyProjectiles.Clear();
                StoredProjectiles.Clear();
            }

            player.direction = (Main.MouseWorld.X > player.Center.X) ? 1 : -1;

            if (distance < 120)
            {
                if (distance < 90)
                    distance += 4;
                else
                {
                    if (distance < 110) distance += 2;
                    else distance += 1;
                }
            }

            if (StoredProjectiles.Count != MAX_PROJECTILES)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/GamerMagicDrawingSound")
                {
                    Volume = 1.25f,
                }, player.Center);

                for (int i = 0; MAX_PROJECTILES > i; i++)
                {
                    int proj = Projectile.NewProjectile(null, Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<VerutumProjectile>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(player.HeldItem, 1f), player.whoAmI, 0, 0);
                    StoredProjectiles.Add(proj);
                }
            }

            CloseEnoughProjectiles.Clear();

            #region Idle
            for (int i = 0; StoredProjectiles.Count > i; i++)
            {
                if (ReadyProjectiles.Contains(StoredProjectiles[i]))
                {
                    if (RotAdd < MathHelper.TwoPi) RotAdd += 0.005;
                    else RotAdd = 0.0;

                    rotationCounter = 0.0;
                    rotationCounter += ((double)MathHelper.TwoPi / MAX_PROJECTILES) * (i + 1);
                    rotationCounter += RotAdd;

                    int Xpos = (int)(player.Center.X + Math.Cos(rotationCounter * player.direction) * distance * player.direction);
                    int Ypos = (int)(player.Center.Y + Math.Sin(rotationCounter * player.direction) * distance * player.direction);
                    Vector2 CoolPosition = new Vector2(Xpos, Ypos);
                    CoolPosition = new Vector2(Xpos + (float)Math.Cos(rotationCounter * 8) * 10, Ypos + (float)Math.Sin(rotationCounter * 8) * 10);
                    ProjRotationAdd = MathHelper.Clamp(ProjRotationAdd, -15f, 15f);

                    if (Main.projectile[StoredProjectiles[i]].Distance(player.Center) < 70000)
                        CloseEnoughProjectiles.Add(i);

                    Main.projectile[StoredProjectiles[i]].hostile = false;
                    Main.projectile[StoredProjectiles[i]].friendly = false;
                    Main.projectile[StoredProjectiles[i]].tileCollide = false;
                    Main.projectile[StoredProjectiles[i]].velocity = ((CoolPosition + PositionOffset) - Main.projectile[StoredProjectiles[i]].Center) / 10;
                    Main.projectile[StoredProjectiles[i]].rotation = Main.projectile[StoredProjectiles[i]].DirectionTo(Main.MouseWorld).ToRotation() + (MathHelper.TwoPi / 4) + (ProjRotationAdd * RotDirection);
                }
            }
            #endregion

            ItemHoldTime++;
        }
        public override void UpdateInventory(Player player)
        {
            // Reset variables and remove the projectiles if the player isn't holding this weapon
            if (player.HeldItem.type != Item.type)
            {
                ItemHoldTime = 0;
                rotationCounter = 0;
                for (int i = 0; Main.projectile.Length > i; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<VerutumProjectile>() && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                        Main.projectile[i].Kill();
                }

                distance = 0;
            }
        }
    }
}