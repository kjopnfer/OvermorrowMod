using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;

namespace OvermorrowMod.Common.Players
{
    partial class OvermorrowModPlayer : ModPlayer
    {
        // All accessory booleans are ordered alphabetically
        #region Accessories
        public bool EruditeDamage;
        public bool SerpentTooth;
        public bool SickeningSnack;
        public bool SnakeBite;
        public bool PredatorTalisman;
        #endregion

        #region Accessory Visibility
        public bool SnakeBiteHide;
        #endregion

        // Buffs
        public bool atomBuff;
        public bool smolBoi;

        public Vector2 AltarCoordinates;
        public bool UIToggled = false;
        public bool StoleArtifact = false;

        public int PlatformTimer = 0;

        public override void ResetEffects()
        {
            EruditeDamage = false;
            SerpentTooth = false;
            SickeningSnack = false;
            SnakeBite = false;
            PredatorTalisman = false;

            atomBuff = false;
            smolBoi = false;
        }

        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            foreach (var list in itemsByMod)
            {
                list.Value.Clear();
            }
        }

        // Example of how to replace cursor texture to remember for later
        public override void PostUpdateMiscEffects()
        {
            /*if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                Asset<Texture2D> emptyTex = ModContent.Request<Texture2D>(AssetDirectory.Empty);
                Asset<Texture2D> cursor0 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_0");
                Asset<Texture2D> cursor1 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_1");
                Asset<Texture2D> cursor11 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_11");
                Asset<Texture2D> cursor12 = ModContent.Request<Texture2D>("Terraria/Images/UI/Cursor_12");

                if (OvermorrowModSystem.Instance.ScreenColor.IsVisible())
                {
                    TextureAssets.Cursors[0] = emptyTex;
                    TextureAssets.Cursors[1] = emptyTex;
                    TextureAssets.Cursors[11] = emptyTex;
                    TextureAssets.Cursors[12] = emptyTex;
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

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
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
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
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
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {

        }

        public override void PreUpdate()
        {
            PlatformTimer--;
        }

        public override void PostUpdate()
        {

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
