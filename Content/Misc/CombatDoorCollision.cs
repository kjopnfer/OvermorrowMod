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
    /// <summary>Visible collision NPC spawned by CombatDoor_TE.</summary>
    public class CombatDoorCollision : TileCollisionNPC
    {
        public override string Texture => AssetDirectory.Misc + "CombatDoor";

        public int ParentDoorTEID { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public float ClosedY { get => NPC.ai[1]; set => NPC.ai[1] = value; }

        private CombatDoor_TE DoorInstance =>
            TileEntity.ByID.TryGetValue(ParentDoorTEID, out var te) ? te as CombatDoor_TE : null;

        public override void SafeSetDefaults()
        {
            NPC.width = 16;
            NPC.height = 144;
            NPC.hide = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

        public override bool PreAI()
        {
            NPC.ShowNameOnHover = false;

            var inst = DoorInstance;
            if (inst != null)
                NPC.position.Y = ClosedY + inst.YOffsetPixels;

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
                Main.LocalPlayer.mouseInterface = true;

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
