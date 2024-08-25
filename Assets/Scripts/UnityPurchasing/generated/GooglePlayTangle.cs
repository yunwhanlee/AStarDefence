// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HJ+Rnq4cn5ScHJ+fngml5xmQCc9WEv6MhSwndgxZNPrTiS1Hq3T6a6l4FBtbCLkaXCNjBVAS4BKOtxZbDzmB6awzBs8fZYwLb5NnO3EJHALnSRyUIo+BecgUjwJjoe2sv2tCXQ/kPT9xqmfP2AUC4g1GDUWhS0iL+PB7WtURz2TB/3YQqQiPYv6AU8XzfUsX/jXhCl5ykvjgISv41Y9Bga4cn7yuk5iXtBjWGGmTn5+fm56d8WnNIe40dLvSs3zCruHFgBYhqUtMlLy06HvGJtZERuWtV/qRHADLbVP3IrXaLqUmthN1HbMNKWIYMXCMmp5+VQwF+E1Nzzfpu+SIsHpS9Mx0MdGTS1qA5gAN5ZMyuI5nMIEzn2jlH0zoJ4s1XZydn56f");
        private static int[] order = new int[] { 1,7,10,6,6,6,6,10,10,10,10,13,13,13,14 };
        private static int key = 158;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
