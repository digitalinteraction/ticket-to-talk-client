using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Article.
	/// </summary>
	public class Article : INotifyPropertyChanged, IComparable
	{
		private string _title;
		private string _link;
		private string _notes;

		[PrimaryKey]
		public int id { get; set;}

		[NotNull]
		public string title 
		{
			get 
			{
				return _title;
			} 
			set 
			{
				if (value != _title) 
				{
					_title = value;
					NotifyPropertyChanged();
				}
			}
		}

		[NotNull]
		public string link 
		{ 
			get 
			{
				return _link;
			} 
			set 
			{
				if (value != _link) 
				{
					_link = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string notes 
		{ 
			get 
			{
				return _notes;
			} 
			set 
			{
				if (value != _notes) 
				{
					_notes = value;
					NotifyPropertyChanged();
				}
			}
		}

		[Ignore]
		public string favicon 
		{
			get;
			set;
		}

		public DateTime created_at { get; set; }

		public DateTime updated_at { get; set; }

		//public int user_id { get; set;}

		[Ignore]
		public string iconFilePath { get; set;}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

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
			return string.Format("[Article: id={0}, title={1}, link={2}, notes={3}, created_at={4}, updated_at={5}]", 
			                     id, title, link, notes, created_at, updated_at);
		}

		/// <summary>
		/// Compare two articles
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="obj">Object.</param>
		public int CompareTo(object obj)
		{
			Article lhs = obj as Article;

			var comp = _title.CompareTo(lhs.title);
			if (comp == 0) 
			{
				comp = _link.CompareTo(lhs.link);
			}

			return comp;
		}
	}
}

