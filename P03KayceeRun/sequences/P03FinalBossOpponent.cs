using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.LightningBolt;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Cards;
using Infiniscryption.P03KayceeRun.Encounters;
using Infiniscryption.P03KayceeRun.Patchers;
using Infiniscryption.Spells.Patchers;
using InscryptionAPI.Card;
using Pixelplacement;
using Sirenix.Serialization.Utilities;
using Sirenix.Utilities;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Sequences
{
    [HarmonyPatch]
    public class P03AscensionOpponent : Part3BossOpponent
    {
        public override string PreIntroDialogueId => "";
        public override bool GiveCurrencyOnDefeat => false;
        public override string PostDefeatedDialogueId => "P03AscensionDefeated";

        private readonly List<string> PhaseTwoWeirdCards = new() { "MantisGod", "Moose", "Grizzly", "FrankNStein", "Amalgam", "Adder", "JuniorSage", "PracticeMage", "Revenant", "Bonehound", "RubyGolem" };

        private CardInfo PhaseTwoBlocker;
        private CardInfo PhaseTwoTree
        {
            get
            {
                CardInfo info = CardLoader.GetCardByName("PracticeMage");
                info.mods.Add(new(Ability.Reach));
                return info;
            }
        }

        private static readonly CardSlot CardSlotPrefab = ResourceBank.Get<CardSlot>("Prefabs/Cards/CardSlot_Part3");

        private static readonly Dictionary<CardTemple, GameObject> CardPrefabs = new()
        {
            { CardTemple.Undead, ResourceBank.Get<GameObject>("Prefabs/Cards/PlayableCard_Grimora") },
            { CardTemple.Wizard, ResourceBank.Get<GameObject>("Prefabs/Cards/PlayableCard_Magnificus") },
            { CardTemple.Nature, ResourceBank.Get<GameObject>("Prefabs/Cards/PlayableCard") }
        };

        private static readonly Dictionary<CardTemple, GameObject> CardRenderCameraPrefabs = new()
        {
            { CardTemple.Undead, ResourceBank.Get<GameObject>("Prefabs/Cards/CardRenderCamera_Grimora") },
            { CardTemple.Wizard, ResourceBank.Get<GameObject>("Prefabs/Cards/CardRenderCamera_Magnificus") },
            { CardTemple.Nature, ResourceBank.Get<GameObject>("Prefabs/Cards/CardRenderCamera") },
            //{ CardTemple.Tech, ResourceBank.Get<GameObject>("Prefabs/Cards/CardRenderCamera_Part3") }
        };

        private static Dictionary<CardTemple, CardRenderCamera> CardRenderCameras = null;

        private static readonly HighlightedInteractable OpponentQueueSlotPrefab = ResourceBank.Get<HighlightedInteractable>("Prefabs/Cards/QueueSlot");

        private bool FasterEvents = false;
        private bool HasDoneFirstWeirdCards = false;

        public P03FinalBossScreenArray ScreenArray;

        private readonly GameObject audioObject = new("P03BossMusicAudioObject");
        public AudioSource audioSource;

        private List<Color> slotColors;
        private List<Color> queueSlotColors;

        private void InitializeCards()
        {
            FasterEvents = StoryEventsData.EventCompleted(EventManagement.HAS_DEFEATED_P03);

            PhaseTwoBlocker = CardLoader.GetCardByName("MoleMan");

            int difficulty = AscensionSaveData.Data.GetNumChallengesOfTypeActive(AscensionChallenge.BaseDifficulty);

            if (difficulty >= 1)
            {
                PhaseTwoWeirdCards.Remove("Grizzly");
                PhaseTwoWeirdCards.Remove("PracticeMage");
                PhaseTwoWeirdCards.Add("Shark");
                PhaseTwoWeirdCards.Add("Moose");
                PhaseTwoBlocker.mods.Add(new(Ability.DeathShield));
            }
            if (difficulty == 2)
            {
                PhaseTwoWeirdCards.Remove("FrankNStein");
                PhaseTwoWeirdCards.Remove("Adder");
                PhaseTwoWeirdCards.Remove("Revenant");
                PhaseTwoWeirdCards.Add("Urayuli");
                PhaseTwoBlocker.mods.Add(new(Ability.Sharp));
            }
        }

        public override IEnumerator PreDefeatedSequence()
        {
            ViewManager.Instance.SwitchToView(View.Default, false, false);
            ScreenArray.EndLoadingFaces(P03AnimationController.Face.SurrenderFlag);
            ScreenArray.ShowFaceImmediate(P03AnimationController.Face.SurrenderFlag);
            yield return new WaitForSeconds(1.5f);

            //Turn off the boss music
            // GameObject bossMusic = GameObject.Find("P03BossMusicAudioObject");
            // GameObject.Destroy(bossMusic);
            AudioController.Instance.StopAllLoops();
            yield return new WaitForSeconds(0.1f);
            ScreenArray.Collapse();
            yield return new WaitForSeconds(5f);
        }

        [HarmonyPatch(typeof(BountyHunter), nameof(BountyHunter.OnDie))]
        [HarmonyPostfix]
        public static IEnumerator NoOuttroDuringBoss(IEnumerator sequence)
        {
            if (TurnManager.Instance.opponent is P03AscensionOpponent)
                yield break;

            yield return sequence;
        }

        private CardInfo GenerateCard(int turn)
        {
            if (NextTurnQueueSpecial.Count > 0)
            {
                CardInfo next = NextTurnQueueSpecial[0];
                NextTurnQueueSpecial.Remove(next);
                return next;
            }

            if (NumLives == 3)
                return BountyHunterGenerator.GenerateCardInfo(BountyHunterGenerator.GenerateMod(turn, (4 * turn) + 6));

            if (NumLives == 2)
            {
                int randomSeed = P03AscensionSaveData.RandomSeed + (100 * TurnManager.Instance.TurnNumber) + BoardManager.Instance.opponentSlots.Where(s => BoardManager.Instance.GetCardQueuedForSlot(s) != null).Count();
                string cardName = PhaseTwoWeirdCards[SeededRandom.Range(0, PhaseTwoWeirdCards.Count, randomSeed)];
                return CardLoader.GetCardByName(cardName);
            }

            return null;
        }

        public override void SetSceneEffectsShown(bool shown)
        {
            if (shown)
            {
                ScreenArray = P03FinalBossScreenArray.Create(BoardManager.Instance.gameObject.transform);
                ScreenArray.transform.localPosition = new(0f, 0f, 7f);
                ScreenArray.ShowFace(P03AnimationController.Face.Happy);
            }
            else
            {
                if (ScreenArray != null)
                {
                    ScreenArray.StopAllCoroutines();
                    Destroy(ScreenArray.gameObject);
                }
            }
        }

        public override IEnumerator IntroSequence(EncounterData encounter)
        {
            yield return TextDisplayer.Instance.PlayDialogueEvent(PreIntroDialogueId, TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.P03Face, false, true);
            yield return new WaitForSeconds(0.1f);

            // Pause background audio?
            AudioController.Instance.SetLoopPaused(true);

            // audioSource = audioObject.AddComponent<AudioSource>();
            // string path = AudioHelper.FindAudioClip("P03_Phase1");
            // AudioClip audioClip = InscryptionAPI.Sound.SoundManager.LoadAudioClip(path);
            // audioSource.clip = audioClip;
            // audioSource.loop = true;
            // audioSource.volume = BossManagement.bossMusicVolume;
            // audioSource.Play();

            AudioController.Instance.SetLoopAndPlay($"P03_Phase1", 0, true, true);
            AudioController.Instance.SetLoopVolumeImmediate(0.35f, 0);
            yield return StartBattleSequence();
            yield break;
        }

        public override IEnumerator StartBattleSequence()
        {
            NumLives = 3;

            InitializeCards();

            yield return new WaitForSeconds(1f);

            yield return TextDisplayer.Instance.PlayDialogueEvent("P03AboutMe", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.P03FaceClose, false, false);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03AboutMe2", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.Default, false, false);
            SetSceneEffectsShown(true);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03AboutMe3", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            yield return new WaitForSeconds(1f);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03IntroductionToModding", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            // ViewManager.Instance.SwitchToView(View.P03FaceClose, false, false);
            // yield return TextDisplayer.Instance.PlayDialogueEvent("P03IntroductionClose", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.BoardCentered, false, false);

            yield return QueueCard(GenerateCard(0), BoardManager.Instance.OpponentSlotsCopy[2], true, true, true);
            yield return new WaitForSeconds(0.15f);

            yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseOne", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            yield return new WaitForSeconds(0.45f);
            ViewManager.Instance.SwitchToView(View.Default, false, false);
        }

        public override IEnumerator StartNewPhaseSequence()
        {
            if (NumLives == 2)
            {
                yield return new WaitForSeconds(1f);
                ViewManager.Instance.SwitchToView(View.Board, false, false);
                yield return ClearBoard();
                yield return ClearQueue();
                ScreenArray.ShowFace(P03AnimationController.Face.Angry, P03AnimationController.Face.Bored);

                // Go ahead and instantiate the card render cameras for the cameos from part 1/2/3 cards in this stage
                CardRenderCamera oldInstance = CardRenderCamera.m_Instance;
                CardRenderCameras = new();
                int idx = 2;
                CardRenderCameras[CardTemple.Tech] = oldInstance;
                foreach (KeyValuePair<CardTemple, GameObject> cameraInfo in CardRenderCameraPrefabs)
                {
                    GameObject obj = Instantiate(cameraInfo.Value, Part3GameFlowManager.Instance.transform);
                    obj.transform.position = obj.transform.position + (Vector3.down * 10f * idx++);
                    CardRenderCamera camera = obj.GetComponentInChildren<CardRenderCamera>();
                    camera.gameObject.name = $"SpecialRenderCamera{cameraInfo.Key}";


                    RenderTexture newRendTex = new(camera.snapshotRenderTexture.width, camera.snapshotRenderTexture.height, camera.snapshotRenderTexture.depth, camera.snapshotRenderTexture.format);
                    newRendTex.Create();

                    RenderTexture newEmTex = new(camera.snapshotEmissionRenderTexture.width, camera.snapshotEmissionRenderTexture.height, camera.snapshotEmissionRenderTexture.depth, camera.snapshotEmissionRenderTexture.format);
                    newEmTex.Create();

                    foreach (Camera unityCamera in camera.GetComponentsInChildren<Camera>())
                    {
                        if (unityCamera.targetTexture == camera.snapshotRenderTexture)
                            unityCamera.targetTexture = camera.snapshotRenderTexture = newRendTex;
                        if (unityCamera.targetTexture == camera.snapshotEmissionRenderTexture)
                            unityCamera.targetTexture = camera.snapshotEmissionRenderTexture = newEmTex;
                    }

                    CardRenderCameras[cameraInfo.Key] = camera;
                }
                CardRenderCamera.m_Instance = oldInstance;

                ViewManager.Instance.SwitchToView(View.Default, false, false);
                P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Angry, true, true);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseTwo", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                // string path = AudioHelper.FindAudioClip("P03_Phase2");
                // AudioClip audioClip = InscryptionAPI.Sound.SoundManager.LoadAudioClip(path);
                // audioSource.clip = audioClip;
                // audioSource.loop = true;
                // audioSource.volume = BossManagement.bossMusicVolume;
                // audioSource.Play();

                AudioController.Instance.SetLoopAndPlay($"P03_Phase2", 1, true, false);
                AudioController.Instance.SetLoopVolumeImmediate(0f, 1);
                AudioController.Instance.SetLoopVolume(0f, 0f, 0, false);
                AudioController.Instance.loopSources[1].Stop();
                AudioController.Instance.loopSources[1].time = AudioController.Instance.loopSources[0].time;
                AudioController.Instance.loopSources[1].Play();
                AudioController.Instance.SetLoopVolume(0.35f, 1f, 1, false);

                ViewManager.Instance.SwitchToView(View.P03Face, false, false);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseTwoInControl", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                yield return new WaitForSeconds(1f);

                ScreenArray.ShowFace(P03AnimationController.Face.Angry, P03AnimationController.Face.Default, P03AnimationController.Face.Bored);
                ViewManager.Instance.SwitchToView(View.Default, false, false);
                yield return new WaitForSeconds(0.15f);
                ViewManager.Instance.SwitchToView(View.Board, false, false);
                yield return BoardManager.Instance.CreateCardInSlot(PhaseTwoBlocker, BoardManager.Instance.OpponentSlotsCopy[2]);
                yield return new WaitForSeconds(0.75f);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseTwoWeirdCards", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                yield return new WaitForSeconds(1f);

                if (!FasterEvents)
                {
                    ViewManager.Instance.SwitchToView(View.BoneTokens, false, false);
                    GameObject prefab = Resources.Load<GameObject>("prefabs/cardbattle/CardBattle").GetComponentInChildren<Part1ResourcesManager>().gameObject;
                    GameObject part1ResourceManager = Instantiate(prefab, Part3ResourcesManager.Instance.gameObject.transform.parent);
                    WeirdManager = part1ResourceManager.GetComponent<Part1ResourcesManager>();

                    yield return WeirdManager.AddBones(50);
                    yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseTwoBones", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

                    yield return new WaitForSeconds(1f);
                }

                ViewManager.Instance.SwitchToView(View.Board, false, false);

                foreach (CardSlot slot in BoardManager.Instance.OpponentSlotsCopy)
                {
                    if (slot.Card == null && slot.opposingSlot.Card != null)
                    {
                        yield return BoardManager.Instance.CreateCardInSlot(PhaseTwoTree, slot);
                        yield return new WaitForSeconds(0.45f);
                    }
                }

                adjustedPlanP2 = new int[TurnManager.Instance.TurnNumber + MODDERS_PART_2.Length];
                for (int i = 0; i < MODDERS_PART_2.Length; i++)
                    adjustedPlanP2[TurnManager.Instance.TurnNumber + i] = MODDERS_PART_2[i];

                yield return new WaitForSeconds(0.35f);
                yield return QueueNewCards(true, true);
                yield return new WaitForSeconds(0.5f);

                ViewManager.Instance.SwitchToView(View.Default, false, false);
                yield break;
            }

            yield return PhaseThreeSequence();

        }

        [HarmonyPatch(typeof(ViewManager), nameof(ViewManager.GetViewInfo))]
        [HarmonyPostfix]
        public static void ChangeFOVForBoss(ref ViewInfo __result, View view)
        {
            if (view is View.Board or View.BoardCentered)
            {
                if (BoardManager.Instance != null && BoardManager.Instance.playerSlots != null && BoardManager.Instance.playerSlots.Count == 7)
                {
                    __result.fov = 63f;
                }
            }
        }

        private static void FixOpposingSlots()
        {
            for (int i = 0; i < BoardManager.Instance.playerSlots.Count; i++)
            {
                if (BoardManager.Instance.playerSlots[i].opposingSlot == null)
                    BoardManager.Instance.playerSlots[i].opposingSlot = BoardManager.Instance.opponentSlots[i];

                if (BoardManager.Instance.opponentSlots[i].opposingSlot == null)
                    BoardManager.Instance.opponentSlots[i].opposingSlot = BoardManager.Instance.playerSlots[i];
            }
            MultiverseBattleSequencer.ClearAllSlotCacheShenanigans();
        }

        private static float GetXPos(bool beginning, bool isOpponent, bool isQueue)
        {
            return !isOpponent
                ? beginning ? BoardManager.Instance.playerSlots.First().transform.localPosition.x : BoardManager.Instance.playerSlots.Last().transform.localPosition.x
                : isQueue
                ? beginning ? BoardManager.Instance.opponentQueueSlots.First().transform.localPosition.x : BoardManager.Instance.opponentQueueSlots.Last().transform.localPosition.x
                : beginning ? BoardManager.Instance.opponentSlots.First().transform.localPosition.x : BoardManager.Instance.opponentSlots.Last().transform.localPosition.x;
        }

        internal static HighlightedInteractable CreateSlot(bool beginning, bool isOpponent, bool isQueue)
        {
            HighlightedInteractable prefab = isQueue ? OpponentQueueSlotPrefab : CardSlotPrefab;
            Transform parent = BoardManager3D.Instance.gameObject.transform.Find(isOpponent ? "OpponentSlots" : "PlayerSlots");

            HighlightedInteractable slot = Instantiate(prefab, parent);
            string nameBase = isOpponent ? "OpponentSlot" : "Playerslot";
            nameBase += beginning ? "-1" : "5";
            slot.name = nameBase;

            float deltaX = BoardManager.Instance.playerSlots[1].transform.localPosition.x - BoardManager.Instance.playerSlots[0].transform.localPosition.x;

            float xPos = beginning ? GetXPos(beginning, isOpponent, isQueue) - deltaX : GetXPos(beginning, isOpponent, isQueue) + deltaX;

            Vector3 refVec = !isOpponent ? BoardManager.Instance.playerSlots[0].transform.localPosition : isQueue ? BoardManager.Instance.opponentQueueSlots[0].transform.localPosition : BoardManager.Instance.opponentSlots[0].transform.localPosition;

            slot.transform.localPosition = new Vector3(xPos, refVec.y, refVec.z);

            if (isQueue)
            {
                if (beginning) BoardManager.Instance.opponentQueueSlots.Insert(0, slot);
                else BoardManager.Instance.opponentQueueSlots.Add(slot);
            }
            else
            {
                if (isOpponent)
                {
                    Transform quad = slot.transform.Find("Quad");
                    quad.rotation = Quaternion.Euler(90f, 180f, 0f);
                }

                List<CardSlot> slots = isOpponent ? BoardManager.Instance.opponentSlots : BoardManager.Instance.playerSlots;
                if (beginning) slots.Insert(0, slot as CardSlot);
                else slots.Add(slot as CardSlot);
            }

            BoardManager.Instance.allSlots = null;
            List<CardSlot> dummy = BoardManager.Instance.AllSlots;

            return slot;
        }

        private IEnumerator CreateSlotSequence(bool beginning, bool isOpponent, bool isQueue)
        {
            // Force the boardmanager to reset its list of slots
            HighlightedInteractable slot = CreateSlot(beginning, isOpponent, isQueue);

            if (isQueue)
                slot.SetColors(queueSlotColors[0], queueSlotColors[1], queueSlotColors[2]);
            else
                slot.SetColors(slotColors[0], slotColors[1], slotColors[2]);

            GameObject lightning = Instantiate(ResourceBank.Get<GameObject>("Prefabs/Environment/TableEffects/LightningBolt"));
            lightning.GetComponent<LightningBoltScript>().EndObject = slot.gameObject;
            Destroy(lightning, 0.65f);
            slot.OnCursorEnter();
            yield return new WaitForSeconds(0.95f);
            slot.OnCursorExit();
            yield break;
        }

        internal static void CreateAllSlots()
        {
            CreateSlot(true, false, false);
            CreateSlot(true, true, false);
            CreateSlot(true, true, true);
            CreateSlot(false, false, false);
            CreateSlot(false, true, false);
            CreateSlot(false, true, true);
            FixOpposingSlots();
        }

        private IEnumerator CreateAllSlotsSequence()
        {
            yield return CreateSlotSequence(true, false, false);
            yield return CreateSlotSequence(true, true, false);
            yield return CreateSlotSequence(true, true, true);
            yield return CreateSlotSequence(false, false, false);
            yield return CreateSlotSequence(false, true, false);
            yield return CreateSlotSequence(false, true, true);
            FixOpposingSlots();
        }

        private IEnumerator PhaseThreeSequence()
        {
            // Phase three
            yield return ClearQueue();
            yield return ClearBoard();

            CardRenderCamera defaultCam = CardRenderCameras[CardTemple.Tech];
            CardRenderCamera.m_Instance = defaultCam;
            CardRenderCameras.Remove(CardTemple.Tech);
            foreach (var cam in CardRenderCameras.Values)
            {
                cam.snapshotRenderTexture.Release();
                cam.snapshotEmissionRenderTexture.Release();
                Destroy(cam.gameObject);
            }
            CardRenderCameras = null;

            OpponentAnimationController.Instance.ClearLookTarget();

            ScreenArray.ShowFace(P03AnimationController.Face.Bored);

            yield return new WaitForSeconds(1f);

            if (WeirdManager != null)
            {
                yield return WeirdManager.SpendBones(WeirdManager.PlayerBones);
                yield return new WaitForSeconds(0.5f);
                Destroy(WeirdManager.gameObject, 0.25f);
                WeirdManager = null;
            }
            ViewManager.Instance.SwitchToView(View.Default, false, false);
            P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Angry, true, true);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseThree", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            AudioController.Instance.SetLoopVolumeImmediate(0f, 1);

            yield return new WaitForSeconds(FasterEvents ? 0.6f : 1.5f);
            P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Happy, true, true);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseThreeStartShowingOff", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Thinking, true, true);
            yield return new WaitForSeconds(0.2f);
            yield return new WaitForSeconds(FasterEvents ? 1f : 2f);
            PhaseTwoEffects();
            yield return new WaitForSeconds(FasterEvents ? 1f : 2f);

            float durationOfEffect = FasterEvents ? 3f : 6.5f;

            CameraEffects.Instance.Shake(0.05f, 100f); // Essentially just shake forever; I'll manually stop the shake later
            AudioSource source = AudioController.Instance.PlaySound2D("glitch_escalation", MixerGroup.TableObjectsSFX, volume: 0.4f);
            yield return new WaitForSeconds(0.2f);
            ScreenArray.ShowBigMoon();
            yield return new WaitForSeconds(0.2f);

            // Tween each of the four things that need to move
            Transform itemTrans = ItemsManager.Instance.gameObject.transform;
            Vector3 newItemPos = new(6.75f, itemTrans.localPosition.y, itemTrans.localPosition.z + 1.3f);
            Tween.LocalPosition(itemTrans, newItemPos, durationOfEffect, 0f);

            Transform hammerTrans = ItemsManager.Instance.Slots.FirstOrDefault(s => s.name.ToLowerInvariant().StartsWith("hammer")).gameObject.transform;
            Vector3 newHammerPos = new(-9.5f, hammerTrans.localPosition.y, hammerTrans.localPosition.z - 1.3f);
            Tween.LocalPosition(hammerTrans, newHammerPos, durationOfEffect, 0f);

            Transform bellTrans = (BoardManager.Instance as BoardManager3D).bell.gameObject.transform;
            Vector3 newBellPos = new(-5f, bellTrans.localPosition.y, bellTrans.localPosition.z);
            Tween.LocalPosition(bellTrans, newBellPos, durationOfEffect, 0f);

            Transform scaleTrans = LifeManager.Instance.Scales3D.gameObject.transform;
            Vector3 newScalePos = new(-6, scaleTrans.localPosition.y, scaleTrans.localPosition.z);
            Tween.LocalPosition(scaleTrans, newScalePos, durationOfEffect, 0f);
            yield return new WaitForSeconds(durationOfEffect);

            // Create two new slots
            yield return CreateAllSlotsSequence();

            CameraEffects.Instance.StopShake();
            AudioController.Instance.FadeSourceVolume(source, 0f, 1f);
            yield return new WaitForSeconds(1f);
            source.Stop();
            yield return new WaitForSeconds(1f);

            // string path = AudioHelper.FindAudioClip("P03_Phase3");
            // AudioClip audioClip = InscryptionAPI.Sound.SoundManager.LoadAudioClip(path);
            // audioSource.clip = audioClip;
            // audioSource.loop = true;
            // audioSource.volume = BossManagement.bossMusicVolume;
            // audioSource.Play();

            AudioController.Instance.SetLoopAndPlay($"P03_Phase3", 0, true, true);
            AudioController.Instance.SetLoopVolumeImmediate(0.35f, 0);

            yield return new WaitForSeconds(1f);

            yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseThreeBehold", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            ViewManager.Instance.SwitchToView(View.Board, false, false);

            yield return TextDisplayer.Instance.PlayDialogueEvent("P03PhaseThreeSevenSlots1", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            // We're guaranteed that two lanes will be empty so this is going to work for sure
            foreach (CardSlot slot in BoardManager.Instance.playerSlots.Where(s => s.Card != null))
            {
                CardInfo bot = CardLoader.GetCardByName("BrokenBot");
                bot.mods = new() { new() { abilities = new() { Ability.Reach } } };
                yield return BoardManager.Instance.CreateCardInSlot(bot, slot.opposingSlot);
                yield return new WaitForSeconds(0.66f);
            }
            yield return new WaitForSeconds(0.33f);

            CardInfo firewallA = CardLoader.GetCardByName(CustomCards.FIREWALL_LARGE);
            // firewallA.mods.Add(new(Ability.GuardDog));
            // firewallA.mods.Add(new(Ability.DeathShield));
            yield return BoardManager.Instance.CreateCardInSlot(firewallA, BoardManager.Instance.opponentSlots[0]);
            yield return new WaitForSeconds(0.66f);

            CardInfo firewallB = CardLoader.GetCardByName(CustomCards.FIREWALL_LARGE);
            // firewallB.mods.Add(new(Ability.StrafeSwap));
            // firewallB.mods.Add(new(Ability.DeathShield));
            yield return BoardManager.Instance.CreateCardInSlot(firewallB, BoardManager.Instance.opponentSlots[6]);
            yield return new WaitForSeconds(1.5f);

            yield return ReplaceBlueprint("P03FinalBoss");
            yield return new WaitForSeconds(1f);
            ViewManager.Instance.SwitchToView(View.Default);
        }

        private Part1ResourcesManager WeirdManager = null;

        private void PhaseTwoEffects(bool showEffects = true)
        {
            TableVisualEffectsManager.Instance.SetDustParticlesActive(!showEffects);
            if (showEffects)
            {
                ScreenArray.EndLoadingFaces(P03FinalBossExtraScreen.LOOKUP_FACE);
                ScreenArray.RecolorFrames(P03FinalBossExtraScreen.RedFrameColor);
                UIManager.Instance.Effects.GetEffect<ScreenColorEffect>().SetColor(GameColors.Instance.nearWhite);
                UIManager.Instance.Effects.GetEffect<ScreenColorEffect>().SetAlpha(1f);
                UIManager.Instance.Effects.GetEffect<ScreenColorEffect>().SetIntensity(0f, 1f);
                //SpawnScenery("LightQuadTableEffect");

                Color angryColor = GameColors.Instance.red;
                Color partiallyTransparentRed = new(angryColor.r, angryColor.g, angryColor.b, 0.5f);

                TableVisualEffectsManager.Instance.ChangeTableColors(angryColor, Color.black, GameColors.Instance.nearWhite, partiallyTransparentRed, angryColor, Color.white, GameColors.Instance.gray, GameColors.Instance.gray, GameColors.Instance.lightGray);

                slotColors = new() { partiallyTransparentRed, angryColor, Color.white };
                queueSlotColors = new() { GameColors.Instance.gray, GameColors.Instance.gray, GameColors.Instance.lightGray };

                FactoryManager.Instance.HandLight.color = GameColors.Instance.orange;
            }
            else
            {
                TableVisualEffectsManager.Instance.ResetTableColors();
                FactoryManager.Instance.HandLight.color = GameColors.Instance.blue;
                P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Default, true, true);

                if (WeirdManager != null)
                {
                    Destroy(WeirdManager, 0.25f);
                    WeirdManager = null;
                }
            }
        }

        private static readonly int[] MODDERS_PART_1 = new int[] { 0, 1, 0, 1, 1, 1, 0, 2, 1, 0, 2 };

        private static readonly int[] MODDERS_PART_2 = new int[] { 2, 1, 2, 0, 1, 2, 0, 1, 2, 1 };

        private int[] adjustedPlanP2;

        private static CardTemple GetRendererTemple(CardInfo info)
        {
            if (info.HasTrait(Trait.Giant))
                return info.temple;

            string rendererOverrideTemple = info.GetExtendedProperty("Renderer.OverrideTemple");

            if (!string.IsNullOrEmpty(rendererOverrideTemple))
            {
                bool success = Enum.TryParse<CardTemple>(rendererOverrideTemple, out CardTemple rendTemple);
                if (success)
                    return rendTemple;
            }

            string packManagerTemple = info.GetExtendedProperty("PackManager.OriginalTemple");
            if (!string.IsNullOrEmpty(packManagerTemple))
            {
                bool success = Enum.TryParse<CardTemple>(packManagerTemple, out CardTemple packTemple);
                if (success)
                    return packTemple;
            }
            return info.temple;
        }

        private static CardTemple? SaveFileOverride = null;

        [HarmonyPatch(typeof(SaveFile), nameof(SaveFile.IsPart1), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool HackPart1(ref bool __result)
        {
            if (SaveFileOverride.HasValue && SaveFileOverride.Value == CardTemple.Nature)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SaveFile), nameof(SaveFile.IsPart3), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool HackPart3(ref bool __result)
        {
            if (SaveFileOverride.HasValue && SaveFileOverride.Value != CardTemple.Tech)
            {
                __result = false;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SaveFile), nameof(SaveFile.IsGrimora), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool HackGrimora(ref bool __result)
        {
            if (SaveFileOverride.HasValue && SaveFileOverride.Value == CardTemple.Undead)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(SaveFile), nameof(SaveFile.IsMagnificus), MethodType.Getter)]
        [HarmonyPrefix]
        private static bool HackMagnificus(ref bool __result)
        {
            if (SaveFileOverride.HasValue && SaveFileOverride.Value == CardTemple.Wizard)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(RenderStatsLayer), nameof(RenderStatsLayer.RenderCard))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.VeryHigh)]
        private static bool OverrideRenderInP03Boss(RenderStatsLayer __instance, CardRenderInfo info)
        {
            if (TurnManager.Instance != null
                && TurnManager.Instance.Opponent != null
                && TurnManager.Instance.Opponent is P03AscensionOpponent p03
                && p03.NumLives == 2
                && CardRenderCameras != null)
            {
                if (__instance.Renderer != null)
                {
                    SaveFileOverride = GetRendererTemple(info.baseInfo);
                    bool emissionEnabled = CardDisplayer3D.EmissionEnabledForCard(info, __instance.PlayableCard);
                    if (!emissionEnabled)
                        __instance.DisableEmission();

                    P03Plugin.Log.LogInfo($"Rendering {info.baseInfo.name} {GetRendererTemple(info.baseInfo)} {CardRenderCameras[GetRendererTemple(info.baseInfo)].gameObject.name}");
                    CardRenderCameras[GetRendererTemple(info.baseInfo)].QueueStatsLayerForRender(info, __instance, __instance.PlayableCard, __instance.RenderToMainTexture, emissionEnabled);
                    SaveFileOverride = null;
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(GravestoneCardAnimationController), nameof(GravestoneCardAnimationController.SetCardRendererFlipped))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.VeryHigh)]
        private static bool HackForGrimAnim(GravestoneCardAnimationController __instance, bool flipped)
        {
            if (P03AscensionSaveData.IsP03Run)
            {
                __instance.armAnim.transform.localEulerAngles = !flipped ? new Vector3(-270f, 90f, -90f) : new Vector3(-90f, 0f, 0f);
                __instance.armAnim.transform.localPosition = !flipped ? new Vector3(0f, -0.1f, -0.1f) : new Vector3(0f, 0.24f, -0.1f);
                __instance.damageMarks.transform.localPosition = !flipped ? new Vector3(0.19f, -0.37f, -0.01f) : new Vector3(-0.21f, -0.1f, -0.01f);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(CardSpawner), nameof(CardSpawner.SpawnPlayableCard))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.VeryHigh)]
        private static bool MakeCardWithAppropriatePrefab(CardInfo info, ref PlayableCard __result)
        {
            if (TurnManager.Instance != null
                && TurnManager.Instance.Opponent != null
                && TurnManager.Instance.Opponent is P03AscensionOpponent p03
                && p03.NumLives == 2
                && GetRendererTemple(info) != CardTemple.Tech)
            {
                P03Plugin.Log.LogInfo("In Custom Make Card");
                SaveFileOverride = GetRendererTemple(info);
                GameObject card = Instantiate(CardPrefabs[GetRendererTemple(info)]);
                PlayableCard playableCard = card.GetComponent<PlayableCard>();
                playableCard.SetInfo(info);
                __result = playableCard;

                // Kind of a funny hack...there's got to be a better way to fix this
                // If I don't do this, the gravestone cards are upside down.
                if (GetRendererTemple(info) == CardTemple.Undead)
                {
                    GameObject newParent = new("Part3Parent");
                    newParent.transform.SetParent(card.transform);
                    //card.transform.Find("SkeletonAttackAnim").SetParent(newParent.transform);
                    card.transform.Find("RotatingParent").SetParent(newParent.transform);
                    newParent.transform.localPosition = Vector3.zero;
                    newParent.transform.localScale = Vector3.one;
                    newParent.transform.localEulerAngles = new(90f, 180f, 0f);
                }

                SaveFileOverride = null;
                return false;
            }
            return true;
        }

        private List<CardInfo> NextTurnQueueSpecial = new();

        public override IEnumerator QueueNewCards(bool doTween = true, bool changeView = true)
        {
            if (NumLives == 1)
            {
                yield return base.QueueNewCards(doTween, changeView);
                yield break;
            }

            List<CardSlot> slotsToQueue = BoardManager.Instance.OpponentSlotsCopy.FindAll((CardSlot x) => x.Card == null || (x.Card != null && !x.Card.Info.HasTrait(Trait.Terrain)));
            slotsToQueue.RemoveAll((CardSlot x) => Queue.Exists((PlayableCard y) => y.QueuedSlot == x));
            int numCardsToQueue = 0;
            int[] plan = (NumLives == 3) ? MODDERS_PART_1 : adjustedPlanP2;
            if (TurnManager.Instance.TurnNumber < plan.Length)
                numCardsToQueue = plan[TurnManager.Instance.TurnNumber];
            numCardsToQueue += NextTurnQueueSpecial.Count;

            if (NumLives == 2 && !HasDoneFirstWeirdCards)
            {
                int difficulty = AscensionSaveData.Data.GetNumChallengesOfTypeActive(AscensionChallenge.BaseDifficulty);
                List<string> firstTurnCards = new();
                if (difficulty == 0)
                    firstTurnCards = new() { "JuniorSage", "Revenant" };
                else if (difficulty == 1)
                    firstTurnCards = new() { "RubyGolem", "FrankNStein" };
                else if (difficulty == 2)
                    firstTurnCards = new() { "RubyGolem", "Bonehound", "Adder" };

                foreach (string cardName in firstTurnCards)
                {
                    if (slotsToQueue.Count > 0)
                    {
                        //int statPoints = Mathf.RoundToInt((float)Mathf.Min(6, TurnManager.Instance.TurnNumber + 1) * 2.5f);
                        CardSlot slot = slotsToQueue[UnityEngine.Random.Range(0, slotsToQueue.Count)];
                        CardInfo card = CardLoader.GetCardByName(cardName);
                        if (card != null)
                        {
                            yield return QueueCard(card, slot, doTween, changeView, true);
                            slotsToQueue.Remove(slot);
                        }
                    }
                }
                HasDoneFirstWeirdCards = true;
            }
            else
            {
                for (int i = 0; i < numCardsToQueue; i++)
                {
                    if (slotsToQueue.Count > 0)
                    {
                        //int statPoints = Mathf.RoundToInt((float)Mathf.Min(6, TurnManager.Instance.TurnNumber + 1) * 2.5f);
                        CardSlot slot = slotsToQueue[UnityEngine.Random.Range(0, slotsToQueue.Count)];
                        CardInfo card = GenerateCard(TurnManager.Instance.TurnNumber);
                        if (card != null)
                        {
                            yield return QueueCard(card, slot, doTween, changeView, true);
                            slotsToQueue.Remove(slot);
                        }
                    }
                }
            }
            yield return base.QueueNewCards();
        }

        public IEnumerator ShopForModSequence(string modName, bool shopping = true, bool firstPlay = false)
        {
            ViewManager.Instance.SwitchToView(View.P03Face, false, false);

            if (shopping)
            {
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03ShoppingForMod", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                ScreenArray.StartLoadingFaces();
                yield return new WaitForSeconds(0.3f);
            }
            yield return !firstPlay
                ? TextDisplayer.Instance.PlayDialogueEvent("P03ReplayMod", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[] { modName }, null)
                : (object)TextDisplayer.Instance.PlayDialogueEvent("P03SelectedMod", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, new string[] { modName }, null);
            P03AnimationController.Instance.SwitchToFace(P03AnimationController.Face.Thinking);
            yield return new WaitForSeconds(0.3f);
            ViewManager.Instance.SwitchToView(View.Default, false, false);
        }

        private bool scalesHidden = false;

        [HarmonyPatch(typeof(Scales3D), nameof(Scales3D.AddDamage))]
        [HarmonyPostfix]
        public static IEnumerator DontShowDamageWhenScalesHidden(IEnumerator sequence)
        {
            if (TurnManager.Instance != null &&
                TurnManager.Instance.Opponent is P03AscensionOpponent &&
                (TurnManager.Instance.Opponent as P03AscensionOpponent).scalesHidden)
            {
                yield break;
            }

            yield return sequence;
        }

        public IEnumerator UnityEngineSequence()
        {
            ViewManager.Instance.SwitchToView(View.P03Face, false, false);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03UnityMod", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ScreenArray.EndLoadingFaces();
            ViewManager.Instance.SwitchToView(View.Consumables, false, false);
            yield return new WaitForSeconds(0.5f);
            ResourceDrone.Instance.gameObject.transform.localPosition = ResourceDrone.Instance.gameObject.transform.localPosition + (Vector3.up * 6f);
            yield return new WaitForSeconds(0.5f);
            ViewManager.Instance.SwitchToView(View.Scales, false, false);
            yield return new WaitForSeconds(0.5f);
            foreach (Renderer rend in LifeManager.Instance.Scales3D.gameObject.GetComponentsInChildren<Renderer>())
                rend.enabled = false;

            scalesHidden = true;
            yield return new WaitForSeconds(0.5f);
            ViewManager.Instance.SwitchToView(View.P03Face, false, false);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03UnityModDone", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.Default);
        }

        public IEnumerator APISequence()
        {
            ScreenArray.EndLoadingFaces();
            ViewManager.Instance.SwitchToView(View.P03Face, false, false);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03ApiInstalled", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
            ViewManager.Instance.SwitchToView(View.Consumables, false, false);
            InteractionCursor.Instance.InteractionDisabled = true;
            yield return new WaitForSeconds(0.2f);
            yield return ResourcesManager.Instance.RefreshEnergy();
            yield return new WaitForSeconds(0.6f);
            int maxEnergy = ResourcesManager.Instance.PlayerMaxEnergy;
            Traverse resourceTrav = Traverse.Create(ResourcesManager.Instance).Property("PlayerMaxEnergy");

            while (maxEnergy > 3)
            {
                yield return ResourcesManager.Instance.SpendEnergy(ResourcesManager.Instance.PlayerEnergy);
                maxEnergy -= 1;
                resourceTrav.SetValue(maxEnergy);
                yield return ResourcesManager.Instance.RefreshEnergy();
                yield return new WaitForSeconds(0.6f);
            }

            InteractionCursor.Instance.InteractionDisabled = false;
            ViewManager.Instance.SwitchToView(View.Default);
        }

        public IEnumerator DraftSequence()
        {
            ViewManager.Instance.SwitchToView(View.BoardCentered, false, false);
            for (int i = 0; i < 2; i++)
            {
                IEnumerable<CardSlot> slots = BoardManager.Instance.OpponentSlotsCopy.Where(c => c != null && c.Card == null);
                CardSlot slot = i == 0 ? slots.FirstOrDefault() : slots.LastOrDefault();
                if (slot == null)
                    continue;

                CardInfo draftToken = CardLoader.GetCardByName(CustomCards.DRAFT_TOKEN);
                yield return BoardManager.Instance.CreateCardInSlot(draftToken, slot);
                yield return new WaitForSeconds(0.55f);
            }
            yield return new WaitForSeconds(1f);
            ViewManager.Instance.SwitchToView(View.Default, false, false);
        }

        private static bool ValidCard(PlayableCard card) => card != null && card.Info.name != CustomCards.DRAFT_TOKEN && !card.Info.IsSpell() && !card.Info.HasSpecialAbility(GoobertCenterCardBehaviour.AbilityID);

        public IEnumerator ExchangeTokensSequence()
        {

            ViewManager.Instance.SwitchToView(View.P03Face, false, false);

            yield return TextDisplayer.Instance.PlayDialogueEvent("P03Drafting", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            if (PlayerHand.Instance.CardsInHand.Count == 0)
            {
                ScreenArray.EndLoadingFaces(P03AnimationController.Face.Angry);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03NoCardsInHand", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                yield break;
            }

            if (PlayerHand.Instance.CardsInHand.Where(ValidCard).Count() == 0)
            {
                ScreenArray.EndLoadingFaces(P03AnimationController.Face.Angry);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03NoDraftableCardsInHand", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                yield break;
            }

            ScreenArray.EndLoadingFaces();

            InteractionCursor.Instance.InteractionDisabled = true;

            int seed = P03AscensionSaveData.RandomSeed + (10 * TurnManager.Instance.TurnNumber);

            List<CardSlot> possibleSlots = BoardManager.Instance.OpponentSlotsCopy.Where(s => !Queue.Exists(p => p.QueuedSlot == s)).ToList();
            CardSlot slot = possibleSlots[SeededRandom.Range(0, possibleSlots.Count, seed++)];

            ViewManager.Instance.SwitchToView(View.Hand, false, false);
            float delay = 2f / PlayerHand.Instance.CardsInHand.Count;
            foreach (PlayableCard card in PlayerHand.Instance.CardsInHand)
            {
                PlayerHand.Instance.OnCardInspected(card);
                yield return new WaitForSeconds(delay);
            }
            List<PlayableCard> possibles = PlayerHand.Instance.CardsInHand.Where(ValidCard).ToList();
            PlayableCard cardToSteal = possibles[SeededRandom.Range(0, possibles.Count, seed++)];
            PlayerHand.Instance.OnCardInspected(cardToSteal);
            yield return new WaitForSeconds(0.75f);

            PlayerHand.Instance.RemoveCardFromHand(cardToSteal);
            cardToSteal.SetEnabled(false);
            cardToSteal.Anim.SetTrigger("fly_off");
            Tween.Position(cardToSteal.transform, cardToSteal.transform.position + new Vector3(0f, 3f, 5f), 0.4f, 0f, Tween.EaseInOut, Tween.LoopType.None, null, delegate ()
            {
                Destroy(cardToSteal.gameObject);
            }, true);
            yield return new WaitForSeconds(0.75f);

            CardInfo draftToken = CardLoader.GetCardByName(CustomCards.DRAFT_TOKEN);
            draftToken.mods.Add(new(Ability.DrawRandomCardOnDeath));
            PlayableCard tokenCard = CardSpawner.SpawnPlayableCard(draftToken);
            yield return PlayerHand.Instance.AddCardToHand(tokenCard, Vector3.zero, 0f);
            yield return new WaitForSeconds(0.6f);

            // ViewManager.Instance.SwitchToView(View.BoardCentered, false, false);
            // yield return QueueCard(cardToSteal.Info, slot, true, true, true);
            // //yield return BoardManager.Instance.CreateCardInSlot(cardToSteal.Info, slot);
            // yield return new WaitForSeconds(0.65f);
            NextTurnQueueSpecial.Add(cardToSteal.Info);

            ViewManager.Instance.SwitchToView(View.Default);
            InteractionCursor.Instance.InteractionDisabled = false;
            yield break;
        }

        public IEnumerator HammerSequence()
        {
            List<CardSlot> slots = BoardManager.Instance.PlayerSlotsCopy.Where(s => s != null && s.Card != null).ToList();

            if (slots.Count == 0)
            {
                ViewManager.Instance.SwitchToView(View.P03Face, false, false);
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03AngryNoHammer", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                ScreenArray.EndLoadingFaces(P03AnimationController.Face.Angry);
                ViewManager.Instance.SwitchToView(View.Default);
                yield return new WaitForSeconds(0.1f);
                yield break;
            }

            ScreenArray.EndLoadingFaces();

            ViewManager.Instance.SwitchToView(View.P03Face, false, false);
            yield return TextDisplayer.Instance.PlayDialogueEvent("P03HammerModHappy", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);

            int seed = P03AscensionSaveData.RandomSeed + (10 * TurnManager.Instance.TurnNumber) + 234;
            CardSlot target = slots[SeededRandom.Range(0, slots.Count, seed)];

            // Find the hammer item
            ItemSlot hammerSlot = ItemsManager.Instance.Slots.First(s => s.Item is HammerItem);
            HammerItem hammer = hammerSlot.Item as HammerItem;

            hammer.PlayExitAnimation();
            InteractionCursor.Instance.InteractionDisabled = true;
            yield return new WaitForSeconds(0.1f);
            //UIManager.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0.6f, 0.2f);
            ViewManager.Instance.SwitchToView(hammer.SelectionView, false, false);
            InteractionCursor.Instance.InteractionDisabled = false;

            ViewManager.Instance.Controller.LockState = ViewLockState.Locked;

            foreach (CardSlot slot in BoardManager.Instance.PlayerSlotsCopy.Where(s => s != null && s.Card != null))
            {
                Transform firstPersonItem = FirstPersonController.Instance.AnimController.SpawnFirstPersonAnimation(hammer.FirstPersonPrefabId, null).transform;
                firstPersonItem.localPosition = hammer.FirstPersonItemPos + (Vector3.right * 3f) + (Vector3.forward * 1f);
                firstPersonItem.localEulerAngles = hammer.FirstPersonItemEulers;
                yield return new WaitForSeconds(0.15f);
                hammer.MoveItemToPosition(firstPersonItem, slot.transform.position);
                yield return new WaitForSeconds(0.25f);
                yield return hammer.OnValidTargetSelected(slot, firstPersonItem.gameObject);
                yield return new WaitForSeconds(.5f);
                Destroy(firstPersonItem.gameObject);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
            }

            ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;

            //UIManager.Instance.Effects.GetEffect<EyelidMaskEffect>().SetIntensity(0f, 0.2f);
            InteractionCursor.Instance.InteractionDisabled = false;

            hammerSlot.OnCursorEnter();
            hammerSlot.OnCursorExit();

            yield break;
        }

        [HarmonyPatch(typeof(Opponent), nameof(ReplaceBlueprint))]
        [HarmonyPostfix]
        public static IEnumerator Postfix(IEnumerator sequence, string blueprintId, bool removeLockedCards = false)
        {
            if (!SaveFile.IsAscension || TurnManager.Instance.opponent is not P03AscensionOpponent || !blueprintId.Equals("P03FinalBoss"))
            {
                yield return sequence;
                yield break;
            }

            TurnManager.Instance.Opponent.Blueprint = EncounterHelper.P03FinalBoss;

            List<List<CardInfo>> plan = EncounterBuilder.BuildOpponentTurnPlan(TurnManager.Instance.Opponent.Blueprint, EventManagement.EncounterDifficulty, removeLockedCards);
            TurnManager.Instance.Opponent.ReplaceAndAppendTurnPlan(plan);
            yield return TurnManager.Instance.Opponent.QueueNewCards(true, true);
            yield break;
        }
    }
}