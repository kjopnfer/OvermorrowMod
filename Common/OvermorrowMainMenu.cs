using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowMainMenu : ModMenu
    {
        public override int Music => MusicLoader.GetMusicSlot("OvermorrowMod/Sounds/Music/SandstormBoss");
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("OvermorrowMod/logo");
    }
}
