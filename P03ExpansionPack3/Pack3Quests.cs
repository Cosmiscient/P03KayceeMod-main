using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Items;
using Infiniscryption.P03KayceeRun.Patchers;
using Infiniscryption.P03KayceeRun.Quests;
using Infiniscryption.PackManagement;
using InscryptionAPI.Encounters;
using InscryptionAPI.Guid;
using UnityEngine;

namespace Infiniscryption.P03ExpansionPack3
{
    [HarmonyPatch]
    internal static class Pack3Quests
    {
        internal static QuestDefinition KrakenLord { get; private set; }
        internal static QuestDefinition UfoQuest { get; private set; }
        internal static Opponent.Type UfoOpponent { get; private set; }

        internal static bool Pack3QuestsActive() => PackManager.GetActivePacks<PackInfo>().Any(pi => pi != null && !string.IsNullOrEmpty(pi.ModPrefix) && pi.ModPrefix.Equals(P03Pack3Plugin.CardPrefix));

        public class UFOBossOpponent : Part3BossOpponent
        {
            public override string PreIntroDialogueId => string.Empty;
            public override string PostDefeatedDialogueId => string.Empty;

            public override IEnumerator DefeatedPlayerSequence()
            {
                UfoQuest.CurrentState.Status = QuestState.QuestStateStatus.Failure;
                yield return base.DefeatedPlayerSequence();
                yield break;
            }

            public override IEnumerator PreDefeatedSequence()
            {
                UfoQuest.CurrentState.Status = QuestState.QuestStateStatus.Success;
                yield return base.PreDefeatedSequence();
                yield break;
            }
        }

        public class UfoBattleSequencer : BossBattleSequencer
        {
            public override Opponent.Type BossType => BossManagement.DredgerOpponent;
            public override StoryEvent DefeatedStoryEvent => GuidManager.GetEnumValue<StoryEvent>(P03Pack3Plugin.PluginGuid, "DefeatedUFO");

            public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
            {
                return new()
                {
                    opponentType = UfoOpponent,
                    opponentTurnPlan = DiskCardGame.EncounterBuilder.BuildOpponentTurnPlan(Pack3EncounterHelper.UFOBattle, 1, false)
                };
            }
        }

        [HarmonyPatch(typeof(Part3BossOpponent), nameof(Part3BossOpponent.StartingLives), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool StartingLivesForUFO(Part3BossOpponent __instance, ref int __result)
        {
            if (__instance is UFOBossOpponent)
            {
                __result = 1;
                return false;
            }
            return true;
        }

        internal static void CreatePack3Quests()
        {
            KrakenLord = QuestManager.Add(P03Pack3Plugin.PluginGuid, "KrakenQuest")
                .SetGenerateCondition(Pack3QuestsActive);

            KrakenLord.AddDialogueState("GLUG GLUG GLUG", "KrakenTransformerQuestIntro")
                      .AddDialogueState("FIGHT THE TENTACLES", "KrakenTransformerQuestStart")
                      .AddDefaultActiveState("FIGHT THE TENTACLES", "KrakenTransformerQuestInProgress", 3)
                      .AddDialogueState("DEATH TO THE TENTACLES", "KrakenTransformerQuestComplete")
                      .AddGainCardReward(P03Pack3Plugin.CardPrefix + "_Kraken");

            // Ufo Battle
            UfoQuest = QuestManager.Add(P03Pack3Plugin.PluginGuid, "UfoBattle");//.OverrideNPCDescriptor(new(P03ModularNPCFace.FaceSet.DredgerSolo, CompositeFigurine.FigurineType.Robot));

            UfoOpponent = OpponentManager.Add(P03Pack3Plugin.PluginGuid, "UFOOpponent", string.Empty, typeof(UFOBossOpponent))
                .SetNewSequencer(P03Pack3Plugin.PluginGuid, "UFOSequencer", typeof(UfoBattleSequencer))
                .Id;

            var nodeData = new CardBattleNodeData()
            {
                specialBattleId = BossBattleSequencer.GetSequencerIdForBoss(UfoOpponent),
                difficulty = 1,
                blueprint = Pack3EncounterHelper.UFOBattle
            };

            var battleState = UfoQuest.SetGenerateCondition(Pack3QuestsActive)
                         .AddDialogueState("WARNING!!!", "UfoQuestStart")
                         .AddSpecialNodeState("HERE THEY COME!!!", nodeData);

            battleState.AddDialogueState("CONGRATULATIONS!!!", "UfoQuestBattleVictory")
                       .AddGainItemReward(UfoItem.ItemData.name);

            battleState.AddDialogueState("DISASTER!!!", "UfoQuestBattleLoss", QuestState.QuestStateStatus.Failure);
        }

        [HarmonyPatch(typeof(BountyHunterGenerator), nameof(BountyHunterGenerator.TryAddBountyHunterToTurnPlan))]
        [HarmonyPrefix]
        [HarmonyPriority(HarmonyLib.Priority.VeryHigh)]
        private static bool AddBountyTargetInstead(List<List<CardInfo>> turnPlan, ref List<List<CardInfo>> __result)
        {
            if (!P03AscensionSaveData.IsP03Run)
                return true;

            if (!KrakenLord.IsDefaultActive() || (Part3SaveData.Data.battlesSinceBountyHunter >= 3 && Part3SaveData.Data.BountyTier >= 1))
                return true;

            if (turnPlan == null || turnPlan.Count < 2)
                return true;

            // So we're always going to add the tenatcle to turn 2. Always.
            Part3SaveData.Data.battlesSinceBountyHunter += 1;

            // Find the card with the lowest power level
            List<CardInfo> turnTwo = turnPlan[1];
            if (turnTwo.Count > 0)
            {
                int minLevel = turnTwo.Min(c => c.PowerLevel);
                turnTwo.Remove(turnTwo.First(c => c.PowerLevel == minLevel));
            }
            turnTwo.Add(CardLoader.GetCardByName(P03Pack3Plugin.CardPrefix + "_Technicle"));

            __result = turnPlan;
            return false;
        }
    }
}