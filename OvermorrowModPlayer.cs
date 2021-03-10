using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Debuffs;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public class OvermorrowModPlayer : ModPlayer
    {
		// Accessories
		public bool BloodyHeart;
		public bool BloodyTeeth;
		public bool DripplerEye;
		public bool ShatteredOrb;
        public bool StormScale;

		// Accessory Counters
		public int dripplerStack;
		private int sparkCounter;

		// Buffs
		public bool mirrorBuff;
		public bool moonBuff;
		public bool treeBuff;

        public override void ResetEffects()
        {
			BloodyHeart = false;
			BloodyTeeth = false;
			DripplerEye = false;
			ShatteredOrb = false;
            StormScale = false;

			mirrorBuff = false;
			moonBuff = false;
			treeBuff = false;
        }

		// In MP, other clients need accurate information about your player or else bugs happen.
		// clientClone, SyncPlayer, and SendClientChanges, ensure that information is correct.
		// We only need to do this for data that is changed by code not executed by all clients, 
		// or data that needs to be shared while joining a world.
		// For example, examplePet doesn't need to be synced because all clients know that the player is wearing the ExamplePet item in an equipment slot. 
		// The examplePet bool is set for that player on every clients computer independently (via the Buff.Update), keeping that data in sync.
		// ExampleLifeFruits, however might be out of sync. For example, when joining a server, we need to share the exampleLifeFruits variable with all other clients.
		// In addition, in ExampleUI we have a button that toggles "Non-Stop Party". We need to sync this whenever it changes.
		public override void clientClone(ModPlayer clientClone)
		{
			OvermorrowModPlayer clone = clientClone as OvermorrowModPlayer;
			// Here we would make a backup clone of values that are only correct on the local players Player instance.
			// Some examples would be RPG stats from a GUI, Hotkey states, and Extra Item Slots
		}


		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (BloodyTeeth)
			{
				int bleedChance = Main.rand.Next(4);
				if(bleedChance == 0 && item.melee)
				{
					target.AddBuff(ModContent.BuffType<Bleeding>(), 360);
				}
			}
		}

		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (BloodyHeart)
			{
				int projectiles = 3;
				for (int i = 0; i < projectiles; i++)
				{
					Projectile.NewProjectile(player.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<BouncingBlood>(), 19, 2, player.whoAmI);
				}
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			if (mirrorBuff)
			{
				damage /= 2;
			}
			base.ModifyHitByNPC(npc, ref damage, ref crit);
		}

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			if (DripplerEye)
			{
				player.rangedCrit += dripplerStack;
			}

			if (StormScale) // Create sparks while moving, increase defense if health is below 50%
			{
				if(player.statLife <= player.statLifeMax2 * 0.5f)
				{
					player.statDefense += 5;
				}

				if(player.velocity.X != 0 || player.velocity.Y != 0)
				{
					if (sparkCounter % 30 == 0)
					{
						for (int i = 0; i < Main.rand.Next(1, 3); i++)
						{
							Projectile.NewProjectile(player.Center.X, player.Center.Y + Main.rand.Next(-15, 18), 0, 0, ModContent.ProjectileType<ElectricSparksFriendly>(), 20, 1, player.whoAmI, 0, 0);
						}
					}
					sparkCounter++;
				}
			}

			if (moonBuff)
			{
				player.allDamage += .25f;
			}

			base.UpdateEquips(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
		}

		public override void UpdateLifeRegen()
		{
			if (treeBuff)
			{
				// lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 2 life gained per second.
				player.lifeRegen += 4;
			}
		}

		// Synchronization Code
		/*public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = mod.GetPacket();
			packet.Write((byte)ExampleModMessageType.ExamplePlayerSyncPlayer);
			packet.Write((byte)player.whoAmI);
			packet.Write(exampleLifeFruits);
			packet.Write(nonStopParty); // While we sync nonStopParty in SendClientChanges, we still need to send it here as well so newly joining players will receive the correct value.
			packet.Send(toWho, fromWho);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			// Here we would sync something like an RPG stat whenever the player changes it.
			ExamplePlayer clone = clientPlayer as ExamplePlayer;
			if (clone.nonStopParty != nonStopParty)
			{
				// Send a Mod Packet with the changes.
				var packet = mod.GetPacket();
				packet.Write((byte)ExampleModMessageType.NonStopPartyChanged);
				packet.Write((byte)player.whoAmI);
				packet.Write(nonStopParty);
				packet.Send();
			}
		}*/


	}
}