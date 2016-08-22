using System;
using SQLite;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TicketToTalk
{
	/// <summary>
	/// Database manager for the article model
	/// </summary>
	public class ArticleDB
	{
		private SQLiteConnection _connection;
		string dbPath;

		/// <summary>
		/// Creates a new connection to the article table.
		/// </summary>
		public ArticleDB()
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
			_connection.CreateTable<Article>();
		}

		/// <summary>
		/// Gets the articles.
		/// </summary>
		/// <returns>The articles.</returns>
		public IEnumerable<Article> GetArticles()
		{
			return (from t in _connection.Table<Article>() select t).ToList();
		}

		/// <summary>
		/// Gets an article by ID.
		/// </summary>
		/// <returns>The article.</returns>
		/// <param name="id">Identifier.</param>
		public Article GetArticle(int id)
		{
			return _connection.Table<Article>().FirstOrDefault(t => t.id == id);
		}

		/// <summary>
		/// Deletes the article by ID.
		/// </summary>
		/// <returns>The article.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteArticle(int id)
		{
			_connection.Delete<Article>(id);
		}

		/// <summary>
		/// Adds the article.
		/// </summary>
		/// <returns>The article.</returns>
		/// <param name="a">The article</param>
		public void AddArticle(Article a)
		{
			_connection.Insert(a);
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

