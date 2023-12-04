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

        public static List<MinorItemType> MinorItemBlacklist { get; private set; } = new List<MinorItemType>();
        //{ MinorItemType.BlueScrap, MinorItemType.RedScrap, MinorItemType.GreenScrap, MinorItemType.GlitchScrap };
        public static List<MajorItem> MajorItemBlacklist { get; private set; } = new List<MajorItem>()
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
    }
}
