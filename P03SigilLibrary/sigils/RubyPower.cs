using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using InscryptionAPI.Triggers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class RubyPower : AbilityBehaviour, IPassiveAttackBuff
    {
        public static Ability AbilityID { get; private set; }
        public override Ability Ability => AbilityID;

        static RubyPower()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Ruby Blessing";
            info.rulebookDescription = "[creature] provides +1 Attack to all creatures you control.";
            info.canStack = false;
            info.powerLevel = 5;
            info.opponentUsable = true;
            info.passive = false;
            info.hasColorOverride = true;
            info.colorOverride = AbilityManager.BaseGameAbilities.AbilityByID(Ability.GainGemOrange).Info.colorOverride;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook };

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(RubyPower),
                TextureHelper.GetImageAsTexture("ability_ruby_power.png", typeof(RubyPower).Assembly)
            ).Id;
        }

        public int GetPassiveAttackBuff(PlayableCard target) => Card.OnBoard && target.OpponentCard == Card.OpponentCard ? 1 : 0;
    }
}