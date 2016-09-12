﻿using System;
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

		//public static string DB = "ttt3_server_test.4.db3";
		//public static readonly string baseUrl = "http://52.35.119.74/api/";

		public static string DB = "tickettotalk.59.db3";
		public static readonly string baseUrl = "http://tickettotalk.app/api/";

		public Session()
		{
		}
	}
}

