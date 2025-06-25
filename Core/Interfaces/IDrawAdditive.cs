using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Used to group together all additive draw calls into a single batch to reduce SpriteBatch restarts.
    /// This interface should be implemented by any drawable object (e.g., NPCs, Projectiles, Particles)
    /// that needs to use additive blending.
    ///
    /// Calling SpriteBatch.Begin() and End() repeatedly—especially within loops or per-object draw calls—
    /// can be expensive, as it flushes the current draw batch and incurs CPU overhead.
    /// Grouping additive draws into a centralized pass improves performance by minimizing state changes
    /// and ensuring all additive content is rendered efficiently in a single draw call.
    /// </summary>
    interface IDrawAdditive
    {
        void DrawAdditive(SpriteBatch spriteBatch);
    }
}
