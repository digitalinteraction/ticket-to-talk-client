using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Models a tag.
	/// </summary>
	public class Tag
	{
		[PrimaryKey]
		public int id { get; set;}
		public string text { get; set;}
		public string created_at { get; set;}
		public string updated_at { get; set;}
		/// <summary>
		/// Initialises a new instance of a Tag.
		/// </summary>
		public Tag()
		{
		}

		/// <summary>
		/// Initializes a new instance of a tag.
		/// </summary>
		/// <param name="text">Text.</param>
		public Tag(string text)
		{
			this.text = text;
		}

		/// <summary>
		/// Initializes a new instance of a tag.
		/// </summary>
		/// <param name="ID">Identifier.</param>
		/// <param name="text">Text.</param>
		public Tag(int ID, string text) 
		{
			this.id = ID;
			this.text = text;
		}

		/// <summary>
		/// Tag to string
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[Tag: id={0}, text={1}, created_at={2}, updated_at={3}]", id, text, created_at, updated_at);
		}

		/// <summary>
		/// Compare two tags.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			Tag t = (Tag) obj;
			return (id == t.id) && (text.Equals(t.text));
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + text.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();
			hash = (hash * 7) + updated_at.GetHashCode();

			return hash;
		}
	}
}

