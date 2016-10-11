using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TicketToTalk
{
	/// <summary>
	/// Database handler for the area model.
	/// </summary>
	public class AreaDB
	{
		private SQLiteConnection _connection;
		private string dbPath;

		/// <summary>
		/// Opens database connection for the area table.
		/// </summary>
		public AreaDB()
		{

			Debug.WriteLine("AreaDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		public void Open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<Area>();
		}

		/// <summary>
		/// Gets the areas.
		/// </summary>
		/// <returns>The areas.</returns>
		public IEnumerable<Area> GetAreas()
		{
			return (from t in _connection.Table<Area>() select t).ToList();
		}

		/// <summary>
		/// Gets an area by ID.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Identifier.</param>
		public Area GetArea(int id)
		{
			return _connection.Table<Area>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the area by ID.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteArea(int id)
		{
			_connection.Delete<Area>(id);
		}

		/// <summary>
		/// Adds the area.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="area">Area.</param>
		public void AddArea(Area area)
		{
			_connection.Insert(area);
		}

		/// <summary>
		/// Gets unique towns/cities.
		/// </summary>
		/// <returns>The unique town city.</returns>
		public IEnumerable<Area> GetUniqueTownCity()
		{
			var stored = _connection.Query<Area>("SELECT DISTINCT townCity FROM Area");
			Console.WriteLine("Printing TOWNS");
			foreach (Area a in stored)
			{
				Console.WriteLine(a.townCity);
			}
			return stored;
		}

		/// <summary>
		/// Delete all records.
		/// </summary>
		/// <returns>The null.</returns>
		public void DeleteNull()
		{
			_connection.Query<Area>("Delete From Area Where 1 = 1");
		}

		/// <summary>
		/// Get the highest ID value
		/// </summary>
		/// <returns>The identifier.</returns>
		public Area MaxID()
		{
			var max = _connection.Query<Area>("Select * FROM Area ORDER BY ID DESC LIMIT 1");

			Console.WriteLine("MAX ID QUERY");
			foreach (Area a in max)
			{
				Console.WriteLine(a);
			}

			if (max.Count != 0)
			{
				return max[0];
			}
			else
			{
				Console.WriteLine("No area records");
				return null;
			}
		}

		/// <summary>
		/// Gets the area by town.
		/// </summary>
		/// <returns>The area by town.</returns>
		/// <param name="town_city">Town city.</param>
		public IEnumerable<Area> GetAreaByTown(string town_city)
		{
			return _connection.Query<Area>("SELECT * FROM Area WHERE townCity = ?", town_city);
		}

		/// <summary>
		/// Clears the table.
		/// </summary>
		public void ClearTable()
		{
			_connection.Query<Ticket>("DELETE FROM Area");
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void Close()
		{
			_connection.Close();
		}
	}
}