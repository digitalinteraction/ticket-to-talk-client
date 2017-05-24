using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Display ticket info.
	/// </summary>
	public class EditTicket : TrackedContentPage
	{
		private string[] accessLevels = ProjectResource.groups;

		private Entry town_city;
		private Entry title;
		private Picker access_level;
		private Button saveButton;
		private Editor description;
		private Picker yearPicker;
		private Picker period_picker;

		private Ticket ticket;

		/// <summary>
		/// Initializes a new instance of the page.
		/// </summary>
		/// <param name="ticket">Ticket: ticket to display</param>
		public EditTicket(Ticket ticket)
		{

            TrackedName = "Edit Ticket";

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(Cancel)
			});

			this.ticket = ticket;
			Title = "Info";

			var titleLabel = new Label
			{
				Text = "Title",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			saveButton = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				IsEnabled = false,
				WidthRequest = Session.ScreenWidth * 0.5,
				Margin = new Thickness(0, 0, 0, 10)
			};
			saveButton.Clicked += SaveChanges;

			title = new Entry
			{
				Placeholder = "Add a ticket title",
				Text = ticket.title,
				TextColor = ProjectResource.color_red
			};

			// Description Label
			var descriptionLabel = new Label
			{
				Text = "Description",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			// Description Entry
			description = new Editor
			{
				Text = ticket.description,
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 2)
			};

			// Area label
			var areaLabel = new Label
			{
				Text = "Area",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			// Area entry
			town_city = new Entry
			{
				//Text = area.townCity,
				Text = ticket.area,
				Placeholder = "Town/City",
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 5)
			};

			// Year label
			var yearLabel = new Label
			{
				Text = "Year",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			// Year Picker
			yearPicker = new Picker
			{
				Title = "Year",
				TextColor = ProjectResource.color_red,
				Margin = new Thickness(0, 0, 0, 2)
			};
			int yearIndex = 0;

			// Add years to picker
			for (int i = int.Parse(Session.activePerson.birthYear); i < DateTime.Now.Year; i++)
			{
				yearPicker.Items.Add(i.ToString());
				if (string.Equals(ticket.year, i.ToString()))
				{
					yearIndex = i;
				}
				else
				{
					yearIndex = 0;
				}
			}
			yearPicker.SelectedIndex = yearIndex;

			var periodLabel = new Label
			{
				Text = "Period",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			period_picker = new Picker
			{
				Title = "Who can see this?",
				TextColor = ProjectResource.color_red
			};
			period_picker.SelectedIndexChanged += Entry_TextChanged;

			var accessLevelLabel = new Label
			{
				Text = "Who Can See This?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			access_level = new Picker
			{
				Title = "Who can see this?",
				TextColor = ProjectResource.color_red
			};
			access_level.SelectedIndexChanged += Entry_TextChanged;

			var periodController = new PeriodController();
			var periods = periodController.GetAllLocalPeriods();
			int j = 0;
			foreach (Period p in periods)
			{
				period_picker.Items.Add(p.text);
				if (ticket.period_id == p.id)
				{
					period_picker.SelectedIndex = j;
				}
				j++;
			}

			j = 0;
			foreach (string s in accessLevels)
			{
				access_level.Items.Add(s);
				if (string.Compare(ticket.access_level, s, StringComparison.Ordinal) == 0)
				{
					access_level.SelectedIndex = j;
				}
				j++;
			}

			Console.WriteLine("Getting tags.");

			StackLayout detailsStack = null;

			if (ticket.mediaType.Equals("Picture"))
			{
				detailsStack = new StackLayout
				{
					Padding = new Thickness(20, 10, 20, 20),
					Spacing = 0,
					Children =
					{
						titleLabel,
						title,
						descriptionLabel,
						description,
						yearLabel,
						yearPicker,
						areaLabel,
						town_city,
						periodLabel,
						period_picker,
						accessLevelLabel,
						access_level,
					}
				};
			}
			else
			{
				detailsStack = new StackLayout
				{
					Padding = new Thickness(20, 10, 20, 20),
					Spacing = 0,
					Children =
					{
						titleLabel,
						title,
						descriptionLabel,
						description,
						yearLabel,
						yearPicker,
						areaLabel,
						periodLabel,
						period_picker,
						accessLevelLabel,
						access_level,
					}
				};
			}

			var buttonStack = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Children =
				{
					saveButton
				}
			};


			var stack = new StackLayout
			{
				Spacing = 0,
				Children =
				{
					detailsStack,
					buttonStack
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		private void Cancel()
		{
			Navigation.PopModalAsync();
		}

		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <returns>The changes.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void SaveChanges(object sender, EventArgs e)
		{
			saveButton.IsEnabled = false;

			var periodController = new PeriodController();
			var period = periodController.GetAllLocalPeriods()[period_picker.SelectedIndex];

			ticket.title = title.Text;
			ticket.description = description.Text;
			ticket.year = (int.Parse(Session.activePerson.birthYear) + yearPicker.SelectedIndex).ToString();

			ticket.access_level = ProjectResource.groups[access_level.SelectedIndex];

			var ticketController = new TicketController();

			if (ticket.mediaType.Equals("Picture"))
			{
				ticket.area = town_city.Text.Trim();
			}
			else
			{
				ticket.area = " ";
			}

			Ticket returned = null;

			try
			{
				returned = await ticketController.UpdateTicketRemotely(ticket, period.text);

				if (returned != null)
				{
					ticketController.UpdateTicketLocally(returned);
					ticketController.UpdateDisplayTicket(returned);

					await Navigation.PopModalAsync();
				}
				else
				{
					await DisplayAlert("Update Ticket", "Ticket could not be updated.", "OK");
					saveButton.IsEnabled = true;
				}
			}
			catch (NoNetworkException ex)
			{
				await DisplayAlert("No Network", ex.Message, "Dismiss");
				saveButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// On entry text change.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!string.IsNullOrEmpty(title.Text))
				&& (!string.IsNullOrEmpty(description.Text))
				&& (!string.IsNullOrEmpty(town_city.Text))
				&& (yearPicker.SelectedIndex != -1)
				&& (period_picker.SelectedIndex != -1)
				&& (access_level.SelectedIndex != -1);

			if (entriesNotNull)
			{
				saveButton.BackgroundColor = ProjectResource.color_blue;
				saveButton.IsEnabled = true;
			}
			else
			{
				saveButton.BackgroundColor = ProjectResource.color_grey;
				saveButton.IsEnabled = false;
			}
		}
	}
}
