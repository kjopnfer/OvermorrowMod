using OvermorrowMod.Common.Detours;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Particles;
using OvermorrowMod.Core.UI;
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

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TitleInterface = new UserInterface();
                TitleCard = new TitleCard();
                TitleInterface.SetState(TitleCard);

                TitleCardManager.Initialize(TitleCard);
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

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, TitleInterface, TitleCard, mouseTextIndex, TitleCard.visible, "Title Card");
            }
        }

        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null)
        {
            string name = customName == null ? state.ToString() : customName;
            layers.Insert(index, new LegacyGameInterfaceLayer("OvermorrowMod: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }
    }
}