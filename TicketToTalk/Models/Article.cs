using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Article.
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

		//public int user_id { get; set;}

		[Ignore]
		public string iconFilePath { get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Article"/> class.
		/// </summary>
		public Article()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Article"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="title">Title.</param>
		/// <param name="link">Link.</param>
		/// <param name="notes">Notes.</param>
		/// <param name="created_at">Created at.</param>
		/// <param name="updated_at">Updated at.</param>
		/// <param name="iconFilePath">Icon file path.</param>
		public Article(int id, string title, string link, string notes, DateTime created_at, DateTime updated_at, string iconFilePath)
		{
			this.id = id;
			this.title = title;
			this.link = link;
			this.notes = notes;
			this.created_at = created_at;
			this.updated_at = updated_at;
			this.iconFilePath = iconFilePath;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Article"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.Article"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Article: id={0}, title={1}, link={2}, notes={3}, created_at={4}, updated_at={5}]", id, title, link, notes, created_at, updated_at);
		}
	}
}

