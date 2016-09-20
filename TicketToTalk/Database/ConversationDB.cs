using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SQLite;

namespace TicketToTalk
{
	/// <summary>
	/// Conversation db.
	/// </summary>
	public class ConversationDB
	{
		private SQLiteConnection _connection;
		string dbPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationDB"/> class.
		/// </summary>
		public ConversationDB()
		{
			Debug.WriteLine("ConversationDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		/// <summary>
		/// Open a database connection.
		/// </summary>
		public void open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<Conversation>();
		}

		/// <summary>
		/// Gets the conversations.
		/// </summary>
		/// <returns>The conversations.</returns>
		public IEnumerable<Conversation> GetConversationsForPerson()
		{
			var stored = _connection.Query<Conversation>("SELECT * FROM Conversation WHERE person_id = ?", Session.activePerson.id);
			var convs = (from t in _connection.Table<Conversation>() select t).ToList();

			var conversations = new List<Conversation>();
			foreach (Conversation c in convs)
			{
				if (c.person_id == Session.activePerson.id)
				{
					conversations.Add(c);
				}
			}

			return stored;
		}

		/// <summary>
		/// Gets the conversation.
		/// </summary>
		/// <returns>The conversation.</returns>
		/// <param name="id">Identifier.</param>
		public Conversation GetConversation(int id)
		{
			return _connection.Table<Conversation>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the conversation.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void DeleteConversation(int id)
		{
			_connection.Delete<Conversation>(id);
		}

		/// <summary>
		/// Adds the article.
		/// </summary>
		/// <returns>The article.</returns>
		/// <param name="a">The article</param>
		public void AddConversation(Conversation c)
		{
			_connection.Insert(c);
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

