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
	/// Database controller for the User database.
	/// </summary>
	public class UserDB
	{
		private SQLiteConnection _connection;
		private string dbPath;

		/// <summary>
		/// Creates a connection to the User table.
		/// </summary>
		public UserDB()
		{
			Debug.WriteLine("UserDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		public void Open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<User>();
		}

		/// <summary>
		/// Gets the users.
		/// </summary>
		/// <returns>The users.</returns>
		public IEnumerable<User> GetUsers()
		{
			return (from t in _connection.Table<User>() select t).ToList();
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="id">Identifier.</param>
		public User GetUser(int id)
		{
			return _connection.Table<User>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteUser(int id)
		{
			_connection.Delete<User>(id);
		}

		/// <summary>
		/// Adds the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="user">User.</param>
		public void AddUser(User user)
		{
			_connection.Insert(user);
		}

		/// <summary>
		/// Gets the user by email.
		/// </summary>
		/// <returns>The user by email.</returns>
		/// <param name="email">Email.</param>
		public User GetUserByEmail(string email)
		{

			var stored = _connection.Query<User>("SELECT * FROM User WHERE email = ?", email);
			//foreach (User u in stored) 
			//{
			//	Console.WriteLine(u);
			//}
			if (stored.Count > 0)
			{
				return stored[0];
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
			_connection.Query<User>("DELETE FROM User");
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