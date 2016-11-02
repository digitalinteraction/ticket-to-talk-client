using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Model to represent an area.
	/// </summary>
	public class Area : IComparable
	{
		[PrimaryKey]
		public int id 
		{ 
			get; 
			set; 
		}
		public string townCity 
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Initializes a new instance of Area.
		/// </summary>
		public Area()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Area"/> class.
		/// </summary>
		/// <param name="town_City">Town city.</param>
		public Area(string town_City)
		{
			townCity = town_City;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Area"/> class.
		/// </summary>
		/// <param name="iD">I d.</param>
		/// <param name="town_City">Town city.</param>
		public Area(int iD, string town_City)
		{
			id = iD;
			townCity = town_City;
		}

		/// <summary>
		/// Area to string
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[Area: id={0}, townCity={1}]", id, townCity);
		}

		/// <summary>
		/// Compares two Areas.
		/// </summary>
		/// <param name="obj">Object.</param>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
				return false;

			Area a = (Area)obj;
			return (id == a.id) && (townCity.Equals(a.townCity));
		}

		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + townCity.GetHashCode();

			return hash;
		}

		public int CompareTo(object obj)
		{
			var a = obj as Area;
			return string.Compare(townCity, a.townCity, StringComparison.Ordinal);
		}
	}
}

