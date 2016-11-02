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
	/// Database controller for the Tag model.
	/// </summary>
	public class TagDB
	{
		private SQLiteConnection _connection;
		private string dbPath;

		/// <summary>
		/// Create a connection to the Tag database.
		/// </summary>
		public TagDB()
		{
			Debug.WriteLine("TagDB: Establishing DB connection");
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Session.DB);
		}

		/// <summary>
		/// Open the database.
		/// </summary>
		public void Open()
		{
			_connection = new SQLiteConnection(dbPath);
			_connection.CreateTable<Tag>();
		}

		/// <summary>
		/// Gets the tags.
		/// </summary>
		/// <returns>The tags.</returns>
		public IEnumerable<Tag> GetTags()
		{
			return (from t in _connection.Table<Tag>() select t).ToList();
		}

		/// <summary>
		/// Gets tag by ID
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="id">Identifier.</param>
		public Tag GetTag(int id)
		{
			return _connection.Table<Tag>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes tag by ID
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteTag(int id)
		{
			_connection.Delete<Tag>(id);
		}

		/// <summary>
		/// Adds the tag.
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="tag">Tag.</param>
		public void AddTag(Tag tag)
		{
			_connection.Insert(tag);
		}

		/// <summary>
		/// Gets the tag by text.
		/// </summary>
		/// <returns>The tag by text.</returns>
		/// <param name="text">Text.</param>
		public Tag GetTagByText(string text)
		{

			var stored = _connection.Query<Tag>("SELECT * FROM Tag WHERE Text = ?", text);
			if (stored != null && stored.Count < 2)
			{
				return new Tag(stored[0].id, stored[0].text);
			}
			return null;
		}

		/// <summary>
		/// Close the database connection.
		/// </summary>
		public void Close()
		{
			_connection.Close();
		}
	}
}