using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite;

namespace TicketToTalk
{
	public class PersonPeriodDB
	{
		private SQLiteConnection _connection;
		private string dbPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.PersonPeriodDB"/> class.
		/// </summary>
		public PersonPeriodDB()
		{
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		/// <summary>
		/// Opens the database connection.
		/// </summary>
		public void Open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<PersonPeriod>();
		}

		/// <summary>
		/// Gets the relations.
		/// </summary>
		/// <returns>The relations.</returns>
		public IEnumerable<PersonPeriod> GetRelations()
		{
			return (from t in _connection.Table<PersonPeriod>() select t).ToList();
		}

		/// <summary>
		/// Gets the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public PersonPeriod GetRelation(int id)
		{
			return _connection.Table<PersonPeriod>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteRelation(int id)
		{
			_connection.Delete<PersonPeriod>(id);
		}

		/// <summary>
		/// Adds the person period relationship.
		/// </summary>
		/// <returns>The person period relationship.</returns>
		/// <param name="relation">Relation.</param>
		public void AddPersonPeriodRelationship(PersonPeriod relation)
		{
			_connection.Insert(relation);
		}

		/// <summary>
		/// Gets the relation by person identifier.
		/// </summary>
		/// <returns>The relation by person identifier.</returns>
		/// <param name="person_id">Person identifier.</param>
		public List<PersonPeriod> GetRelationByPersonID(int person_id)
		{
			var results = _connection.Query<PersonPeriod>("SELECT * FROM PersonPeriod WHERE person_id = ?", person_id);
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
		/// <param name="period_id">Period identifier.</param>
		public List<PersonPeriod> GetRelationByTicketID(int period_id)

		{
			var results = _connection.Query<PersonPeriod>("SELECT * FROM PersonPeriod WHERE period_id = ?", period_id);

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
		/// Gets the relation by person and period identifier.
		/// </summary>
		/// <returns>The relation by person and period identifier.</returns>
		/// <param name="person_id">Person identifier.</param>
		/// <param name="period_id">Period identifier.</param>
		public PersonPeriod GetRelationByPersonAndPeriodID(int person_id, int period_id)
		{
			var results = _connection.Query<PersonPeriod>(String.Format("SELECT * FROM PersonPeriod WHERE person_id = {0} AND period_id = {1}", person_id, period_id));
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

