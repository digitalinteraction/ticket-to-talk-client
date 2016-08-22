using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Container for ticket tabs.
	/// </summary>
	public class ViewTickets : TabbedPage
	{
		//TODO: Remove
		//public string test { get; set; }

		public List<Ticket> tickets = new List<Ticket>();

		public ViewTickets()
		{
			NavigationPage.SetHasNavigationBar(this, false);

			// Set title
			this.Title = "Tickets";

			var ticketController = new TicketController();
			Task.Run(() => ticketController.updateTicketsFromAPI()).Wait();

			Debug.WriteLine("ViewTickets: Got tickets from API");

			tickets = ticketController.getTickets();
			Debug.WriteLine("ViewTickets: Got tickets from local storage");

			foreach (Ticket t in tickets) 
			{
				Debug.WriteLine("ViewTickets: " + t);
			}

			// Set navigation tabs
			// Set ages
			var nav = new NavigationPage(new TicketsByPeriod(tickets));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Periods";
			nav.Icon = "ic_schedule_white_.png";
			this.Children.Add(nav);

			Debug.WriteLine("Set periods page");

			// Sort tickets into songs.
			List<Ticket> ticketSongs = new List<Ticket>();
			List<Ticket> ticketPhotos = new List<Ticket>();
			List<Ticket> ticketVideo = new List<Ticket>();
			foreach (Ticket t in tickets)
			{
				switch (t.mediaType)
				{
					case ("Photo"):
					case ("Picture"):
						ticketPhotos.Add(t);
						break;
					case ("Song"):
					case ("Sound"):
						ticketSongs.Add(t);
						break;
					case ("Video"):
						ticketVideo.Add(t);
						break;
				}
			}
			nav = new NavigationPage(new TicketsSounds(ticketSongs));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Sounds";
			nav.Icon = "ic_audiotrack_white_.png";
			this.Children.Add(nav);

			nav = new NavigationPage(new TicketsPicture(ticketPhotos));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Pictures";
			nav.Icon = "ic_photo_white_.png";
			this.Children.Add(nav);

			// Set areas
			nav = new NavigationPage(new TicketsVideos(ticketVideo));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Icon = "ic_videocam_white_.png";
			nav.Title = "Video";
			this.Children.Add(nav);

		}

		/// <summary>
		/// Launch view to add a new ticket.
		/// </summary>
		/// <returns>The ticket.</returns>
		//public void newTicket()
		//{
		//	Navigation.PushAsync(new NewTicket());
		//}
	}
}