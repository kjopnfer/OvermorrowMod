using OvermorrowMod.Core.UI.Shop;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Detours
{
    public class ChatBoxOverride : ILoadable
    {
        public void Load(Mod mod)
        {
            if (Main.dedServ) return;
            Terraria.On_Main.GUIChatDrawInner += SuppressForShopkeeper;
        }

        public void Unload() { }

        private void SuppressForShopkeeper(Terraria.On_Main.orig_GUIChatDrawInner orig, Main self)
        {
            if (ShopDialogue.IsActive) return;
            orig(self);
        }
    }
}
