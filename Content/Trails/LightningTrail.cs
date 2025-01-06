using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class LightningTrail : SimpleTrail
    {
        public LightningTrail() : base(40, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Electricity").Value, true)
        {
        }

        /*private bool setup = false;
        private float distanceBetweenSegments = 20;
        public override void Update()
        {
            if (!setup)
            {
                for (int i = 0; i < Positions.Capacity; i++)
                {
                    Positions.PushBack(Entity.Center);
                }

                setup = true;
            }

            Positions[0] = Entity.Center;
            for (int i = 1; i < Positions.Count; i++)
            {
                if (Vector2.Distance(Positions[i], Positions[i - 1]) > distanceBetweenSegments)
                {
                    Vector2 offset = Vector2.Normalize(Positions[i - 1] - Positions[i]) * distanceBetweenSegments;
                    Positions[i] = Positions[i - 1] + offset;
                }
            }
        }*/
    }
}