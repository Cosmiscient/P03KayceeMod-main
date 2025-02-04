using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class AcceleratedLifecycle : ActivatedAbilityBehaviour
    {
        public override Ability Ability => AbilityID;
        public static Ability AbilityID { get; private set; }

        internal static List<CardSlot> BuffedSlots = new();

        private bool ActivatedThisTurn = false;

        public override int EnergyCost => 1;

        static AcceleratedLifecycle()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Rapid Recycle";
            info.rulebookDescription = "Pay 1 Energy to choose another card you control to die and immediately respawn. You may only activate this ability once per turn.";
            info.canStack = false;
            info.powerLevel = 3;
            info.opponentUsable = true;
            info.activated = true;
            info.passive = false;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(AcceleratedLifecycle),
                TextureHelper.GetImageAsTexture("ability_lifecycle.png", typeof(AcceleratedLifecycle).Assembly)
            ).Id;
        }

        public override bool RespondsToUpkeep(bool playerUpkeep) => true;

        private int OpponentRecyclePriority(CardSlot slot)
        {
            try
            {
                if (!slot.Card)
                    return 0;

                if (slot.Card == Card)
                    return 0;

                if (slot.Card.AllAbilities().Any(ab => AbilityManager.AllAbilities.AbilityByID(ab).AbilityBehavior.IsSubclassOf(typeof(Latch))))
                {
                    return slot.Card.HasAbility(LatchDeathLatch.AbilityID) ? 20 : slot.Card.HasAbility(Ability.LatchBrittle) ? 15 : 10;
                }

                if (slot.Card.HasAbility(Ability.ExplodeOnDeath))
                {
                    // If the bomb is adjacent to the angel, no
                    int powerLevelCount = 0;
                    foreach (CardSlot adj in BoardManager.Instance.GetAdjacentSlots(slot))
                    {
                        if (adj == Card.slot)
                            return 0;

                        if (adj.Card != null)
                            powerLevelCount -= adj.Card.PowerLevel;
                    }

                    CardSlot op = slot.Card.OpposingSlot();
                    if (op.Card != null)
                        powerLevelCount += op.Card.PowerLevel;

                    return powerLevelCount;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public override IEnumerator OnUpkeep(bool playerUpkeep)
        {
            if (playerUpkeep && !Card.OpponentCard)
            {
                ActivatedThisTurn = false;
                yield break;
            }
            else if (!playerUpkeep && Card.OpponentCard)
            {
                // If we have a latch card, we want to kill it:
                List<CardSlot> possibles = BoardManager.Instance.OpponentSlotsCopy;
                possibles.Sort((a, b) => OpponentRecyclePriority(b) - OpponentRecyclePriority(a));
                if (OpponentRecyclePriority(possibles[0]) > 0)
                {
                    ViewManager.Instance.SwitchToView(View.Board, false, false);
                    yield return new WaitForSeconds(0.2f);
                    yield return RecycleCard(possibles[0]);
                }
            }
        }

        public override bool CanActivate() => GetValidTargets().Count > 0 && !ActivatedThisTurn;

        private List<CardSlot> GetValidTargets()
        {
            List<CardSlot> retval = new();
            foreach (CardSlot slot in BoardManager.Instance.GetSlots(!Card.OpponentCard))
            {
                if (slot.Card != null && slot.Card != Card)
                    retval.Add(slot);
            }

            return retval;
        }

        private IEnumerator RecycleCard(CardSlot target)
        {
            Card.Anim.StrongNegationEffect();
            AudioController.Instance.PlaySound3D("angel_reveal", MixerGroup.TableObjectsSFX, Card.Slot.gameObject.transform.position, 1f, 0f, new AudioParams.Pitch(AudioParams.Pitch.Variation.Small), null, null, null, false);
            yield return new WaitForSeconds(1.0f);
            CardInfo targetInfo = target.Card.Info;
            yield return target.Card.Die(false, null, true);
            yield return BoardManager.Instance.CreateCardInSlot(targetInfo, target);
            ActivatedThisTurn = true;
            ViewManager.Instance.SwitchToView(View.Default, false, false);
        }

        public override IEnumerator Activate()
        {
            ViewManager.Instance.SwitchToView(View.Board, false, false);
            yield return new WaitForSeconds(0.2f);
            yield return BoardManager.Instance.ChooseSlot(GetValidTargets(), true);
            if (BoardManager.Instance.LastSelectedSlot == null)
            {
                ViewManager.Instance.SwitchToView(View.Default, false, false);
                yield break;
            }
            ActivatedThisTurn = true;
            yield return RecycleCard(BoardManager.Instance.LastSelectedSlot);
        }
    }
}
