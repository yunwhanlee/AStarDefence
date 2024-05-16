// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rjaSfrFrK+SN7COd8b6a30l+9hQTy+PrtySZeYkbGbryCKXOQ1+UMglNodPac3gpUwZrpYzWchj0K6U0ULtiYC71OJCHWl29UhlSGv4UF9SnryQFik6QO56gKU/2V9A9od8MmvFDwOPxzMfI60eJRzbMwMDAxMHCK26OzBQF37lfUrrMbefROG/ebMBDwM7B8UPAy8NDwMDBVvq4Rs9WkMXBIQpTWqcSEpBotuS71+8lDauTrCIUSKFqvlUBLc2nv350p4rQHt64FkPLfdDeJpdL0F08/rLz4DQdAvYnS0QEV+ZFA3w8Wg9Nv03R6EkEUGbetvNsWZBAOtNUMMw4ZC5WQ10MqH3qhXH6eelMKkLsUnY9R24v0ze6QBO3eNRqAsPCwMHA");
        private static int[] order = new int[] { 8,9,7,4,5,8,12,9,11,9,11,11,12,13,14 };
        private static int key = 193;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
