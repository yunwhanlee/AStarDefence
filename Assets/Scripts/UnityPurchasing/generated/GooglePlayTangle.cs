// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("gVlxeSW2C+sbiYsoYJo3XNHNBqA+sIbaM/gsx5O/XzUt7OY1GEKMTML0TCRh/ssC0qhBxqJeqva8xNHPm98zQUjh6rvBlPk3HkTgima5N6bRUlxTY9FSWVHRUlJTxGgq1F3EAmS12daWxXTXke6uyJ3fLd9DetuWY9FScWNeVVp51RvVpF5SUlJWU1A8pADsI/m5dh9+sQ9jLAhN2+xkhp4673gX42jre9640H7A5K/V/L1BNT22lxjcAqkMMrvdZMVCrzNNnghXU7OYwcg1gIAC+iR2KUV9t585ASqE0VnvQky0BdlCz65sIGFypo+Qwinw8rxnqgIVyM8vwIvAiGyGhUa5/BxehpdNK83AKF7/dUOq/Uz+UqUo0oEl6kb4kFFQUlNS");
        private static int[] order = new int[] { 9,2,6,7,6,10,9,8,13,10,11,13,13,13,14 };
        private static int key = 83;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
