using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Models a ticket area relationship.
	/// </summary>
	public class TicketArea
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }

		[NotNull]
		public int ticket_id { get; set; }

		[NotNull]
		public int area_id { get; set; }

		/// <summary>
		/// Creates a new instance of a ticket_area
		/// </summary>
		public TicketArea()
		{
		}

		/// <summary>
		/// Creates a new instance of a ticket area.
		/// </summary>
		/// <param name="ticketID">Ticket identifier.</param>
		/// <param name="areaID">Area identifier.</param>
		public TicketArea(int ticketID, int areaID)
		{
			ticket_id = ticketID;
			area_id = areaID;
		}

		/// <summary>
		/// Ticket area to string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[TicketArea: ID={0}, TicketID={1}, AreaID={2}]", id, ticket_id, area_id);
		}

		/// <summary>
		/// Compares two ticket area relationships.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			TicketArea ta = (TicketArea)obj;
			return (id == ta.id) && (ticket_id == ta.ticket_id) && (area_id == ta.area_id);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="T:TicketToTalk.TicketArea"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}

