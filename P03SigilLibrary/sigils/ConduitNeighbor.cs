using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    [HarmonyPatch]
    public class ConduitNeighbor : AbilityBehaviour
    {
        public override Ability Ability => AbilityID;
        public static Ability AbilityID { get; private set; }

        static ConduitNeighbor()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Static Electricity";
            info.rulebookDescription = "[creature] will cause all friendly cards to behave as if they are part of a completed conduit.";
            info.canStack = false;
            info.powerLevel = 3;
            info.opponentUsable = true;
            info.passive = false;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.Part1Rulebook };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(ConduitNeighbor),
                TextureHelper.GetImageAsTexture("ability_staticelectricity.png", typeof(ConduitNeighbor).Assembly)
            ).Id;
        }

        //This code makes sure that sigils that only activate within conduits work properly
        // [HarmonyPatch(typeof(ConduitCircuitManager))]
        // [HarmonyPatch(nameof(ConduitCircuitManager.SlotIsWithinCircuit))]
        // [HarmonyPrefix]
        // private static bool SatisfyConduitSigils(ConduitCircuitManager __instance, ref bool __result, CardSlot slot)
        // {
        //     __result = __instance.GetConduitsForSlot(slot).Count > 0;

        //     if (__result) // No need to continue
        //         return false;

        //     CardSlot toLeft = BoardManager.Instance.GetAdjacent(slot, adjacentOnLeft: true);
        //     CardSlot toRight = BoardManager.Instance.GetAdjacent(slot, adjacentOnLeft: false);

        //     //If adjacent to conduit neighbor, slot is within circuit
        //     if (toLeft != null)
        //     {
        //         if (toLeft.Card != null)
        //         {
        //             if (toLeft.Card.HasAbility(AbilityID))
        //             {
        //                 __result = true;
        //                 return false;
        //             }
        //         }
        //     }

        //     if (toRight != null)
        //     {
        //         if (toRight.Card != null)
        //         {
        //             if (toRight.Card.HasAbility(AbilityID))
        //             {
        //                 __result = true;
        //             }
        //         }
        //     }

        //     return false; // Skip the original method
        // }

        //This code shows the circuit effect visually
        [HarmonyPatch(typeof(ConduitCircuitManager))]
        [HarmonyPatch(nameof(ConduitCircuitManager.UpdateCircuitsForSlots))]
        [HarmonyPrefix]
        private static bool CircuitEffect(ConduitCircuitManager __instance, List<CardSlot> slots)
        {
            foreach (CardSlot slot in slots)
            {
                if (__instance.SlotIsWithinCircuit(slot))
                {
                    if (slot.Card != null && !slot.WithinConduitCircuit)
                    {
                        slot.Card.RenderCard();
                    }
                    slot.SetWithinConduitCircuit(inCircuit: true);
                }
                else
                {
                    if (slot.Card != null && slot.WithinConduitCircuit)
                    {
                        slot.Card.RenderCard();
                    }
                    slot.SetWithinConduitCircuit(inCircuit: false);
                }
            }

            return false; // Skip the original method
        }

        [HarmonyPatch(typeof(ConduitCircuitManager))]
        [HarmonyPatch(nameof(ConduitCircuitManager.GetConduitsForSlot))]
        [HarmonyPrefix]
        private static bool ConduitNeighborAsConduit(ConduitCircuitManager __instance, ref List<PlayableCard> __result, CardSlot slot)
        {
            List<PlayableCard> list = new();
            if (slot == null)
            {
                __result = list;
                return false;
            }
            List<CardSlot> slots = BoardManager.Instance.GetSlots(slot.IsPlayerSlot);
            int num = slots.IndexOf(slot);
            bool circuitOnLeft = false;
            bool circuitOnRight = false;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i]?.Card?.HasConduitAbility() ?? false)
                {
                    if (i < num)
                    {
                        circuitOnLeft = true;
                        list.Add(slots[i].Card);
                    }
                    else if (i > num)
                    {
                        circuitOnRight = true;
                        list.Add(slots[i].Card);
                    }
                }
            }
            if (!circuitOnLeft || !circuitOnRight)
            {
                list.Clear();
            }

            // IF this ability is on any slot, add it to the list
            foreach (var s in slots)
                if (s?.Card?.HasAbility(AbilityID) ?? false)
                    list.Add(s.Card);

            __result = list;

            return false; // Skip the original method
        }


    }
}
