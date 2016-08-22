using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Represents a person.
	/// </summary>
	public class Person
	{
		[PrimaryKey]
		public int id { get; set; }
		public int admin_id { get; set; }
		public string name { get; set; }
		public string birthYear { get; set; }
		public string birthPlace { get; set; }
		public string pathToPhoto { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		public string notes { get; set; }
		[Ignore]
		public Pivot pivot { get; set; }
		[Ignore]
		public string relation { get; set; }

		public class Pivot
		{
			public string user_type { get; set; }
			public string relation { get; set; }

			public override string ToString()
			{
				return string.Format("[Pivot: user_type={0}, relation={1}]", user_type, relation);
			}
		}

		/// <summary>
		/// Initializes a new instance of a Person.
		/// </summary>
		public Person()
		{
		}

		/// <summary>
		/// Tos the string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[Person: id={0}, name={1}, birthYear={2}, birthPlace={3}, pathToPhoto={4}, created_at={5}, updated_at={6}, notes={7}, pivot={8}]", id, name, birthYear, birthPlace, pathToPhoto, created_at, updated_at, notes, pivot);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + name.GetHashCode();
			hash = (hash * 7) + birthYear.GetHashCode();
			hash = (hash * 7) + birthPlace.GetHashCode();
			hash = (hash * 7) + created_at.GetHashCode();
			hash = (hash * 7) + updated_at.GetHashCode();
			hash = (hash * 7) + notes.GetHashCode();

			return hash;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
	}
}

