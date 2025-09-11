// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("P7yyvY0/vLe/P7y8vRtcxtLQbbMaJgtwl42mKz51LMzRxc1DQ39i7sdtQvLKLuslmSyXJXgF8Pen55QRjT+8n42wu7SXO/U7SrC8vLy4vb4Y21qzuHYHzTAdPZowRJBZqj/HDz1VhYbvD8M+SWicWP1txedufLjGsIlqFK7Xc+tqt/Bgy5Xv+WOKeSNM47uAWc6PpIsjRl61UL6pqFm2xo+g4y0xxa5mdlXEGFfcnGa7igiOyDxJVdf3It3/fvA6ZoGZptRwTNjxDyqbRMBuryV41GDpadZyFSWUdzdfYIbNncNlBwmc/XDIK3mxEzZGQxnsCG25TOt/JFCg9GNZtAHmPk10qEDUXcc8XLB4+GSGty1u1pjHqlNLYiiF8wLP7r++vL28");
        private static int[] order = new int[] { 1,7,12,7,12,12,7,12,10,13,11,11,13,13,14 };
        private static int key = 189;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
