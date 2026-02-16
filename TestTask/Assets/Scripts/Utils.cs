using System;


namespace TestTask
{
    public static class Utils
    {
        public static T GetRandomEnumValue<T>(params T[] exceptValues) where T : struct, Enum
        {
            var values = Enum.GetValues(typeof(T));
            if (values.Length == exceptValues.Length)
                throw new Exception("All values are excluded");

            while (true)
            {
                var i = UnityEngine.Random.Range(0, values.Length);
                var value = (T)values.GetValue(i);
                if (exceptValues != null && Array.IndexOf(exceptValues, value) < 0)
                    return value;
            }
        }
    }
}