using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Database controller for the ticket Model.
	/// </summary>
	public class TicketDB
	{
		private SQLiteConnection _connection;
		string dbPath;

		/// <summary>
		/// Creates a connection to the Ticket database.
		/// </summary>
		public TicketDB()
		{
			Debug.WriteLine("TicketDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		public void open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<Ticket>();
		}

		/// <summary>
		/// Gets the tickets.
		/// </summary>
		/// <returns>The tickets.</returns>
		public IEnumerable<Ticket> GetTickets()
		{
			return (from t in _connection.Table<Ticket>() select t).ToList();
		}

		/// <summary>
		/// Gets the ticket by ID.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="id">Identifier.</param>
		public Ticket GetTicket(int id)
		{
			return _connection.Table<Ticket>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteTicket(int id)
		{
			_connection.Delete<Ticket>(id);
		}

		/// <summary>
		/// Adds the ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		/// <param name="ticket">Ticket.</param>
		public int AddTicket(Ticket ticket)
		{
			var id = _connection.Insert(ticket);
			return id;
		}

		/// <summary>
		/// Gets the lasts the ticket added.
		/// </summary>
		/// <returns>The ticket added.</returns>
		public Ticket lastTicketAdded()
		{
			var max = _connection.Query<Ticket>("Select * FROM Ticket ORDER BY ID DESC LIMIT 1");
			return max[0];
		}

		/// <summary>
		/// Gets the tickets by person.
		/// </summary>
		/// <returns>The tickets by person.</returns>
		/// <param name="person_id">Person identifier.</param>
		public List<Ticket> getTicketsByPerson(int person_id)
		{
			return _connection.Query<Ticket>("SELECT * FROM Ticket WHERE person_id = ?", person_id);
		}

		/// <summary>
		/// Gets the tickets by period identifier.
		/// </summary>
		/// <returns>The tickets by period identifier.</returns>
		/// <param name="period_id">Period identifier.</param>
		public List<Ticket> getTicketsByPeriodID(int period_id)
		{
			return _connection.Query<Ticket>("SELECT * FROM Ticket WHERE period_id = ?", period_id);
		}

		/// <summary>
		/// Clears the table.
		/// </summary>
		/// <returns>The table.</returns>
		public void clearTable()
		{
			_connection.Query<Ticket>("DELETE FROM Ticket");
		}

		/// <summary>
		/// Close this instance.
		/// </summary>
		public void close()
		{
			_connection.Close();
		}
	}
}

