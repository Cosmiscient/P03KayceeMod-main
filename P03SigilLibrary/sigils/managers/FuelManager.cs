using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Helpers;
using UnityEngine;

namespace Infiniscryption.P03SigilLibrary.Sigils
{
    [HarmonyPatch]
    public class FuelManager : ManagedBehaviour
    {
        public static readonly Color GreenFuel = new Color(0.18f, .78f, .22f) * 0.75f;
        public static readonly Color RedFuel = new Color(0.8f, .2f, .2f) * 0.25f;

        private Texture2D _scratchedTexture;
        private Texture2D ScratchedTexture
        {
            get
            {
                if (_scratchedTexture != null)
                    return _scratchedTexture;
                _scratchedTexture = TextureHelper.GetImageAsTexture("scratched_large.png", typeof(FuelManager).Assembly);
                return _scratchedTexture;
            }
        }

        private static List<Texture2D> FuelTextures = new()
        {
            TextureHelper.GetImageAsTexture("fuel_gauge_0.png", typeof(FuelManager).Assembly),
            TextureHelper.GetImageAsTexture("fuel_gauge_1.png", typeof(FuelManager).Assembly),
            TextureHelper.GetImageAsTexture("fuel_gauge_2.png", typeof(FuelManager).Assembly),
            TextureHelper.GetImageAsTexture("fuel_gauge_3.png", typeof(FuelManager).Assembly),
            TextureHelper.GetImageAsTexture("fuel_gauge_4.png", typeof(FuelManager).Assembly),
        };

        private static FuelManager m_instance;
        public static FuelManager Instance
        {
            get
            {
                if (m_instance != null)
                    return m_instance;

                if (BoardManager.Instance == null)
                    return null;

                m_instance = BoardManager.Instance.gameObject.AddComponent<FuelManager>();
                return m_instance;
            }
        }

        public class Status
        {
            public int StartingFuel { get; internal set; }
            public int CurrentFuel { get; internal set; }
        }

        [HarmonyPatch(typeof(Card), nameof(Card.SetInfo))]
        [HarmonyPrefix]
        private static void EstablishFuelReserves(Card __instance, CardInfo info)
        {
            Status status = FuelExtensions.FuelStatus.GetOrCreateValue(__instance);
            if (status.StartingFuel == 0 && info.GetStartingFuel() > 0)
            {
                status.StartingFuel = info.GetStartingFuel();
                status.CurrentFuel = info.GetStartingFuel();
            }
        }

        // internal void GenerateGaugeSegment(Transform parent, Color c, float offsetMultiplier, float scale = 0.15f, float width = 0.055f)
        // {
        //     string name = $"FuelSegment_{offsetMultiplier * 10f}";
        //     if (parent.Find(name) == null)
        //     {
        //         var redCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //         redCylinder.name = name;
        //         redCylinder.layer = LayerMask.NameToLayer("SpecificLighting");
        //         redCylinder.transform.SetParent(parent);
        //         redCylinder.transform.localPosition = new Vector3(0.02f + 0.35f * offsetMultiplier, -0.6275f, 0.0028f);
        //         redCylinder.transform.localEulerAngles = new(0f, 0f, 90f);
        //         redCylinder.transform.localScale = new(width, scale, width);

        //         var renderer = redCylinder.GetComponent<Renderer>();
        //         renderer.material.color = c;
        //         renderer.material.EnableKeyword("_EMISSION");
        //         renderer.material.SetColor("_Color", c);
        //     }
        // }

        internal void Render3DFuelGauge(Card card)
        {
            DiskRenderStatsLayer stats = card.StatsLayer as DiskRenderStatsLayer;
            if (stats == null)
            {
                card.RenderCard();
                return;
            }

            var railsParent = stats.gameObject.transform.Find("Rails");

            Transform gauge = railsParent.Find("FuelGauge");
            if (gauge == null)
            {
                var fuelGauge = GameObject.Instantiate(ResourceBank.Get<GameObject>("p03kcm/prefabs/fuelgauge"), railsParent);
                fuelGauge.name = "FuelGauge";
                fuelGauge.transform.localEulerAngles = new(0f, 359.68f, 270f);
                fuelGauge.transform.localPosition = new(1.63f, 0f, 0f);
                fuelGauge.transform.localScale = new(1f, 1f, 0.43f);
            }

            // Make each of the four cylinders
            // GenerateGaugeSegment(railsParent, new(0.8f, .2f, .2f), 0f);
            // GenerateGaugeSegment(railsParent, new(0.86f, .48f, .17f), 1f);
            // GenerateGaugeSegment(railsParent, new(0.91f, .71f, .08f), 2f);
            // GenerateGaugeSegment(railsParent, new(0.18f, .78f, .22f), 3f);
            // GenerateGaugeSegment(railsParent, new(0.5f, .5f, .3f), 1.5f, scale: 0.55f, width: 0.04f);

            // PlayableCard playableCard = card as PlayableCard;
            // int fuelToDisplay = playableCard == null ? card.Info.GetStartingFuel() : (playableCard.GetCurrentFuel() ?? -1);

            // for (int i = 0; i < 4; i++)
            // {
            //     string name = $"FuelSegment_{(float)(i * 10)}";
            //     GameObject obj = railsParent.Find(name).gameObject;
            //     Material mat = obj.GetComponent<Renderer>().material;
            //     if (fuelToDisplay > i)
            //         mat.SetColor("_EmissionColor", GreenFuel);
            //     else
            //         mat.SetColor("_EmissionColor", RedFuel);
            // }
        }

        internal void RenderCurrentFuel(Card card)
        {
            // Currently this is just a stub until we get the full 3D renderer
            if (card.StatsLayer is DiskRenderStatsLayer)
                Render3DFuelGauge(card);
            else
                card.RenderCard();
        }

        private static Renderer GetDefaultDecalRenderer(CardDisplayer3D displayer)
        {
            if (displayer == null)
                return null;

            if (displayer.decalRenderers == null)
                return null;

            if (displayer.decalRenderers.Count == 0)
                return null;

            return displayer.decalRenderers[0];
        }

        private static Transform GetDecalParent(CardDisplayer3D displayer)
        {
            return GetDefaultDecalRenderer(displayer)?.gameObject.transform.parent;
        }

        [HarmonyPatch(typeof(Card), nameof(Card.RenderCard))]
        [HarmonyPostfix]
        private static void Render3DFuel(Card __instance)
        {
            PlayableCard playableCard = __instance as PlayableCard;
            int fuelToDisplay = playableCard == null ? __instance.Info.GetStartingFuel() : (playableCard.GetCurrentFuel() ?? -1);
            bool displayFuel = playableCard == null ? fuelToDisplay > 0 : playableCard.HasFuel();
            if (displayFuel)
                FuelManager.Instance.Render3DFuelGauge(__instance);
        }

        [HarmonyPatch(typeof(CardDisplayer3D), nameof(CardDisplayer3D.DisplayInfo))]
        [HarmonyPrefix]
        private static void AddFuelDecal(ref CardDisplayer3D __instance)
        {
            if (__instance is DiskScreenCardDisplayer)
                return;

            var decalParent = GetDecalParent(__instance);
            if (decalParent == null)
                return;

            if (decalParent.Find("Decal_Fuel") == null)
            {
                GameObject portrait = __instance.portraitRenderer.gameObject;
                GameObject decalFull = GetDefaultDecalRenderer(__instance)?.gameObject;
                if (decalFull == null)
                    return;

                GameObject decalGood = UnityEngine.Object.Instantiate(decalFull, decalParent);
                decalGood.name = "Decal_Fuel";
                decalGood.transform.localPosition = portrait.transform.localPosition + new Vector3(-.1f, 0f, -0.0001f);
                decalGood.transform.localScale = new(1.2f, 1f, 0f);
            }
        }

        [HarmonyPatch(typeof(CardDisplayer3D), nameof(CardDisplayer3D.DisplayInfo))]
        [HarmonyPostfix]
        private static void DecalsForFuel(CardDisplayer3D __instance, CardRenderInfo renderInfo, PlayableCard playableCard)
        {
            if (__instance is DiskScreenCardDisplayer)
                return;
            var decalParent = GetDecalParent(__instance);
            var fuelDecalObj = decalParent?.Find("Decal_Fuel");
            if (fuelDecalObj == null)
                return;

            int fuelToDisplay = playableCard == null ? (renderInfo?.baseInfo?.GetStartingFuel() ?? -1) : (playableCard.GetCurrentFuel() ?? -1);
            bool displayFuel = playableCard == null ? fuelToDisplay > 0 : playableCard.HasFuel();
            var fuelDecal = fuelDecalObj.GetComponent<Renderer>();
            if (displayFuel)
            {
                fuelDecal.enabled = true;
                fuelDecal.material.mainTexture = FuelTextures[fuelToDisplay];
            }
            else
            {
                fuelDecal.enabled = false;
            }
        }
    }
}