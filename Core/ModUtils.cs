using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Common.TilePiles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Content.Tiles.TilePiles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core
{
    public static class ModUtils
    {
        public static MethodInfo startRain;
        public static MethodInfo stopRain;
        public static void Load(bool unload)
        {
            if (unload)
            {
                startRain = null;
                stopRain = null;
            }
            else
            {
                startRain = typeof(Main).GetMethod("StartRain", BindingFlags.Static | BindingFlags.NonPublic);
                stopRain = typeof(Main).GetMethod("StopRain", BindingFlags.Static | BindingFlags.NonPublic);
            }
        }

        public static void SandstormStuff()
        {
            Sandstorm.IntendedSeverity = 20; //0.4f;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
        }

        public static void PlaceObject(int x, int y, int TileType, int style = 0, int direction = -1)
        {
            WorldGen.PlaceObject(x, y, TileType, true, style, 0, -1, direction);
            NetMessage.SendObjectPlacment(-1, x, y, TileType, style, 0, -1, direction);
        }

        public static void StartRain()
        {
            startRain.Invoke(null, null);
        }
        public static void StopRain()
        {
            stopRain.Invoke(null, null);
        }

        public static void SetWeaponType(this Item item, GunType gunType)
        {
            item.GetGlobalItem<GlobalGun>().WeaponType = gunType;
        }

        public static GunType GetWeaponType(this Item item)
        {
            return item.GetGlobalItem<GlobalGun>().WeaponType;
        }

        public static void PlaceTilePile<T, TE>(int x, int y) where T : ModTilePile<TE> where TE : BaseTilePile
        {
            PlaceObject(x, y, ModContent.TileType<T>());
            int id = ModContent.GetInstance<TE>().Place(x - 1, y - 2); // this represents the top left corner
            TE te = TileEntity.ByID[id] as TE;
            te.SetPosition(new Vector2(x, y));
            te.CreateTilePile();
        }

        public static Rectangle toRect(Vector2 pos, int w, int h)
        {
            return new Rectangle((int)(pos.X - Main.screenPosition.X), (int)(pos.Y - Main.screenPosition.Y), w, h);
        }

        public static Vector2 GetInventoryPosition(Vector2 position, Rectangle frame, Vector2 origin, float scale)
        {
            return position + (((frame.Size() / 2f) - origin) * scale * Main.inventoryScale) + new Vector2(1.5f, 1.5f);
        }

        public static Vector2 NearestPoint(this Vector2 vec, Rectangle rect)
        {
            float nearX = vec.X > rect.Right ? rect.Right : vec.X < rect.Left ? rect.Left : vec.X;
            float nearY = vec.Y > rect.Bottom ? rect.Bottom : vec.Y < rect.Top ? rect.Top : vec.Y;
            return new Vector2(nearX, nearY);
        }
        public static float Bezier(float x1, float x2, float x3, float x4, float t)
        {
            return (float)(
                x1 * Math.Pow(1 - t, 3) +
                x2 * 3 * t * Math.Pow(1 - t, 2) +
                x3 * 3 * Math.Pow(t, 2) * (1 - t) +
                x4 * Math.Pow(t, 3)
                );
        }

        public static Vector2 Bezier(Vector2 from, Vector2 to, Vector2 cp1, Vector2 cp2, float amount)
        {
            Vector2 output = new Vector2();
            output.X = Bezier(from.X, cp1.X, cp2.X, to.X, amount);
            output.Y = Bezier(from.Y, cp1.Y, cp2.Y, to.Y, amount);
            return output;
        }
        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }
        public static Vector2 AdjustToGravity(this Vector2 velocity, float gravity, float time)
        {
            velocity.X = velocity.X / time;
            velocity.Y = velocity.Y / time - 0.5f * gravity * time;
            return velocity;
        }
        public static bool HasParameter(this Effect effect, string name)
        {
            foreach (EffectParameter param in effect.Parameters)
            {
                if (param.Name == name) return true;
            }
            return false;
        }

        public static OvermorrowModPlayer Overmorrow(this Player player)
        {
            return player.GetModPlayer<OvermorrowModPlayer>();
        }
        public static void SafeSetParameter(this Effect effect, string name, float value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void SafeSetParameter(this Effect effect, string name, Vector2 value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void SafeSetParameter(this Effect effect, string name, Color value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value.ToVector3());
        }
        public static void SafeSetParameter(this Effect effect, string name, Texture2D value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void SafeSetParameter(this Effect effect, string name, Matrix value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void Reload(this SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if (spriteBatch.HasBegun())
            {
                spriteBatch.End();
            }
            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }
        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = null, SpriteSortMode sortMode = default)
        {
            if (spriteBatch.HasBegun())
            {
                spriteBatch.End();
            }
            if (blendState == null) blendState = (BlendState)spriteBatch.GetField("blendState");
            SamplerState state = (SamplerState)spriteBatch.GetField("samplerState");
            DepthStencilState state2 = (DepthStencilState)spriteBatch.GetField("depthStencilState");
            RasterizerState state3 = (RasterizerState)spriteBatch.GetField("rasterizerState");
            Effect effect = (Effect)spriteBatch.GetField("customEffect");
            Matrix matrix = (Matrix)spriteBatch.GetField("transformMatrix");
            spriteBatch.Begin(sortMode, blendState, state, state2, state3, effect, matrix);
        }
        public static bool HasBegun(this SpriteBatch spriteBatch)
        {
            return (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }
        public static object GetField(this object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        public static float Magnitude(Vector2 mag)
        {
            return (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);
        }

        /// <summary>
        /// Moves the npc to a Vector2.
        /// The lower the turnResistance, the less time it takes to adjust direction.
        /// Example: npc.Move(new Vector2(100, 0), 10, 14);
        /// toPlayer makes the vector consider the player.Center for you.
        /// </summary>
        public static void Move(this NPC npc, Vector2 vector, float speed, float turnResistance = 10f,
            bool toPlayer = false)
        {
            Player player = Main.player[npc.target];
            Vector2 moveTo = toPlayer ? player.Center + vector : vector;
            Vector2 move = moveTo - npc.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            npc.velocity = move;
        }

        public static void Move(this Projectile projectile, Vector2 vector, float speed, float turnResistance = 10f, bool toPlayer = false)
        {
            Player player = Main.player[projectile.owner];
            Vector2 moveTo = toPlayer ? player.Center + vector : vector;
            Vector2 move = moveTo - projectile.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            move = (projectile.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }

            projectile.velocity = move;
        }

        /// <summary>
        /// Gets the top-left tile of a multitile
        /// </summary>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        public static Point16 GetTileOrigin(int i, int j)
        {
            //Framing.GetTileSafely ensures that the returned Tile instance is not null
            //Do note that neither this method nor Framing.GetTileSafely check if the wanted coordiates are in the world!
            Tile tile = Framing.GetTileSafely(i, j);

            Point16 coord = new Point16(i, j);
            Point16 frame = new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18);

            return coord - frame;
        }

        /// <summary>
        /// Uses <seealso cref="GetTileOrigin(int, int)"/> to try to get the entity bound to the multitile at (<paramref name="i"/>, <paramref name="j"/>).
        /// </summary>
        /// <typeparam name="T">The type to get the entity as</typeparam>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        /// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
        /// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
        public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
        {
            Point16 origin = GetTileOrigin(i, j);

            //TileEntity.ByPosition is a Dictionary<Point16, TileEntity> which contains all placed TileEntity instances in the world
            //TryGetValue is used to both check if the dictionary has the key, origin, and get the value from that key if it's there
            if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
            {
                entity = existingAsT;
                return true;
            }

            entity = null;
            return false;
        }

        public static void Kill(this NPC npc)
        {
            npc.life = 0;
            npc.HitEffect();
            npc.NPCLoot();
            npc.active = false;

            npc.netUpdate = true;
        }

        public static float isLeft(Vector2 P0, Vector2 P1, Vector2 P2)
        {
            return ((P1.X - P0.X) * (P2.X - P0.X) - (P2.X - P0.X) * (P1.X - P0.X));
        }
        public static bool PointInShape(Vector2 P, params Vector2[] args)
        {
            bool result = true;
            for (int i = 0; i < args.Length; i++)
            {
                int i1 = i;
                int i2 = (i + 1) % args.Length;
                Vector2 a = args[i1];
                Vector2 b = args[i2];
                Vector2 c = P;
                if (isLeft(a, b, c) < 0) result = false;
            }
            return result;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int c = list.Count;
            List<T> current = new List<T>();
            for (int i = 0; i < c; i++)
            {
                int index = Main.rand.Next(list.Count);
                current.Add(list[index]);
                list.RemoveAt(index);
            }

            return current;
        }

        public static T[] Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Main.rand.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
            //return Shuffle<T>(new List<T>(array)).ToArray();
        }

        public static float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float EaseOutCirc(float x)
        {
            return (float)Math.Sqrt(1 - Math.Pow(x - 1, 2));
        }

        public static float EaseOutQuint(float x)
        {
            return (float)(1 - Math.Pow(1 - x, 5));
        }

        public static float EaseInQuad(float x)
        {
            return x * x;
        }

        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        public static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }

        /// <summary>
        /// Modified version of Player.Hurt, which ignores defense.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="damage"></param>
        /// <param name="dramatic"></param>
        /// <param name="dot"></param>
        public static void HurtDirect(this Player player, PlayerDeathReason deathReason, int damage, bool dramatic = false, bool dot = false)
        {
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.DamagedFriendly, damage, dramatic, dot);
            player.statLife -= damage;

            if (player.statLife <= 0)
            {
                player.statLife = 0;
                player.KillMe(deathReason, 10, 0);
            }
        }

        public static Color Lerp3(Color a, Color b, Color c, float t)
        {
            if (t < 0.5f) // 0.0 to 0.5 goes to a -> b
                return Color.Lerp(a, b, t / 0.5f);
            else // 0.5 to 1.0 goes to b -> c
                return Color.Lerp(b, c, (t - 0.5f) / 0.5f);
        }

        public static XmlDocument GetXML(string directory)
        {
            XmlDocument doc = new XmlDocument();
            string text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(directory));
            doc.LoadXml(text);

            return doc;
        }

        public static void AddElement(UIElement element, int x, int y, int width, int height, UIElement appendTo)
        {
            element.Left.Set(x, 0);
            element.Top.Set(y, 0);
            element.Width.Set(width, 0);
            element.Height.Set(height, 0);
            appendTo.Append(element);
        }

        public static void AddElement(UIElement element, int x, int y, int width, float widthPercent, int height, float heightPercent, UIElement appendTo)
        {
            element.Left.Set(x, 0);
            element.Top.Set(y, 0);
            element.Width.Set(width, widthPercent);
            element.Height.Set(height, heightPercent);
            appendTo.Append(element);
        }

        /// <summary>
        /// Loops through the player's inventory and then places any suitable ammo types into the ammo slots if they are empty or the wrong ammo type.
        /// </summary>
        public static void AutofillAmmoSlots(Player player, int ammoID)
        {
            for (int j = 0; j <= 3; j++) // Check if any of the ammo slots are empty or are the right ammo
            {
                Item ammoItem = player.inventory[54 + j];
                if (ammoItem.type != ItemID.None && ammoItem.ammo == ammoID) continue;

                // Loop through the player's inventory in order to find any useable ammo types to use
                for (int i = 0; i <= 49; i++)
                {
                    Item item = player.inventory[i];
                    if (item.type == ItemID.None || item.ammo != ammoID) continue;

                    //Main.NewText("Swapping " + i + " with " + (54 + j));

                    Item tempItem = ammoItem;
                    player.inventory[54 + j] = item;
                    player.inventory[i] = tempItem;

                    break;
                }
            }
        }
    }
}
