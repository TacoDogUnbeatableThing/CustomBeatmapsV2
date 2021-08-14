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
        private static readonly Dictionary<string, object> States = new Dictionary<string, object>();
        private static readonly Dictionary<string, object[]> EffectDependencies = new Dictionary<string, object[]>();

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
            return UseState(() => defaultValue, lineNumber);
        }

        public static (T, Action<T>) UseState<T>(Func<T> getDefaultValue, [CallerLineNumber] int lineNumber = 0)
        {
            var key = GetNewKeyNow(lineNumber);

            if (!States.ContainsKey(key)) States[key] = getDefaultValue.Invoke();

            var result = (T) States[key];
            Action<T> setter = t => States[key] = t;

            return (result, setter);
        }

        public static void UseEffect(Action onChange, object[] dependencies, [CallerLineNumber] int lineNumber = 0)
        {
            var key = GetNewKeyNow(lineNumber);

            if (!EffectDependencies.ContainsKey(key))
            {
                EffectDependencies[key] = dependencies;
                onChange.Invoke();
            }
            else
            {
                var oldDeps = EffectDependencies[key];
                if (oldDeps.Length != dependencies.Length)
                    onChange.Invoke();
                else
                    for (var i = 0; i < oldDeps.Length; ++i)
                        if (!Equals(oldDeps[i], dependencies[i]))
                        {
                            onChange.Invoke();
                            break;
                        }

                EffectDependencies[key] = dependencies;
            }
        }
    }
}