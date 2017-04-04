using System;
using System.IO;
using SQLite;

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

		//public const string DB = "t3.26.db3";
		//public static readonly string baseUrl = "http://localhost:8080/api/";

		public const string DB = "t3_live.65.db3";
		public static readonly string baseUrl = "https://tickettotalk.openlab.ncl.ac.uk/api/";

		private static SQLiteConnection _connection = null;
		public static SQLiteConnection Connection 
		{
			get
			{
				if (_connection == null)
				{
					var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
					_connection = new SQLiteConnection(dbPath);

					_connection.CreateTable<Area>();
					_connection.CreateTable<Article>();
					_connection.CreateTable<Conversation>();
					_connection.CreateTable<Inspiration>();
					_connection.CreateTable<Period>();
					_connection.CreateTable<Person>();
					_connection.CreateTable<PersonPeriod>();
					_connection.CreateTable<PersonUser>();
					_connection.CreateTable<Tag>();
					_connection.CreateTable<Ticket>();
					_connection.CreateTable<TicketTag>();
					_connection.CreateTable<User>();

					return _connection;
				}
				else 
				{
					return _connection;
				}
			}
			set {
				if (_connection != value) 
				{
					_connection = value;
				}
			}
		}

		public Session()
		{
		}
	}
}

