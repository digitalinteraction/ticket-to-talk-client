using System;
using SQLite;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TicketToTalk
{
	/// <summary>
	/// Database controller for the ticket_tag relationship model.
	/// </summary>
	public class TicketTagDB
	{
		private SQLiteConnection _connection;
		private string dbPath;

		/// <summary>
		/// Creates a connection to the ticket tag table.
		/// </summary>
		public TicketTagDB()
		{
			Debug.WriteLine("TicketTagDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		/// <summary>
		/// Opens a connection to the database.
		/// </summary>
		public void Open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<TicketTag>();
		}

		/// <summary>
		/// Gets the relations.
		/// </summary>
		/// <returns>The relations.</returns>
		public IEnumerable<TicketTag> GetRelations()
		{
			return (from t in _connection.Table<TicketTag>() select t).ToList();
		}

		/// <summary>
		/// Gets the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public TicketTag GetRelation(int id)
		{
			return _connection.Table<TicketTag>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteRelation(int id)
		{
			_connection.Delete<TicketTag>(id);
		}

		/// <summary>
		/// Adds the ticket tag relationship.
		/// </summary>
		/// <returns>The ticket tag relationship.</returns>
		/// <param name="relation">Relation.</param>
		public void AddTicketTagRelationship(TicketTag relation)
		{
			_connection.Insert(relation);
		}

		/// <summary>
		/// Gets the relation by tag identifier.
		/// </summary>
		/// <returns>The relation by tag identifier.</returns>
		/// <param name="TagID">Tag identifier.</param>
		public List<TicketTag> GetRelationByTagID(int TagID)
		{
			var results = _connection.Query<TicketTag>("SELECT * FROM TicketTag WHERE tag_id = ?", TagID);
			if (results.Count > 0)
			{
				return results;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the relation by ticket identifier.
		/// </summary>
		/// <returns>The relation by ticket identifier.</returns>
		/// <param name="TicketID">Ticket identifier.</param>
		public List<TicketTag> GetRelationByTicketID(int TicketID)
		{
			var results = _connection.Query<TicketTag>("SELECT * FROM TicketTag WHERE ticket_id = ?", TicketID);

			if (results.Count > 0)
			{
				return results;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the relation by ticket and tag identifier.
		/// </summary>
		/// <returns>The relation by ticket and tag identifier.</returns>
		/// <param name="ticket_id">Ticket identifier.</param>
		/// <param name="tag_id">Tag identifier.</param>
		public TicketTag GetRelationByTicketAndTagID(int ticket_id, int tag_id)
		{
			var results = _connection.Query<TicketTag>(String.Format("SELECT * FROM TicketTag WHERE ticket_id = {0} AND tag_id = {1}", ticket_id, tag_id));
			if (results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
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
