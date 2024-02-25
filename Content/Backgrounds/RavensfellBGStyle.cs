using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Backgrounds
{
    public class RavensfellBGStyle : ModSurfaceBackgroundStyle
    {
        public override int ChooseFarTexture()
        {
            //return base.ChooseFarTexture();
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/Empty");

            //return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/forest_test_close");
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            scale *= 0.75f;

            // I DONT KNOW WHY THESE DONT JUST LET ME USE THE EMPTY TEXTURE ASSETDIRECTORY CALL???????
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/Empty");

           /* return base.ChooseCloseTexture(ref scale, ref parallax, ref a, ref b);
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/forest_test_close");*/
        }

        public override int ChooseMiddleTexture()
        {
            return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/Empty");

            //return BackgroundTextureLoader.GetBackgroundSlot(AssetDirectory.Textures + "Backgrounds/forest_test_close");

            //return base.ChooseMiddleTexture();
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
}