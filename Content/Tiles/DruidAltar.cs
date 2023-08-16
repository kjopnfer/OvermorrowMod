using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles
{
    public class DruidAltar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = false;

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Resonance Altar");
            AddMapEntry(Color.ForestGreen, name);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.addTile(Type);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {

        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            player.GetModPlayer<OvermorrowModPlayer>().AltarCoordinates = new Vector2(i * 16, j * 16);

            float distance = Vector2.Distance(new Vector2(i * 16, j * 16), player.Center);
            if (distance <= 80)
            {
                player.GetModPlayer<OvermorrowModPlayer>().UIToggled = true;
            }
            else
            {
                player.GetModPlayer<OvermorrowModPlayer>().UIToggled = false;
            }

            base.PostDraw(i, j, spriteBatch);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            //Main.PlaySound(SoundID.MenuOpen);
            /*float distance = Vector2.Distance(new Vector2(i * 16, j * 16), player.Center);
            if (distance <= 200)
            {
                player.GetModPlayer<OvermorrowModPlayer>().UIToggled = true;
                Main.NewText("on");
            }*/

            return base.RightClick(i, j);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = .32f;
            b = .1f;
        }
    }
}