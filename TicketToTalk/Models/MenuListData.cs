using System;
using System.Collections.Generic;
using TicketToTalk;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Holds the data for the menu list.
	/// </summary>
	public class MenuListData : List<NavMenuItem>
	{
		public MenuListData()
		{
			this.Add(new NavMenuItem()
			{
				Title = "Tickets",
				IconSource = "red_ticket_icon.png",
				TargetType = typeof(ViewTickets),
			});

			this.Add(new NavMenuItem()
			{
				Title = "Inspiration",
				IconSource = "light_icon.png",
				TargetType = typeof(InspirationView)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Conversations",
				IconSource = "chat_icon.png",
				TargetType = typeof(ConversationsView)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Useful Information",
				IconSource = "file_icon.png",
				TargetType = typeof(AllArticles)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Profiles",
				IconSource = "face_icon.png",
				TargetType = typeof(AllProfiles)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Change Person",
				IconSource = "face_icon.png",
				TargetType = typeof(SelectActivePerson)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Help",
				IconSource = "help_icon.png",
				TargetType = typeof(Help)
			});

			this.Add(new NavMenuItem()
			{
				Title = "About",
				IconSource = "help_icon.png",
				TargetType = typeof(About)
			});

			this.Add(new NavMenuItem()
			{
				Title = "Logout",
				TargetType = typeof(Logout),
				IconSource = "logout_icon.png"
			});
		}
	}
}

