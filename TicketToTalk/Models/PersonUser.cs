using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Person user relationship.
	/// </summary>
	public class PersonUser
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public int person_id { get; set; }
		public int user_id { get; set; }
		public string user_type { get; set; }
		public string relationship { get; set; }

		/// <summary>
		/// Initializes a new instance of the PersonUser class.
		/// </summary>
		public PersonUser()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ticket_to_Talk.PersonUser"/> class.
		/// </summary>
		/// <param name="person_id">Person identifier.</param>
		/// <param name="user_id">User identifier.</param>
		/// <param name="user_type">User type.</param>
		/// <param name="relationship">Relationship.</param>
		public PersonUser(int person_id, int user_id, string user_type, string relationship)
		{
			this.person_id = person_id;
			this.user_id = user_id;
			this.user_type = user_type;
			this.relationship = relationship;
		}

		/// <summary>
		/// Tos the string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[PersonUser: person_id={0}, user_id={1}, user_type={2}, relationship={3}]", person_id, user_id, user_type, relationship);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			int hash = 13;
			hash = (hash * 7) + id.GetHashCode();
			hash = (hash * 7) + person_id.GetHashCode();
			hash = (hash * 7) + user_id.GetHashCode();
			hash = (hash * 7) + user_type.GetHashCode();
			hash = (hash * 7) + relationship.GetHashCode();

			return hash;
		}
	}
}

