using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;

namespace OvermorrowMod.Common.Weapons.Guns
{
    public class GunBuilder
    {
        private HeldGun gun;

        public GunBuilder(HeldGun gun)
        {
            this.gun = gun;
        }

        public GunBuilder AsType(GunType type)
        {
            switch (type)
            {
                case GunType.Revolver:
                    GunTemplates.ApplyRevolverDefaults(gun);
                    break;
                case GunType.Shotgun:
                    GunTemplates.ApplyShotgunDefaults(gun);
                    break;
                case GunType.Handgun:
                    GunTemplates.ApplyHandgunDefaults(gun);
                    break;
                case GunType.SubMachineGun:
                    GunTemplates.ApplySubMachineGunDefaults(gun);
                    break;
                case GunType.MachineGun:
                    GunTemplates.ApplyMachineGunDefaults(gun);
                    break;
                case GunType.Sniper:
                    GunTemplates.ApplySniperDefaults(gun);
                    break;
            }
            return this;
        }

        public GunBuilder WithMaxShots(int maxShots)
        {
            gun.MaxShots = maxShots;
            return this;
        }

        public GunBuilder WithReloadTime(int reloadTime)
        {
            gun.MaxReloadTime = reloadTime;
            return this;
        }

        public GunBuilder WithRecoil(int recoil)
        {
            gun.RecoilAmount = recoil;
            return this;
        }

        public GunBuilder WithSound(SoundStyle sound)
        {
            gun.ShootSound = sound;
            return this;
        }

        public GunBuilder WithReloadSound(SoundStyle sound)
        {
            gun.ReloadFinishSound = sound;
            return this;
        }

        public GunBuilder WithReloadZones(params (int start, int end)[] zones)
        {
            gun.ClickZones = zones.Select(z => new ReloadZone(z.start, z.end)).ToList();
            return this;
        }

        public GunBuilder WithPositionOffset(Vector2 leftOffset, Vector2 rightOffset)
        {
            gun.PositionOffsetValue = (leftOffset, rightOffset);
            return this;
        }

        public GunBuilder WithBulletPosition(Vector2 leftPos, Vector2 rightPos)
        {
            gun.BulletShootPositionValue = (leftPos, rightPos);
            return this;
        }

        public GunBuilder WithScale(float scale)
        {
            gun.ProjectileScaleValue = scale;
            return this;
        }

        public GunBuilder TwoHanded(bool twoHanded = true)
        {
            gun.TwoHandedValue = twoHanded;
            return this;
        }

        public GunBuilder CanRightClick(bool canRightClick = true)
        {
            gun.CanRightClickValue = canRightClick;
            return this;
        }

        public GunBuilder WithRightClickDelay(bool usesDelay = true)
        {
            gun.UsesRightClickDelay = usesDelay;
            return this;
        }

        public HeldGun Build()
        {
            return gun;
        }
    }
}