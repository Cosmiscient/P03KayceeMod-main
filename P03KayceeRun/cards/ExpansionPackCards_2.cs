using System.Linq;
using DiskCardGame;
using Infiniscryption.P03SigilLibrary.Sigils;
using InscryptionAPI.Card;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Cards
{
    public static class ExpansionPackCards_2
    {
        internal const string EXP_2_PREFIX = "P03KCMXP2";

        internal const string FLAME_CHARMER_CARD = "P03KCMXP2_FlameCharmer";

        internal const string RINGWORM_CARD = "P03KCMXP2_RoboRingworm";
        internal const string LEAPBOT_NEO = "P03KCMXP2_LeapBotNeo";

        static ExpansionPackCards_2()
        {
            // Swapper Latcher
            CardManager.New(EXP_2_PREFIX, "SwapperLatcher", "Swapper Latcher", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_swapper_latcher.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_swapper_latcher.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Undead)
                .AddAbilities(LatchSwapper.AbilityID);

            // Box Bot
            CardManager.New(EXP_2_PREFIX, "BoxBot", "Box Bot", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_boxbot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_boxbot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAbilities(Ability.Brittle, VesselHeart.AbilityID);

            // Scrap Bot
            CardManager.New(EXP_2_PREFIX, "ScrapBot", "Scrap Bot", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_scrapbot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .AddMetaCategories(CardMetaCategory.Part3Random)
                .SetNeutralP03Card()
                .AddAbilities(ScrapDumper.AbilityID);

            // Zip Bomb
            CardManager.New(EXP_2_PREFIX, "ZipBomb", "Zip Bomb", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_zipbomb.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetRare()
                .SetNeutralP03Card()
                .AddAbilities(TakeDamageSigil.AbilityID);

            // Robot Ram
            CardManager.New(EXP_2_PREFIX, "RobotRam", "R4M", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_ram.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_ram.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Nature)
                .AddAbilities(Overheat.AbilityID);

            // Elektron
            CardManager.New(EXP_2_PREFIX, "Elektron", "Elektron", 1, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_elektron.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRare()
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(ConduitNeighbor.AbilityID);

            // Shovester
            CardManager.New(EXP_2_PREFIX, "Shovester", "Billdozer", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_billdozer.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetNeutralP03Card()
                .SetStrafeFlipsPortrait(true)
                .AddMetaCategories(CardMetaCategory.Part3Random)
                .AddAbilities(Shove.AbilityID);

            // Librarian
            CardManager.New(EXP_2_PREFIX, "Librarian", "Librarian", 1, 2)
                .SetPortrait(Resources.Load<Texture2D>("art/cards/part 3 portraits/portrait_librarian").ConvertTexture(TextureHelper.SpriteType.CardPortrait))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("syntax_librarian.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetRegionalP03Card(CardTemple.Undead)
                .AddAbilities(Ability.Reach, DeadByte.AbilityID);

            // Ruby Guardian
            CardManager.New(EXP_2_PREFIX, "RubyGuardian", "Ruby Sentinel", 2, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_ruby_guardian.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 6)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAbilities(RubyWarrior.AbilityID);

            // Emerald Guardian
            CardManager.New(EXP_2_PREFIX, "EmeraldGuardian", "Emerald Sentinel", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_emerald_guardian.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 5)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAbilities(EmeraldExtraction.AbilityID);

            // Sapphire Guardian
            CardManager.New(EXP_2_PREFIX, "SapphireGuardian", "Sapphire Sentinel", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_sapphire_guardian.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAbilities(SapphireEnergy.AbilityID);

            // PyroBot
            CardManager.New(EXP_2_PREFIX, "PyroBot", "Ignitron", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_ignitron.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetNeutralP03Card()
                .AddMetaCategories(CardMetaCategory.Part3Random)
                .SetWeaponMesh(
                    "p03kcm/prefabs/flamethrower",
                    localPosition: new Vector3(0f, 0f, 0f),
                    localRotation: new Vector3(0f, 90f, 0f),
                    localScale: new Vector3(0.75f, 0.75f, 0.75f)
                )
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_ignitron.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(FireBomb.AbilityID);

            // GlowBot
            CardManager.New(EXP_2_PREFIX, "GlowBot", "GlowBot", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_glowbot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_glowbot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(SolarHeart.AbilityID);

            // M0l0t0v
            CardManager.New(EXP_2_PREFIX, "Molotov", "M0l0t0v", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_m010t0v.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetNeutralP03Card()
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_molotov.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(Molotov.AbilityID);

            // Flaming Exeskeleton
            CardManager.New(EXP_2_PREFIX, "FlamingExeskeleton", "Revignite", 2, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_flaming_exeskeleton.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_flaming_exeskeleton.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetRegionalP03Card(CardTemple.Undead)
                .AddAbilities(Ability.Brittle, FireBomb.AbilityID, BurntOut.AbilityID);

            // Gas Conduit
            CardManager.New(EXP_2_PREFIX, "GasConduit", "Gas Conduit", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_gasconduit.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(ConduitGas.AbilityID, BurntOut.AbilityID);

            // Street Sweeper
            CardManager.New(EXP_2_PREFIX, "StreetSweeper", "Street Sweeper", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_streetcleaner.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 6)
                .SetNeutralP03Card()
                .SetWeaponMesh(
                    "p03kcm/prefabs/flamethrower",
                    localPosition: new Vector3(0f, 0f, 0f),
                    localRotation: new Vector3(0f, 90f, 0f),
                    localScale: new Vector3(0.75f, 0.75f, 0.75f)
                )
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_street_sweeper.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(FireBomb.AbilityID, Ability.Strafe);

            // Give-A-Way
            CardManager.New(EXP_2_PREFIX, "GiveAWay", "Shield Smuggler", 2, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_giveaway.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_giveaway.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetNeutralP03Card()
                .AddAbilities(EnemyGainShield.AbilityID);

            // Pity Seeker
            CardManager.New(EXP_2_PREFIX, "PitySeeker", "Pity Seeker", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_pity_seeker.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetRegionalP03Card(CardTemple.Tech, CardTemple.Undead)
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_pity_seeker.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(LatchNullConduit.AbilityID);

            // Urch1n Conduit
            CardManager.New(EXP_2_PREFIX, "UrchinConduit", "Urch1n Conduit", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_urchin_conduit.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Tech, CardTemple.Nature)
                .SetRare()
                //.SetRegionalP03Card(CardTemple.Tech, CardTemple.Nature)
                .AddAbilities(ConduitSpawnUrchin.AbilityID);

            // Rh1n0
            CardManager.New(EXP_2_PREFIX, "Rhino", "Rh1n0", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_rhino.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 5)
                .SetRegionalP03Card(CardTemple.Nature)
                .SetRare()
                .AddAbilities(ActivatedGainPowerCharge.AbilityID);

            // 3leph4nt
            CardManager.New(EXP_2_PREFIX, "Elephant", "3leph4nt", 1, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_elephant.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 5)
                .SetRegionalP03Card(CardTemple.Nature)
                .AddMetaCategories(CardMetaCategory.Part3Random)
                .AddAbilities(Stomp.AbilityID);

            // Jimmy Jr
            CardManager.New(EXP_2_PREFIX, "JimmyJr", "Jimmy Jr.", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_jimmy_jr.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetNeutralP03Card()
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_jimmy_jr.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(DrawTwoZap.AbilityID);

            // OP Bot
            CardManager.New(EXP_2_PREFIX, "OpBot", "OP Bot", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_op_bot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetNeutralP03Card()
                .AddAbilities(RandomNegativeAbility.AbilityID);

            // Emerald Squid
            CardManager.New(EXP_2_PREFIX, "EmeraldSquid", "656D6572616C64", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_greengemsquid.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetNeutralP03Card()
                .SetStatIcon(GreenGemPower.AbilityID);

            // Weeper
            CardManager.New(EXP_2_PREFIX, "Weeper", "Weeper", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_weeper.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_weeper.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetRegionalP03Card(CardTemple.Undead)
                .AddAbilities(EnergySiphon.AbilityID);

            // Suicell
            CardManager.New(EXP_2_PREFIX, "Suicell", "Sui-Cell", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_suicell.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_suicell.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(CellExplodonate.AbilityID);

            // Bedazzling Conduit
            CardManager.New(EXP_2_PREFIX, "KindnessGiver", "Bedazzling Conduit", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_bedazzled_conduit.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .AddPart3Decal(TextureHelper.GetImageAsTexture("decal_bedazzled_conduit.png", typeof(ExpansionPackCards_2).Assembly))
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(ConduitGemify.AbilityID, Ability.GainGemBlue);

            // Gopher
            CardManager.New(EXP_2_PREFIX, "Gopher", "G0ph3r", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_gopher.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_gopher.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 1)
                .SetRegionalP03Card(CardTemple.Nature)
                .AddAbilities(Ability.BuffEnemy, Miner.AbilityID);

            // P00dl3
            CardManager.New(EXP_2_PREFIX, "Poodle", "P00dl3", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_poodle.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_poodle.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Nature)
                .AddMetaCategories(CardMetaCategory.Part3Random)
                .AddAbilities(Ability.MoveBeside, Ability.BuffNeighbours);

            // Flame Charmter
            CardManager.New(EXP_2_PREFIX, "FlameCharmer", "Flamecharmer", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_flame_charmer.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .AddAbilities(FlameStoker.AbilityID, FriendliesMadeOfStone.AbilityID)
                .AddAppearances(RareDiscCardAppearance.ID)
                .temple = CardTemple.Tech;

            // Artillery Droid
            CardManager.New(EXP_2_PREFIX, "ArtilleryDroid", "Artillery Droid", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_artillery_droid.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetNeutralP03Card()
                .AddAbilities(MissileStrike.AbilityID);

            // Ultra Bot
            CardManager.New(EXP_2_PREFIX, "UltraBot", "Ultra Bot", 1, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_ultra_droid.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 5)
                .SetNeutralP03Card()
                .SetRare()
                .SetExtendedProperty(MissileStrike.NUMBER_OF_MISSILES, 3)
                .AddAbilities(MissileStrike.AbilityID);

            // Hellfire Droid
            CardManager.New(EXP_2_PREFIX, "HellfireDroid", "Hellfire Commando", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_hellfire_droid.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Undead)
                .SetRare()
                .AddAbilities(MissileStrikeSelf.AbilityID, Ability.Deathtouch);

            // Energy Vampire
            CardManager.New(EXP_2_PREFIX, "EnergyVampire", "Energy Vampire", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_energy_vampire.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_energy_vampire.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetRegionalP03Card(CardTemple.Undead)
                .AddAbilities(AbsorbShield.AbilityID);

            // Orange Moxduster
            CardManager.New(EXP_2_PREFIX, "OrangeMoxduster", "Orange Juicer", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_orange_moxduster.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddTraits(Trait.Gem)
                .AddPart3Decal(TextureHelper.GetImageAsTexture("decal_orange_moxduster.png", typeof(ExpansionPackCards_2).Assembly))
                .AddAbilities(Ability.GainGemOrange, MagicDust.AbilityID);

            // Dr Zambot
            CardManager.New(EXP_2_PREFIX, "DrZambot", "Dr Zambot", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_dr_zambot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_dr_zambot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Nature)
                .SetRare()
                .SetWeaponMesh(
                    "p03kcm/prefabs/syringe_gun_low",
                    localPosition: new Vector3(0f, -.3f, 0f),
                    localRotation: new Vector3(0f, 270f, 45f),
                    localScale: new Vector3(0.04f, 0.03f, 0.03f)
                )
                .AddAbilities(DrawUpgrade.AbilityID);

            // Trash Compactor
            CardManager.New(EXP_2_PREFIX, "TrashCompactor", "Chippy", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_disposal_bot.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Undead)
                .SetRare()
                .AddAbilities(Shred.AbilityID);

            // L33pbot Neo
            CardInfo lpneo = CardManager.New(EXP_2_PREFIX, "LeapBotNeo", "L33pB0t Neo", 2, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_leapbot_neo.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetCardTemple(CardTemple.Tech)
                .AddAppearances(RareDiscCardAppearance.ID)
                .AddAbilities(Ability.Reach, Hopper.AbilityID);

            CardManager.BaseGameCards.First(c => c.name == "LeapBot").SetEvolve(lpneo, 1);

            // Encapsulator
            CardManager.New(EXP_2_PREFIX, "Encapsulator", "Encapsulator", 0, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_encapsulator.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_encapsulator.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetNeutralP03Card()
                .AddAbilities(ActivatedDrawCharge.AbilityID);

            // Sir Blast
            CardManager.New(EXP_2_PREFIX, "SirBlast", "Sir Blast", 2, 3)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_sirblast.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetNeutralP03Card()
                .SetRare()
                .AddAbilities(MolotovAll.AbilityID);

            CardManager.New(EXP_2_PREFIX, "RoboRingworm", "Tapeworm", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_tapeworm.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_ringworm.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetTraits(CustomCards.UpgradeVirus)
                .SetNeutralP03Card();

            // Lockjaw Cell
            CardManager.New(EXP_2_PREFIX, "LockjawCell", "Lockjaw Cell", 1, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_lockjaw_cell.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Tech)
                .AddAbilities(Ability.Reach, CellSteelTrap.AbilityID);

            // Bleene's Acolyte
            CardManager.New(EXP_2_PREFIX, "BleeneAcolyte", "Replica Bleene", 0, 2)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_bleene_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_bleene_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 2)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAppearances(ReplicaAppearanceBehavior.ID)
                .SetExtendedProperty(ReplicaAppearanceBehavior.REPLICA_TYPE, "bleene")
                .AddDecal(
                    TextureHelper.GetImageAsTexture("decal_bleene_acolyte_blue.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_bleene_acolyte_green.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_bleene_acolyte_bleene.png", typeof(ExpansionPackCards_2).Assembly)
                )
                .AddAbilities(GemBlueGift.AbilityID, GemGreenGift.AbilityID);

            // Orlu's Replica
            CardManager.New(EXP_2_PREFIX, "OrluAcolyte", "Replica Orlu", 1, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_orlu_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_orlu_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAppearances(ReplicaAppearanceBehavior.ID)
                .SetExtendedProperty(ReplicaAppearanceBehavior.REPLICA_TYPE, "orlu")
                .AddDecal(
                    TextureHelper.GetImageAsTexture("decal_orlu_acolyte_orange.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_orlu_acolyte_blue.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_orlu_acolyte_orlu.png", typeof(ExpansionPackCards_2).Assembly)
                )
                .AddAbilities(GemOrangeFlying.AbilityID, GemBlueLoot.AbilityID);

            // Goranj's Replica
            CardManager.New(EXP_2_PREFIX, "GoranjAcolyte", "Replica Goranj", 2, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_goranj_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetPixelPortrait(TextureHelper.GetImageAsTexture("pixelportrait_goranj_acolyte.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAppearances(ReplicaAppearanceBehavior.ID)
                .SetExtendedProperty(ReplicaAppearanceBehavior.REPLICA_TYPE, "goranj")
                .AddDecal(
                    TextureHelper.GetImageAsTexture("decal_goranj_acolyte_green.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_goranj_acolyte_orange.png", typeof(ExpansionPackCards_2).Assembly),
                    TextureHelper.GetImageAsTexture("decal_goranj_acolyte_goranj.png", typeof(ExpansionPackCards_2).Assembly)
                )
                .AddAbilities(GemOrangeBrittle.AbilityID, GemGreenBuffEnemy.AbilityID);

            // Gem Augur
            CardManager.New(EXP_2_PREFIX, "GemAugur", "Gem Auger", 2, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_gem_augur.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 6)
                .SetRare()
                .SetRegionalP03Card(CardTemple.Wizard)
                .AddAbilities(GemStrike.AbilityID);

            // Sparkplug Cell
            CardManager.New(EXP_2_PREFIX, "SparkplugCell", "Sparkplug Cell", 0, 4)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_short_circuit_cell.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 3)
                .SetRegionalP03Card(CardTemple.Tech)
                .SetRare()
                .AddAbilities(CellDrawZapUpkeep.AbilityID);

            // Splice Conduit
            CardManager.New(EXP_2_PREFIX, "SpliceConduit", "Splice Conduit", 0, 1)
                .SetPortrait(TextureHelper.GetImageAsTexture("portrait_splice_conduit.png", typeof(ExpansionPackCards_2).Assembly))
                .SetCost(energyCost: 4)
                .SetRegionalP03Card(CardTemple.Tech)
                .SetRare()
                .AddAbilities(ConduitAbsorb.AbilityID);
        }
    }
}