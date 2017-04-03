using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets by age page.
	/// </summary>
	public partial class TicketsByAgePage : ContentPage
	{
		ObservableCollection<Ticket> tickets { get; set; } = new ObservableCollection<Ticket>();
		TicketController ticketController = new TicketController();

		/// <summary>
		/// Initializes a new instance of the Tickets by age View.
		/// </summary>
		/// <param name="age">Age: string</param>
		public TicketsByAgePage(string age)
		{
			Padding = new Thickness(20);
			// Split the text to get upper and lower bounds.
			char[] delimiters = { ' ' };
			string[] ageRange = age.Split(delimiters);

			int lowerBound = int.Parse(ageRange[0]);
			int upperBound = int.Parse(ageRange[2]);

			var person = Session.activePerson;

			// Check if ticket year falls within bounds.
			var rawTickets = ticketController.GetTicketsByPerson(person.id);
			foreach (Ticket t in rawTickets)
			{
				int tYear = int.Parse(t.year);
				if (tYear >= lowerBound && tYear <= upperBound)
				{
					switch (t.mediaType)
					{
						case "Photo":
							t.displayIcon = "photograph_icon.png";
							break;
						case "Video":
							t.displayIcon = "video_icon.png";
							break;
						case "Audio":
							t.displayIcon = "audio_icon.png";
							break;
						case "Area":
							t.displayIcon = "area_icon.png";
							break;
					}
					// add to source
					tickets.Add(t);
				}
			}

			// Format image cell
			var cell = new DataTemplate(typeof(ImageCell));
			cell.SetBinding(TextCell.TextProperty, "title");
			cell.SetBinding(TextCell.DetailProperty, new Binding("year"));
			cell.SetBinding(ImageCell.ImageSourceProperty, "displayIcon");
			cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_blue);
			cell.SetValue(TextCell.DetailColorProperty, ProjectResource.color_dark);

			ListView ticketList = new ListView();
			ticketList.ItemsSource = tickets;
			ticketList.ItemTemplate = cell;
			ticketList.SeparatorColor = Color.Transparent;
			ticketList.ItemSelected += OnSelection;

			Content = new StackLayout
			{
				Spacing = 12,
				Children = { ticketList }
			};
		}

		/// <summary>
		/// On listview item selection
		/// </summary>
		/// <returns>The selection.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			var ticket = (Ticket)e.SelectedItem;

			Navigation.PushAsync(new EditTicket(ticket));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}
	}
}

