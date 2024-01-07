using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Netcode;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.NPCs.Bosses.Eye;
using OvermorrowMod.Core;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
//Mercenaries
using OvermorrowMod.Content.NPCs.Mercenary;
using static OvermorrowMod.Content.NPCs.Mercenary.Paladin.Paladin;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.TilePiles;

namespace OvermorrowMod.Common
{
    public partial class OvermorrowModFile : Mod
    {
        private Vector2 lastScreenSize;
        private Vector2 lastViewSize;
        private Viewport lastViewPort;

        // Hotkeys
        public static ModKeybind SandModeKey;
        public static ModKeybind ToggleUI;
        public static ModKeybind AmuletKey;
        public static ModKeybind AmmoSwapKey;
        public static ModKeybind BearTrapKey;

        public static List<int[]> ModBowsToOverride = new List<int[]>();
        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;

        public RenderTarget2D Render;

        public Asset<Effect> BeamShader;
        public Asset<Effect> ContainedFlash;
        public Asset<Effect> Dissolve;
        public Asset<Effect> Flash;
        public Asset<Effect> RadialBlur;
        public Asset<Effect> Ring;
        public Asset<Effect> Shockwave;
        public Asset<Effect> Shockwave2;
        public Asset<Effect> Tentacle;
        public Asset<Effect> TentacleBlack;
        public Asset<Effect> TileOverlay;
        public Asset<Effect> TrailShader;
        public Asset<Effect> TextShader;
        public Asset<Effect> Whiteout;
        public Asset<Effect> Portal;
        public Asset<Texture2D> BlurTestTexture;

        public static Effect BigTentacle;
        //public static Effect Dissolve;

        public static List<Asset<Texture2D>> TrailTextures;
        //Mercenaries
        internal List<MercenaryDrawHelper> drawHelpers = new List<MercenaryDrawHelper>();
        public override void PostSetupContent()
        {
            drawHelpers.Add(new PaladinDrawHelper());
        }

        public static void PremultiplyTexture(ref Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(
                    buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }

        public override void Load()
        {
            // Keys
            SandModeKey = KeybindLoader.RegisterKeybind(this, "Swap Sand Mode", "Q");
            AmuletKey = KeybindLoader.RegisterKeybind(this, "Artemis Amulet Attack", "C");
            ToggleUI = KeybindLoader.RegisterKeybind(this, "Toggle UI", "R"); // This is for debugging
            AmmoSwapKey = KeybindLoader.RegisterKeybind(this, "Activate Ammo Swap", "Q");
            BearTrapKey = KeybindLoader.RegisterKeybind(this, "Place Bear Trap", "V");

            ShaderLoader.Load();

            if (!Main.dedServ)
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                lastViewSize = Main.ViewSize;
                lastViewPort = Main.graphics.GraphicsDevice.Viewport;

                // TODO: These are gone, need to figure out what to do.
                // Main.logoTexture = Instance.GetTexture("logo");
                // Main.logo2Texture = Instance.GetTexture("logo");

                // Effects
                BeamShader = Assets.Request<Effect>("Effects/Beam");
                ContainedFlash = Assets.Request<Effect>("Effects/ContainedFlash");
                Dissolve = Assets.Request<Effect>("Effects/Dissolve");
                Flash = Assets.Request<Effect>("Effects/Flash");
                Ring = Assets.Request<Effect>("Effects/Ring");
                Shockwave = Assets.Request<Effect>("Effects/Shockwave1");
                Shockwave2 = Assets.Request<Effect>("Effects/ShockwaveEffect");
                Tentacle = Assets.Request<Effect>("Effects/Tentacle");
                TentacleBlack = Assets.Request<Effect>("Effects/TentacleBlack");
                TextShader = Assets.Request<Effect>("Effects/TextShader");
                TileOverlay = Assets.Request<Effect>("Effects/TileOverlay");
                TrailShader = Assets.Request<Effect>("Effects/Trail");
                Whiteout = Assets.Request<Effect>("Effects/Whiteout");
                RadialBlur = Assets.Request<Effect>("Effects/CenterBlurShader");
                BlurTestTexture = Assets.Request<Texture2D>("Effects/testpattern");
                Portal = Assets.Request<Effect>("Effects/PortalOverride");

                BigTentacle = Assets.Request<Effect>("Effects/BigTentacle", AssetRequestMode.ImmediateLoad).Value;
                //Dissolve = Assets.Request<Effect>("Effects/Dissolve", AssetRequestMode.ImmediateLoad).Value;

                Ref<Effect> ref1 = new Ref<Effect>(Shockwave.Value);
                Ref<Effect> ref2 = new Ref<Effect>(Shockwave2.Value);
                Ref<Effect> ref3 = new Ref<Effect>(Flash.Value);
                Ref<Effect> ref4 = new Ref<Effect>(Dissolve.Value);

                GameShaders.Misc["OvermorrowMod: Shockwave"] = new MiscShaderData(ref1, "ForceField");
                GameShaders.Misc["OvermorrowMod: DeathAnimation"] = new MiscShaderData(ref4, "DeathAnimation").UseImage0("Images/Misc/Perlin");

                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(ref2, "Shockwave"), EffectPriority.VeryHigh);
                //Filters.Scene["EyeVortex"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.271f, 0.278f, 0.322f).UseOpacity(0.75f), EffectPriority.VeryHigh);
                Filters.Scene["EyeVortex"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0.6f), EffectPriority.VeryHigh);

                Ref<Effect> filterRef = new Ref<Effect>(Assets.Request<Effect>("Effects/Flash").Value);
                Filters.Scene["Flash"] = new Filter(new ScreenShaderData(filterRef, "ScreenFlash"), EffectPriority.VeryHigh);

                Ref<Effect> flashRef = new Ref<Effect>(Assets.Request<Effect>("Effects/ContainedFlash").Value);
                Filters.Scene["ContainedFlash"] = new Filter(new ScreenShaderData(flashRef, "ModdersToolkitShaderPass"), EffectPriority.VeryHigh);

                TrailTextures = new List<Asset<Texture2D>>();
                for (int i = 0; i < 7; i++)
                {
                    TrailTextures.Add(ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail" + i));
                }

                Main.QueueMainThreadAction(() =>
                {
                    Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Glow", AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref glow);
                });

                ReplaceVanillaTextures();
            }

            // RadianShadoo shadres :dread:
            Ref<Effect> specialRef = new Ref<Effect>(Assets.Request<Effect>("Effects/UVBG").Value);
            GameShaders.Misc["UVBG"] = new MiscShaderData(specialRef, "UVBG");//.UseImage0("OvermorrowMod/Effects/IMAGE");

            Ref<Effect> screenRef = new Ref<Effect>(Assets.Request<Effect>("Effects/UVShader").Value); // The path to the compiled shader file.
            Filters.Scene["UVShader0"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader0"].Load();

            Filters.Scene["UVShader1"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader1"].Load();

            Filters.Scene["UVShader2"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader2"].Load();

            Filters.Scene["UVShader3"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader3"].Load();

            Filters.Scene["UVShader4"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader4"].Load();

            Filters.Scene["UVShader5"] = new Filter(new ScreenShaderData(screenRef, "UVShader"), EffectPriority.VeryHigh);
            Filters.Scene["UVShader5"].Load();


            ModDetours.Load();

            //On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture;
            //Main.OnResolutionChanged += Main_OnResolutionChanged;
            //CreateRender();

            ModUtils.Load(false);
            HexLoader.Load(false);
            ILEdits.Load();
            Particle.Load();
            TileObjects.Load();
            Quests.Quests.Load(this);

            foreach (Type type in Code.GetTypes())
            {
                HexLoader.TryRegisteringHex(type);
                Particle.TryRegisteringParticle(type);
                TileObjects.RegisterTileObject(type);
            }

            base.Load();
        }

        private void ReplaceVanillaTextures()
        {
            TextureAssets.Item[ItemID.Boomstick] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Boomstick");
            TextureAssets.Item[ItemID.ChainKnife] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "ChainKnife");
            TextureAssets.Item[ItemID.Handgun] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Handgun");
            TextureAssets.Item[ItemID.Musket] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Musket");
            TextureAssets.Item[ItemID.Minishark] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Minishark");
            TextureAssets.Item[ItemID.PhoenixBlaster] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "PhoenixBlaster");
            TextureAssets.Item[ItemID.QuadBarrelShotgun] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "QuadBarrel");
            TextureAssets.Item[ItemID.Revolver] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Revolver");
            TextureAssets.Item[ItemID.TheUndertaker] = ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Undertaker");
        }

        public override void Unload()
        {
            Instance = null;

            BigTentacle = null;
            BeamShader = null;
            Dissolve = null;
            Flash = null;
            RadialBlur = null;
            Ring = null;
            Shockwave = null;
            Tentacle = null;
            TentacleBlack = null;
            TileOverlay = null;
            TrailShader = null;
            TextShader = null;
            Whiteout = null;

            TrailTextures = null;
            ModDetours.Unload();
            //On.Terraria.Graphics.Effects.FilterManager.EndCapture -= FilterManager_EndCapture;
            //Main.OnResolutionChanged -= Main_OnResolutionChanged;

            ModUtils.Load(true);
            HexLoader.Load(true);
            Quests.Quests.Unload();
            ILEdits.Unload();
            Particle.Unload();
            TileObjects.Unload();

            SandModeKey = null;
            AmuletKey = null;
            ToggleUI = null;
            AmmoSwapKey = null;
            BearTrapKey = null;

            ModBowsToOverride.Clear();

        }

        public override void AddRecipes()/* tModPorter Note: Removed. Use ModSystem.AddRecipes */
        {
            Recipe.Create(ItemID.ChainKnife)
                .AddIngredient(ItemID.Chain, 6)
                .AddRecipeGroup("IronBar", 1)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.HermesBoots)
                .AddIngredient(ItemID.Silk, 6)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.WaterWalkingBoots)
                .AddIngredient(ItemID.HermesBoots)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Loom)
                .Register();

            Recipe.Create(ItemID.CloudinaBottle)
                .AddIngredient(ItemID.Bottle)
                .AddIngredient(ItemID.Cloud, 10)
                .AddIngredient<EruditeOrb>(3)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.GoldBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.LuckyHorseshoe)
                .AddIngredient(ItemID.PlatinumBar, 10)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ClimbingClaws)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.ShoeSpikes)
                .AddRecipeGroup("IronBar", 8)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();

            Recipe.Create(ItemID.JellyfishNecklace)
                .AddIngredient(ItemID.Coral, 6)
                .AddIngredient(ItemID.Starfish, 4)
                .AddIngredient<EruditeOrb>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public void CreateRender()
        {
            Main.QueueMainThreadAction(() =>
            {
                Render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetworkMessageHandler.HandlePacket(reader, whoAmI);
        }

        private void Main_OnResolutionChanged(Vector2 obj)
        {
            CreateRender();
        }

        private void FilterManager_EndCapture(Terraria.Graphics.Effects.On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            //gd.SetRenderTarget(Main.screenTargetSwap);
            //gd.Clear(Color.Transparent);
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            //sb.End();

            //gd.SetRenderTarget(OvermorrowModFile.Instance.Render);
            //gd.Clear(Color.Transparent);
            //sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //CosmicFlame.DrawAll(sb);
            //sb.End();

            gd.SetRenderTarget(Main.screenTargetSwap);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(Render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            sb.Draw(TextureAssets.MagicPixel.Value, new Vector2(800, 500), new Rectangle(0, 0, 50, 50), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            sb.End();

            gd.SetRenderTarget(Main.screenTarget);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.Draw(Render, Vector2.Zero, Color.White);
            sb.End();

            /*foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<DarkTest>() && proj.active)
                {
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * proj.ai[1]);
                    sb.End();

                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    Player player = Main.player[Main.myPlayer];
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, 0, Vector2.Zero);
                    sb.End();
                }

                if (proj.active)
                {
                    Main.NewText("bruh");
                }
            }*/

            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }

        public void CheckScreenSize()
        {
            if (!Main.dedServ && !Main.gameMenu)
            {
                if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
                {
                    if (Detours.TileOverlay.projTarget != null) Detours.TileOverlay.ResizeTarget();
                }

                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                lastViewSize = Main.ViewSize;
                lastViewPort = Main.graphics.GraphicsDevice.Viewport;
            }
        }
    }
}
