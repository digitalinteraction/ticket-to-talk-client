using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace TicketToTalk
{
	public class PeriodDB
	{
		private SQLiteConnection _connection;
		string dbPath;

		/// <summary>
		/// Create a connection to the Person table.
		/// </summary>
		public PeriodDB()
		{

			Console.WriteLine("Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		/// <summary>
		/// Open a database connection.
		/// </summary>
		public void open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<Period>();
		}

		/// <summary>
		/// Gets the periods.
		/// </summary>
		/// <returns>The periods.</returns>
		public List<Period> GetPeriods()
		{
			List<Period> people = new List<Period>();
			var query = _connection.Query<Period>("SELECT * FROM Period");
			for (int i = 0; i < query.Count; i++)
			{
				people.Add(query[i]);
			}
			return people;
		}

		/// <summary>
		/// Gets the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="id">Identifier.</param>
		public Period GetPeriod(int id)
		{
			return _connection.Table<Period>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="id">Identifier.</param>
		public void DeletePeriod(int id)
		{
			_connection.Delete<Period>(id);
		}

		/// <summary>
		/// Adds the period.
		/// </summary>
		/// <returns>The period.</returns>
		/// <param name="period">Period.</param>
		public void AddPeriod(Period period)
		{
			_connection.Insert(period);
		}

		public void clearTable()
		{
			_connection.Query<Period>("DELETE FROM Period");
		}

		/// <summary>
		/// Close the database connection.
		/// </summary>
		public void close()
		{
			_connection.Close();
		}
	}
}

