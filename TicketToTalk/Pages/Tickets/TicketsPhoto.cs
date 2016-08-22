using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets photo.
	/// </summary>
	public class TicketsPhoto : ContentPage
	{

		public static ObservableCollection<Ticket> photoTickets = new ObservableCollection<Ticket>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsPhoto"/> class.
		/// </summary>
		public TicketsPhoto()
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}


