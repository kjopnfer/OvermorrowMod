using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace OvermorrowMod.Content.Items.Weapons.Magic
{
    public class GlimsporeScepter : ModItem
    {
        public override string Texture => "OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepter";
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepterGlow"), new Vector2(Item.Center.X - Main.screenPosition.X, Item.position.Y - Main.screenPosition.Y + Item.height - Item.height / 2 + 2f), new Rectangle(0, 0, Item.width, Item.height), Color.White, rotation, Item.Size / 2, scale, SpriteEffects.None, 0f);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glimspore Gasser");
            //Item.staff[item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 26;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 30;
            Item.mana = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.reuseDelay = 16;
            Item.shootSpeed = 12;
            Item.shoot = ModContent.ProjectileType<GlimsporeSpore>();
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
        }
        /*public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (Item.staff[Item.type])
            {
                Vector2 Off = new Vector2(Item.width / -2, Item.height / 2) + Item.Size / 2f;
                Vector2 Pos = Main.LocalPlayer.itemLocation - Main.screenPosition;
                bool Left = Main.LocalPlayer.direction < 0;
                if (Left)
                    Off.Y -= Item.height;

                if (Main.LocalPlayer.itemAnimation > 0 && Main.LocalPlayer.HeldItem.type == Item.type)
                    spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepterGlow"), Pos, null, Color.White, (Main.LocalPlayer.itemRotation + MathHelper.ToRadians(45f)) + (Left ? MathHelper.ToRadians(90) : 0), Off, 1, Left ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            }
            else
            {
                Vector2 Off = new Vector2(Item.width/ -4, Item.height / 2);
                Vector2 Pos = Main.LocalPlayer.itemLocation - Main.screenPosition + (Item.Size / 2);
                bool Left = Main.LocalPlayer.direction < 0;

                if (Main.LocalPlayer.itemAnimation > 0 && Main.LocalPlayer.HeldItem.type == Item.type)
                    spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepterGlow"), Pos, null, Color.White, Main.LocalPlayer.itemRotation + (Left ? MathHelper.ToRadians(180) : 0), Off, 1, Left ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
            }
        }*/
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.name.ToLower().Contains("fg"))
                SoundEngine.PlaySound(new SoundStyle($"OvermorrowMod/Sounds/Poof{Main.rand.Next(2)}") { Volume = 1.25f, PitchVariance = 1.1f }, player.Center);
            else
                SoundEngine.PlaySound(new SoundStyle($"OvermorrowMod/Sounds/Fart{Main.rand.Next(3)}"), player.Center);

            for (int i = 0; Main.rand.Next(8, 11) > i; i++)
                Projectile.NewProjectile(null, player.itemLocation + (Item.Size / 2).RotatedBy(MathHelper.ToRadians(-22.5f) + player.itemRotation + ((player.direction < 0) ? MathHelper.ToRadians(Item.staff[Item.type] ? 90 : 170) : 0)), player.velocity + player.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.Pi / 12) * Item.shootSpeed, ModContent.ProjectileType<GlimsporeSpore>(), player.GetWeaponDamage(Item), 0, player.whoAmI);
            return false;
        }
        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Tiles.GlimsporeVines.GlimsporeItem>(), 30);
            recipe.AddIngredient(ModContent.ItemType<Tiles.UVCrystalShards.UVCrystalShardsItem>(), 15);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }*/
    }
    public class GlimsporeScepterGlowmask : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<GlimsporeScepter>();
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D Glowmask = (Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepterGlow");

            Vector2 Position = drawInfo.Center - Vector2.UnitY * drawInfo.mountOffSet - Main.screenPosition - Vector2.UnitY * 9f;
            Position = Position.ToPoint().ToVector2();

            float alpha = (255 - drawInfo.drawPlayer.immuneAlpha) / 255f;
            Color color = Color.White;
            Vector2 origin = drawInfo.headVect;


            bool Left = Main.LocalPlayer.direction < 0;
            Item item = ItemLoader.GetItem(ModContent.ItemType<GlimsporeScepter>()).Item;
            Vector2 Off = new Vector2(item.width / -4, item.height / 2);
            Vector2 Pos = Main.LocalPlayer.itemLocation - Main.screenPosition + (item.Size / 2);
            Pos = Pos.ToPoint().ToVector2();

            /*if (Main.LocalPlayer.itemAnimation > 0 && Main.LocalPlayer.HeldItem.type == Item.type)
                spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("OvermorrowMod/Content/Items/Weapons/Magic/GlimsporeScepter/GlimsporeScepterGlow"), Pos, null, Color.White, Main.LocalPlayer.itemRotation + (Left ? MathHelper.ToRadians(180) : 0), Off, 1, Left ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);*/

            drawInfo.DrawDataCache.Add(new DrawData(Glowmask, Pos, null, color * alpha, Main.LocalPlayer.itemRotation + (Left ? MathHelper.ToRadians(180) : 0), Off, 1f, Left ? SpriteEffects.FlipVertically : SpriteEffects.None, 0));
        }
    }
}