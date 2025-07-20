using System;
using Terraria;
using Terraria.DataStructures;

namespace OvermorrowMod.Core.Items.Accessories
{
    public abstract class AccessoryKeyword { }
    public class RetaliateKeyword : AccessoryKeyword { }
    public class StrikeKeyword : AccessoryKeyword { }
    public class StrikeProjectileKeyword : AccessoryKeyword { }
    public class VigorKeyword : AccessoryKeyword { }
    public class DeathsDoorKeyword : AccessoryKeyword { }
    public class MindDownKeyword : AccessoryKeyword { }
    public class ExecuteKeyword : AccessoryKeyword { }
    public class AmbushKeyword : AccessoryKeyword { }
    public class AerialKeyword : AccessoryKeyword { }
    public class AwakenedKeyword : AccessoryKeyword { }
    public class FocusKeyword : AccessoryKeyword { }
    public class ReloadKeyword : AccessoryKeyword { }
    public class MisfireKeyword : AccessoryKeyword { }
    public class QuickslotKeyword : AccessoryKeyword { }
    public class RespiteKeyword : AccessoryKeyword { }
    public class SecondaryKeyword : AccessoryKeyword { }
    public class TrueMeleeKeyword : AccessoryKeyword { }


    public class ProjectileSpawnKeyword : AccessoryKeyword { }
    public class ProjectileModifyHitKeyword : AccessoryKeyword { }
    public class ProjectileKillKeyword : AccessoryKeyword { }

    public static class AccessoryKeywords
    {
        public static event Action<Player, NPC, Player.HurtInfo> OnRetaliate;
        public static event Action<Player, NPC, NPC.HitInfo, int> OnStrike;
        public static event Action<Player, Projectile, NPC, NPC.HitInfo, int> OnStrikeProjectile;
        public static event Action<Player> OnVigor;
        public static event Action<Player> OnDeathsDoor;
        public static event Action<Player> OnMindDown;
        public static event Action<Player, NPC> OnExecute;
        public static event Action<Player, NPC, NPC.HitInfo, int> OnAmbush;
        public static event Action<Player> OnAerial;
        public static event Action<Player> OnAwakened;
        public static event Action<Player, Projectile> OnFocus;
        public static event Action<Player, bool> OnReload;
        public static event Action<Player> OnMisfire;
        public static event Action<Player, Item> OnQuickslot;
        public static event Action<Player> OnRespite;
        public static event Action<Player, Item> OnSecondary;
        //public static event Action<Player, Item, Projectile, NPC, NPC.HitModifiers> OnTrueMelee;

        public delegate void TrueMeleeDelegate(Player player, Item item, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers);
        public static event TrueMeleeDelegate OnTrueMelee;

        public static event Action<Player, Projectile, IEntitySource> OnProjectileSpawn;
        public static event Action<Player, Projectile, NPC, NPC.HitModifiers> OnProjectileModifyHit;
        public static event Action<Player, Projectile, int> OnProjectileKill;

        public static void TriggerRetaliate(Player player, NPC attacker, Player.HurtInfo hurtInfo) => OnRetaliate?.Invoke(player, attacker, hurtInfo);
        public static void TriggerStrike(Player player, NPC target, NPC.HitInfo hit, int damageDone) => OnStrike?.Invoke(player, target, hit, damageDone);
        public static void TriggerProjectileStrike(Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnStrikeProjectile?.Invoke(player, projectile, target, hit, damageDone);
        }
        public static void TriggerVigor(Player player) => OnVigor?.Invoke(player);
        public static void TriggerDeathsDoor(Player player) => OnDeathsDoor?.Invoke(player);
        public static void TriggerMindDown(Player player) => OnMindDown?.Invoke(player);
        public static void TriggerExecute(Player player, NPC killedNPC) => OnExecute?.Invoke(player, killedNPC);
        public static void TriggerAmbush(Player player, NPC target, NPC.HitInfo hit, int damageDone) => OnAmbush?.Invoke(player, target, hit, damageDone);
        public static void TriggerAerial(Player player) => OnAerial?.Invoke(player);
        public static void TriggerAwakened(Player player) => OnAwakened?.Invoke(player);
        public static void TriggerFocus(Player player, Projectile projectile) => OnFocus?.Invoke(player, projectile);
        public static void TriggerReload(Player player, bool wasSuccessful) => OnReload?.Invoke(player, wasSuccessful);
        public static void TriggerMisfire(Player player) => OnMisfire?.Invoke(player);
        public static void TriggerQuickslot(Player player, Item item) => OnQuickslot?.Invoke(player, item);
        public static void TriggerRespite(Player player) => OnRespite?.Invoke(player);
        public static void TriggerSecondary(Player player, Item item) => OnSecondary?.Invoke(player, item);
        //public static void TriggerTrueMelee(Player player, Item item, Projectile projectile, NPC target, NPC.HitModifiers modifiers) =>OnTrueMelee?.Invoke(player, item, projectile, target, modifiers);
        public static void TriggerTrueMelee(Player player, Item item, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) => OnTrueMelee?.Invoke(player, item, projectile, target, ref modifiers);
        public static void TriggerProjectileSpawn(Player player, Projectile projectile, IEntitySource source) => OnProjectileSpawn?.Invoke(player, projectile, source);
        public static void TriggerProjectileModifyHit(Player player, Projectile projectile, NPC target, NPC.HitModifiers modifiers) => OnProjectileModifyHit?.Invoke(player, projectile, target, modifiers);
        public static void TriggerProjectileKill(Player player, Projectile projectile, int timeLeft) => OnProjectileKill?.Invoke(player, projectile, timeLeft);
    }

    public static class AccessoryKeywordTypes
    {
        public static readonly Type Retaliate = typeof(RetaliateKeyword);
        public static readonly Type Strike = typeof(StrikeKeyword);
        public static readonly Type StrikeProjectile = typeof(StrikeProjectileKeyword);
        public static readonly Type Vigor = typeof(VigorKeyword);
        public static readonly Type DeathsDoor = typeof(DeathsDoorKeyword);
        public static readonly Type MindDown = typeof(MindDownKeyword);
        public static readonly Type Execute = typeof(ExecuteKeyword);
        public static readonly Type Ambush = typeof(AmbushKeyword);
        public static readonly Type Aerial = typeof(AerialKeyword);
        public static readonly Type Awakened = typeof(AwakenedKeyword);
        public static readonly Type Focus = typeof(FocusKeyword);
        public static readonly Type Reload = typeof(ReloadKeyword);
        public static readonly Type Misfire = typeof(MisfireKeyword);
        public static readonly Type Quickslot = typeof(QuickslotKeyword);
        public static readonly Type Respite = typeof(RespiteKeyword);
        public static readonly Type Secondary = typeof(SecondaryKeyword);
        public static readonly Type TrueMelee = typeof(TrueMeleeKeyword);

        public static readonly Type ProjectileSpawn = typeof(ProjectileSpawnKeyword);
        public static readonly Type ProjectileModifyHit = typeof(ProjectileModifyHitKeyword);
        public static readonly Type ProjectileKill = typeof(ProjectileKillKeyword);
    }
}