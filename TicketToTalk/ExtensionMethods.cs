using System;
using System.Collections.Generic;

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
	}
}

