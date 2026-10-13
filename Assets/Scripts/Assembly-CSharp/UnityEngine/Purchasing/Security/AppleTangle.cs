using System;

namespace UnityEngine.Purchasing.Security
{
	public class AppleTangle
	{
		private static byte[] data = Convert.FromBase64String("1PLWxMGXxsHRz4Oysq6n4pCtrbZNsUOiBNmZy+1QcDqGijKi+lzXN+3yQwHEyunEw8fHxcDA8kN02ENx4oGD8kDD4PLPxMvoRIpENc/Dw8Pk8ubEwZfGydHfg7KyrqfigaewtraqrbCrtrvz1PLWxMGXxsHRz4OyoK6n4rG2o6ymo7Cm4ransK+x4qP/5KXiSPGoNc9ADRwpYe07kaiZprarpKuho7an4qC74qOsu+Kyo7C27IJkNYWPvcqc8t3EwZff4cba8tTio6ym4qGnsLarpKuho7arrazisrW17KOysq6n7KGtr+2jsrKup6Gjsq6n4oGnsLarpKuho7arrazig7fPxMvoRIpENc/Dw8fHwsFAw8PCngvbsDefzBe9nVkw58F4l02Pn88zc/KaLpjG8E6qcU3fHKexPaWcp36up+KLrKHs8+Ty5sTBl8bJ0d+DsosatF3x1qdjtVYL78DBw8LDYUDDq6SroaO2q62s4oO3tqqtsKu2u/PEwZffzMbUxtbpEquFVrTLPDapTwKh8bU1+MXulCkYzePMGHix24137uKhp7C2q6SroaO2p+Kyra6robvoRIpENc/Dw8fHwvKg88nyy8TBlxv0vQNFlxtlW3vwgDkaF7NcvGOQSdtLHDuJrjfFaeDywCra/DqSyxFAw8LEy+hEikQ1oabHw/JDMPLoxELW6RKrhVa0yzw2qU/sgmQ1hY+94q2k4raqp+K2qqes4qOysq6roaO9g2paOxMIpF7mqdMSYXkm2egB3fLTxMGXxsjRyIOysq6n4ousoezz8fSY8qDzyfLLxMGXxsTRwJeR89HE8s3EwZff0cPDPcbH8sHDwz3y37jyQMO08szEwZffzcPDPcbGwcDDm2XHy77VgpTT3LYRdUnh+YVhF62HvN2OqZJUg0sGtqDJ0kGDRfFIQ8Uuv/tBSZHiEfoGc31YjcipPek+ddl/UYDm0OgFzd90j16coQqJQtXNX/8x6Yvq2Ao8DHd7zBuc3hQJ//JAxnnyQMFhYsHAw8DAw8Dyz8TLrKbioa2spqu2q62sseKtpOK3saf38PP28vH0mNXP8ffy8PL78PP28nw2sVksEKbNCbuN9hpg/Du6PakKkKeuq6OsoafirazitqqrseKhp7D0W47vunUvTlkeMbVZMLQQtfKNA8rpxMPHx8XAw9Tcqra2srH47e21V1y4zmaFSZkW1PXxCQbNjwzWqxPdUxnchZIpxy+cu0bvKfRglY6XLnf4bzbNzMJQyXPj1Oy2F/7PGaDUsq6n4pCtrbbigYPy3NXP8vTy9vBqHrzg9wjnFxvNFKkWYObh0zVjbsqc8kDD08TBl9/ixkDDyvJAw8byaWGzUIWRlwNt7YNxOjkhsg8kYY674qOxsbevp7Hio6Ghp7K2o6yhp6VNynbiNQlu7uKtsnT9w/JOdYENx8LBQMPNwvJAw8jAQMPDwiZTa8vmICkTdbIdzYcj5Qgzr7ovJXfV1bCjobaroafisbajtqevp6y2sezyxsTRwJeR89Hy08TBl8bI0ciDsrLdR0FH2Vv/hfUwa1mCTO4Wc1LQGqb34deJ15vfcVY1NF5cDZJ4A5qSkmhIFxgmPhLLxfVyt7fj");

		private static int[] order = new int[61]
		{
			12, 55, 37, 13, 10, 5, 42, 56, 39, 31,
			45, 36, 45, 47, 19, 26, 47, 27, 36, 30,
			57, 44, 55, 58, 47, 34, 32, 41, 41, 31,
			39, 31, 41, 52, 34, 51, 59, 55, 45, 43,
			56, 50, 45, 48, 51, 59, 50, 58, 59, 54,
			52, 58, 56, 58, 59, 58, 56, 58, 59, 59,
			60
		};

		private static int key = 194;

		public static readonly bool IsPopulated = true;

		public static byte[] Data()
		{
			if (!IsPopulated)
			{
				return null;
			}
			return Obfuscator.DeObfuscate(data, order, key);
		}
	}
}
