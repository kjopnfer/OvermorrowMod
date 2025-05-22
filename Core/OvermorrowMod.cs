using System;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Content.Skies;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Detours;
using Terraria.GameContent;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core
{
	public partial class OvermorrowModFile : Mod
	{
        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;

        private void ReplaceVanillaTextures()
        {
            //if (!ModContent.GetInstance<TextureSwapConfig>().ReplaceTextures) return;

            TextureAssets.Item[ItemID.Boomstick] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Boomstick");
            TextureAssets.Item[ItemID.Handgun] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Handgun");
            TextureAssets.Item[ItemID.Musket] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Musket");
            TextureAssets.Item[ItemID.Minishark] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Minishark");
            TextureAssets.Item[ItemID.PhoenixBlaster] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "PhoenixBlaster");
            TextureAssets.Item[ItemID.QuadBarrelShotgun] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "QuadBarrel");
            TextureAssets.Item[ItemID.Revolver] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Revolver");
            TextureAssets.Item[ItemID.TheUndertaker] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Undertaker");
        }

        private void UnloadVanillaTextures()
        {
            //if (!ModContent.GetInstance<TextureSwapConfig>().ReplaceTextures) return;

            TextureAssets.Item[ItemID.Boomstick] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.Boomstick);
            TextureAssets.Item[ItemID.ChainKnife] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.ChainKnife);
            TextureAssets.Item[ItemID.Handgun] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.Handgun);
            TextureAssets.Item[ItemID.Musket] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.Musket);
            TextureAssets.Item[ItemID.Minishark] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.Minishark);
            TextureAssets.Item[ItemID.PhoenixBlaster] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PhoenixBlaster);
            TextureAssets.Item[ItemID.QuadBarrelShotgun] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.QuadBarrelShotgun);
            TextureAssets.Item[ItemID.Revolver] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.Revolver);
            TextureAssets.Item[ItemID.TheUndertaker] = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.TheUndertaker);
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Particle.Load();
                DetourLoader.Load();
                LoadEffects();

                foreach (Type type in Code.GetTypes())
                {
                    Particle.TryRegisteringParticle(type);
                }

                // Activate this with ManageSpecialBiomeVisuals probably... 
                //Filters.Scene["OM:RavensfellSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0.3f).UseOpacity(0.5f), EffectPriority.Medium);
                SkyManager.Instance["OM:ArchiveSky"] = new ArchiveSky();
            }
        }

        public override void Unload()
        {
            Particle.Unload();
            DetourLoader.Unload();
            UnloadEffects();
            UnloadVanillaTextures();
        }

        public override void PostSetupContent()
        {
            ReplaceVanillaTextures();
        }
    }
}
