using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    [HarmonyPatch]
    public class ConduitGainFlying : ConduitGainAbility
    {
        public static Ability AbilityID { get; private set; }
        public override Ability Ability => AbilityID;

        protected override Ability AbilityToGive => Ability.Flying;

        static ConduitGainFlying()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Airborne Conduit";
            info.rulebookDescription = "Cards within a circuit completed by [creature] have Airborne.";
            info.canStack = false;
            info.powerLevel = 3;
            info.opponentUsable = false;
            info.conduit = true;
            info.passive = false;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.Part3Modular };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(ConduitGainFlying),
                TextureHelper.GetImageAsTexture("ability_conduit_gain_flying.png", typeof(ConduitGainFlying).Assembly)
            ).Id;
        }

        [HarmonyPatch(typeof(DiskCardAnimationController), nameof(DiskCardAnimationController.SetHovering))]
        [HarmonyPrefix]
        private static void ResetFlying(DiskCardAnimationController __instance, bool hovering)
        {
            if (__instance.Anim.GetBool("hovering") && !hovering)
                __instance.Anim.Rebind();
        }
    }
}