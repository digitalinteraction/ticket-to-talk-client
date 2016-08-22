using System;
using System.IO;
using SQLite;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class ISQLiteImp : ISQLite
	{
		public ISQLiteImp()
		{
		}

		public SQLiteConnection GetConnection()
		{
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"ticketstotalk.db3");
			var db = new SQLiteConnection(dbPath);

			return db;
		}
	}
}