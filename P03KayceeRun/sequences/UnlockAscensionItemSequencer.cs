using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Cards;
using Infiniscryption.P03KayceeRun.Items;
using Infiniscryption.P03KayceeRun.Patchers;
using Infiniscryption.P03KayceeRun.Quests;
using UnityEngine;

namespace Infiniscryption.P03KayceeRun.Sequences
{
    [HarmonyPatch]
    public class UnlockAscensionItemSequencer : SelectItemsSequencer
    {
        public static UnlockAscensionItemSequencer Instance { get; private set; }

        public void StartScreenshotSequence()
        {
            StartCoroutine(ItemScreenshotSequence());
        }

        private static void SetupForItemCamera()
        {
            // The camera's depth setting should make it to where we don't have to turn off a bunch
            // of game objects. We mostly just need to set the color, position, and intensity of lights
            // and the position/far clip plane of the camera

            Transform cameraParent = ViewManager.Instance.cameraParent;
            cameraParent.localPosition = new(0, 6.25f, -6.86f); // 0 8.25 0.54
            cameraParent.localEulerAngles = Vector3.zero;

            TableVisualEffectsManager.Instance.gameObject.SetActive(false);

            Camera camera = ViewManager.Instance.pixelCamera;
            camera.farClipPlane = 5;
        }

        private IEnumerator ItemScreenshotSequence()
        {
            SetupForItemCamera();
            List<string> items = ItemsUtil.AllConsumables.Where(cid => cid.rulebookCategory == AbilityMetaCategory.Part3Rulebook).Select(cid => cid.name).ToList();

            SetSlotCollidersEnabled(false);

            var slot = this.slots[1];

            foreach (var item in items)
            {
                slot.CreateItem(ItemsUtil.GetConsumableByName(item));
                string outfile = $"cardexports/item_{item}.png";
                yield return new WaitUntil(() => InputButtons.GetButton(Button.EndTurn));
                CardExporter.CaptureTransparentScreenshot(ViewManager.Instance.pixelCamera, Screen.width, Screen.height, outfile);
                yield return new WaitForSeconds(0.25f);
            }

        }

        private bool _sequenceRunning = false;
        internal bool GoobertIsForSale => _sequenceRunning && slots[2].Item.Data.name.Equals(GoobertHuh.ItemData.name);

        public override void Start()
        {
            if (slots == null)
            {
                GameObject slots = Instantiate(SpecialNodeHandler.Instance.unlockItemSequencer.gameObject.transform.Find("ItemSlots").gameObject, gameObject.transform);
                GameObject centerSlot = slots.transform.Find("ItemSlot_Center").gameObject;
                GameObject leftSlot = Instantiate(centerSlot, centerSlot.transform.parent);
                leftSlot.name = "ItemSlot_Left";
                leftSlot.transform.localPosition = new(-2.5f, 5f, -2.28f);
                GameObject rightSlot = Instantiate(centerSlot, centerSlot.transform.parent);
                rightSlot.name = "ItemSlot_Right";
                rightSlot.transform.localPosition = new(2.5f, 5f, -2.28f);

                this.slots = new List<SelectableItemSlot>() {
                    leftSlot.GetComponent<SelectableItemSlot>(),
                    centerSlot.GetComponent<SelectableItemSlot>(),
                    rightSlot.GetComponent<SelectableItemSlot>()
                };

                foreach (SelectableItemSlot slot in this.slots)
                {
                    if (slot.gameObject.GetComponent<AlternateInputInteractable>() == null)
                    {
                        var aii = slot.gameObject.AddComponent<GenericAltInputInteractable>();
                        aii.cursorType = CursorType.Inspect;
                    }
                }

                slotsGamepadControl = slots.GetComponentInChildren<GamepadGridControl>();
            }
            base.Start();
        }

        [HarmonyPatch(typeof(HoloMapShopNode), nameof(HoloMapShopNode.CanAfford))]
        [HarmonyPrefix]
        public static bool CanotAffordItemsIfFull(ref bool __result, ref HoloMapShopNode __instance)
        {
            if (SaveFile.IsAscension && __instance.nodeToBuy.nodeType == UnlockAscensionItemNodeData.UnlockItemsAscension)
            {
                if (Part3SaveData.Data.items.Count >= P03AscensionSaveData.MaxNumberOfItems)
                {
                    ItemsManager.Instance.ShakeConsumableSlots(0.1f);
                    __result = false;
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(SpecialNodeHandler), "StartSpecialNodeSequence")]
        [HarmonyPrefix]
        public static bool HandleAscensionItems(ref SpecialNodeHandler __instance, SpecialNodeData nodeData)
        {
            if (nodeData is UnlockAscensionItemNodeData)
            {
                if (Instance == null)
                    Instance = __instance.gameObject.AddComponent<UnlockAscensionItemSequencer>();

                SpecialNodeHandler.Instance.StartCoroutine(Instance.SelectItem(nodeData as UnlockAscensionItemNodeData));
                return false;
            }
            return true;
        }

        public static List<string> ValidItems => ItemsUtil.AllConsumables
                                                 .Where(cid => cid.rulebookCategory == AbilityMetaCategory.Part3Rulebook && cid.name != GoobertHuh.ItemData.name)
                                                 .Select(cid => cid.name).ToList();

        private List<ConsumableItemData> GetItems()
        {
            int randomSeed = P03AscensionSaveData.RandomSeed;
            List<string> items = new(ValidItems);
            while (items.Count > 3)
                items.RemoveAt(SeededRandom.Range(0, items.Count, randomSeed++));

            // If the goobert quest has progressed to the point where it is supposed to be available to buy, force it to appear
            if (DefaultQuestDefinitions.FindGoobert.CurrentState.StateName == "GoobertAvailable"
                && DefaultQuestDefinitions.FindGoobert.CurrentState.Status == QuestState.QuestStateStatus.Active &&
                EventManagement.CompletedZones.Count == 0) // Can only do this at the first zone
            {
                items[2] = GoobertHuh.ItemData.name;
            }

            return items.Select(ItemsUtil.GetConsumableByName).ToList();
        }

        public IEnumerator SelectItem(UnlockAscensionItemNodeData nodeData)
        {
            ViewManager.Instance.SwitchToView(View.Default, false, true);

            yield return new WaitForSeconds(0.1f);
            SelectableItemSlot selectedSlot = null;
            List<ConsumableItemData> data = GetItems();

            foreach (SelectableItemSlot slot in slots)
            {
                ConsumableItemData item = data[slots.IndexOf(slot)];
                slot.gameObject.SetActive(true);
                slot.CreateItem(item, false);
                slot.CursorSelectStarted += i => selectedSlot = i as SelectableItemSlot;
                slot.CursorEntered += i => Singleton<OpponentAnimationController>.Instance.SetLookTarget(i.transform, Vector3.up * 2f);
                slot.GetComponent<AlternateInputInteractable>().AlternateSelectStarted = i => RuleBookController.Instance.OpenToItemPage(slot.Item.Data.name, true);

                yield return new WaitForSeconds(0.1f);
            }

            _sequenceRunning = true;
            SetSlotCollidersEnabled(true);

            yield return new WaitUntil(() => selectedSlot != null);

            if (selectedSlot.Item.Data.name.Equals(GoobertHuh.ItemData.name))
            {
                // If you buy Goobert, you "succeed" at that state of the quest
                yield return TextDisplayer.Instance.PlayDialogueEvent("P03Wut", TextDisplayer.MessageAdvanceMode.Input, TextDisplayer.EventIntersectMode.Wait, null, null);
                DefaultQuestDefinitions.FindGoobert.CurrentState.Status = QuestState.QuestStateStatus.Success;
            }
            else if (slots[2].Item.Data.name.Equals(GoobertHuh.ItemData.name))
            {
                // If you fail to buy Goobert, you "fail" at that state of the quest
                yield return DefaultQuestDefinitions.FindGoobert.CurrentState.Status = QuestState.QuestStateStatus.Failure;
            }

            RuleBookController.Instance.SetShown(false);
            _sequenceRunning = false;
            Part3SaveData.Data.items.Add(selectedSlot.Item.Data.name);

            DisableSlotsAndExitItems(selectedSlot);
            yield return new WaitForSeconds(0.2f);
            selectedSlot.Item.PlayExitAnimation();
            yield return new WaitForSeconds(0.1f);
            ItemsManager.Instance.UpdateItems();

            foreach (SelectableItemSlot slot in slots)
            {
                slot.ClearDelegates();
                slot.GetComponent<AlternateInputInteractable>().ClearDelegates();
            }

            SetSlotsActive(false);

            OpponentAnimationController.Instance.ClearLookTarget();

            foreach (SelectableItemSlot slot in slots)
                Destroy(slot.Item.gameObject);

            SaveManager.SaveToFile(false);

            GameFlowManager.Instance?.TransitionToGameState(GameState.Map, null);
        }
    }
}