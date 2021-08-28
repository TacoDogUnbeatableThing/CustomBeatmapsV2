using Rhythm;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
    // Design Credits to Ratismal's CustomBeats Plugin
    public class CustomBeatmapInfo : BeatmapInfo
    {
        public readonly string Artist;
        public readonly string BeatmapCreator;
        public readonly string RealAudioKey;

        public CustomBeatmapInfo(TextAsset textAsset, string songName, string difficulty, string artist,
            string beatmapCreator, string realAudioKey) : base(textAsset, songName, difficulty)
        {
            RealAudioKey = realAudioKey;
            Artist = artist;
            BeatmapCreator = beatmapCreator;
        }
    }
}
