using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.P03KayceeRun.Cards;
using Infiniscryption.P03KayceeRun.Faces;
using Infiniscryption.P03KayceeRun.Helpers;
using InscryptionAPI.Card;
using InscryptionAPI.Dialogue;
using InscryptionAPI.Guid;
using Sirenix.Serialization.Utilities;
using Unity.Collections.LowLevel.Unsafe;

namespace Infiniscryption.P03KayceeRun.Patchers
{
    [HarmonyPatch]
    public static class DialogueManagement
    {
        internal static List<string> AllStringsToTranslate = new();
        internal static bool TrackForTranslation = false;
        private static readonly Dictionary<string, DialogueEvent.Speaker> cardSpeakers = new();

        internal static DialogueEvent.Speaker NewCardSpeaker(string speaker)
        {
            var result = GuidManager.GetEnumValue<DialogueEvent.Speaker>(P03Plugin.PluginGuid, speaker);
            cardSpeakers.Add(speaker.ToLowerInvariant(), result);
            return result;
        }


        private static Emotion FaceEmotion(this P03AnimationController.Face face)
        {
            return face == P03AnimationController.Face.Angry
                ? Emotion.Anger
                : face == P03AnimationController.Face.Thinking
                ? Emotion.Curious
                : face == P03AnimationController.Face.Bored
                ? Emotion.Anger
                : face == P03AnimationController.Face.Happy
                ? Emotion.Neutral
                : face == P03TrollFace.ID
                ? Emotion.Laughter
                : face == P03AnimationController.Face.MycologistAngry
                ? Emotion.Anger
                : face == P03AnimationController.Face.MycologistLaughing
                ? Emotion.Laughter
                : face == P03AnimationController.Face.TelegrapherBoss
                ? Emotion.Curious
                : face == P03AnimationController.Face.PhotographerBoss
                ? Emotion.Anger
                : Emotion.Neutral;
        }

        private static string ParseCommandShortcuts(string dialogue, DialogueEvent.Speaker speaker, Emotion emotion)
        {
            string retval = dialogue.Replace("[james:]", "[anim:voice.speechblip_jamescobb_internal][c:bR]")
                           .Replace("[default:]", "[c:][w:0.3][anim:voice.]");

            if (speaker == TalkingCardJames.ID)
                retval = "[c:bR]" + retval + "[c:]";

            if (cardSpeakers.ContainsValue(speaker))
                retval += "[w:0.3]";

            return retval;
        }

        private static Emotion ParseEmotion(this string face, Emotion defaultEmotion = Emotion.Neutral)
        {
            if (string.IsNullOrEmpty(face))
                return defaultEmotion; ;
            if (face.ToLowerInvariant().Contains("goocurious"))
                return Emotion.Neutral;
            if (face.ToLowerInvariant().Contains("goo"))
                return Emotion.Anger;
            if (face.ToLowerInvariant().Contains("curious"))
                return Emotion.Curious;
            if (face.ToLowerInvariant().Contains("frustrated"))
                return Emotion.Anger;
            if (face.ToLowerInvariant().Contains("angry"))
                return Emotion.Anger;
            if (face.ToLowerInvariant().Contains("anger"))
                return Emotion.Anger;
            if (face.ToLowerInvariant().Contains("happy"))
                return Emotion.Neutral;
            if (face.ToLowerInvariant().Contains("laughter"))
                return Emotion.Laughter;
            if (face.ToLowerInvariant().Contains("grimora"))
                return Emotion.Neutral;
            if (face.ToLowerInvariant().Contains("magnificus"))
                return Emotion.Neutral;
            if (face.ToLowerInvariant().Contains("leshy"))
                return Emotion.Neutral;
            if (face.ToLowerInvariant().Contains("dredger"))
                return Emotion.Anger;
            return face.ParseFace().FaceEmotion();
        }

        private static P03AnimationController.Face ParseEnumFace(this string face)
        {
            string faceLower = face.ToLowerInvariant();
            foreach (var enumVal in Enum.GetValues(typeof(P03AnimationController.Face)))
            {
                if (faceLower.Contains(enumVal.ToString().ToLowerInvariant()))
                    return (P03AnimationController.Face)enumVal;
            }
            return P03AnimationController.Face.NoChange;
        }

        private static P03AnimationController.Face ParseFace(this string face)
        {
            return String.IsNullOrEmpty(face)
                ? P03AnimationController.Face.NoChange
                : face.ToLowerInvariant().StartsWith("npc")
                ? P03AnimationController.Face.NoChange
                : face.ToLowerInvariant().Contains("troll")
                ? P03TrollFace.ID
                : face.ParseEnumFace();
        }

        private static DialogueEvent.Speaker ParseSpeaker(this string face, DialogueEvent.Speaker defaultSpeaker = DialogueEvent.Speaker.P03)
        {
            if (string.IsNullOrEmpty(face))
                return defaultSpeaker;

            foreach (var kvp in cardSpeakers)
                if (face.ToLowerInvariant().Contains(kvp.Key))
                    return kvp.Value;

            DialogueEvent.Speaker speaker = defaultSpeaker;
            if (face.ToLowerInvariant().Contains("card"))
                speaker = DialogueEvent.Speaker.AnglerTalkingCard;
            else if (face.ToLowerInvariant().Contains("bounty"))
                speaker = DialogueEvent.Speaker.P03BountyHunter;
            else if (face.ToLowerInvariant().Contains("leshy"))
                speaker = DialogueEvent.Speaker.Leshy;
            else if (face.ToLowerInvariant().Contains("grimora"))
                speaker = DialogueEvent.Speaker.Grimora;
            else if (face.ToLowerInvariant().Contains("magnificus"))
                speaker = DialogueEvent.Speaker.Magnificus;
            else if (face.ToLowerInvariant().Contains("telegrapher"))
                speaker = DialogueEvent.Speaker.P03Telegrapher;
            else if (face.ToLowerInvariant().Contains("archivist"))
                speaker = DialogueEvent.Speaker.P03Archivist;
            else if (face.ToLowerInvariant().Contains("photographer"))
                speaker = DialogueEvent.Speaker.P03Photographer;
            else if (face.ToLowerInvariant().Contains("canvas"))
                speaker = DialogueEvent.Speaker.P03Canvas;
            else if (face.ToLowerInvariant().Contains("goo"))
                speaker = DialogueEvent.Speaker.Goo;
            else if (face.ToLowerInvariant().Contains("side"))
                speaker = DialogueEvent.Speaker.P03MycologistSide;
            else if (face.ToLowerInvariant().Contains("mycolo"))
                speaker = DialogueEvent.Speaker.P03MycologistMain;

            return speaker;
        }

        public static List<string> SplitColumn(string col, char sep = ',', char quote = '"')
        {
            bool isQuoted = false;
            List<string> retval = new();
            string cur = string.Empty;
            foreach (char c in col)
            {
                if (c == sep && !isQuoted)
                {
                    retval.Add(cur);
                    cur = string.Empty;
                    continue;
                }

                if (c == quote && cur == string.Empty)
                {
                    isQuoted = true;
                    continue;
                }

                if (c == quote && cur != string.Empty && isQuoted)
                {
                    isQuoted = false;
                    continue;
                }

                cur += c;
            }
            retval.Add(cur);
            return retval;
        }

        public static void AddSequenceDialogue()
        {
            string database = DataHelper.GetResourceString("dialogue_database", "csv");
            string[] lines = database.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string dialogueId = string.Empty;
            string lastSeenFaceInstruction = string.Empty;
            DialogueEvent.Speaker lastSpeaker = DialogueEvent.Speaker.P03;
            Emotion lastEmotion = Emotion.Neutral;

            List<DialogueEvent.Line> currentLines = new();
            List<List<DialogueEvent.Line>> replacementLines = new();
            List<DialogueEvent.Speaker> currentSpeakers = new() { DialogueEvent.Speaker.Single };

            foreach (string line in lines.Skip(1))
            {
                List<string> cols = SplitColumn(line);

                if (!string.IsNullOrEmpty(cols[0]))
                {
                    if (cols[0].StartsWith("|"))
                    {
                        replacementLines.Add(new());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dialogueId))
                        {
                            DialogueEvent newEvent = new()
                            {
                                id = dialogueId,
                                speakers = currentSpeakers,
                                mainLines = new(currentLines)
                            };

                            if (replacementLines.Count > 0)
                                newEvent.repeatLines = replacementLines.Select(
                                    llines => new DialogueEvent.LineSet(llines)
                                ).ToList();

                            currentLines = new();
                            currentSpeakers = new() { DialogueEvent.Speaker.Single };
                            replacementLines = new();

                            DialogueManager.Add(P03Plugin.PluginGuid, newEvent);
                        }

                        dialogueId = cols[0];
                        lastSeenFaceInstruction = string.Empty;
                        lastSpeaker = DialogueEvent.Speaker.P03;
                        lastEmotion = Emotion.Neutral;
                    }
                }

                if (!string.IsNullOrEmpty(cols[1]))
                    lastSeenFaceInstruction = cols[1];

                string faceInstruction = lastSeenFaceInstruction;

                P03AnimationController.Face face = faceInstruction.ParseFace();
                DialogueEvent.Speaker speaker = faceInstruction.ParseSpeaker(lastSpeaker);
                lastSpeaker = speaker;
                Emotion emotion = faceInstruction.ParseEmotion(lastEmotion);
                lastEmotion = emotion;
                string dialogue = ParseCommandShortcuts(cols[3], speaker, emotion);
                bool wavy = !string.IsNullOrEmpty(cols[2]) && cols[2].ToLowerInvariant().Contains("y");

                P03Plugin.Log.LogDebug($"{dialogueId} has speaker {speaker}");

                if (!currentSpeakers.Contains(speaker))
                    currentSpeakers.Add(speaker);

                if (cardSpeakers.ContainsValue(speaker))
                {
                    currentSpeakers.Remove(DialogueEvent.Speaker.Single);
                }

                AllStringsToTranslate.Add(dialogue);

                DialogueEvent.Line newLine = new()
                {
                    text = dialogue,
                    specialInstruction = "",
                    p03Face = face,
                    speakerIndex = currentSpeakers.IndexOf(speaker),
                    emotion = emotion,
                    letterAnimation = wavy ? TextDisplayer.LetterAnimation.Jitter : TextDisplayer.LetterAnimation.None
                };

                if (replacementLines.Count > 0)
                    replacementLines.Last().Add(newLine);
                else
                    currentLines.Add(newLine);
            }
        }

        private static int offset = 0;

        [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowThenClear))]
        [HarmonyPrefix]
        private static void Profanity(ref string message)
        {
            // the fuck are you doing here
            // this is a fucking easter egg
            // don't be a fucking narc
            if (P03AscensionSaveData.IsP03Run && P03Plugin.Instance.DebugCode.ToLowerInvariant().Contains("fuck"))
            {
                List<string> words = message.Split(' ').ToList();
                int findex = SeededRandom.Range(0, words.Count, P03AscensionSaveData.RandomSeed + offset++);
                words.Insert(findex, "fucking");
                message = String.Join(" ", words);
            }
        }

        [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowUntilInput))]
        [HarmonyPrefix]
        private static void Profanity2(ref string message) => Profanity(ref message);

        [HarmonyPatch(typeof(CardManager), nameof(CardManager.New))]
        [HarmonyPrefix]
        private static void TrackCardNames(string displayName)
        {
            if (TrackForTranslation)
                AllStringsToTranslate.Add(displayName);
        }

        [HarmonyPatch(typeof(AbilityManager), nameof(AbilityManager.Add))]
        [HarmonyPrefix]
        private static void TrackAbilityNames(AbilityInfo info)
        {
            if (TrackForTranslation)
            {
                AllStringsToTranslate.Add(info.rulebookName);
                AllStringsToTranslate.Add(info.rulebookDescription);
            }
        }

        internal static void ResolveCurrentTranslation()
        {
            // Load existing dialogue
            string basePath = Path.GetDirectoryName(P03Plugin.Instance.Info.Location);
            string filePath = Path.Combine(basePath, "P03Translations.gttsv");
            Dictionary<string, List<string>> existingData = new();
            if (File.Exists(filePath))
            {
                string fileContents = File.ReadAllText(filePath);
                List<List<string>> contents = new(fileContents.Split('\n').Select(line => line.Split('\t').ToList()));
                foreach (var lineContents in contents)
                    existingData[lineContents[0]] = lineContents;
            }

            // Merge with all tracked dialogue
            List<List<string>> newData = new();
            newData.Add(new() { "Original", "English", "German", "Japanese", "Korean", "French", "Italian", "Spanish", "BrazilianPortuguese", "Turkish", "Russian", "ChineseSimplified", "ChineseTraditional" });
            foreach (string line in AllStringsToTranslate.Distinct())
            {
                if (existingData.ContainsKey(line))
                    newData.Add(existingData[line]);
                else
                    newData.Add(new() { line, null, null, null, null, null, null, null, null, null, null, null, null });
            }

            // Create output
            string fileOutput = string.Join("\n", newData.Select(l => string.Join("\t", l)));
            string outFilePath = Path.Combine(basePath, "P03Translated_Unfinished.gttsv");
            File.WriteAllText(outFilePath, fileOutput);
        }
    }
}