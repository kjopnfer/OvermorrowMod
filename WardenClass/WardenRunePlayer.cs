using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass
{
    public class WardenRunePlayer : ModPlayer
    {
        public bool ActiveRune;

        public enum Runes
        {
            None = 0,
            HellRune = 1,
            BoneRune = 2,
            SkyRune = 3,
            MushroomRune = 4,
            CrimsonRune = 5,
            CorruptionRune = 6,
            JungleRune = 7
        }

        public Runes RuneID;

        public override void ResetEffects()
        {
            ActiveRune = false;
            //RuneID = 0;
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (RuneID != 0)
            {
                int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsBack);
                if (body < 0)
                {
                    return;
                }

                layers.Insert(body - 1, Rune);
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().mirrorBuff)
            {
                int body = layers.FindIndex(l => l == PlayerLayer.MiscEffectsFront);
                if (body < 0)
                {
                    return;
                }

                layers.Insert(body - 1, Mirror);
            }
        }

        public int runeCounter;
        public int rotateCounter;
        public bool runeDeactivate = false;
        public static readonly PlayerLayer Rune = new PlayerLayer("OvermorrowMod", "Body", delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("OvermorrowMod");
            WardenRunePlayer modPlayer = drawPlayer.GetModPlayer<WardenRunePlayer>();

            if (modPlayer.runeDeactivate && modPlayer.runeCounter == 0)
            {
                modPlayer.ActiveRune = false;
                modPlayer.runeCounter = 0;
                modPlayer.rotateCounter = 0;
                modPlayer.runeDeactivate = false;
                modPlayer.RuneID = 0;
                return;
            }

            if (modPlayer.RuneID == 0)
            {
                return;
            }

            // Probably use an enum or something
            if (!modPlayer.ActiveRune && !modPlayer.runeDeactivate)
            {
                modPlayer.runeDeactivate = true;
            }

            if (!modPlayer.runeDeactivate)
            {
                if (modPlayer.runeCounter < 300)
                {
                    modPlayer.runeCounter++;
                }
            }
            else
            {
                if (modPlayer.runeCounter > 0)
                {
                    modPlayer.runeCounter--;
                }
            }

            modPlayer.rotateCounter++;

            Texture2D symbolTexture = null;

            // Symbol Texture
            switch (modPlayer.RuneID)
            {
                case Runes.HellRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/HellRuneCircle");
                    break;
                case Runes.BoneRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DungeonRuneCircle2");
                    break;
                case Runes.SkyRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/SkyRuneCircle");
                    break;
                case Runes.MushroomRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/MushroomRuneCircle");
                    break;
                case Runes.CrimsonRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CrimsonRuneCircle");
                    break;
                case Runes.CorruptionRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CorruptionRuneCircle");
                    break;
                case Runes.JungleRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/JungleRuneCircle");
                    break;
                default:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/temp");
                    break;
            }

            Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2)) + new Vector2((-drawPlayer.direction), 0);

            // Replaced drawPlayer.miscCounter with modPlayer.symbolCounter, there might be syncing issues idk
            double deg = (modPlayer.rotateCounter * 0.8) * MathHelper.Lerp(1, 4, (float)(!modPlayer.runeDeactivate ? modPlayer.runeCounter / 300.0 : 1));
            float rad = (float)(deg * (Math.PI / 180));

            float scale = (float)((modPlayer.runeCounter * 2 >= 300 ? 300 : modPlayer.runeCounter * 2) / 300.0) * 1.25f;

            DrawData data = new DrawData(symbolTexture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.White, rad, symbolTexture.Size() / 2f, scale, SpriteEffects.None, 0);
            Main.playerDrawData.Add(data);
        });

        public static readonly PlayerLayer Mirror = new PlayerLayer("OvermorrowMod", "Body", delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("OvermorrowMod");

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/WardenClass/Textures/boble");

            Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2)) + new Vector2((-drawPlayer.direction), 0);

            DrawData data = new DrawData(texture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.Lerp(Color.White, Color.Transparent, (float)Math.Sin(drawPlayer.miscCounter / 100f)), drawPlayer.bodyRotation, texture.Size() / 2f, 1, SpriteEffects.None, 0);
            Main.playerDrawData.Add(data);
        });
    }
}