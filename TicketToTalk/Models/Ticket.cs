using System;
using System.Collections.Generic;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Models a ticket.
	/// </summary>
	public class Ticket
	{

		[PrimaryKey]
		public int id { get; set; }
		public string title { get; set; }
		public string description { get; set; }
		public string mediaType { get; set; }
		public string year { get; set; }
		public string pathToFile { get; set; }
		[Ignore]
		public string displayIcon { get; set; }
		public string access_level { get; set; }
		public int area_id { get; set; }
		public int person_id { get; set;}
		public int period_id { get; set;}
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		[Ignore]
		public Tag[] tags { get; set; }

		/// <summary>
		/// Creates a new instance of a ticket.
		/// </summary>
		public Ticket()
		{
		}

		/// <summary>
		/// Creates a new instance of a ticket.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="mediaType">Media type.</param>
		/// <param name="dateAdded">Date added.</param>
		/// <param name="year">Year.</param>
		public Ticket(string title, string mediaType, DateTime dateAdded, string year)
		{
			this.title = title;
			this.mediaType = mediaType;
			this.created_at = dateAdded;
			this.year = year;
		}

		public override string ToString()
		{
			return string.Format("[Ticket: id={0}, title={1}, description={2}, mediaType={3}, year={4}, " +
			                     "pathToFile={5}, displayIcon={6}, access_level={7}, area_id={8}, person_id={9}, " +
			                     "period_id={10}, created_at={11}, updated_at={12}, tags={13}]", 
			                     id, title, description, mediaType, year, pathToFile, displayIcon, access_level, 
			                     area_id, person_id, period_id, created_at, updated_at, tags);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + title.GetHashCode();
			hash = (hash * 7) + description.GetHashCode();
			hash = (hash * 7) + mediaType.GetHashCode();
			hash = (hash * 7) + access_level.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();
			hash = (hash * 7) + updated_at.GetHashCode();

			return hash;
		}
	}
}

