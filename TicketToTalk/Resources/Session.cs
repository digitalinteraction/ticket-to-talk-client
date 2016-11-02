using System;
namespace TicketToTalk
{
	/// <summary>
	/// Session information store.
	/// </summary>
	public class Session
	{
		public static Token Token;

		public static User activeUser;
		public static Person activePerson;

		public static int ScreenWidth;
		public static int ScreenHeight;

		//public static string DB = "tickettotalk_server.4.db3";
		//public static readonly string baseUrl = "http://52.35.119.74/api/";

		//public static string DB = "tickettotalk_1.5.db3";
		//public static readonly string baseUrl = "http://tickettotalk.app/api/";

		public static string DB = "tickettotalk.ol.1.db3";
		public static readonly string baseUrl = "http://tickettotalk.openlab.ncl.ac.uk/api/";

		public Session()
		{
		}
	}
}

