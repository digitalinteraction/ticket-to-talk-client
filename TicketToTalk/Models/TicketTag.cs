using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Models a ticket_tag relationship
	/// </summary>
	public class TicketTag
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		[NotNull]
		public int ticket_id { get; set; }

		[NotNull]
		public int tag_id { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }

		/// <summary>
		/// Creates a new instance of a ticket tag relationship.
		/// </summary>
		public TicketTag() { }

		/// <summary>
		/// Creates a new instance of a ticket tag relationship.
		/// </summary>
		/// <param name="ticketID">Ticket identifier.</param>
		/// <param name="tagID">Tag identifier.</param>
		public TicketTag(int ticketID, int tagID)
		{
			ticket_id = ticketID;
			tag_id = tagID;
		}

		/// <summary>
		/// Ticket tag relationship to string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[TicketTag: id={0}, ticket_id={1}, tag_id={2}]", id, ticket_id, tag_id);
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.TicketTag"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:TicketToTalk.TicketTag"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:TicketToTalk.TicketTag"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			TicketTag tt = (TicketTag)obj;
			return (id == tt.id) && (ticket_id == tt.ticket_id) && (tag_id == tt.tag_id);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="T:TicketToTalk.TicketTag"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + ticket_id.GetHashCode();
			hash = (hash * 7) + tag_id.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();

			return hash;
		}
	}
}