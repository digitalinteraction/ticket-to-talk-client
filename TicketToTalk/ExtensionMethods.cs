using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TicketToTalk
{
	public static class ExtensionMethods
	{
		private static Random rng = new Random();

		/// <summary>
		/// Shuffle the specified list.
		/// Source: http://stackoverflow.com/questions/273313/randomize-a-listt
		/// </summary>
		/// <param name="list">List.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		/// <summary>
		/// Hashs the array.
		/// </summary>
		/// <returns>The array.</returns>
		/// <param name="arr">Arr.</param>
		public static string HashArray(this byte[] arr)
		{
			SHA256 sha = new SHA256Managed();
			byte[] hash = sha.ComputeHash(arr);
			var hashString = byteToHex(hash);
			return hashString;
		}

		/// <summary>
		/// Hashs the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="str">String.</param>
		public static string HashString(this string str)
		{
			var arr = Encoding.UTF8.GetBytes(str);
			return arr.HashArray();
		}

		/// <summary>
		/// Bytes to hex.
		/// Source: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
		/// </summary>
		/// <returns>The to hex.</returns>
		/// <param name="ba">Ba.</param>
		private static string byteToHex(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				hex.AppendFormat("{0:X2}", b);
			}
			return hex.ToString();
		}
	}
}

