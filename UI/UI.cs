using System;
using System.Collections.Generic;

namespace CustomBeatmaps.UI
{
    /**
     * I'm really stupid for trying this
     * but dammit if they're gonna render UI in code,
     * I'm gonna try shoving my own system into this
     */
    public class UI
    {
        private static Dictionary<string, object> _states = new Dictionary<string, object>();
        private static string GetNewKeyNow()
        {
            return Environment.StackTrace;
        }

        public static int GetUniqueId()
        {
            return GetNewKeyNow().GetHashCode();
        }

        public static (T, Action<T>) UseState<T>(T defaultValue)
        {
            return UseState(() => defaultValue);
        }

        public static (T, Action<T>) UseState<T>(Func<T> getDefaultValue)
        {
            string key = GetNewKeyNow();

            if (!_states.ContainsKey(key))
            {
                _states[key] = getDefaultValue.Invoke();
            }

            T result = (T)_states[key];
            Action<T> setter = t => _states[key] = t;

            return (result, setter);
        }
    };
}
