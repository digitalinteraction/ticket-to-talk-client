using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Tickets picture.
	/// </summary>
	public class TicketsPicture : TrackedContentPage
	{
		private ProgressSpinner indicator;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.TicketsPicture"/> class.
		/// </summary>
		public TicketsPicture()
		{

            TrackedName = "Photo Tickets";

            indicator = new ProgressSpinner(this, ProjectResource.color_white, ProjectResource.color_dark);

			// Set Padding
			Padding = new Thickness(20);
			Title = "Pictures";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Add",
				Icon = "add_icon.png",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(LaunchNewTicketView)
			});

			BackgroundColor = ProjectResource.color_white;

			var ticketsListView = new ListView();

			// Set display icons
			foreach (Ticket t in ViewTickets.ticketPhotos)
			{
				switch (t.mediaType)
				{
					case "Photo":
						t.displayIcon = "photo_icon.png";
						break;
					case "Picture":
						t.displayIcon = "photo_icon.png";
						break;
				}
                t.imageSource = ImageSource.FromFile(t.displayIcon);
			}

			// Format image cell
			var cell = new DataTemplate(typeof(StyledTicketCell));

			ticketsListView.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			ticketsListView.BindingContext = ViewTickets.ticketPhotos;
			ticketsListView.ItemTemplate = cell;
			ticketsListView.ItemSelected += OnSelection;
			ticketsListView.SeparatorColor = Color.Transparent;
            ticketsListView.HasUnevenRows = true;

			var stack = new StackLayout
			{
				Spacing = 12,
				Children =
				{
					ticketsListView
				}
			};

			var layout = new AbsoluteLayout();

			AbsoluteLayout.SetLayoutBounds(stack, new Rectangle(0.5, 0.5, 1, 1));
			AbsoluteLayout.SetLayoutFlags(stack, AbsoluteLayoutFlags.All);

			layout.Children.Add(stack);
			layout.Children.Add(indicator);

			Content = layout;
		}

		/// <summary>
		/// On ticket selection, launch the view to see the ticket.
		/// </summary>
		/// <returns>The selection.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void OnSelection(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem == null)
			{
				return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
			}

			Ticket ticket = (Ticket)e.SelectedItem;
			ToolbarItems.Clear();

			IsBusy = true;

			var nav = new ViewTicket(ticket);
			var ready = await nav.SetUpTicketForDisplay();

			if (ready)
			{
				IsBusy = false;

				await Navigation.PushAsync(nav);
			}
			else
			{
				IsBusy = false;

				await DisplayAlert("No Network", "Ticket could not be downloaded", "OK");
			}

			//Navigation.PushAsync(new ViewTicket(ticket));
			((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
		}

		/// <summary>
		/// Launchs the new ticket view.
		/// </summary>
		/// <returns>The new ticket view.</returns>
		public void LaunchNewTicketView()
		{
			var nav = new NavigationPage(new SelectNewTicketType());
			nav.BarTextColor = ProjectResource.color_white;
			nav.BarBackgroundColor = ProjectResource.color_blue;

			Navigation.PushModalAsync(nav);
		}
	}
}


