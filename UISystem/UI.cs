using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CustomBeatmaps.UISystem
{
    /**
     * I'm really stupid for trying this
     * but dammit if they're gonna render UI in code,
     * I'm gonna try shoving my own system into this
     */
    public class UI
    {
        private static Dictionary<string, object> _states = new Dictionary<string, object>();
        private static Dictionary<string, object[]> _effectDependencies = new Dictionary<string, object[]>();
        private static string GetNewKeyNow(int lineNumber)
        {
            return Environment.StackTrace + $"->{lineNumber}";
        }

        public static int GetUniqueId([CallerLineNumber] int lineNumber = 0)
        {
            return GetNewKeyNow(lineNumber).GetHashCode();
        }

        public static (T, Action<T>) UseState<T>(T defaultValue, [CallerLineNumber] int lineNumber = 0)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return UseState(() => defaultValue, lineNumber: lineNumber);
        }

        public static (T, Action<T>) UseState<T>(Func<T> getDefaultValue, [CallerLineNumber] int lineNumber = 0)
        {
            string key = GetNewKeyNow(lineNumber);

            if (!_states.ContainsKey(key))
            {
                _states[key] = getDefaultValue.Invoke();
            }

            T result = (T)_states[key];
            Action<T> setter = t => _states[key] = t;

            return (result, setter);
        }

        public static void UseEffect(Action onChange, object[] dependencies, [CallerLineNumber] int lineNumber = 0)
        {
            string key = GetNewKeyNow(lineNumber);

            if (!_effectDependencies.ContainsKey(key))
            {
                _effectDependencies[key] = dependencies;
                onChange.Invoke();
            }
            else
            {
                object[] oldDeps = _effectDependencies[key];
                if (oldDeps.Length != dependencies.Length)
                {
                    onChange.Invoke();
                }
                else
                {
                    for (int i = 0; i < oldDeps.Length; ++i)
                    {
                        if (!Equals(oldDeps[i], dependencies[i]))
                        {
                            onChange.Invoke();
                            break;
                        }
                    }
                }
                _effectDependencies[key] = dependencies;
            }
        } 
    };
}
