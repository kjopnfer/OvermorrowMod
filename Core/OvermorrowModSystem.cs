using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Particles;
using OvermorrowMod.Core.UI;
using OvermorrowMod.Core.UI.ClassSelection;
using OvermorrowMod.Core.UI.Shop;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
namespace OvermorrowMod.Core
{
    public class OvermorrowModSystem : ModSystem
    {
        public static int ArchiveTiles;

        internal UserInterface TitleInterface;
        public TitleCard TitleCard;

        internal UserInterface RewardInterface;
        public RewardSelection RewardSelection;

        internal UserInterface ClassInterface;
        public ClassSelection ClassSelection;

        internal UserInterface ShopInterface;
        public ShopDialogue ShopDialogue;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TitleInterface = new UserInterface();
                TitleCard = new TitleCard();
                TitleInterface.SetState(TitleCard);

                TitleCardManager.Initialize(TitleCard);

                RewardInterface = new UserInterface();
                RewardSelection = new RewardSelection();
                RewardInterface.SetState(RewardSelection);

                RewardSelectionManager.Initialize(RewardSelection);

                ClassInterface = new UserInterface();
                ClassSelection = new ClassSelection();
                ClassInterface.SetState(ClassSelection);

                ClassSelectionManager.Initialize(ClassSelection);

                ShopInterface = new UserInterface();
                ShopDialogue = new ShopDialogue();
                ShopInterface.SetState(ShopDialogue);
            }
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            ArchiveTiles = tileCounts[TileType<CastleBrick>()] + tileCounts[TileType<ArchiveWood>()] + tileCounts[TileType<CastlePlatform>()];
        }

        public override void ResetNearbyTileEffects()
        {
            ArchiveTiles = 0;
        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                ParticleManager.UpdateParticles();
            }
        }

        public override void PostUpdateEverything()
        {
            PrimitiveManager.UpdateTrails();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (TitleCard != null && TitleCard.visible) TitleInterface?.Update(gameTime);
            if (RewardSelection != null && RewardSelection.visible) RewardInterface?.Update(gameTime);
            if (ClassSelection != null && ClassSelection.visible) ClassInterface?.Update(gameTime);
            if (ShopDialogue.IsActive) ShopInterface?.Update(gameTime);
            else ShopDialogue?.NotifyClosed();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, TitleCard, mouseTextIndex, TitleCard.visible, "Title Card");
                AddInterfaceLayer(layers, RewardSelection, mouseTextIndex, RewardSelection.visible, "Reward Selection");
                AddInterfaceLayer(layers, ClassSelection, mouseTextIndex, ClassSelection.visible, "Class Selection");
                AddInterfaceLayer(layers, ShopDialogue, mouseTextIndex, ShopDialogue.IsActive, "Shop Dialogue");
            }
        }

        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UIState state, int index, bool visible, string customName = null)
        {
            string name = customName == null ? state.ToString() : customName;
            layers.Insert(index, new LegacyGameInterfaceLayer("OvermorrowMod: " + name,
                delegate
                {
                    if (visible) state.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));
        }
    }
}