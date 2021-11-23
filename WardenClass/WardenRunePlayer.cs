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
            JungleRune = 7,
            DefaultRune = 8
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

                int body2 = layers.FindIndex(l => l == PlayerLayer.MiscEffectsFront);
                if (body2 < 0)
                {
                    return;
                }

                layers.Insert(body2 - 1, Front);
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
        public int outerCounter;
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
            Texture2D ringTexture = null;
            Texture2D ringTexture2 = null; // Reserved for certain Rune Circles like the Chain Knife

            // Symbol Texture
            switch (modPlayer.RuneID)
            {
                case Runes.HellRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/HellRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/HellCircle_Outer");
                    break;
                case Runes.BoneRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DungeonRuneCircle2");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DungeonCircle_Outer");
                    break;
                case Runes.SkyRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/SkyRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/SkyCircle_Outer");
                    break;
                case Runes.MushroomRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/MushroomRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/MushroomCircle_Outer");
                    break;
                case Runes.CrimsonRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CrimsonRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CrimsonCircle_Outer");
                    break;
                case Runes.CorruptionRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CorruptionRuneCircle");
                    break;
                case Runes.JungleRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/JungleRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/JungleCircle_Outer");
                    break;
                case Runes.DefaultRune:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DefaultRuneCircle");
                    ringTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DefaultCircle_Outer1");
                    ringTexture2 = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DefaultCircle_Outer2");
                    break;
                default:
                    symbolTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/temp");
                    break;
            }

            Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2));

            // Replaced drawPlayer.miscCounter with modPlayer.symbolCounter, there might be syncing issues idk
            double deg = (modPlayer.rotateCounter * 0.8) * MathHelper.Lerp(1, 4, (float)(!modPlayer.runeDeactivate ? modPlayer.runeCounter / 300.0 : 1));
            float rad = (float)(deg * (Math.PI / 180));

            float scale = (float)((modPlayer.runeCounter * 2 >= 300 ? 300 : modPlayer.runeCounter * 2) / 300.0);

            DrawData data = new DrawData(symbolTexture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.White, rad, symbolTexture.Size() / 2f, scale, SpriteEffects.None, 0);
            Main.playerDrawData.Add(data);

            if (ringTexture != null)
            {
                if (!modPlayer.runeDeactivate && scale == 1)
                {
                    if (modPlayer.outerCounter < 300)
                    {
                        modPlayer.outerCounter++;
                    }
                }
                else
                {
                    if (modPlayer.outerCounter > 0)
                    {
                        modPlayer.outerCounter -= 5;
                    }
                }

                Color runeColor = Color.Lerp(Color.Transparent, Color.White, (float)((modPlayer.outerCounter * 2 >= 300 ? 300 : modPlayer.outerCounter * 2) / 300.0));

                DrawData data2 = new DrawData(ringTexture, position, new Microsoft.Xna.Framework.Rectangle?(), runeColor, rad * -1, ringTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data2);

                if (ringTexture2 != null)
                {
                    //Main.NewText(deg % 720 * 0.5f);
                    DrawData data3 = new DrawData(ringTexture2, position, new Microsoft.Xna.Framework.Rectangle?(), runeColor, rad * -1 * 0.5f, ringTexture.Size() / 2f, 1f, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data3);
                }
            }
        });

        public float runeCounter2;
        public static readonly PlayerLayer Front = new PlayerLayer("OvermorrowMod", "Body", delegate (PlayerDrawInfo drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("OvermorrowMod");
            WardenRunePlayer modPlayer = drawPlayer.GetModPlayer<WardenRunePlayer>();

            if (modPlayer.runeDeactivate && modPlayer.runeCounter == 0)
            {
                modPlayer.runeCounter2 = 0;
                return;
            }

            Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2));

            Texture2D runeTexture = null;

            switch (modPlayer.RuneID)
            {
                case Runes.HellRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/HellRune");
                    break;
                case Runes.BoneRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DungeonRune");
                    break;
                case Runes.SkyRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/SkyRune");
                    break;
                case Runes.MushroomRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/MushroomRune");
                    break;
                case Runes.CrimsonRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CrimsonRune");
                    break;
                case Runes.CorruptionRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/CorruptionRune");
                    break;
                case Runes.JungleRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/JungleRune");
                    break;
                case Runes.DefaultRune:
                    runeTexture = ModContent.GetTexture("OvermorrowMod/WardenClass/RuneCircles/DefaultRune");
                    break;
                default:
                    break;
            }

            float scaleCap = 70f;
            float runeScale = (float)MathHelper.Lerp(0, 2f, modPlayer.runeCounter2 / scaleCap);
            if (runeTexture != null && runeScale != 2f)
            {
                // Slows down the counter near the end
                if (modPlayer.runeCounter2 < scaleCap - 10)
                {
                    modPlayer.runeCounter2++;
                }
                else
                {
                    modPlayer.runeCounter2 += 0.25f;
                }

                DrawData data = new DrawData(runeTexture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.Lerp(Color.White, Color.Transparent, (float)Math.Sin(modPlayer.runeCounter2 / scaleCap)), drawPlayer.bodyRotation, runeTexture.Size() / 2f, runeScale, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer Mirror = new PlayerLayer("OvermorrowMod", "Body", delegate (PlayerDrawInfo drawInfo)
            {
                Player drawPlayer = drawInfo.drawPlayer;
                Mod mod = ModLoader.GetMod("OvermorrowMod");
                WardenRunePlayer modPlayer = drawPlayer.GetModPlayer<WardenRunePlayer>();

                Texture2D texture = ModContent.GetTexture("OvermorrowMod/WardenClass/Textures/boble");

                Vector2 position = new Vector2((int)(drawPlayer.position.X - (double)Main.screenPosition.X - (drawPlayer.bodyFrame.Width / 2) + (drawPlayer.width / 2)), (int)(drawPlayer.position.Y - (double)Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4.0)) + drawPlayer.bodyPosition + new Vector2((drawPlayer.bodyFrame.Width / 2), (drawPlayer.bodyFrame.Height / 2));

                DrawData data = new DrawData(texture, position, new Microsoft.Xna.Framework.Rectangle?(), Color.Lerp(Color.White, Color.Transparent, (float)Math.Sin(drawPlayer.miscCounter / 100f)), drawPlayer.bodyRotation, texture.Size() / 2f, 1, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            });
    }
}