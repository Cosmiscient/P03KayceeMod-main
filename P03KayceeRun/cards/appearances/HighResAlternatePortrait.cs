using DiskCardGame;
using InscryptionAPI.Card;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Cards
{
    public class HighResAlternatePortrait : CardAppearanceBehaviour
    {
        public static Appearance ID { get; private set; }

        public class DynamicPortrait : DynamicCardPortrait
        {
            public override void ApplyCardInfo(CardInfo card)
            {
                SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                renderer.sprite = card.alternatePortrait;
            }
        }

        private static GameObject prefabPortrait = null;

        internal static GameObject CloneSpecialPortrait()
        {
            CardInfo mole = CardLoader.GetCardByName("Mole_Telegrapher");
            GameObject myObj = Instantiate(mole.AnimatedPortrait);
            SpriteRenderer rend = myObj.GetComponentInChildren<SpriteRenderer>();
            myObj.AddComponent<DynamicPortrait>();
            return myObj;
        }

        public override void ApplyAppearance()
        {
            if (prefabPortrait == null)
            {
                prefabPortrait = CloneSpecialPortrait();
            }

            Card.RenderInfo.prefabPortrait = prefabPortrait;
            Card.RenderInfo.hidePortrait = true;
            Card.renderInfo.hiddenCost = true;
        }

        static HighResAlternatePortrait()
        {
            ID = CardAppearanceBehaviourManager.Add(P03Plugin.PluginGuid, "HighResAlternatePortrait", typeof(HighResAlternatePortrait)).Id;
        }
    }
}