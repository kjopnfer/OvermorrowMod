using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.Core.TmodFile;

namespace OvermorrowMod.Core
{
    public class ShaderLoader
    {
        public static void Load()
        {
            if (Main.dedServ)
                return;

            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var file = (TmodFile)info.Invoke(OvermorrowModFile.Instance, null);

            var shaders = file.Where(n => n.Name.StartsWith("Effects/") && n.Name.EndsWith(".xnb"));

            foreach (FileEntry entry in shaders)
            {
                var name = entry.Name.Replace(".xnb", "").Replace("Effects/", "");
                var path = entry.Name.Replace(".xnb", "");
                LoadShader(name, path);
            }
        }


        public static void LoadShader(string name, string path)
        {
            var screenRef = new Ref<Effect>(OvermorrowModFile.Instance.Assets.Request<Effect>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene[name] = new Filter(new ScreenShaderData(screenRef, name + "Pass"), EffectPriority.High);
            Filters.Scene[name].Load();
        }
    }
}
