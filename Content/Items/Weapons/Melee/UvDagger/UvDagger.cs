using Terraria;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;
using Terraria.Audio;
using OvermorrowMod.Common.Players;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class UvDagger : ModItem
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/UvDagger/UvDagger";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blinding Blades");
            Tooltip.SetDefault("Throws 5 sharp and magical Tyfloite Taggers in a spread");
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 34;
            Item.damage = 35; //35
            Item.knockBack = 0.5f;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 12;
            Item.useAnimation = 12;
            //item.reuseDelay = 80;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;
        }
        Vector2 ShootPos = Vector2.Zero;
        public static int Thrown = 0;
        public override bool? UseItem(Player player)
        {
            /*Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Blade"), player.Center);
            for (int Rot = -2; 3 > Rot; Rot++)
            {
                Vector2 Pos = player.Center + new Vector2(100, 0).RotatedBy(player.DirectionTo(Main.MouseWorld).ToRotation() + (Rot / 4f));
                    Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<UvDaggerProjectile>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(player.HeldItem, item.knockBack), player.whoAmI);
            }*/
            ShootPos = player.Center;
            //TargetPos = Main.MouseWorld; //want to set here
            ReuseDelayButWorking = 50;
            player.GetModPlayer<OvermorrowModPlayer>().AddScreenShake(5, 4);
            //Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, $"Sounds/Blade{Main.rand.Next(4)}").WithVolume(1f), player.Center);
            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/Blade{Main.rand.Next(4)}") { Volume = 1f }, player.Center);
            return true; 
        }
        float Rot = -2f;
        public override void UpdateInventory(Player player)
        {
            if (ReuseDelayButWorking > 0)
                ReuseDelayButWorking--;
            if (player.HeldItem != Item || player.itemTime == 0)
                Rot = -2f;
        }
        public override void HoldItem(Player player)
        {
            if (player.itemTime != 0)
                player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
            if (player.itemTime % 2 == 0 && player.itemTime != 0)
            {
                //Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Blade"), player.Center);
                Vector2 Pos = ShootPos + new Vector2(100, 0).RotatedBy(player.DirectionTo(Main.MouseWorld).ToRotation() + ((Rot * player.direction) / 4f));
                float ProjRotation = (Pos - ShootPos).ToRotation();
                Projectile.NewProjectile(null, Pos, Vector2.Zero, ModContent.ProjectileType<UvDaggerProjectile>(), player.GetWeaponDamage(player.HeldItem), player.GetWeaponKnockback(Item, Item.knockBack), player.whoAmI, 0, ProjRotation);
                Thrown++;
                Rot++;
            }     
        }
        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Tiles.UVCrystalShards.UVCrystalShardsItem>(), 35);
            recipe.AddIngredient(ModContent.ItemType<Tiles.GlimsporeVines.GlimsporeItem>(), 15);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
        int ReuseDelayButWorking = 0;
        public override bool CanUseItem(Player player)
        {
            return ReuseDelayButWorking == 0;
            /*for (int i = 0; Main.projectile.Length > i; i++)
            {
                if (Main.projectile[i].type == ModContent.ProjectileType<UvDaggerProjectile>() && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                    return false;
            }
            return true;*/
        }
    }
    public class UvDaggerProjectile : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Melee/UvDagger/UvDagger";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }
        float FadeColor = 1f;
        public override void PostDraw(Color lightColor)
        {
            Color color = Color.White * FadeColor;
            if (FadeColor > 0)
                FadeColor -= 0.1f; //0.05f;
            Main.spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Melee/UvDagger/TarterSauce"), Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, Projectile.Size / 2, 1f, SpriteEffects.None, 1f);
        }
        float FloatDir = -2;
        bool PosFloat = false;
        bool FloatingState = true;
        Vector2 TargetPos = Vector2.Zero;
        bool Interscected = false;
        Rectangle TargetIntersectionBox;
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!FloatingState)
            {
                int Slash = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SlashEffect>(), 0, 0, 0, 0, Projectile.rotation - (MathHelper.TwoPi / 4));
                Main.projectile[Slash].Center = Projectile.Center;
                SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/Blade{Main.rand.Next(4)}") { Volume = 1f }, Main.LocalPlayer.Center);
            }
            base.OnHitNPC(target, damage, knockback, crit);
        }
        static int SequenceTimer = 0;
        public override void AI()
        {
            if (Projectile.ai[0] == 0) 
            {
                SequenceTimer = 0;
                Projectile.rotation = Projectile.ai[1] + (MathHelper.TwoPi / 4);
                TargetPos = Main.MouseWorld;
                const int BoxSize = 60;
                TargetIntersectionBox = new Rectangle((int)TargetPos.X - BoxSize, (int)TargetPos.Y - BoxSize, BoxSize * 2, BoxSize * 2);
                if (Main.LocalPlayer.direction == 1)
                    Projectile.spriteDirection = -1;
            }
            if (FloatingState)
            {
                if (PosFloat && FloatDir != 2 && Projectile.ai[0] % 2 == 0)
                    FloatDir += 0.25f;
                if (!PosFloat && FloatDir != -2 && Projectile.ai[0] % 2 == 0)
                    FloatDir -= 0.25f;
                if (Projectile.ai[0] % 32 == 0)
                    PosFloat = !PosFloat;
                FloatDir = MathHelper.Clamp(FloatDir, -2, 0); //remove for pos bob
                Projectile.Center += Projectile.DirectionTo(Projectile.Center + new Vector2(5, 0).RotatedBy(Projectile.ai[1])) * FloatDir;
            }
            else
            {
                Projectile.friendly = true;
                float DistanceToMouse = (float)Math.Sqrt(MathHelper.Distance(Projectile.Center.X, TargetPos.X) + MathHelper.Distance(Projectile.Center.Y, TargetPos.Y));
                if(Projectile.ai[0] <= 50)
                    Projectile.velocity += Projectile.DirectionTo(Projectile.Center + new Vector2(5, 0).RotatedBy(Projectile.ai[1])) * 1.5f;
                    //projectile.velocity = projectile.DirectionTo(projectile.Center + new Vector2(5, 0).RotatedBy(projectile.ai[1])) * 20;
                if (Projectile.ai[0] >= 40)
                    Projectile.tileCollide = true;
                /* no mouse crossover :(
                else if (!Interscected)
                {
                    projectile.velocity += projectile.DirectionTo(TargetPos);
                    projectile.rotation = projectile.velocity.ToRotation() + (MathHelper.TwoPi / 4);
                    if (TargetIntersectionBox.Intersects(projectile.Hitbox))
                        Interscected = true;
                }*/
            }
            Projectile.ai[0]++;
            if (SequenceTimer == 25)
            {
                Projectile.ai[0] = 25;
                FloatingState = false;
            }
            if(Projectile.ai[0] == 35)
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 2.5f /*2.25f*/, Vector2.Zero, ModContent.ProjectileType<Projectiles.RingCloud>(), 0, 0, 0, 0, Projectile.rotation);
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Projectile.type == Main.projectile[i].type && Main.projectile[i].active)
                {
                    if(Main.projectile[i] == Projectile)
                        SequenceTimer++;
                    if(SequenceTimer == 25)
                    {
                        Main.LocalPlayer.GetModPlayer<OvermorrowModPlayer>().AddScreenShake(4, 4);
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ES_TennisRacquet24-SFXProducer") { Volume = 0.75f, PitchVariance = 1.25f }, Projectile.Center);
                    }
                    break;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            //Main.PlaySound(SoundID.Dig, projectile.Center);

            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/CoolShatter") { Volume = 0.15f, MaxInstances = 20 }, Projectile.Center);
            for (int i = 0; 3 > i; i++) 
                Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(22.5f)), ModContent.ProjectileType<SmallUvShard>(), Main.rand.Next(10, 16), 0f, Projectile.owner);
            
            UvDagger.Thrown--;
        }
    }
}