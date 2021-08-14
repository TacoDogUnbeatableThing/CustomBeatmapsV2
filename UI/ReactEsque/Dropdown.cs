using System;
using System.Collections.Generic;
using System.Linq;
using CustomBeatmaps.UI.Structure;
using CustomBeatmaps.UISystem;
using UnityEngine;

namespace CustomBeatmaps.UI.ReactEsque
{
    public static class Dropdown
    {
        public static void Render(string selected, IEnumerable<string> options, Action<int> onSelect, params GUILayoutOption[] layoutOptions)
        {
            (bool open, var setOpen) = Reacc.UseState(false);
            // TODO: Button toggles open
            // TODO: Clicking outside of window sets open to false?
        }

        public static void RenderEnum<T>(T selected, Action<T> onSelect, params GUILayoutOption[] layoutOptions) where T : Enum
        {
            // Prolly could use "List.Select" more cleverly here but that wasn't working with `Array`.
            var enumValues = typeof(T).GetEnumValues();
            List<string> options = new List<string>();
            for (int i = 0; i < enumValues.Length; ++i)
            {
                options.Add(enumValues.GetValue(i).ToString());
            }
            Render(selected.ToString(), options.ToArray(), index => onSelect((T) enumValues.GetValue(index)), layoutOptions);
        }
    }
}
