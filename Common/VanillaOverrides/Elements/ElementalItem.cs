using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Elements
{
    public partial class ElementalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public HashSet<Element> ElementTypes = new HashSet<Element>() { Element.None };

        private Texture2D ElementTexture(Element element)
        {
            switch (element)
            {
                case Element.Fire:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Fire").Value;
                case Element.Ice:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Ice").Value;
                case Element.Water:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Water").Value;
                case Element.Earth:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Earth").Value;
                case Element.Nature:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Nature").Value;
                case Element.Wind:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Wind").Value;
                case Element.Electric:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Electric").Value;
                case Element.Light:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Light").Value;
                case Element.Dark:
                    return ModContent.Request<Texture2D>(AssetDirectory.UI + "Elements/Dark").Value;
            }

            return null;
        }

        private const int OFFSET = 32;
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Name == "ItemName")
            {
                if (ElementTypes.Contains(Element.None)) return true;

                int offsetCounter = 0;
                foreach (Element e in ElementTypes)
                {
                    Texture2D texture = ElementTexture(e);
                    Vector2 position = new Vector2(line.X - 16, line.Y + 8 + (OFFSET * offsetCounter));
                    spriteBatch.Draw(texture, position, null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 1f);

                    offsetCounter++;
                }
            }

            return true;
        }
    }
}