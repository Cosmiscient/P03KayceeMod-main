using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using InscryptionAPI.Helpers.Extensions;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    public class EnemyGainShield : AbilityBehaviour
    {
        public static Ability AbilityID { get; private set; }
        public override Ability Ability => AbilityID;

        static EnemyGainShield()
        {
            AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
            info.rulebookName = "Armor Giver";
            info.rulebookDescription = "When [creature] enters, all opposing cards gain Nano Armor.";
            info.canStack = false;
            info.powerLevel = -1;
            info.opponentUsable = true;
            info.passive = false;
            info.metaCategories = new List<AbilityMetaCategory>() { AbilityMetaCategory.Part3Rulebook };
            info.SetPixelAbilityIcon(TextureHelper.GetImageAsTexture("pixelability_enemygainshield.png", typeof(EnemyGainShield).Assembly));

            AbilityID = AbilityManager.Add(
                P03SigilLibraryPlugin.PluginGuid,
                info,
                typeof(EnemyGainShield),
                TextureHelper.GetImageAsTexture("ability_enemygainshield.png", typeof(EnemyGainShield).Assembly)
            ).Id;
        }

        public override bool RespondsToResolveOnBoard() => true;

        public override IEnumerator OnResolveOnBoard()
        {
            foreach (CardSlot slot in BoardManager.Instance.GetSlotsCopy(!Card.IsPlayerCard()))
            {
                if (slot.Card == null)
                {
                    continue;
                }

                if (!slot.Card.HasAbility(Ability.DeathShield))
                {
                    CardModificationInfo mod = new(Ability.DeathShield);
                    slot.Card.Status.hiddenAbilities.Add(Ability.DeathShield);
                    slot.Card.AddTemporaryMod(mod);
                }

                slot.Card.ResetShield();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}