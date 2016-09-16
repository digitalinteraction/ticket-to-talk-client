using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Display ticket info.
	/// </summary>
	public partial class EditTicket : ContentPage
	{
		string[] accessLevels = ProjectResource.groups;

		Entry town_city;
		Entry title;
		Picker access_level;
		Button saveButton;
		Editor description;
		Picker yearPicker;
		Picker period_picker;

		Ticket ticket;

		/// <summary>
		/// Initializes a new instance of the page.
		/// </summary>
		/// <param name="ticket">Ticket: ticket to display</param>
		public EditTicket(Ticket ticket)
		{

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

			this.ticket = ticket;
			Title = "Info";

			Console.WriteLine("Displaying ticket info");

			//var areaController = new AreaController();
			//var area = areaController.getArea(ticket.area_id);

			Label titleLabel = new Label
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
			saveButton.Clicked += saveChanges;

			title = new Entry
			{
				Placeholder = "Add a ticket title",
				Text = ticket.title,
				TextColor = ProjectResource.color_red
			};

			// Description Label
			Label descriptionLabel = new Label
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
			Label areaLabel = new Label
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
			Label yearLabel = new Label
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
			for (int i = Int32.Parse(Session.activePerson.birthYear); i < DateTime.Now.Year; i++)
			{
				yearPicker.Items.Add(i.ToString());
				if (String.Equals(ticket.year, i.ToString()))
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
			var periods = periodController.getAllLocalPeriods();
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
					Debug.WriteLine(j);
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
		void cancel()
		{
			Navigation.PopModalAsync();
		}

		async

		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <returns>The changes.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void saveChanges(object sender, EventArgs e)
		{
			saveButton.IsEnabled = false;

			var periodController = new PeriodController();
			var period = periodController.getAllLocalPeriods()[period_picker.SelectedIndex];

			ticket.title = title.Text;
			ticket.description = description.Text;
			ticket.year = (Int32.Parse(Session.activePerson.birthYear) + yearPicker.SelectedIndex).ToString();

			ticket.access_level = ProjectResource.groups[access_level.SelectedIndex];

			var ticketController = new TicketController();
			Ticket returned = null;
			if (ticket.mediaType.Equals("Picture"))
			{
				ticket.area = town_city.Text.Trim();
			}
			else
			{
				ticket.area = " ";
			}
			returned = await ticketController.updateTicketRemotely(ticket, period.text);

			if (returned != null)
			{
				ticketController.updateTicketLocally(returned);
				ticketController.updateDisplayTicket(returned);

				await Navigation.PopModalAsync();
			}
			else
			{
				await DisplayAlert("Update Ticket", "Ticket could not be updated.", "OK");
				saveButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// On entry text change.
		/// </summary>
		/// <returns>The text changed.</returns>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = (!String.IsNullOrEmpty(title.Text))
				&& (!String.IsNullOrEmpty(description.Text))
				&& (!String.IsNullOrEmpty(town_city.Text))
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
