using System;
using SQLite;

namespace TicketToTalk
{
	/// <summary>
	/// Period Model
	/// </summary>
	public class Period
	{
		[PrimaryKey]
		public int id { get; set; }
		public string text { get; set; }
		[Ignore]
		public int ticket_count { get; set; }
		[Ignore]
		public Pivot pivot { get; set; }

		/// <summary>
		/// Pivot.
		/// </summary>
		public class Pivot 
		{
			public string person_id { get; set; }
			public string period_id { get; set; }

			public override string ToString()
			{
				return string.Format("[Pivot: person_id={0}, period_id={1}]", person_id, period_id);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Period"/> class.
		/// </summary>
		public Period()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Period"/> class.
		/// </summary>
		/// <param name="text">Text.</param>
		public Period(string text)
		{
			this.text = text;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Period"/> class.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="text">Text.</param>
		public Period(int id, string text)
		{
			this.id = id;
			this.text = text;
		}

		/// <summary>
		/// Compare two periods.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			Tag t = (Tag)obj;
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

			return hash;
		}

		public override string ToString()
		{
			return string.Format("[Period: id={0}, text={1}, ticket_count={2}, pivot={3}]", id, text, ticket_count, pivot);
		}
	}
}

