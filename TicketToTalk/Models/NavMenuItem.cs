using System;
namespace TicketToTalk
{
	/// <summary>
	/// Models a menu item.
	/// </summary>
	public class NavMenuItem
	{
		public string Title { get; set; }

		public string IconSource { get; set; }

		public Type TargetType { get; set; }
	}
}

