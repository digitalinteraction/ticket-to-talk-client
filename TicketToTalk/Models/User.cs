using System;
using SQLite;
namespace TicketToTalk
{
	/// <summary>
	/// Models a User.
	/// </summary>
	public class User
	{
		[PrimaryKey]
		public int id { get; set;}
		public string name { get; set; }
		public string email { get; set; }
		public string password { get; set;}
		public string pathToPhoto { get; set; }
		public DateTime created_at { get; set; }
		public DateTime updated_at { get; set; }
		[Ignore]
		public Pivot pivot { get; set;}

		/// <summary>
		/// Creates a new instance of a user.
		/// </summary>
		public User() { }

		/// <summary>
		/// Creates a new instance of a user.
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="Email">Email.</param>
		public User(int id, string Name, string Email)
		{
			this.id = id;
			this.name = Name;
			this.email = Email;
		}

		/// <summary>
		/// User to string.
		/// </summary>
		/// <returns>The string.</returns>
		public override string ToString()
		{
			return string.Format("[User: id={0}, name={1}, email={2}, password={3}, pathToPhoto={4}, created_at={5}, updated_at={6}, pivot={7}]", id, name, email, password, pathToPhoto, created_at, updated_at, pivot);
		}

		/// <summary>
		/// Class to hold pivot table information from api calls.
		/// </summary>
		public class Pivot 
		{
			public string user_type { get; set; }

			public override string ToString()
			{
				return string.Format("[Pivot: user_type={0}]", user_type);
			}
		}
	}
}