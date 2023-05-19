using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using OvermorrowMod.Core;

namespace OvermorrowMod.Content.UI.ReadableBook
{
    public class TestBook : UIBook
    {
        public override List<UIBookPage> bookPages => new List<UIBookPage>() { 
            new TestBookPage(), new TestBookPage2(), new TestBookPage3(), new TestBookPage4(), new TestBookPage5(),
            new TestBookPage6(), new TestBookPage7(), new TestBookPage8(), new TestBookPage9(),
        };
    }

    public class TestBookPage : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(100, 1, 300, 1), "Overmorrow Public Demo", Color.DarkSlateGray, 2f));
            AddElement(new UIBookText(new Rectangle(30, 125, 300, 1), 
                "This build showcases the various milestone features developed over a four month period. " +
                "These features include: Quests, Interactable Tiles, Various Ranger Reworks, and this Book UI. ", Color.Black, 0.9f));
            //AddElement(new UIBookText(new Rectangle(25, 125, 300, 1), "this is a demo build to show you we never died because legends never die", Color.Black));
            AddElement(new UIBookText(new Rectangle(30, 275, 300, 1), "Altogether these have been combined into the Guide's camp to showcase the standard new player experience when starting the game. " +
                "On the other hand, this book will give an overview on the Ranger changes thus far.", Color.Black, 0.9f));
            AddElement(new UIBookImage(new Rectangle(135, 50, 1, 1), ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/BlueFairy").Value, Color.White, 0.5f));
        }
    }

    public class TestBookPage2 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(20, 0, 300, 1), "Ranger", Color.Gray, 1.5f));
            AddElement(new UIBookText(new Rectangle(20, 40, 280, 1), "While developing content we ran into various creative " +
                "limitations imposed by trying to adhere to vanilla. One example of this is Ranger, which we felt was the most " +
                "lacking out of the other vanilla classes in terms of interesting gameplay.", Color.Black, 0.9f));
            AddElement(new UIBookText(new Rectangle(20, 250, 280, 1), "As such we took on the task of rebuilding Ranger's core mechanics, " +
                "creating a class centered around powerful builds with devastating long-range combos kept in check by the difficulty and commitment " +
                "of the class.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage3 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookImage(new Rectangle(25, 0, 310, 230), ModContent.Request<Texture2D>(AssetDirectory.UI + "Books/PowerShotImage").Value, Color.White));
            AddElement(new UIBookText(new Rectangle(45, 225, 300, 1), "Vanilla Bow Reworks", Color.Gray, 1.5f));
            AddElement(new UIBookText(new Rectangle(40, 275, 280, 1), "Our first change to Ranger reworks Pre-Hardmode bows to have a draw back mechanic and Power Shots as pictured above.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage4 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(20, 0, 280, 1), "Power Shots build on Ranger's identity by giving bows a time interval where " +
                "they will deal increased damage after being fully charged. However, Power Shots last only a brief second so Players must be prepared " +
                "to capitalize on the bonus when using bows.", Color.Black, 0.9f));

            AddElement(new UIBookText(new Rectangle(20, 200, 280, 1), "In the near future, there will be various items geared toward the Power Shot mechanic to further maximize " +
                "the payoff and encourage Players to think strategically. See the end of this book for planned changes to the class.", Color.Black, 0.9f));

            AddElement(new UIBookText(new Rectangle(20, 370, 280, 1), "See the end of this book for planned changes to the class.", Color.Black, 0.9f));

            //AddElement(new UIBookText(new Rectangle(0, 0, 280, 1), "", Color.Black, 0.9f));
        }
    }

    public class TestBookPage5 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookImage(new Rectangle(25, 0, 310, 230), ModContent.Request<Texture2D>(AssetDirectory.UI + "Books/GunAmmoImage").Value, Color.White));
            AddElement(new UIBookText(new Rectangle(45, 225, 300, 1), "Vanilla Gun Reworks", Color.Gray, 1.5f));
            AddElement(new UIBookText(new Rectangle(40, 275, 280, 1), "In addition to bows, guns have been reworked to utilize ammo clips. " +
                "When adding this mechanic, we wanted to make sure this wasn't used to just slow down the class.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage6 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(20, 0, 280, 1), "As such, when running out ammo, the Player will enter into a reloading " +
                "skill check as indicated by the bar below. Depending on the weapon, each skill check may be different from the other.", Color.Black, 0.9f));
            AddElement(new UIBookImage(new Rectangle(10, 90, 310, 230), ModContent.Request<Texture2D>(AssetDirectory.UI + "Books/GunReloadImage").Value, Color.White));
            AddElement(new UIBookText(new Rectangle(20, 280, 280, 1), "When the bar reaches the end, the Player will always reload their next clip. " +
                "However, if the Player clicks on the orange boxes during this period of time, they will be able to receive various bonuses for their weapon.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage7 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookImage(new Rectangle(25, 0, 310, 230), ModContent.Request<Texture2D>(AssetDirectory.UI + "Books/AmmoSwapImage").Value, Color.White));
            AddElement(new UIBookText(new Rectangle(45, 225, 300, 1), "Ammo Swapping", Color.Gray, 1.5f));
            AddElement(new UIBookText(new Rectangle(40, 275, 280, 1), "To build on the theme of Ranger's powerful versatility, we added a " +
                "much needed feature of being able to quickly swap between ammo types to best utilize the numerous vanilla ammos and the " +
                "ammos we add in the future.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage8 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(20, 0, 280, 1), "This can be activated by binding a key to activate Quick Swapping " +
                "within settings. \n\n" +
                "As the combo class, this will be crucial in swapping between different ammo types that synegize with " +
                "each other or have special use cases for a niche situation\n\n" +
                "For example, being able to detonate gas clouds using Fire Arrows, or swapping to Meteor Bullets " +
                "while in an enclosed space. Whichever the case, we hope it will vastly improve the decision making for Ranger moving forward.", Color.Black, 0.9f));
        }
    }

    public class TestBookPage9 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(45, 0, 300, 1), "Planned Changes", Color.Gray, 1.5f));
            AddElement(new UIBookText(new Rectangle(40, 40, 280, 1), "There is still a lot of ideas and features we want to add to Ranger, so consider " +
                "this a brief road map on where we want to arrive at. Within the coming months we hope to add", Color.Black, 0.9f));
            AddElement(new UIBookText(new Rectangle(40, 180, 280, 1), "- More Gun Family Support, such Pistols or Muskets\n" +
                "- Vanilla Ranger Armor Reworks\n" +
                "- Bow Families, particularly 'Minion' based bows for cross-Summoner support\n" +
                "- Accessory Diversity, to support a wider range of viable playstyles", Color.Black, 0.9f));
        }
    }
}