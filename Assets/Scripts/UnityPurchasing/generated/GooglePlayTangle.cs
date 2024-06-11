// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("KvuXmNiLOpnfoOCG05FjkQ00ldhy6k6ibbf3OFEw/0EtYkYDlaIqyPeyUhDI2QNlg45mELE7DeSzArAcnxwSHS2fHBcfnxwcHYomZJoTikx7c/jZVpJM50J89ZMqiwzhfQPQRmTKnxehDAL6S5cMgeAibi886MHeLZ8cPy0QGxQ3m1Wb6hAcHBwYHR7PFz83a/hFpVXHxWYu1HkSn4NI7tB0oTZZrSalNZD2njCOquGbsvMPGR391o+Ge87OTLRqOGcLM/nRd0/VkX0PBq+k9Y/at3lQCq7EKPd56Iy6AmovsIVMnOYPiOwQ5Ljyip+BcP7IlH22Yond8RF7Y6Koe1YMwgKMZ7688inkTFuGgWGOxY7GIsjLCOtmnM9rpAi23h8eHB0c");
        private static int[] order = new int[] { 10,8,12,8,5,8,10,9,13,11,11,11,12,13,14 };
        private static int key = 29;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
