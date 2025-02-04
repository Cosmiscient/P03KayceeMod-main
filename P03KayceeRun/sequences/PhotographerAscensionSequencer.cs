using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Encounters;
using Infiniscryption.P03KayceeRun.Patchers;

namespace Infiniscryption.P03KayceeRun.Sequences
{
    [HarmonyPatch]
    public class PhotographerAscensionSequencer : PhotographerBattleSequencer
    {
        public override EncounterData BuildCustomEncounter(CardBattleNodeData nodeData)
        {
            if (!SaveFile.IsAscension)
                return base.BuildCustomEncounter(nodeData);

            EncounterData encounterData = base.BuildCustomEncounter(nodeData);
            EncounterBlueprintData blueprint = EncounterHelper.PhotographerBossP1;
            encounterData.opponentTurnPlan = EncounterBuilder.BuildOpponentTurnPlan(blueprint, EventManagement.EncounterDifficulty, false);
            return encounterData;
        }


        [HarmonyPatch(typeof(Opponent), nameof(Opponent.ReplaceBlueprint))]
        [HarmonyPostfix]
        public static IEnumerator Postfix(IEnumerator sequence, string blueprintId, bool removeLockedCards = false)
        {
            if (!P03AscensionSaveData.IsP03Run || TurnManager.Instance.opponent is not PhotographerBossOpponent || !blueprintId.Equals("PhotographerBossP2"))
            {
                yield return sequence;
                yield break;
            }

            TurnManager.Instance.Opponent.Blueprint = EncounterHelper.PhotographerBossP2;

            List<List<CardInfo>> plan = EncounterBuilder.BuildOpponentTurnPlan(TurnManager.Instance.Opponent.Blueprint, EventManagement.EncounterDifficulty, removeLockedCards);
            TurnManager.Instance.Opponent.ReplaceAndAppendTurnPlan(plan);
            yield return TurnManager.Instance.Opponent.QueueNewCards(true, true);
            yield break;
        }
    }
}