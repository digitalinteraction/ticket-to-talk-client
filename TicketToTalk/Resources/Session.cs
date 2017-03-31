using System;
namespace TicketToTalk
{
	/// <summary>
	/// Session information store.
	/// </summary>
	public class Session
	{
		public static Token Token;

		public static User activeUser = new User();
		public static Person activePerson;

		public static int ScreenWidth;
		public static int ScreenHeight;

		//public const string DB = "t3.23.db3";
		//public static readonly string baseUrl = "http://localhost:8080/api/";

		public const string DB = "t3_live.2.db3";
		public static readonly string baseUrl = "https://tickettotalk.openlab.ncl.ac.uk/api/";

		public Session()
		{
		}
	}
}

