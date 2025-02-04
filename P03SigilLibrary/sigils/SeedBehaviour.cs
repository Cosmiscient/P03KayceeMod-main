using System.Collections;
using System.Linq;
using DiskCardGame;
using InscryptionAPI.Card;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class SeedBehaviour : SpecialCardBehaviour
    {
        public static SpecialTriggeredAbility AbilityID => SpecialTriggeredAbilityManager.Add(P03SigilLibraryPlugin.PluginGuid, "SeedBehaviour", typeof(SeedBehaviour)).Id;

        private int triggerPriority;
        public override int Priority => triggerPriority;

        public override bool RespondsToUpkeep(bool playerUpkeep) => PlayableCard.OpponentCard != playerUpkeep && PlayableCard.FaceDown;

        public override bool RespondsToResolveOnBoard() => true;

        public override IEnumerator OnResolveOnBoard()
        {
            yield return OnTurnEnd(!PlayableCard.OpponentCard);
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            ViewManager.Instance.SwitchToView(View.Board, false, true);
            yield return new WaitForSeconds(0.15f);
            PlayableCard.SetFaceDown(false, false);
            PlayableCard.UpdateFaceUpOnBoardEffects();
            yield return new WaitForEndOfFrame();

            CardInfo newCard = CardLoader.GetCardByName("Tree_Hologram");
            newCard.mods = new(Card.Info.Mods.Select(m => (CardModificationInfo)m.Clone()));
            yield return PlayableCard.TransformIntoCard(newCard);

            yield return new WaitForSeconds(0.3f);
            triggerPriority = int.MinValue;
            yield break;
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd) => PlayableCard.OpponentCard != playerTurnEnd && !PlayableCard.FaceDown;

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
            View currentView = ViewManager.Instance.CurrentView;
            ViewManager.Instance.SwitchToView(View.Board, false, true);
            yield return new WaitForSeconds(0.15f);
            Card.SetCardbackSubmerged();
            Card.SetFaceDown(true, false);
            yield return new WaitForSeconds(0.3f);
            triggerPriority = int.MaxValue;
            ViewManager.Instance.SwitchToView(currentView, false, false);
            ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;
            yield break;
        }
    }
}