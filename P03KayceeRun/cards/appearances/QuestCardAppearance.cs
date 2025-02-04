using DiskCardGame;
using InscryptionAPI.Card;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Cards
{
    public class QuestCardAppearance : DiscCardColorAppearance
    {
        public static new Appearance ID { get; private set; }

        public override Color? BorderColor => GameColors.Instance.darkBlue;

        static QuestCardAppearance()
        {
            ID = CardAppearanceBehaviourManager.Add(P03Plugin.PluginGuid, "QuestCardAppearance", typeof(QuestCardAppearance)).Id;
        }
    }
}