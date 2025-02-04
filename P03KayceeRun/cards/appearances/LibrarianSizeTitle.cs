using DiskCardGame;
using Infiniscryption.P03KayceeRun.Patchers;
using InscryptionAPI.Card;

namespace Infiniscryption.P03KayceeRun.Cards
{
    public class LibrarianSizeTitle : CardAppearanceBehaviour
    {
        public static readonly Appearance ID = CardAppearanceBehaviourManager.Add(P03Plugin.PluginGuid, "LibrarySizeTitle", typeof(LibrarianSizeTitle)).Id;

        private static readonly int[] damages = new int[] { 4, 3, 2, 1 };
        public override void ApplyAppearance()
        {
            if (SaveFile.IsAscension)
            {
                Card.renderInfo.nameOverride = Card.Info.DisplayedNameLocalized + " <" + damages[EventManagement.CompletedZones.Count].ToString() + ">";
            }
        }
    }
}