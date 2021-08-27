using System;

namespace CustomBeatmaps.UI.Structure
{
    public interface IOsuUIMain
    {
        void Init(OsuUIMainProps props);
        void Open();
        void Close();
    }

    public struct OsuUIMainProps
    {
        public Action<bool> OnSetPaused;
        public Action<float> OnSetTime;
        public Func<bool> GetPaused;
        public Func<float> GetCurrentTime;
        public Func<float> GetSongTotalLength;

        public OsuUIMainProps(Action<bool> onSetPaused, Action<float> onSetTime, Func<bool> getPaused, Func<float> getCurrentTime, Func<float> getSongTotalLength)
        {
            OnSetPaused = onSetPaused;
            OnSetTime = onSetTime;
            GetPaused = getPaused;
            GetCurrentTime = getCurrentTime;
            GetSongTotalLength = getSongTotalLength;
        }
    }
}
