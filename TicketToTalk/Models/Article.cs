using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Model to represent an Article.
	/// </summary>
	public class Article
	{
		[PrimaryKey]
		public int id { get; set;}

		[NotNull]
		public string title { get; set; }

		[NotNull]
		public string link { get; set; }

		public string notes { get; set; }

		public DateTime created_at { get; set; }

		public DateTime updated_at { get; set; }

		public int user_id { get; set;}

		[Ignore]
		public string iconFilePath { get; set;}

		/// <summary>
		/// Initializes a new instance of an Article.
		/// </summary>
		public Article()
		{
		}

		/// <summary>
		/// Atricle to string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[Article: ID={0}, Title={1}, link={2}, notes={3}]", id, title, link, notes);
		}
	}
}

