using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using OvermorrowMod.Core;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using OvermorrowMod.Content.Items.Weapons.Ranged;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Items.Misc;
using OvermorrowMod.Content.Items;

namespace OvermorrowMod.Common.Players
{
    partial class OvermorrowModPlayer : ModPlayer
    {
        // All accessory booleans are ordered alphabetically
        #region Accessories
        public bool BearTrap;
        public bool CapturedMirage;
        public bool EruditeDamage;
        public bool GuideLantern;
        public bool ImbuementPouch;
        public bool SerpentTooth;
        public bool SickeningSnack;
        public bool SnakeBite;
        public bool PracticeTarget;
        public bool PredatorTalisman;
        #endregion

        #region Accessory Visibility
        public bool BearTrapHide;
        public bool SnakeBiteHide;
        public bool PracticeTargetHide;
        #endregion

        #region Accessory Counters
        public int BearTrapCounter = 0;

        #endregion
        
        public bool atomBuff;
        public bool smolBoi;

        //UV Biome
        public List<UVBubble> UVBubbles = new List<UVBubble>();
        public class UVBubble
        {
            public Vector2 Position;
            public float Radius;
            public UVBubble(Vector2 position, float radius)
            {
                Position = position;
                Radius = radius;
            }
        }

        // Misc
        public int IorichGuardianEnergy = 0;
        public int PlatformTimer = 0;

        public Vector2 AltarCoordinates;
        public bool UIToggled = false;
        public bool StoleArtifact = false;

        public override void ResetEffects()
        {
            BearTrap = false;
            CapturedMirage = false;
            EruditeDamage = false;
            GuideLantern = false;
            ImbuementPouch = false;
            SerpentTooth = false;
            SickeningSnack = false;
            SnakeBite = false;
            PracticeTarget = false;
            PredatorTalisman = false;

            atomBuff = false;
            smolBoi = false;
            // windBuff = false;

            DashShadow = false;

            UVBubbles.Clear();
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            return new[] {
                new Item(ModContent.ItemType<ModBook>()),
                new Item(ModContent.ItemType<TesterBag>()),
            };
        }

        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            foreach (var list in itemsByMod)
            {
                if (list.Key != "OvermorrowMod")
                    list.Value.Clear();
            }
        }

        // Example of how to replace cursor texture to remember for later
        public override void PostUpdateMiscEffects()
        {
            for (int i = 0; i < 6; i++)
            {

                // activate shadre
                if (MathHelper.Clamp(UVBubbles.Count- 1, 0, 5) < i || UVBubbles.Count == 0)
                {
                    if (Main.netMode != NetmodeID.Server && Filters.Scene[$"UVShader{i}"].IsActive())
                    {
                        Filters.Scene[$"UVShader{i}"].GetShader().UseColor(0f, 0f, 0f).UseTargetPosition(Vector2.Zero);
                        Filters.Scene[$"UVShader{i}"].Deactivate();
                    }
                }
            }

            /*if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> emptyTexture = ModContent.Request<Texture2D>(AssetDirectory.Empty);
                Asset<Texture2D> cursor0 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_0");
                Asset<Texture2D> cursor1 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_1");
                Asset<Texture2D> cursor11 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_11");
                Asset<Texture2D> cursor12 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_12");


                if (Player.HeldItem.type == ModContent.ItemType<Farlander>() && Main.mouseRight &&
                    Player.ownedProjectileCounts[ModContent.ProjectileType<Farlander_Scope>()] > 0)
                {
                    TextureAssets.Cursors[0] = emptyTexture;
                    TextureAssets.Cursors[1] = emptyTexture;
                    TextureAssets.Cursors[11] = emptyTexture;
                    TextureAssets.Cursors[12] = emptyTexture;
                }
                else
                {
                    TextureAssets.Cursors[0] = cursor0;
                    TextureAssets.Cursors[1] = cursor1;
                    TextureAssets.Cursors[11] = cursor11;
                    TextureAssets.Cursors[12] = cursor12;
                }
            }*/
        }

        public override void OnEnterWorld(Player player)
        {
            //if (!player.name.ToLower().Contains("frankfires")) // THIS IS SO FUCKING ANNOYTINGhUFHURFHI
            //    OvermorrowModSystem.Instance.ScreenColor.SetDarkness(0, 60, 60, true);

            // Manually apply them because the random reroll doesn't seem to work half the time
            /*int item = Item.NewItem(null, player.Center, ModContent.ItemType<MeleeReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.meleePrefixes[Main.rand.Next(0, ReforgeStone.meleePrefixes.Length)]);

            item = Item.NewItem(null, player.Center, ModContent.ItemType<RangedReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.rangedPrefixes[Main.rand.Next(0, ReforgeStone.rangedPrefixes.Length)]);

            item = Item.NewItem(null, player.Center, ModContent.ItemType<MagicReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.magicPrefixes[Main.rand.Next(0, ReforgeStone.magicPrefixes.Length)]);

            item = Item.NewItem(null, player.Center, ModContent.ItemType<MeleeReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.meleePrefixes[Main.rand.Next(0, ReforgeStone.meleePrefixes.Length)]);

            item = Item.NewItem(null, player.Center, ModContent.ItemType<RangedReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.rangedPrefixes[Main.rand.Next(0, ReforgeStone.rangedPrefixes.Length)]);

            item = Item.NewItem(null, player.Center, ModContent.ItemType<MagicReforge>(), 1, false, -1);
            Main.item[item].Prefix(ReforgeStone.magicPrefixes[Main.rand.Next(0, ReforgeStone.magicPrefixes.Length)]);*/

            base.OnEnterWorld(player);
        }

        private int FindFlaskBuff()
        {
            for (int i = 0; i < Player.buffType.Length; i++)
            {
                switch (Player.buffType[i])
                {
                    case BuffID.WeaponImbueCursedFlames:
                        return BuffID.CursedInferno;
                    case BuffID.WeaponImbueFire:
                        return BuffID.OnFire;
                    case BuffID.WeaponImbueGold:
                        return BuffID.Midas;
                    case BuffID.WeaponImbueIchor:
                        return BuffID.Ichor;
                    case BuffID.WeaponImbueNanites:
                        return BuffID.Confused;
                    case BuffID.WeaponImbuePoison:
                        return BuffID.Poisoned;
                    case BuffID.WeaponImbueVenom:
                        return BuffID.Venom;
                }
            }

            return -1;
        }

        private void ApplyFlaskBuffs(NPC target)
        {
            int buffID = FindFlaskBuff();
            if (buffID != -1)
            {
                target.AddBuff(buffID, 360);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
        {
            if (SnakeBite)
            {
                target.AddBuff(BuffID.Poisoned, 120);

                if (Player.GetArmorPenetration(DamageClass.Generic) + Player.GetArmorPenetration(DamageClass.Melee) > target.defense)
                {
                    for (int i = 0; i < target.buffType.Length; i++)
                    {
                        if (target.buffType[i] == BuffID.Poisoned) target.DelBuff(i);
                    }

                    target.AddBuff(BuffID.Venom, 120);
                }
            }

            if (ImbuementPouch) ApplyFlaskBuffs(target);
        }


        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.DamageType == DamageClass.Ranged)
            {
                if (SickeningSnack)
                {
                    for (int i = 0; i < target.buffTime.Length; i++)
                    {
                        if (target.buffTime[i] < 960)
                            target.buffTime[i] += target.life <= target.lifeMax * 0.5f ? 240 : 120;
                    }

                    if (Main.rand.NextBool(3)) target.AddBuff(ModContent.BuffType<FungalInfection>(), 180);
                }

                if (BearTrap && damageDone >= 70)
                {
                    if (BearTrapCounter < 3)
                    {
                        BearTrapCounter++;

                        Vector2 offset = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 72;
                        Projectile.NewProjectile(null, Player.Center + offset, Vector2.Zero, ModContent.ProjectileType<Content.Items.Accessories.BearTrap.BearTrapIcon>(), 0, 0f, Player.whoAmI, 0f);
                    }
                }
            }

            if (ImbuementPouch) ApplyFlaskBuffs(target);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {

        }

        public override void PreUpdate()
        {
            PlatformTimer--;
        }

        public override void PostUpdate()
        {

        }

        public bool DashShadow = false;
        public override void PostUpdateEquips()
        {
            if (DashShadow)
            {
                Player.eocDash = 50;
                Player.armorEffectDrawShadowEOCShield = true;
            }
        }

        private bool CheckOnGround()
        {
            Tile leftTile = Framing.GetTileSafely(Player.Hitbox.BottomLeft());
            Tile rightTile = Framing.GetTileSafely(Player.Hitbox.BottomRight());
            if (leftTile.HasTile && Main.tileSolid[leftTile.TileType] && rightTile.HasTile && Main.tileSolid[rightTile.TileType])
            {
                return true;
            }

            return false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (OvermorrowModFile.BearTrapKey.JustPressed && BearTrap && BearTrapCounter > 0 && CheckOnGround())
            {
                Main.NewText(CheckOnGround());
                BearTrapCounter--;

                Projectile.NewProjectile(new EntitySource_Misc("PlayerTrap"), Player.Center, Vector2.Zero, ModContent.ProjectileType<Content.Items.Accessories.BearTrap.PlacedBearTrap>(), 0, 0f, Player.whoAmI, 0f);

                //Main.NewText("pressed bear trap");
            }

            base.ProcessTriggers(triggersSet);
        }

        private bool IsInRange(Vector2 coordinates)
        {
            float distance = Vector2.Distance(coordinates, Player.Center);
            if (distance <= 80)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
