using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.CustomCollision;
using OvermorrowMod.Common.RoomManager;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    /// <summary>
    /// Visible collision NPC spawned by CombatDoor_TE. 1 tile wide, 9 tiles
    /// tall. Reads its TE's DoorState each tick to position itself: stays
    /// at the spawn Y when Closed, slides up by 9 tiles during Opening,
    /// rests there during Open, slides back down during Closing. Collision
    /// is active only while Closed so the player can pass while open.
    /// Right-click within range tells the TE to Open(), which syncs the
    /// sibling door so both halves of the room move together.
    /// </summary>
    public class CombatDoorCollision : TileCollisionNPC
    {
        public ModTileEntity tileEntity;

        private CombatDoor_TE DoorInstance =>
            tileEntity != null ? (CombatDoor_TE)TileEntity.ByID[tileEntity.ID] : null;

        public override string Texture => AssetDirectory.Misc + "CombatDoor";

        // Closed-state Y, captured first tick; all animation offsets are relative to this.
        private float closedYPixels;
        private bool capturedClosedY = false;

        public override void SafeSetDefaults()
        {
            NPC.width = 16;
            NPC.height = 144;   // 9 tiles
            NPC.hide = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override bool PreAI()
        {
            NPC.ShowNameOnHover = false;

            if (!capturedClosedY)
            {
                closedYPixels = NPC.position.Y;
                capturedClosedY = true;
            }

            var inst = DoorInstance;
            if (inst != null)
                NPC.position.Y = closedYPixels + inst.YOffsetPixels;

            // Colliders only while Closed; otherwise the player walks through.
            if (inst != null && inst.IsBlocking)
            {
                colliders = new CollisionSurface[]
                {
                    new CollisionSurface(NPC.TopLeft, NPC.BottomLeft,
                        new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                    new CollisionSurface(NPC.TopRight, NPC.BottomRight,
                        new int[] { CollisionID.Solid, CollisionID.Solid, 0, 0 }, true),
                };
            }
            else
            {
                colliders = null;
            }
            return true;
        }

        public override void AI()
        {
            if (colliders != null)
                foreach (var c in colliders) c.Update();

            const float InteractRangeTiles = 7f;
            float interactRange = InteractRangeTiles * 16f;
            bool playerInRange = Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) <= interactRange;

            if (playerInRange && NPC.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
            {
                Main.LocalPlayer.cursorItemIconEnabled = true;
                Main.LocalPlayer.cursorItemIconID = ItemID.WoodenDoor;
                Main.LocalPlayer.noThrow = 2;

                if (Main.mouseRight && Main.mouseRightRelease)
                    DoorInstance?.Open();
            }
        }

        public override void PostAI()
        {
            if (colliders != null)
                foreach (var c in colliders) c.PostUpdate();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(tex, NPC.Center - screenPos, null, Color.White, 0f, tex.Size() / 2f, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}
