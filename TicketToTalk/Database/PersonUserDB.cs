using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace TicketToTalk
{
	public class PersonUserDB
	{
		private SQLiteConnection _connection;

		/// <summary>
		/// Creates a connection to the ticket tag table.
		/// </summary>
		public PersonUserDB()
		{

			Console.WriteLine("Establishing DB connection");
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
			_connection = new SQLiteConnection(dbPath);

			_connection.CreateTable<PersonUser>();
		}

		/// <summary>
		/// Gets the relations.
		/// </summary>
		/// <returns>The relations.</returns>
		public IEnumerable<PersonUser> GetRelations()
		{
			return (from t in _connection.Table<PersonUser>() select t).ToList();
		}

		/// <summary>
		/// Gets the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public PersonUser GetRelation(int id)
		{
			return _connection.Table<PersonUser>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the relation.
		/// </summary>
		/// <returns>The relation.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteRelation(int id)
		{
			_connection.Delete<PersonUser>(id);
		}

		/// <summary>
		/// Adds the person user.
		/// </summary>
		/// <returns>The person user.</returns>
		/// <param name="relation">Relation.</param>
		public void AddPersonUser(PersonUser relation)
		{
			_connection.Insert(relation);
		}

		/// <summary>
		/// Gets the relation by person identifier.
		/// </summary>
		/// <returns>The relation by person identifier.</returns>
		/// <param name="person_id">Person identifier.</param>
		public List<PersonUser> getRelationByPersonID(int person_id)
		{
			var results = _connection.Query<PersonUser>("SELECT * FROM PersonUser WHERE person_id = ?", person_id);

			if (results.Count < 0)
			{
				return null;
			}
			else
			{
				return results;
			}
		}

		/// <summary>
		/// Gets the relation by user identifier.
		/// </summary>
		/// <returns>The relation by user identifier.</returns>
		/// <param name="user_id">User identifier.</param>
		public List<PersonUser> getRelationByUserID(int user_id)
		{
			var results = _connection.Query<PersonUser>("SELECT * FROM PersonUser WHERE user_id = ?", user_id);

			if (results.Count < 0)
			{
				return null;
			}
			else 
			{
				return results;
			}
		}

		public PersonUser getRelationByUserAndPersonID(int user_id, int person_id) 
		{
			var results = _connection.Query<PersonUser>(String.Format("SELECT * FROM PersonUser WHERE user_id = {0} AND person_id = {1}", user_id, person_id));
			if (results.Count > 0)
			{
				return results[0];
			}
			else 
			{
				return null;
			}
		}

		public void clearTable() 
		{
			_connection.Query<PersonUser>("DELETE FROM PersonUser");
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

