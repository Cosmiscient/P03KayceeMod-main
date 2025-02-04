using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class LatchRampage : Latch
    {
        public static Ability AbilityID { get; private set; }
        public override Ability Ability => AbilityID;

        public override Ability LatchAbility => Ability.StrafeSwap;

        static LatchRampage()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Rampage Latch";
            info.rulebookDescription = "When [creature] perishes, its owner chooses a creature to gain the Rampager sigil. The target will immediately rampage once when the latch is applied.";
            info.canStack = false;
            info.powerLevel = 1;
            info.opponentUsable = true;
            info.passive = false;
            info.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("pixelability_latch_conveyor.png", typeof(LatchRampage).Assembly));
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.Part3Modular };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(LatchRampage),
                TextureHelper.GetImageAsTexture("ability_latch_rampage.png", typeof(LatchRampage).Assembly)
            ).Id;
        }

        private PlayableCard lastTarget = null;

        public override void OnSuccessfullyLatched(PlayableCard target) => lastTarget = target;

        public override IEnumerator OnPreDeathAnimation(bool wasSacrifice)
        {
            yield return base.OnPreDeathAnimation(wasSacrifice);
            if (lastTarget != null)
            {
                yield return lastTarget.GetComponent<StrafeSwap>().OnTurnEnd(!lastTarget.OpponentCard);
            }
        }
    }
}