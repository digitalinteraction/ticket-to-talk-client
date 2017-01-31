using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite;

namespace TicketToTalk
{
	public class InspirationDB
	{
		private SQLiteConnection _connection;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.InspirationDB"/> class.
		/// </summary>
		public InspirationDB()
		{

			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
			_connection = new SQLiteConnection(dbPath);

			_connection.CreateTable<Inspiration>();
		}

		/// <summary>
		/// Gets the inspirations.
		/// </summary>
		/// <returns>The inspirations.</returns>
		public IEnumerable<Inspiration> GetInspirations()
		{
			return (from t in _connection.Table<Inspiration>() select t).ToList();
		}

		/// <summary>
		/// Gets the inspiration.
		/// </summary>
		/// <returns>The inspiration.</returns>
		/// <param name="id">Identifier.</param>
		public Inspiration GetInspiration(int id)
		{
			return _connection.Table<Inspiration>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the inspiration.
		/// </summary>
		/// <returns>The inspiration.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteInspiration(int id)
		{
			_connection.Delete<Inspiration>(id);
		}

		/// <summary>
		/// Adds the inspiration.
		/// </summary>
		/// <returns>The inspiration.</returns>
		/// <param name="a">The alpha component.</param>
		public void AddInspiration(Inspiration a)
		{
			_connection.Insert(a);
		}

		/// <summary>
		/// Gets a random inspiration.
		/// </summary>
		/// <returns>The random inspiration.</returns>
		public Inspiration GetRandomInspiration()
		{
			var query = _connection.Query<Inspiration>("SELECT * FROM Inspiration WHERE used = ?", false);

			if (query.Count > 0)
			{
				var idx = new Random().Next(query.Count());
				return query[idx];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Clears the table.
		/// </summary>
		public void ClearTable()
		{
			_connection.Query<Inspiration>("DELETE FROM Inspiration");
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

