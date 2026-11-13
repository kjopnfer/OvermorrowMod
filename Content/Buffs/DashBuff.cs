using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Content.Buffs
{
    /// <summary>
    /// Readout of the run dash. Kept on the player by <see cref="DashPlayer"/> every tick while in a run.
    /// </summary>
    public class DashBuff : ModBuff
    {
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Swiftness;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            tip = Description.Format(Main.LocalPlayer.GetModPlayer<DashPlayer>().Charges, DashPlayer.MaxCharges);
        }

        public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams)
        {
            DashPlayer dash = Main.LocalPlayer.GetModPlayer<DashPlayer>();
            Rectangle icon = new Rectangle((int)drawParams.Position.X, (int)drawParams.Position.Y, drawParams.SourceRectangle.Width, drawParams.SourceRectangle.Height);

            int shadeHeight = (int)(icon.Height * dash.RechargeFraction);
            if (shadeHeight > 0) spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(icon.X, icon.Y, icon.Width, shadeHeight), Color.Black * 0.6f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, dash.Charges.ToString(), drawParams.Position + new Vector2(4f, 14f), Color.White, 0f, Vector2.Zero, new Vector2(0.8f));
        }
    }
}
