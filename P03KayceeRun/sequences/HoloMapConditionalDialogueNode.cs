using System.Collections;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Faces;
using Infiniscryption.P03KayceeRun.Patchers;
using Infiniscryption.P03KayceeRun.Quests;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Sequences
{
    [HarmonyPatch]
    public class HoloMapConditionalDialogueNode : HoloMapDialogueNode
    {
        [HarmonyPatch(typeof(HoloFloatingLabel), nameof(HoloFloatingLabel.ManagedUpdate))]
        [HarmonyPrefix]
        private static bool DontIfLabelIsNull(HoloFloatingLabel __instance) => __instance.line != null;

        public override void OnCursorSelectEnd()
        {
            SetHoveringEffectsShown(false);
            OnSelected();
            StartCoroutine(DialogueThenStorySequence());
        }

        public override void OnCursorEnter()
        {
            label.gameObject.SetActive(true);
            QuestDefinition quest = QuestManager.Get(eventId);
            label.SetText(Localization.Translate(quest.CurrentState.NPCHoverText));
            base.OnCursorEnter();
        }

        public override void OnCursorExit()
        {
            label.gameObject.SetActive(false);
            base.OnCursorExit();
        }

        private IEnumerator DialogueThenStorySequence()
        {
            // Go ahead and get a reference to the quest
            QuestDefinition quest = QuestManager.Get(eventId);

            // This happens when you're coming back for a reward that you
            // couldn't get before. We track this here so we don't accidentally
            // increments stats too much (i.e., "complete" an "already completed"
            // quest)
            bool hasCompletedAlready = quest.IsCompleted;

            // Time to figure out if this is a special node. If so, we hand it off
            // and back out
            if (!hasCompletedAlready)
            {
                if (quest.CurrentState.SpecialNodeData is CardBattleNodeData)
                {
                    GameFlowManager.Instance.TransitionToGameState(GameState.CardBattle, quest.CurrentState.SpecialNodeData);
                    yield break;
                }
                if (quest.CurrentState.SpecialNodeData is SpecialNodeData)
                {
                    GameFlowManager.Instance.TransitionToGameState(GameState.SpecialCardSequence, quest.CurrentState.SpecialNodeData);
                    yield break;
                }
            }

            MapNodeManager.Instance.SetAllNodesInteractable(false);
            (GameFlowManager.Instance as Part3GameFlowManager).DisableTransitionToFirstPerson = true;
            ViewManager.Instance.Controller.LockState = ViewLockState.Locked;

            NPCDescriptor npc = NPCDescriptor.GetDescriptorForNPC(eventId);
            P03ModularNPCFace.Instance.SetNPCFace(npc.faceCode);

            yield return HoloGameMap.Instance.FlickerHoloElements(false, 1);
            ViewManager.Instance.SwitchToView(View.P03Face, false, false);
            yield return new WaitForSeconds(0.1f);
            P03AnimationController.Instance.SwitchToFace(npc.P03Face, true, true);
            yield return new WaitForSeconds(0.1f);

            // Need to play the dialogue associated with the current state of the quest
            string dialogueId = quest.CurrentState.DynamicDialogueId != null ? quest.CurrentState.DynamicDialogueId() : quest.CurrentState.DialogueId;
            yield return TextDisplayer.Instance.PlayDialogueEvent(dialogueId, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[] { quest.GetQuestCounter().ToString() }, null);
            yield return new WaitForSeconds(0.1f);

            // Now we advance the quest if necessary (if this is an autocomplete quest state)
            if (quest.CurrentState.AutoComplete)
                quest.CurrentState.Status = QuestState.QuestStateStatus.Success;

            // Now we play all quest rewards that haven't yet been granted
            ViewManager.Instance.SwitchToView(View.Default);
            yield return quest.GrantAllUngrantedRewards();

            if (!hasCompletedAlready && quest.IsCompleted && quest.CurrentState.Status == QuestState.QuestStateStatus.Success)
                AscensionStatsData.TryIncrementStat(StatManagement.QUESTS_COMPLETED);

            // Reset back to normal game state
            ViewManager.Instance.SwitchToView(View.MapDefault, false, false);
            yield return new WaitForSeconds(0.15f);
            HoloGameMap.Instance.StartCoroutine(HoloGameMap.Instance.FlickerHoloElements(true, 2));
            MapNodeManager.Instance.SetAllNodesInteractable(true);
            (GameFlowManager.Instance as Part3GameFlowManager).DisableTransitionToFirstPerson = false;
            ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;

            P03Plugin.Log.LogDebug($"After dialogue, the current state of the quest is {quest.CurrentState.StateName} with a status of {quest.CurrentState.Status}. Is the quest completed? {quest.IsCompleted}");

            TryResolveQuest(quest);

            yield break;
        }

        internal void TryResolveQuest(QuestDefinition quest)
        {
            if (quest.IsCompleted)
            {
                if (!quest.HasUngrantedRewards)
                {
                    SetCompleted();
                    this.npc?.SetActive(false);
                }
                else
                {
                    SetHidden(false, false);
                }

                // Check to see if we should unlock the "every quest" achievement
                // This happens if you've completed four full quests and you're in the final zone
                if (EventManagement.CompletedZones.Count == 3)
                {
                    if (QuestManager.AllQuestDefinitions.Count(
                                              q => !q.IsSpecialQuest
                                           && q.IsCompleted
                                           && q.CurrentState.Status == QuestState.QuestStateStatus.Success
                                           && q.IsEndOfQuest) == 4)
                    {
                        AchievementManager.Unlock(P03AchievementManagement.ALL_QUESTS_COMPLETED);
                    }
                }
            }
            else
            {
                SetHidden(false, false);
            }
        }

        [SerializeField]
        public SpecialEvent eventId;

        [SerializeField]
        public HoloFloatingLabel label;
    }
}