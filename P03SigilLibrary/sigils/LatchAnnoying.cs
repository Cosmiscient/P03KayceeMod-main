using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using InscryptionAPI.RuleBook;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class LatchAnnoying : Latch
    {
        public static Ability AbilityID { get; private set; }
        public override Ability Ability => AbilityID;

        public override Ability LatchAbility => Ability.BuffEnemy;

        static LatchAnnoying()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Annoying Latch";
            info.rulebookDescription = "When [creature] perishes, its owner chooses a creature to gain the Annoying sigil.";
            info.canStack = false;
            info.powerLevel = 1;
            info.opponentUsable = true;
            info.passive = false;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook, AbilityMetaCategory.Part1Rulebook };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(LatchAnnoying),
                TextureHelper.GetImageAsTexture("ability_latch_alarm.png", typeof(LatchAnnoying).Assembly)
            ).Id;

            info.SetAbilityRedirect("Annoying", Ability.BuffEnemy, GameColors.Instance.limeGreen);
        }
    }
}