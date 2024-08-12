// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("x89EZeou8Fv+wEkvljewXcG/bPqloUFqMzrHcnLwCNaE27ePRW3L8yOgrqGRI6CroyOgoKE2mtgmrzbwlkcrJGQ3hiVjHFw6by3fLbGIKWSRI6CDkaynqIsn6SdWrKCgoKShojDbAgBOlVjw5zo93TJ5MnqedHe0zlbyHtELS4TtjEP9kd76vykelnRLDu6sdGW/2T8y2qwNh7FYD74MoGktwbO6ExhJM2YLxey2EniUS8VUbMgdiuURmhmJLEoijDIWXScOT7MwBr7Wkww58CBaszRQrFgETjYjPcxCdCjBCt41YU2tx98eFMfqsH6+2HYjqx2wvkb3K7A9XJ7Sk4BUfWJzq4OL10T5Gel7edqSaMWuIz/0UlfaIHPXGLQKYqOioKGg");
        private static int[] order = new int[] { 5,11,11,10,5,5,8,12,12,13,12,11,12,13,14 };
        private static int key = 161;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
