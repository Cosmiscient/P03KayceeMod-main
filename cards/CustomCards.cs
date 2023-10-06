using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Helpers;
using Infiniscryption.P03KayceeRun.Patchers;
using InscryptionAPI.Card;
using InscryptionAPI.Guid;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Cards
{
    [HarmonyPatch]
    public static class CustomCards
    {
        public static readonly CardMetaCategory NeutralRegion = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "NeutralRegionCards");
        public static readonly CardMetaCategory WizardRegion = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "WizardRegionCards");
        public static readonly CardMetaCategory TechRegion = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "TechRegionCards");
        public static readonly CardMetaCategory NatureRegion = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "NatureRegionCards");
        public static readonly CardMetaCategory UndeadRegion = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "UndeadRegionCards");

        public static readonly CardMetaCategory NewBeastTransformers = GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, "NewBeastTransformers");

        public static readonly Trait UpgradeVirus = GuidManager.GetEnumValue<Trait>(P03Plugin.PluginGuid, "UpgradeMachineVirus");
        public static readonly Trait Unrotateable = GuidManager.GetEnumValue<Trait>(P03Plugin.PluginGuid, "Unrotateable");
        public static readonly Trait QuestCard = GuidManager.GetEnumValue<Trait>(P03Plugin.PluginGuid, "QuestCard");

        public static readonly Texture2D DUMMY_DECAL = TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_1.png", typeof(CustomCards).Assembly);

        public const string DRAFT_TOKEN = "P03KCM_Draft_Token";
        public const string UNC_TOKEN = "P03KCM_Draft_Token_Uncommon";
        public const string RARE_DRAFT_TOKEN = "P03KCM_Draft_Token_Rare";
        public const string GOLLYCOIN = "P03KCM_GollyCoin";
        public const string BLOCKCHAIN = "P03KCM_Blockchain";
        public const string NFT = "P03KCM_NFT";
        public const string OLD_DATA = "P03KCM_OLD_DATA";
        public const string VIRUS_SCANNER = "P03KCM_VIRUS_SCANNER";
        public const string CODE_BLOCK = "P03KCM_CODE_BLOCK";
        public const string CODE_BUG = "P03KCM_CODE_BUG";
        public const string PROGRAMMER = "P03KCM_PROGRAMMER";
        public const string ARTIST = "P03KCM_ARTIST";
        public const string FIREWALL = "P03KCM_FIREWALL";
        public const string FIREWALL_NORMAL = "P03KCM_FIREWALL_BATTLE";
        public const string BRAIN = "P03KCM_BOUNTYBRAIN";
        public const string BOUNTY_HUNTER_SPAWNER = "P03KCM_BOUNTY_SPAWNER";
        public const string CONTRABAND = "P03KCM_CONTRABAND";
        public const string RADIO_TOWER = "P03KCM_RADIO_TOWER";
        public const string SKELETON_LORD = "P03KCM_SKELETON_LORD";
        public const string GENERATOR_TOWER = "P03KCM_GENERATOR_TOWER";
        public const string POWER_TOWER = "P03KCM_POWER_TOWER";
        public const string FAILED_EXPERIMENT_BASE = "P03KCM_FAILED_EXPERIMENT";
        public const string MYCO_HEALING_CONDUIT = "P03KCM_MYCO_HEALING_CONDUIT";
        public const string MYCO_CONSTRUCT_BASE = "P03KCM_MYCO_CONSTRUCT_BASE";

        public const string TURBO_MINECART = "P03KCM_TURBO_MINECART";

        public const string TURBO_VESSEL = "P03KCM_TURBO_VESSEL";
        public const string TURBO_VESSEL_BLUEGEM = "P03KCM_TURBO_VESSEL_BLUEGEM";
        public const string TURBO_VESSEL_REDGEM = "P03KCM_TURBO_VESSEL_REDGEM";
        public const string TURBO_VESSEL_GREENGEM = "P03KCM_TURBO_VESSEL_GREENGEM";
        public const string TURBO_LEAPBOT = "P03KCM_TURBO_LEAPBOT";

        private static readonly List<CardMetaCategory> GBC_RARE_PLAYABLES = new() { CardMetaCategory.GBCPack, CardMetaCategory.GBCPlayable, CardMetaCategory.Rare, CardMetaCategory.ChoiceNode };

        [HarmonyPatch(typeof(CardLoader), nameof(CardLoader.Clone))]
        [HarmonyPostfix]
        private static void ModifyCardForAscension(ref CardInfo __result)
        {
            if (P03AscensionSaveData.IsP03Run || ScreenManagement.ScreenState == CardTemple.Tech)
            {
                string compName = __result.name.ToLowerInvariant();
                if (compName.StartsWith("sentinel") || __result.name == "TechMoxTriple")
                {
                    if (compName.StartsWith("sentinel"))
                    {
                        __result.energyCost = 3;
                    }
                    else
                    {
                        __result.baseHealth = 1;
                    }

                    if (!__result.Gemified)
                    {
                        __result.mods.Add(new() { gemify = true });
                    }
                }
                else if (compName.Equals("abovecurve"))
                {
                    __result.energyCost = 3;
                }
                else if (compName.Equals("automaton"))
                {
                    __result.energyCost = 2;
                }
                else if (compName.Equals("thickbot"))
                {
                    __result.baseHealth = 5;
                }
                else if (compName.Equals("steambot"))
                {
                    __result.abilities = new() { Ability.DeathShield };
                }
                else if (compName.Equals("bolthound"))
                {
                    __result.baseHealth = 3;
                }
                else if (compName.Equals("energyroller"))
                {
                    __result.abilities = new() { ExpensiveActivatedRandomPowerEnergy.AbilityID };
                }
                else if (compName.Equals("amoebot"))
                {
                    __result.energyCost = 3;
                }
                else if (compName.Equals("factoryconduit"))
                {
                    __result.energyCost = 2;
                }
                else if (compName.Equals("cellbuff"))
                {
                    __result.baseHealth = 1;
                }
                else if (compName.Equals("celltri"))
                {
                    __result.baseHealth = 1;
                }
                else if (compName.Equals("attackconduit"))
                {
                    __result.energyCost = 3;
                }
                else if (compName.Equals("gemshielder"))
                {
                    __result.baseAttack = 0;
                }
                else if (compName.Equals("gemexploder"))
                {
                    __result.baseHealth = 1;
                }
                else if (compName.Equals("insectodrone"))
                {
                    __result.energyCost = 2;
                }
                else if (compName.Equals("gemripper"))
                {
                    __result.mods.Add(new() { gemify = true, abilities = new() { Ability.GemDependant } });
                    __result.energyCost = 6;
                }
                else if (compName.Equals("sentinelblue"))
                {
                    __result.energyCost = 4;
                }
                else if (compName.Equals("sentinelorange"))
                {
                    __result.energyCost = 2;
                }
                else if (compName.Equals("robomice"))
                {
                    __result.abilities = new() { Ability.DrawCopy, Ability.DrawCopy };
                }
                else if (compName.Equals("energyconduit"))
                {
                    __result.baseAttack = 0;
                    __result.abilities = new() { NewConduitEnergy.AbilityID };
                    __result.appearanceBehaviour = new(__result.appearanceBehaviour) {
                        EnergyConduitAppearnace.ID
                    };
                }
            }
        }

        public static void printAllCards()
        {
            List<string> cardDataList = new();
            new List<CardInfo>();
            List<CardInfo> cardList = CardLoader.AllData;

            foreach (CardInfo card in cardList)
            {
                if (card.HasCardMetaCategory(UndeadRegion) || card.HasCardMetaCategory(TechRegion) || card.HasCardMetaCategory(WizardRegion) || card.HasCardMetaCategory(NatureRegion) || card.HasCardMetaCategory(NeutralRegion))
                {
                    if (card.temple == CardTemple.Tech)
                    {
                        string cardData = "";

                        cardData += string.Format("\n\nInternal Name: {0}\n", card.name);
                        cardData += string.Format("Displayed Name: {0}\n", card.displayedName);
                        cardData += string.Format("Stats: {0}/{1}\n", card.Attack, card.Health);
                        cardData += string.Format("Energy: {0}\n", card.EnergyCost);

                        bool isRare = card.HasCardMetaCategory(CardMetaCategory.Rare);

                        cardData += string.Format("Rare: {0}\n", isRare);

                        string sigils = "";

                        foreach (Ability ab in card.abilities)
                        {
                            if (!string.IsNullOrEmpty(sigils))
                            {
                                sigils += ", ";
                            }
                            sigils += ab;
                        }

                        if (string.IsNullOrEmpty(sigils))
                        {
                            sigils = "None";
                        }

                        cardData += string.Format("Sigils: {0}\n", sigils);

                        if (card.HasCardMetaCategory(NeutralRegion))
                        {
                            cardData += "Region: Neutral\n";
                        }

                        if (card.HasCardMetaCategory(UndeadRegion))
                        {
                            cardData += "Region: Undead\n";
                        }

                        if (card.HasCardMetaCategory(NatureRegion))
                        {
                            cardData += "Region: Nature\n";
                        }

                        if (card.HasCardMetaCategory(WizardRegion))
                        {
                            cardData += "Region: Wizard\n";
                        }

                        if (card.HasCardMetaCategory(TechRegion))
                        {
                            cardData += "Region: Tech\n";
                        }

                        if (card.name.Contains("P03KCM_"))
                        {
                            cardData += "Source: New P03 KCM Card\n";
                        }

                        if (card.name.Contains("P03KCMXP1_"))
                        {
                            cardData += "Source: Expansion Pack 1 Card\n";
                        }

                        if (card.name.Contains("P03KCMXP2_"))
                        {
                            cardData += "Source: Expansion Pack 2 Card\n";
                        }

                        if (!card.name.Contains("P03KCMXP1_") && !card.name.Contains("P03KCMXP2_") && !card.name.Contains("P03KCM_"))
                        {
                            cardData += "Source: Base Game Card\n";
                        }

                        // Add the card data to the list
                        cardDataList.Add(cardData);
                    }
                }
            }

            foreach (string cardData in cardDataList)
            {
                Console.WriteLine(cardData);
            }

        }

        [HarmonyPatch(typeof(Ouroboros), nameof(Ouroboros.OnDie))]
        [HarmonyPostfix]
        private static IEnumerator OnlyIfDiedInCombat(IEnumerator sequence, PlayableCard killer)
        {
            if (SaveFile.IsAscension && SaveManager.SaveFile.IsPart3 && killer == null)
            {
                yield return EventManagement.SayDialogueOnce("P03HammerOrb", EventManagement.SAW_NEW_ORB);
                SaveManager.SaveFile.OuroborosDeaths = SaveManager.SaveFile.OuroborosDeaths - 1;
            }
            yield return sequence;
        }

        private static void UpdateExistingCard(string name, string textureKey, string pixelTextureKey, string regionCode, string decalTextureKey, string colorPortraitKey, bool isPackCard)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            CardInfo card = CardManager.BaseGameCards.FirstOrDefault(c => c.name == name);
            if (card == null)
            {
                P03Plugin.Log.LogInfo($"COULD NOT MODIFY CARD {name} BECAUSE I COULD NOT FIND IT");
                return;
            }

            P03Plugin.Log.LogInfo($"MODIFYING {name} -> {card.displayedName}");

            if (!string.IsNullOrEmpty(textureKey))
            {
                card.SetPortrait(TextureHelper.GetImageAsTexture($"{textureKey}.png", typeof(CustomCards).Assembly));
            }

            if (!string.IsNullOrEmpty(colorPortraitKey))
            {
                card.SetAltPortrait(TextureHelper.GetImageAsTexture($"{colorPortraitKey}.png", typeof(CustomCards).Assembly));
                card.AddAppearances(HighResAlternatePortrait.ID);
            }

            if (!string.IsNullOrEmpty(pixelTextureKey))
            {
                card.SetPixelPortrait(TextureHelper.GetImageAsTexture($"{pixelTextureKey}.png", typeof(CustomCards).Assembly));
            }

            if (!string.IsNullOrEmpty(regionCode))
            {
                card.metaCategories ??= new();
                card.metaCategories.Add(GuidManager.GetEnumValue<CardMetaCategory>(P03Plugin.PluginGuid, regionCode));
            }

            if (!string.IsNullOrEmpty(decalTextureKey))
            {
                card.decals = new() { TextureHelper.GetImageAsTexture($"{decalTextureKey}.png", typeof(CustomCards).Assembly) };
            }

            if (isPackCard)
            {
                (card.metaCategories ??= new()).Add(CardMetaCategory.TraderOffer);
            }
        }

        internal static void RegisterCustomCards(Harmony harmony)
        {
            // Load the custom cards from the CSV database
            string database = DataHelper.GetResourceString("card_database", "csv");
            string[] lines = database.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines.Skip(1))
            {
                string[] cols = line.Split(new char[] { ',' }, StringSplitOptions.None);
                //InfiniscryptionP03Plugin.Log.LogInfo($"I see line {string.Join(";", cols)}");
                UpdateExistingCard(cols[0], cols[1], cols[2], cols[3], cols[4], cols[6], cols[5] == "Y");
            }

            CardManager.BaseGameCards.CardByName("PlasmaGunner").AddAppearances(ForceRevolverAppearance.ID);

            CardManager.BaseGameCards.CardByName("TechMoxTriple").AddDecal(
                TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_1.png", typeof(CustomCards).Assembly),
                TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_1.png", typeof(CustomCards).Assembly),
                TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_2.png", typeof(CustomCards).Assembly)
            );

            // This creates all the sprites behind the scenes so we're ready to go
            RandomStupidAssApePortrait.RandomApePortrait.GenerateApeSprites();

            // Update the librarian to display its size
            CardManager.BaseGameCards.CardByName("Librarian").AddAppearances(LibrarianSizeTitle.ID);

            CardManager.ModifyCardList += delegate (List<CardInfo> cards)
            {
                if (P03AscensionSaveData.IsP03Run)
                {
                    cards.CardByName("EnergyConduit").AddMetaCategories(TechRegion);
                    cards.CardByName("TechMoxTriple").AddMetaCategories(WizardRegion);
                    cards.CardByName("EnergyRoller").AddMetaCategories(CardMetaCategory.Rare);
                }

                return cards;
            };

            CardManager.New(P03Plugin.CardPrefx, DRAFT_TOKEN, "Basic Token", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_drafttoken.png", typeof(CustomCards).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixel_drafttoken.png", typeof(CustomCards).Assembly))
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, UNC_TOKEN, "Improved Token", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_drafttoken_plus.png", typeof(CustomCards).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixel_drafttoken_plus.png", typeof(CustomCards).Assembly))
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, RARE_DRAFT_TOKEN, "Rare Token", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_drafttoken_plusplus.png", typeof(CustomCards).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixel_drafttoken.png", typeof(CustomCards).Assembly))
                .AddAppearances(RareDiscCardAppearance.ID)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, BLOCKCHAIN, "Blockchain", 0, 5)
                .SetAltPortrait(TextureHelper.GetImageAsTexture("portrait_blockchain.png", typeof(CustomCards).Assembly, FilterMode.Trilinear))
                .AddAbilities(Ability.ConduitNull, ConduitSpawnCrypto.AbilityID)
                .AddAppearances(HighResAlternatePortrait.ID)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, GOLLYCOIN, "GollyCoin", 0, 3)
                .SetAltPortrait(TextureHelper.GetImageAsTexture("portrait_gollycoin.png", typeof(CustomCards).Assembly, FilterMode.Trilinear))
                .AddAbilities(Ability.Reach)
                .AddAppearances(HighResAlternatePortrait.ID)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, NFT, "Stupid-Ass Ape", 0, 1)
                .AddAppearances(RandomStupidAssApePortrait.ID)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, OLD_DATA, "UNSAFE.DAT", 0, 1)
                .SetPortrait(Resources.Load<Texture2D>("art/cards/part 3 portraits/portrait_captivefile"))
                .AddAbilities(LoseOnDeath.AbilityID)
                .AddTraits(QuestCard)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, CODE_BLOCK, "Code Snippet", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_code.png", typeof(CustomCards).Assembly))
                .AddTraits(Programmer.CodeTrait)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, CODE_BUG, "Bug", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_bug.png", typeof(CustomCards).Assembly))
                .AddTraits(Programmer.CodeTrait)
                .AddAbilities(Ability.Brittle)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, VIRUS_SCANNER, "VIRSCAN.EXE", 1, 7)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_virusscanner.png", typeof(CustomCards).Assembly))
                .AddAbilities(Ability.Deathtouch, Ability.StrafeSwap)
                .AddDecal(
                    TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_1.png", typeof(CustomCards).Assembly),
                    TextureHelper.GetImageAsTexture("portrait_triplemox_color_decal_1.png", typeof(CustomCards).Assembly),
                    TextureHelper.GetImageAsTexture("portrait_virusscanner_decal.png", typeof(CustomCards).Assembly)
                )
                .temple = CardTemple.Tech;

            CardInfo ch2 = CardManager.New(P03Plugin.CardPrefx, "AboveCurve2", "Curve Hopper Hopper", 3, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_abovecurve_2.png", typeof(CustomCards).Assembly))
                .AddAppearances(RareDiscCardAppearance.ID);
            ch2.temple = CardTemple.Tech;
            CardManager.BaseGameCards.CardByName("AboveCurve").SetEvolve(ch2, 1);


            // CardManager.New(P03Plugin.CardPrefx, PROGRAMMER, "Programmer", 0, 2)
            //     .SetPortrait(TextureHelper.GetImageAsTexture("portrait_codemonkey.png", typeof(CustomCards).Assembly))
            //     .AddAbilities(Programmer.AbilityID)
            //     .temple = CardTemple.Tech;

            // CardManager.New(ARTIST, "Artist", 1, 2)
            //     .SetPortrait(TextureHelper.GetImageAsTexture("portrait_artist"))
            //     .AddAbilities(Artist.AbilityID)
            //     .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, TURBO_VESSEL, "Turbo Vessel", 0, 2)
                    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_turbovessel.png", typeof(CustomCards).Assembly))
                    .SetCost(energyCost: 1)
                    .AddAbilities(DoubleSprint.AbilityID, Ability.ConduitNull)
                    .SetFlippedPortrait()
                    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, TURBO_VESSEL_BLUEGEM, "Turbo Vessel", 0, 2)
                    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_turbovessel.png", typeof(CustomCards).Assembly))
                    .SetCost(energyCost: 1)
                    .AddAbilities(DoubleSprint.AbilityID, Ability.GainGemBlue)
                    .SetFlippedPortrait()
                    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, TURBO_VESSEL_REDGEM, "Turbo Vessel", 0, 2)
                    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_turbovessel.png", typeof(CustomCards).Assembly))
                    .SetCost(energyCost: 1)
                    .AddAbilities(DoubleSprint.AbilityID, Ability.GainGemOrange)
                    .SetFlippedPortrait()
                    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, TURBO_VESSEL_GREENGEM, "Turbo Vessel", 0, 2)
                    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_turbovessel.png", typeof(CustomCards).Assembly))
                    .SetCost(energyCost: 1)
                    .AddAbilities(DoubleSprint.AbilityID, Ability.GainGemGreen)
                    .SetFlippedPortrait()
                    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, TURBO_LEAPBOT, "Turbo L33pb0t", 0, 2)
                    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_TurboL33pBot.png", typeof(CustomCards).Assembly))
                    .SetCost(energyCost: 1)
                    .AddAbilities(Ability.Reach, DoubleSprint.AbilityID)
                    .SetFlippedPortrait()
                    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, FIREWALL, "Firewall", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_firewall.png", typeof(CustomCards).Assembly))
                .AddAbilities(Ability.PreventAttack)
                .SetCost(energyCost: 5)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, FIREWALL_NORMAL, "Firewall", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_firewall.png", typeof(CustomCards).Assembly))
                .AddAbilities(Ability.PreventAttack, Ability.StrafeSwap)
                .SetCost(energyCost: 5)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, BRAIN, "Hunter Brain", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_bounty_hunter_brain.png", typeof(CustomCards).Assembly))
                .AddAppearances(GoldPortrait.ID)
                .SetCost(energyCost: 2)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, BOUNTY_HUNTER_SPAWNER, "Activated Hunter", 0, 0)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_bounty_hunter_brain.png", typeof(CustomCards).Assembly))
                .AddAppearances(ConditionalDynamicPortrait.ID, ForceRevolverAppearance.ID, GoldPortrait.ID)
                .AddAbilities(RandomBountyHunter.AbilityID)
                .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, CONTRABAND, "yarr.torrent", 0, 1)
                .SetPortrait(Resources.Load<Texture2D>("art/cards/part 3 portraits/portrait_captivefile"))
                .AddAbilities(Ability.PermaDeath)
                .AddAppearances(QuestCardAppearance.ID)
                .AddTraits(QuestCard)
                .temple = CardTemple.Tech;

            //NEW BEAST CARDS

            //CardManager.New(P03Plugin.CardPrefx, "CXformerRiverSnapper", "R!V3R 5N4PP3R", 1, 6)
            //    .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_riversnapper.png", typeof(CustomCards).Assembly))
            //    .SetCost(energyCost: 4)
            //    .AddMetaCategories(NewBeastTransformers)
            //    .temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, "CXformerRiverSnapper", "R!V3R 5N4PP3R", 1, 6)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_riversnapper.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 4)
                .SetNewBeastTransformer(4, 1);

            CardManager.New(P03Plugin.CardPrefx, "CXformerMole", "M013", 0, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_mole.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 4)
                .AddAbilities(Ability.WhackAMole)
                .SetNewBeastTransformer(2, 1);

            CardManager.New(P03Plugin.CardPrefx, "CXformerRabbit", "R488!7", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_rabbit.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 0)
                .SetNewBeastTransformer(0, -2);

            CardManager.New(P03Plugin.CardPrefx, "CXformerMantis", "M4N7!5", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_mantis.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 3)
                .AddAbilities(Ability.SplitStrike)
                .SetNewBeastTransformer(0, 0);

            CardManager.New(P03Plugin.CardPrefx, "CXformerAlpha", "41PH4", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_alpha.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 5)
                .AddAbilities(Ability.BuffNeighbours)
                .SetNewBeastTransformer(0, 1);

            CardManager.New(P03Plugin.CardPrefx, "CXformerOpossum", "Robopossum", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_transformer_opossum.png", typeof(CustomCards).Assembly))
                .SetCost(energyCost: 2)
                .SetNewBeastTransformer(0, -1);

            CardInfo radio = CardManager.New(P03Plugin.CardPrefx, RADIO_TOWER, "Radio Tower", 0, 3);
            radio.AddSpecialAbilities(ListenToTheRadio.AbilityID, RerenderOnBoard.AbilityID);
            radio.SetCost(energyCost: 3);
            radio.SetPortrait(TextureHelper.GetImageAsTexture("portrait_radio.png", typeof(CustomCards).Assembly));
            radio.AddAppearances(OnboardHoloPortrait.ID);
            radio.AddAppearances(QuestCardAppearance.ID);
            radio.AddTraits(QuestCard);
            radio.temple = CardTemple.Tech;
            radio.holoPortraitPrefab = Resources.Load<GameObject>("prefabs/cards/hologramportraits/TerrainHologram_AnnoyTower");

            CardInfo gentower = CardManager.New(P03Plugin.CardPrefx, GENERATOR_TOWER, "Generator", 0, 3);
            gentower.temple = CardTemple.Tech;
            gentower.AddAppearances(CardAppearanceBehaviour.Appearance.HologramPortrait);
            gentower.holoPortraitPrefab = Resources.Load<GameObject>("prefabs/cards/hologramportraits/TerrainHologram_AnnoyTower");

            CardInfo powerTower = CardManager.New(P03Plugin.CardPrefx, POWER_TOWER, "Power Sink", 0, 2);
            powerTower.AddSpecialAbilities(RerenderOnBoard.AbilityID, PowerUpTheTower.AbilityID);
            powerTower.SetCost(energyCost: 3);
            powerTower.SetPortrait(TextureHelper.GetImageAsTexture("portrait_radio.png", typeof(CustomCards).Assembly));
            powerTower.AddAppearances(OnboardDynamicHoloPortrait.ID);
            powerTower.AddAppearances(QuestCardAppearance.ID);
            powerTower.AddTraits(QuestCard);
            powerTower.SetExtendedProperty(OnboardDynamicHoloPortrait.PREFAB_KEY, "prefabs/specialnodesequences/teslacoil");
            powerTower.SetExtendedProperty(OnboardDynamicHoloPortrait.OFFSET_KEY, "0,-.39,0");
            powerTower.SetExtendedProperty(OnboardDynamicHoloPortrait.SCALE_KEY, ".6,.6,.6");
            powerTower.temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, FAILED_EXPERIMENT_BASE, "Failed Experiment", 0, 0)
                .SetPortrait(Resources.Load<Texture2D>("art/cards/part 3 portraits/portrait_mycobot"))
                .temple = CardTemple.Tech;

            CardInfo mycoHealConduit = CardManager.New(P03Plugin.CardPrefx, MYCO_HEALING_CONDUIT, "Heal Conduit", 0, 3);
            mycoHealConduit.AddAbilities(Ability.ConduitHeal);
            mycoHealConduit.AddAppearances(OnboardDynamicHoloPortrait.ID);
            mycoHealConduit.SetExtendedProperty(OnboardDynamicHoloPortrait.PREFAB_KEY, "prefabs/specialnodesequences/teslacoil");
            mycoHealConduit.SetExtendedProperty(OnboardDynamicHoloPortrait.OFFSET_KEY, "0,-.39,0");
            mycoHealConduit.SetExtendedProperty(OnboardDynamicHoloPortrait.SCALE_KEY, ".6,.6,.6");
            mycoHealConduit.temple = CardTemple.Tech;

            CardInfo goobertCardBase = CardManager.New(P03Plugin.CardPrefx, MYCO_CONSTRUCT_BASE, "Experiment #1", 0, 5);
            goobertCardBase.SetCost(energyCost: 6);
            goobertCardBase.SetPortrait(TextureHelper.GetImageAsTexture("portrait_goobot.png", typeof(CustomCards).Assembly));
            goobertCardBase.AddAppearances(GooDiscCardAppearance.ID);
            goobertCardBase.AddSpecialAbilities(GoobertCenterCardBehaviour.AbilityID);
            goobertCardBase.AddAbilities(TripleCardStrike.AbilityID, PowerDrain.AbilityID);
            goobertCardBase.AddTraits(Unrotateable, Trait.Uncuttable);
            goobertCardBase.temple = CardTemple.Tech;

            CardManager.New(P03Plugin.CardPrefx, SKELETON_LORD, "Skeleton Master", 0, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_skeleton_lord.png", typeof(CustomCards).Assembly))
                .AddAbilities(BrittleGainsUndying.AbilityID, DrawBrittle.AbilityID)
                .SetCost(energyCost: 2)
                .AddAppearances(RareDiscCardAppearance.ID)
                .temple = CardTemple.Tech;

            // CardManager.New(P03Plugin.CardPrefx, MYCO_CONSTRUCT_PONTOON, "PONTOON", 0, 1)
            //     .AddAppearances(InvisibleCard.ID)
            //     .temple = CardTemple.Tech;

            // This should patch the rulebook. Also fixes a little bit of the game balance
            AbilityManager.ModifyAbilityList += delegate (List<AbilityManager.FullAbility> abilities)
            {
                List<Ability> allP3Abs = CardManager.AllCardsCopy.Where(c => c.temple == CardTemple.Tech).SelectMany(c => c.abilities).Distinct().ToList();

                foreach (AbilityManager.FullAbility ab in abilities)
                {
                    if (allP3Abs.Contains(ab.Id))
                    {
                        ab.Info.AddMetaCategories(AbilityMetaCategory.Part3Rulebook);
                    }

                    if (P03AscensionSaveData.IsP03Run && (ab.Id == Ability.GainGemBlue || ab.Id == Ability.GainGemOrange || ab.Id == Ability.GainGemGreen))
                    {
                        ab.Info.AddMetaCategories(AbilityMetaCategory.Part3Modular);
                    }

                    if (ab.Id is Ability.CellBuffSelf or Ability.CellTriStrike)
                    {
                        ab.Info.powerLevel += 2;
                    }

                    if (ab.Id == Ability.ActivatedRandomPowerEnergy && P03AscensionSaveData.IsP03Run)
                    {
                        ab.Info.metaCategories = new();
                    }

                    if (ab.Id == Ability.DrawCopy && P03AscensionSaveData.IsP03Run)
                    {
                        ab.Info.canStack = true;
                    }
                }

                // Might as well do the stat icons here
                List<SpecialStatIcon> statIcons = CardManager.AllCardsCopy.Where(c => c.temple == CardTemple.Tech).Select(c => c.specialStatIcon).Distinct().ToList();
                foreach (StatIconManager.FullStatIcon icon in StatIconManager.AllStatIcons)
                {
                    if (statIcons.Contains(icon.Id))
                    {
                        icon.Info.metaCategories.Add(AbilityMetaCategory.Part3Rulebook);
                    }
                }

                return abilities;
            };

            CardManager.ModifyCardList += delegate (List<CardInfo> cards)
            {
                foreach (CardInfo ci in cards.Where(ci => ci.temple == CardTemple.Tech))
                {
                    if (ci.metaCategories.Contains(CardMetaCategory.Rare))
                        ci.AddAppearances(RareDiscCardAppearance.ID);

                    for (int i = 0; i < ci.abilities.Count; i++)
                    {
                        if (ci.abilities[i] == Ability.SteelTrap)
                            ci.abilities[i] = BetterSteelTrap.AbilityID;
                    }
                }

                return cards;
            };
        }

        public static CardInfo SetFlippedPortrait(this CardInfo info)
        {
            info.flipPortraitForStrafe = true;
            return info;
        }

        public static CardInfo SetNeutralP03Card(this CardInfo info)
        {
            info.AddMetaCategories(CardMetaCategory.ChoiceNode);
            info.AddMetaCategories(NeutralRegion);
            info.temple = CardTemple.Tech;
            return info;
        }

        public static bool EligibleForGemBonus(this PlayableCard card, GemType gem) => GameFlowManager.IsCardBattle && (card.OpponentCard ? OpponentGemsManager.Instance.HasGem(gem) : ResourcesManager.Instance.HasGem(gem));

        public static CardInfo SetRegionalP03Card(this CardInfo info, params CardTemple[] region)
        {
            info.AddMetaCategories(CardMetaCategory.ChoiceNode);
            info.temple = CardTemple.Tech;
            foreach (CardTemple reg in region)
            {
                switch (reg)
                {
                    case CardTemple.Nature:
                        info.AddMetaCategories(NatureRegion);
                        break;
                    case CardTemple.Undead:
                        info.AddMetaCategories(UndeadRegion);
                        break;
                    case CardTemple.Tech:
                        info.AddMetaCategories(TechRegion);
                        break;
                    case CardTemple.Wizard:
                        info.AddMetaCategories(WizardRegion);
                        break;
                    case CardTemple.NUM_TEMPLES:
                        break;
                    default:
                        break;
                }
            }
            return info;
        }

        public static CardInfo RemoveAbility(this CardInfo info, Ability ability)
        {
            (info.mods ??= new()).Add(new() { negateAbilities = new() { ability } });
            return info;
        }

        public static CardInfo ChangeName(this CardInfo info, string newName)
        {
            (info.mods ??= new()).Add(new() { nameReplacement = newName });
            return info;
        }

        public static CardInfo SetColorProperty(this CardInfo info, string colorKey, Color color)
        {
            return info.SetExtendedProperty(
                colorKey,
                $"{color.r},{color.g},{color.b},{color.a}"
            );
        }

        public static CardInfo SetSpellAppearanceP03(this CardInfo info)
        {
            info.AddAppearances(DiscCardColorAppearance.ID);
            info.SetExtendedProperty("BorderColor", "gold");
            info.SetExtendedProperty("EnergyColor", "gold");
            info.SetExtendedProperty("NameTextColor", "black");
            info.SetExtendedProperty("NameBannerColor", "white");
            info.SetExtendedProperty("AttackColor", "gold");
            info.SetExtendedProperty("HealthColor", "gold");
            info.SetExtendedProperty("DefaultAbilityColor", "gold");
            info.SetExtendedProperty("PortraitColor", "gold");
            info.SetExtendedProperty("Holofy", true);
            return info;
        }

        private static string GetModCode(CardModificationInfo info)
        {
            string retval = "";
            foreach (Ability ab in info.abilities)
            {
                retval += $"+{ab}";
                if (ab == Ability.Transformer)
                {
                    if (!string.IsNullOrEmpty(info.transformerBeastCardId))
                    {
                        retval += "(" + info.transformerBeastCardId.Replace('+', '&').Replace('@', '#') + ")";
                    }
                }
            }
            if (info.gemify)
            {
                retval += "+Gemify";
            }

            if (info.attackAdjustment > 0 || info.healthAdjustment > 0)
            {
                retval += $"+!{info.attackAdjustment},{info.healthAdjustment}";
            }

            if (info.nameReplacement != null)
            {
                retval += $"+;{info.nameReplacement}";
            }

            return retval;
        }

        private static CardModificationInfo GetMod(string modCode)
        {
            CardModificationInfo retval = new()
            {
                nonCopyable = true,
                abilities = new()
            };

            if (modCode.StartsWith(";"))
            {
                retval.nameReplacement = modCode.Replace(";", "");
                return retval;
            }

            if (modCode.StartsWith("!"))
            {
                string[] pieces = modCode.Replace("!", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                retval.attackAdjustment = int.Parse(pieces[0]);
                retval.healthAdjustment = int.Parse(pieces[1]);
                return retval;
            }

            if (modCode.Contains("("))
            {
                string[] codePieces = modCode.Replace(")", "").Split('(');
                Ability ab = (Ability)Enum.Parse(typeof(Ability), codePieces[0]);
                if (ab == Ability.Transformer)
                {
                    string newCode = codePieces[1].Replace("&", "+").Replace("#", "@");
                    P03Plugin.Log.LogInfo($"Setting {newCode} as beast code");
                    retval.transformerBeastCardId = newCode;
                }
                retval.abilities.Add(ab);
            }
            else if (modCode.ToLowerInvariant().Equals("gemify"))
            {
                retval.gemify = true;
            }
            else
            {
                retval.abilities.Add((Ability)Enum.Parse(typeof(Ability), modCode));
            }

            if (retval.abilities.Contains(Ability.PermaDeath) || retval.abilities.Contains(NewPermaDeath.AbilityID))
            {
                retval.attackAdjustment = 1;
            }
            return retval;
        }

        public static string ConvertCardToCompleteCode(CardInfo card) => "@" + card.name + string.Join("", card.Mods.Select(GetModCode));

        public static CardInfo ConvertCodeToCard(string code)
        {
            P03Plugin.Log.LogInfo($"Converting code {code} to a card");
            string[] codePieces = code.Replace("@", "").Split('+');
            CardInfo retval = CardLoader.GetCardByName(codePieces[0]);
            P03Plugin.Log.LogInfo($"Successfully found card {retval.name}");
            retval.mods = new();
            for (int i = 1; i < codePieces.Length; i++)
            {
                retval.mods.Add(GetMod(codePieces[i]));
                P03Plugin.Log.LogInfo($"Successfully found converted {codePieces[i]} to a card mod");
            }
            return retval;
        }

        [HarmonyPatch(typeof(PlayableCard), nameof(PlayableCard.EnergyCost), MethodType.Getter)]
        [HarmonyPostfix]
        private static void AdjustCostForTempMods(ref PlayableCard __instance, ref int __result)
        {
            if (__instance.temporaryMods != null)
            {
                foreach (CardModificationInfo tMod in __instance.temporaryMods)
                {
                    __result += tMod.energyCostAdjustment;
                }
            }

            __result = Mathf.Max(0, __result);
        }

        public static bool SlotHasTripleCard(this CardSlot slot) => slot.Card != null && slot.Card.Info.SpecialAbilities.Contains(GoobertCenterCardBehaviour.AbilityID);

        public static bool SlotCoveredByTripleCard(this CardSlot slot)
        {
            List<CardSlot> container = BoardManager.Instance.GetSlots(slot.IsPlayerSlot);

            return (slot.Index > 0 && container[slot.Index - 1].SlotHasTripleCard())
|| (slot.Index + 1 < container.Count && container[slot.Index + 1].SlotHasTripleCard());
        }

        public static bool SlotCanHoldTripleCard(this CardSlot slot, PlayableCard existingCard = null)
        {
            P03Plugin.Log.LogDebug("Checking if slot can hold triple card");
            List<CardSlot> container = BoardManager.Instance.GetSlots(slot.IsPlayerSlot);
            int index = slot.Index;

            if (index == 0)
            {
                return false;
            }

            if (index + 1 >= container.Count)
            {
                return false;
            }

            if (slot.Card != null && slot.Card != existingCard)
            {
                return false;
            }

            if (container[index - 1].Card != null && container[index - 1].Card != existingCard)
            {
                return false;
            }

            if (container[index + 1].Card != null && container[index + 1].Card != existingCard)
            {
                return false;
            }

            P03Plugin.Log.LogDebug("Slot can hold triple card");
            return true;
        }
    }
}
