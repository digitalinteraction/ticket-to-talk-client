using System;
namespace TicketToTalk
{
	/// <summary>
	/// Invitation.
	/// </summary>
	public class Invitation
	{
		public Person person { get; set; }
		public string name { get; set; }
		public string group { get; set; }
		public string pathToPhoto { get; set; }
		public string person_name { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Invitation"/> class.
		/// </summary>
		public Invitation()
		{
		}
	}
}

