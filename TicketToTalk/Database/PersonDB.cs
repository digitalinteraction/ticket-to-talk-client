using System;
using SQLite;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TicketToTalk
{
	/// <summary>
	/// Database manager to represent the Person model.
	/// </summary>
	public class PersonDB
	{

		public static bool locked = false;

		private SQLiteConnection _connection;
		string dbPath;

		/// <summary>
		/// Create a connection to the Person table.
		/// </summary>
		public PersonDB()
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
			_connection.CreateTable<Person>();
		}

		/// <summary>
		/// Get all people.
		/// </summary>
		/// <returns>The persons.</returns>
		public List<Person> GetPersons()
		{
			List<Person> people = new List<Person>();
			var query = _connection.Query<Person>("SELECT * FROM Person");
			for (int i = 0; i < query.Count; i++)
			{
				people.Add(query[i]);
			}
			return people;
			//return null;
			//return (from t in _connection.Table<Person>() select t).ToList();
		}

		/// <summary>
		/// Get Person by ID
		/// </summary>
		/// <returns>The person.</returns>
		/// <param name="id">Identifier.</param>
		public Person GetPerson(int id)
		{
			return _connection.Table<Person>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the person.
		/// </summary>
		/// <returns>The person.</returns>
		/// <param name="id">Identifier.</param>
		public void DeletePerson(int id)
		{
			_connection.Delete<Person>(id);
		}

		/// <summary>
		/// Adds the person.
		/// </summary>
		/// <returns>The person.</returns>
		/// <param name="person">Person.</param>
		public void AddPerson(Person person)
		{
			_connection.Insert(person);
		}

		public void clearTable()
		{
			_connection.Query<Person>("DELETE FROM Person");
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