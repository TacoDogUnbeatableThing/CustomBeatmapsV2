using Rhythm;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
    // Design Credits to Ratismal's CustomBeats Plugin
    public class CustomBeatmapInfo : BeatmapInfo
    {
        public readonly string Artist;
        public readonly string BeatmapCreator;
        public readonly string SongName;
        public readonly string RealAudioKey;

        public CustomBeatmapInfo(TextAsset textAsset, string difficulty, string artist,
            string beatmapCreator, string songName, string realAudioKey) : base(textAsset, difficulty)
        {
            RealAudioKey = realAudioKey;
            Artist = artist;
            SongName = songName;
            BeatmapCreator = beatmapCreator;
        }
    }
}
