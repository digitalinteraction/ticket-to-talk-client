using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TicketToTalk
{
	/// <summary>
	/// Database controller for the ticket area relationship model.
	/// </summary>
	public class TicketAreaDB
	{
		private SQLiteConnection _connection;

		/// <summary>
		/// Creates a connection to the ticket_area table
		/// </summary>
		public TicketAreaDB()
		{
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
			_connection = new SQLiteConnection(dbPath);

			_connection.CreateTable<TicketArea>();
		}

		/// <summary>
		/// Gets the ticket areas.
		/// </summary>
		/// <returns>The ticket areas.</returns>
		public IEnumerable<TicketArea> GetTicketAreas()
		{
			return (from t in _connection.Table<TicketArea>() select t).ToList();
		}

		/// <summary>
		/// Gets the ticket area by ID.
		/// </summary>
		/// <returns>The ticket area.</returns>
		/// <param name="id">Identifier.</param>
		public TicketArea GetTicketArea(int id)
		{
			return _connection.Table<TicketArea>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the ticket area.
		/// </summary>
		/// <returns>The ticket area.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteTicketArea(int id)
		{
			_connection.Delete<TicketArea>(id);
		}

		/// <summary>
		/// Adds the ticket area.
		/// </summary>
		/// <returns>The ticket area.</returns>
		/// <param name="area">Area.</param>
		public void AddTicketArea(TicketArea area)
		{
			_connection.Insert(area);
		}

		/// <summary>
		/// Gets the relation by ticket identifier.
		/// </summary>
		/// <returns>The relation by ticket identifier.</returns>
		/// <param name="ticketID">Ticket identifier.</param>
		public List<TicketArea> GetRelationByTicketID(int ticketID)
		{
			var results = _connection.Query<TicketArea>("SELECT * FROM TicketArea WHERE ticket_id = ?", ticketID);
			List<TicketArea> relations = new List<TicketArea>();

			foreach (TicketArea r in results)
			{
				relations.Add(r);
			}
			return relations;
		}

		/// <summary>
		/// Gets the relation by area identifier.
		/// </summary>
		/// <returns>The relation by area identifier.</returns>
		/// <param name="areaID">Area identifier.</param>
		public List<TicketArea> GetRelationByAreaID(int areaID)
		{
			var results = _connection.Query<TicketArea>("SELECT * FROM TicketArea WHERE area_id = ?", areaID);
			List<TicketArea> relations = new List<TicketArea>();

			foreach (TicketArea r in results)
			{
				relations.Add(r);
			}
			return relations;
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