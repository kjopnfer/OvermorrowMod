using OvermorrowMod.Projectiles.Accessory;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod
{
    public class OvermorrowModPlayer : ModPlayer
    {
        public bool StormScale;
		private int sparkCounter;

        public override void ResetEffects()
        {
            StormScale = false;
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

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
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
			base.UpdateEquips(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
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