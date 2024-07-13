using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archipelago.ARobotNamedFight
{
	public static class References
	{
		public const string GAME_NAME = "A Robot Named Fight!";

		public static bool ExitingGame = false;

		public static Dictionary<GameMode, int> LocationIDOffsetPerGameMode = new Dictionary<GameMode, int>()
		{
			{ GameMode.Normal, 0 },
			{ GameMode.ClassicBossRush, 50 },
			{ GameMode.Exterminator, 100 },
		};

		public static Dictionary<GameMode, int> LocationIDUpperBoundExclusivePerGameMode = new Dictionary<GameMode, int>()
		{
			{ GameMode.Normal, 36 },
			{ GameMode.ClassicBossRush, 63 },
			{ GameMode.Exterminator, 147 },
		};

		public static List<MajorItem> ActivatedItemList { get; private set; } = new List<MajorItem>()
		{
			MajorItem.DamageBoostAura,
			MajorItem.AttackBoostAura,
			MajorItem.SpeedBoostAura,
			MajorItem.CelestialCharge,
			MajorItem.CloakingDevice,
			MajorItem.EnergyAxe,
			MajorItem.ETHChip,
			MajorItem.TheGlitchedKey,
			MajorItem.HTSChip,
			MajorItem.ModuleTransmogrifier,
			MajorItem.NanoswarmGenerator,
			MajorItem.OrphielsAltar,
			MajorItem.PersonalTeleporter,
			MajorItem.PhantasmalOrbs,
			MajorItem.PowerShield,
			MajorItem.RadialBolts,
			MajorItem.RepairKit,
			MajorItem.SearchBurst,
			MajorItem.SedativeCloud,
			MajorItem.SpontaneousGenerator,
			MajorItem.ToxinCloud,
			MajorItem.WaveBomb,
			MajorItem.ZurvansPentangle,
		};

		private static Dictionary<GameMode, List<MajorItem>> MajorItemBlacklistByGameMode { get; set; } = new Dictionary<GameMode, List<MajorItem>>()
		{
			{
				GameMode.Normal, new List<MajorItem>()
				{
					MajorItem.None,

					MajorItem.Arachnomorph,
					MajorItem.BrightShell,
					MajorItem.BuzzsawShell,
					MajorItem.CognitiveStabilizer,
					MajorItem.Dash,
					MajorItem.DiveShell,
					MajorItem.DoubleJump,
					MajorItem.GlowBot,
					MajorItem.HeatShell,
					MajorItem.HoverBoots,
					MajorItem.Infinijump,
					MajorItem.JetPack,
					MajorItem.PhaseShell,
					MajorItem.PowerJump,
					MajorItem.Slide,
					MajorItem.UpDog,
					MajorItem.ViridianShell,

					MajorItem.ElectroCharge,
					MajorItem.ExplosiveBolt,
					MajorItem.FireBolt,

					MajorItem.BuzzsawGun,
					MajorItem.Flamethrower,
					MajorItem.HellfireCannon,
					MajorItem.Kaboomerang,
					MajorItem.LaserBeam,
					MajorItem.LightningGun,
					MajorItem.NecroluminantSpray,
					MajorItem.Phaserang,
					MajorItem.PhaseShot,
					MajorItem.PulseMGL,
					MajorItem.RailGun,
					MajorItem.RocketLauncher,

					MajorItem.TheRedKey,
					MajorItem.TheGreenKey,
					MajorItem.TheBlueKey,
					MajorItem.TheBlackKey,

					//MajorItem.DamageBoostAura,
					//MajorItem.AttackBoostAura,
					//MajorItem.SpeedBoostAura,
					//MajorItem.CelestialCharge,
					//MajorItem.CloakingDevice,
					//MajorItem.EnergyAxe,
					//MajorItem.ETHChip,
					//MajorItem.TheGlitchedKey,
					//MajorItem.HTSChip,
					//MajorItem.ModuleTransmogrifier,
					//MajorItem.NanoswarmGenerator,
					//MajorItem.OrphielsAltar,
					//MajorItem.PersonalTeleporter,
					//MajorItem.PhantasmalOrbs,
					//MajorItem.PowerShield,
					//MajorItem.RadialBolts,
					//MajorItem.RepairKit,
					//MajorItem.SearchBurst,
					//MajorItem.SedativeCloud,
					//MajorItem.SpontaneousGenerator,
					//MajorItem.ToxinCloud,
					//MajorItem.WaveBomb,
					//MajorItem.ZurvansPentangle,
				}
			},
			{
				GameMode.ClassicBossRush, new List<MajorItem>()
				{
					MajorItem.None,
				}
			},
			{
				GameMode.Exterminator, new List<MajorItem>()
				{
					MajorItem.None,

					MajorItem.Arachnomorph,
					MajorItem.BrightShell,
					MajorItem.BuzzsawShell,
					MajorItem.CognitiveStabilizer,
					MajorItem.Dash,
					MajorItem.DiveShell,
					MajorItem.DoubleJump,
					MajorItem.GlowBot,
					MajorItem.HeatShell,
					MajorItem.HoverBoots,
					MajorItem.Infinijump,
					MajorItem.JetPack,
					MajorItem.PhaseShell,
					MajorItem.PowerJump,
					MajorItem.Slide,
					MajorItem.UpDog,
					MajorItem.ViridianShell,

					MajorItem.ElectroCharge,
					MajorItem.ExplosiveBolt,
					MajorItem.FireBolt,

					MajorItem.BuzzsawGun,
					MajorItem.Flamethrower,
					MajorItem.HellfireCannon,
					MajorItem.Kaboomerang,
					MajorItem.LaserBeam,
					MajorItem.LightningGun,
					MajorItem.NecroluminantSpray,
					MajorItem.Phaserang,
					MajorItem.PhaseShot,
					MajorItem.PulseMGL,
					MajorItem.RailGun,
					MajorItem.RocketLauncher,

					MajorItem.TheRedKey,
					MajorItem.TheGreenKey,
					MajorItem.TheBlueKey,
					MajorItem.TheBlackKey,

					//MajorItem.DamageBoostAura,
					//MajorItem.AttackBoostAura,
					//MajorItem.SpeedBoostAura,
					//MajorItem.CelestialCharge,
					//MajorItem.CloakingDevice,
					//MajorItem.EnergyAxe,
					//MajorItem.ETHChip,
					//MajorItem.TheGlitchedKey,
					//MajorItem.HTSChip,
					//MajorItem.ModuleTransmogrifier,
					//MajorItem.NanoswarmGenerator,
					//MajorItem.OrphielsAltar,
					//MajorItem.PersonalTeleporter,
					//MajorItem.PhantasmalOrbs,
					//MajorItem.PowerShield,
					//MajorItem.RadialBolts,
					//MajorItem.RepairKit,
					//MajorItem.SearchBurst,
					//MajorItem.SedativeCloud,
					//MajorItem.SpontaneousGenerator,
					//MajorItem.ToxinCloud,
					//MajorItem.WaveBomb,
					//MajorItem.ZurvansPentangle,
				}
			}
		};

		public static bool MajorItemIsBlacklisted(MajorItem item)
		{
			GameMode mode = SaveGameManager.activeSlot.activeGameData.gameMode;

			if (MajorItemBlacklistByGameMode.ContainsKey(mode))
			{
				return MajorItemBlacklistByGameMode[mode].Contains(item);
			}

			if (item == MajorItem.None) return true;
			
			return false;
		}

		public static long GetGameModeOffset()
		{
			GameMode mode = SaveGameManager.activeSlot.activeGameData.gameMode;

			if (LocationIDOffsetPerGameMode.ContainsKey(mode))
			{
				return LocationIDOffsetPerGameMode[mode];
			}

			return 0;
		}

		public static long GetGameModeUpperBoundExclusive()
		{
			GameMode mode = SaveGameManager.activeSlot.activeGameData.gameMode;

			if (LocationIDUpperBoundExclusivePerGameMode.ContainsKey(mode))
			{
				return LocationIDUpperBoundExclusivePerGameMode[mode];
			}

			return 0;
		}
	}
}
