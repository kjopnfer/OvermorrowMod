using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Rendering
{
    public class RenderNPC : GlobalNPC
    {
        private bool draw;
        private ExternalRenderable render;
        private Renderer renderer;
        public override bool InstancePerEntity => true;

        public Renderer Renderer
        {
            get => renderer;
            set
            {
                renderer?.Remove(render);
                renderer = value;
                renderer?.Add(render);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            render = new ExternalRenderable(() =>
            {
                draw = true;
                Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
                draw = false;
            });
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return Renderer == null || draw;
        }

        public override void OnKill(NPC npc)
        {
            Renderer?.Remove(render);
        }
    }
}