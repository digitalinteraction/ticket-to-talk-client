using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Container for ticket tabs.
	/// </summary>
	public class ViewTickets : TabbedPage
	{

		public static List<Ticket> tickets = new List<Ticket>();
        public static ObservableCollection<Ticket> ticketVideo = new ObservableCollection<Ticket>();
		public static ObservableCollection<Ticket> ticketPhotos = new ObservableCollection<Ticket>();
		public static ObservableCollection<Ticket> ticketSongs = new ObservableCollection<Ticket>();
		public static bool tutorialShown = false;

		public ViewTickets()
		{
			tickets.Clear();
			ticketVideo.Clear();
			ticketSongs.Clear();
			ticketPhotos.Clear();

			NavigationPage.SetHasNavigationBar(this, false);

			// Set title
			Title = "Tickets";

			var ticketController = new TicketController();

			var task = Task.Run(() => ticketController.GetRemoteTickets());

			try
			{
				task.Wait();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
			}

			tickets = ticketController.GetTickets();

			// Set navigation tabs
			// Set ages
			var nav = new NavigationPage(new TicketsByPeriod(tickets));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Periods";
			nav.Icon = "ic_schedule_white_.png";
			this.Children.Add(nav);

			// Sort tickets into songs.
			foreach (Ticket t in tickets)
			{
				switch (t.mediaType)
				{
					case ("Photo"):
					case ("Picture"):
                        t.displayIcon = "photograph_icon.png";
						ticketPhotos.Add(t);
						break;
					case ("Song"):
					case ("Sound"):
                        t.displayIcon = "audio_icon.png";
						ticketSongs.Add(t);
						break;
					case ("Video"):
					case ("YouTube"):
                        t.displayIcon = "video_icon.png";
						ticketVideo.Add(t);
						break;
				}
			}
			nav = new NavigationPage(new TicketsSounds());
            //nav = new NavigationPage(new DisplayTickets(ticketSongs, "Sounds"));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Sounds";
			nav.Icon = "ic_audiotrack_white_.png";
			this.Children.Add(nav);

            nav = new NavigationPage(new TicketsPicture());
            //nav = new NavigationPage(new DisplayTickets(ticketPhotos, "Pictures"));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Title = "Pictures";
			nav.Icon = "ic_photo_white_.png";
			this.Children.Add(nav);

            // Set areas
            nav = new NavigationPage(new TicketsVideos());
            //nav = new NavigationPage(new DisplayTickets(ticketVideo, "Video"));
			nav.BarBackgroundColor = ProjectResource.color_blue;
			nav.BarTextColor = ProjectResource.color_white;
			nav.Icon = "ic_videocam_white_.png";
			nav.Title = "Video";
			this.Children.Add(nav);
		}

		/// <summary>
		/// Ons the appearing.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("View Tickets");

			if (Session.activeUser.firstLogin && !tutorialShown)
			{

				var text = "Use Tickets to view and add snippets of someone's life through pictures, sounds, and YouTube videos.\n\nPress 'Add' to add a ticket.";

				Navigation.PushModalAsync(new HelpPopup(text, "ticket_icon.png"));
				tutorialShown = true;
			}
		}
	}
}