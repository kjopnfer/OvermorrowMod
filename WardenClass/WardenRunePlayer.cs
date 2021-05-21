using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Graphics.Shaders;
using OvermorrowMod.UI;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod;
using OvermorrowMod.Buffs.Debuffs;

namespace OvermorrowMod.WardenClass
{
    public class WardenRunePlayer : ModPlayer
    {
        public bool ActiveRune;

        // Rune Types
        public bool Rune1;

        public override void ResetEffects()
        {
            ActiveRune = false;
            Rune1 = false;
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (ActiveRune)
            {
                int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsBack);
                if (body < 0)
                {
                    return;
                }

                layers.Insert(body - 1, Rune);
            }
        }

        public int runeCounter;
        public int rotateCounter;
        public static readonly PlayerLayer Rune = new PlayerLayer("OvermorrowMod", "Body", delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("OvermorrowMod");
            WardenRunePlayer modPlayer = drawPlayer.GetModPlayer<WardenRunePlayer>();

            if (modPlayer.Rune1 == false)
            {
                modPlayer.ActiveRune = false;
                modPlayer.runeCounter = 0;
                modPlayer.rotateCounter = 0;
                return;
            }

            if (modPlayer.runeCounter < 300)
            {
                modPlayer.runeCounter++;
            }

            modPlayer.rotateCounter++;

            // Symbol Texture
            Texture2D symbolTexture = ModContent.GetTexture("OvermorrowMod/UI/temp");
            //int num24 = (int)(((float)(drawPlayer.miscCounter / 300.0)).ToRotationVector2().Y * 4.0);
            Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2)) + new Vector2((-drawPlayer.direction), 0);

            // Replaced drawPlayer.miscCounter with modPlayer.symbolCounter, there might be syncing issues idk
            double deg = (modPlayer.rotateCounter * 1.2) * MathHelper.Lerp(1, 4, (float)(modPlayer.runeCounter / 300.0));

            float rad = (float)(deg * (Math.PI / 180));

            float scale = (float)((modPlayer.runeCounter * 2 >= 300 ? 300 : modPlayer.runeCounter * 2) / 300.0) * 1.5f;

            DrawData data = new DrawData(symbolTexture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.White, rad, symbolTexture.Size() / 2f, scale, SpriteEffects.None, 0);
            Main.playerDrawData.Add(data);
        });
    }
}