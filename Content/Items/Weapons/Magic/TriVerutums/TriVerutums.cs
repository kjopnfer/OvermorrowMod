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
        public const int MaxCoolThings = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tri-Verutums");
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
            Projectile NewProj = new Projectile();
            Projectile ClosestCoolProjectile = NewProj;
            for (int i = 0; ReadyCoolProjectiles.Count > i; i++)
            {
                if (CloseEnoughCoolProjectiles.Contains(ReadyCoolProjectiles[i]))
                {
                    ClosestCoolProjectile = Main.projectile[ReadyCoolProjectiles[i]];
                    break;
                }
            }

            for (int i = 0; CoolProjectiles.Count > i; i++)
            {
                if (Main.projectile[CoolProjectiles[i]] == ClosestCoolProjectile)
                {
                    ReadyCoolProjectiles.Remove(CoolProjectiles[i]);
                    Main.projectile[CoolProjectiles[i]].tileCollide = true;
                    Main.projectile[CoolProjectiles[i]].friendly = true;
                    Main.projectile[CoolProjectiles[i]].velocity = Main.projectile[CoolProjectiles[i]].DirectionTo(Main.MouseWorld) * 20;
                    Main.projectile[CoolProjectiles[i]].ai[1] = 1;
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

        public override bool CanUseItem(Player player)
        {
            return CloseEnoughCoolProjectiles.Count != 0 && ReadyCoolProjectiles.Count != 0;
        }

        double distance = 0;
        double rot = 0.0;
        double RotAdd = 0.0;
        float ProjRotationAdd = 0.0f;
        int RotDirection = 1;
        Vector2 CoolPosOffset = Vector2.Zero;
        public static List<int> CoolProjectiles = new List<int>();
        public static List<int> ReadyCoolProjectiles = new List<int>();
        List<int> CloseEnoughCoolProjectiles = new List<int>();
        int ItemHoldTime = 0;
        public override void HoldItem(Player player)
        {
            if (ItemHoldTime == 0)
            {
                ReadyCoolProjectiles.Clear();
                CoolProjectiles.Clear();
            }
            player.direction = (Main.MouseWorld.X > player.Center.X) ? 1 : -1;
            if (distance < 120)
            {
                if (distance < 90)
                    distance += 4;
                else
                {
                    if (distance < 110)
                        distance += 2;
                    else
                        distance += 1;
                }
            }

            if (CoolProjectiles.Count != MaxCoolThings)
            {
                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/GamerMagicDrawingSound")
                {
                    Volume = 1.25f,
                }, player.Center);

                for (int i = 0; MaxCoolThings > i; i++)
                {
                    int proj = Projectile.NewProjectile(null, Main.LocalPlayer.Center, Vector2.Zero, ModContent.ProjectileType<CoolProjectile>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(player.HeldItem, 1f), player.whoAmI, 0, 0);
                    CoolProjectiles.Add(proj);
                }
            }

            CloseEnoughCoolProjectiles.Clear();
            for (int i = 0; CoolProjectiles.Count > i; i++)
            {
                if (ReadyCoolProjectiles.Contains(CoolProjectiles[i]))
                {
                    if (RotAdd < MathHelper.TwoPi)
                        RotAdd += 0.005;
                    else
                        RotAdd = 0.0;
                    rot = 0.0;
                    rot += ((double)MathHelper.TwoPi / MaxCoolThings) * (i + 1);
                    rot += RotAdd;

                    int Xpos = (int)(player.Center.X + Math.Cos(rot * player.direction) * distance * player.direction);
                    int Ypos = (int)(player.Center.Y + Math.Sin(rot * player.direction) * distance * player.direction);
                    Vector2 CoolPosition = new Vector2(Xpos, Ypos);
                    CoolPosition = new Vector2(Xpos + (float)Math.Cos(rot * 8) * 10, Ypos + (float)Math.Sin(rot * 8) * 10);
                    ProjRotationAdd = MathHelper.Clamp(ProjRotationAdd, -15f, 15f);
                    if (Math.Pow(Main.projectile[CoolProjectiles[i]].Center.X - player.Center.X, 2) + Math.Pow(Main.projectile[CoolProjectiles[i]].Center.Y - player.Center.Y, 2) < 70000)
                        CloseEnoughCoolProjectiles.Add(i);
                    Main.projectile[CoolProjectiles[i]].hostile = false;
                    Main.projectile[CoolProjectiles[i]].friendly = false;
                    Main.projectile[CoolProjectiles[i]].tileCollide = false;
                    Main.projectile[CoolProjectiles[i]].velocity = ((CoolPosition + CoolPosOffset) - Main.projectile[CoolProjectiles[i]].Center) / 10;
                    Main.projectile[CoolProjectiles[i]].rotation = Main.projectile[CoolProjectiles[i]].DirectionTo(Main.MouseWorld).ToRotation() + (MathHelper.TwoPi / 4) + (ProjRotationAdd * RotDirection);
                }
            }

            ItemHoldTime++;
        }
        public override void UpdateInventory(Player player)
        {
            if (player.HeldItem.type != Item.type)
            {
                ItemHoldTime = 0;
                rot = 0;
                for (int i = 0; Main.projectile.Length > i; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<CoolProjectile>() && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                        Main.projectile[i].Kill();
                }

                distance = 0;
            }
        }
    }
}