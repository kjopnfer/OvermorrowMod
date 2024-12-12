using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Backgrounds
{
    public class ArchivesBackgroundStyle : ModSurfaceBackgroundStyle
    {
        public override int ChooseFarTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "ArchiveBackground");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "ArchiveBackground");
        }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            // This just fades in the background and fades out other backgrounds.
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                        fades[i] = 1f;
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                        fades[i] = 0f;
                }
            }
        }
    }

    public class ArchivesUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "Empty");
            textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "Empty");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "Empty");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Backgrounds + "Empty");
        }
    }
}