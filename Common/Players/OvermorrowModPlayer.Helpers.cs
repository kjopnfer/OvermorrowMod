using OvermorrowMod.Common.DrawLayers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Players
{
    public partial class OvermorrowModPlayer : ModPlayer
    {

        private int FindFlaskBuff()
        {
            for (int i = 0; i < Player.buffType.Length; i++)
            {
                switch (Player.buffType[i])
                {
                    case BuffID.WeaponImbueCursedFlames:
                        return BuffID.CursedInferno;
                    case BuffID.WeaponImbueFire:
                        return BuffID.OnFire;
                    case BuffID.WeaponImbueGold:
                        return BuffID.Midas;
                    case BuffID.WeaponImbueIchor:
                        return BuffID.Ichor;
                    case BuffID.WeaponImbueNanites:
                        return BuffID.Confused;
                    case BuffID.WeaponImbuePoison:
                        return BuffID.Poisoned;
                    case BuffID.WeaponImbueVenom:
                        return BuffID.Venom;
                }
            }

            return -1;
        }

        private void ApplyFlaskBuffs(NPC target)
        {
            int buffID = FindFlaskBuff();
            if (buffID != -1)
            {
                target.AddBuff(buffID, 360);
            }
        }
    }
}