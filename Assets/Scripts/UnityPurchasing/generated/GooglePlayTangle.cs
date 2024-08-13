// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("E52r9x7VAeq+knIYAMHLGDVvoWHvBN3fkUqHLzjl4gLtpu2lQauoa+/ZYQlM0+Yv/4Vs649zh9uR6fzi/H9xfk78f3R8/H9/fulFB/lw6S9JmPT7u+hZ+rzDg+Ww8gDyblf2u7byHmxlzMeW7LnUGjNpzadLlBqLen6etezlGK2tL9cJWwRoUJqyFCysdFxUCJsmxjakpgVNtxpx/OArjZTRMXOrumAG4O0Fc9JYbofQYdN/EYktwQ7UlFsyU5wiTgElYPbBSasYEJu6NfEvhCEflvBJ6G+CHmCzJU78f1xOc3h3VPg2+Ilzf39/e359sxfCVTrORcZW85X9U+3JgvjRkGwHqfx0wm9hmSj0b+KDQQ1MX4uivYgF/6wIx2vVvXx9f35/");
        private static int[] order = new int[] { 2,4,6,4,10,7,11,9,12,12,12,11,13,13,14 };
        private static int key = 126;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
