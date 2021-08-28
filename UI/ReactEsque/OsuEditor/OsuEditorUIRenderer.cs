using System;
using CustomBeatmaps.UI.Structure;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque.OsuEditor
{
    public class OsuEditorUIRenderer : UIRenderer, IOsuUIMain
    {
        private OsuUIMainProps _props;
        public void Init(OsuUIMainProps props)
        {
            _props = props;
            Open();
        }
        public void Open()
        {
            GameObject.SetActive(true);
        }
        public void Close()
        {
            GameObject.SetActive(false);
        }

        protected override void OnUnityGUI()
        {
            GUILookHelper.SetGUISkin();

            bool paused = _props.GetPaused();
            float songTime = _props.GetCurrentTime();
            float songDuration = _props.GetSongTotalLength();

            BottomPanel.Render(16, 64, () =>
            {
                PauseButton.Render(paused, _props.OnSetPaused, GUILayout.Width(128));
                TimerSlider.Render(songTime, songDuration, _ => {/*Don't set time yet*/}, GUILayout.ExpandWidth(true));
                TimeScaleSlider.Render(_props.OnSetTimeScale, GUILayout.Width(256));
            });

            if (!JeffBezosController.paused && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Space)
            {
                Debug.Log("PAUSY PAUSE");
                // Toggle pause when space is pressed
                _props.OnSetPaused(!paused);
            }
        }
    }
}
