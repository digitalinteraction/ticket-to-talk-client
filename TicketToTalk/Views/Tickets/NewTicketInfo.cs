using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class NewTicketInfo : ContentView
	{
		private Picker access_level;
		private Editor description;
		private Picker period;
		private List<Period> periods;
		private Button saveButton;
		private Entry title;
		private Entry town_city;
		private Picker yearPicker;

		private string mediaType;
		private string filePath;
		private byte[] media;

		public static bool isInTutorial = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewTicketInfo"/> class.
		/// </summary>
		/// <param name="mediaType">Media type.</param>
		/// <param name="filePath">File path.</param>
		public NewTicketInfo(string mediaType, string filePath)
		{

			this.mediaType = mediaType;
			this.filePath = filePath;

			saveButton = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				IsEnabled = false,
				WidthRequest = (Session.ScreenWidth * 0.5),
				Margin = new Thickness(0, 0, 0, 10)
			};
			saveButton.Clicked += UploadTicket;

			// Set UI Elements
			Label titleLabel = new Label
			{
				Text = "Title",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			title = new Entry
			{
				Placeholder = "Add a ticket title",
				TextColor = ProjectResource.color_red
			};
			title.TextChanged += Entry_TextChanged;

			Label descriptionLabel = new Label
			{
				Text = "Description",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			description = new Editor
			{
				Text = "Add a description",
				TextColor = ProjectResource.color_red
			};
			description.TextChanged += Entry_TextChanged;

			Label yearLabel = new Label
			{
				Text = "Year",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};
			yearPicker = new Picker
			{
				Title = "Year",
				TextColor = ProjectResource.color_red
			};
			yearPicker.SelectedIndexChanged += Entry_TextChanged;

			int startYear = Int32.Parse(Session.activePerson.birthYear);
			for (int i = startYear; i < DateTime.Now.Year + 1; i++)
			{
				yearPicker.Items.Add(i.ToString());
			}

			Label area = new Label();
			switch (mediaType)
			{
				case ("Video"):
				case ("Sound"):
				case ("Song"):
					break;
				default:

					area = new Label
					{
						Text = "Location",
						TextColor = ProjectResource.color_dark,
						Margin = new Thickness(0, 10, 0, 2)
					};

					town_city = new Entry
					{
						Placeholder = "Town/City",
						TextColor = ProjectResource.color_red,
						Margin = new Thickness(0, 0, 0, 2)
					};
					town_city.TextChanged += Entry_TextChanged;

					break;
			}

			var periodLabel = new Label
			{
				Text = "What Period is this from?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			period = new Picker
			{
				Title = "What period of their life is this from?",
				TextColor = ProjectResource.color_red
			};

			var periodController = new PeriodController();
			periods = periodController.GetAllLocalPeriods();

			for (int i = 0; i < periods.Count; i++)
			{
				period.Items.Add(periods[i].text);
			}
			period.SelectedIndexChanged += Entry_TextChanged;

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
			foreach (string s in ProjectResource.groups)
			{
				access_level.Items.Add(s);
			}

			var headerStack = new StackLayout
			{
				Padding = new Thickness(10),
				BackgroundColor = ProjectResource.color_red,
				Children =
				{
					new Label
					{
						Text = "Details",
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						TextColor = ProjectResource.color_white,
					}
				}
			};

			StackLayout detailsStack;
			switch (mediaType)
			{
				case ("Picture"):
				case ("Photo"):
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
						periodLabel,
						period,
						area,
						town_city,
						accessLevelLabel,
						access_level,
					}
					};
					break;
				default:
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
						periodLabel,
						period,
						accessLevelLabel,
						access_level,
					}
					};
					break;
			};

			var buttonStack = new StackLayout
			{
				Spacing = 0,
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
					headerStack,
					detailsStack,
					buttonStack
				}
			};

			Content = stack;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewTicketInfo"/> class.
		/// </summary>
		/// <param name="mediaType">Media type.</param>
		/// <param name="media">Media.</param>
		public NewTicketInfo(string mediaType, byte[] media)
		{
			this.media = media;
			this.mediaType = mediaType;

			saveButton = new Button
			{
				Text = "Save",
				TextColor = ProjectResource.color_white,
				BackgroundColor = ProjectResource.color_grey,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				IsEnabled = false,
				WidthRequest = (Session.ScreenWidth * 0.5),
				Margin = new Thickness(0, 0, 0, 10)
			};
			saveButton.Clicked += UploadTicket;

			// Set UI Elements
			Label titleLabel = new Label
			{
				Text = "Title",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			title = new Entry
			{
				Placeholder = "Add a ticket title",
				TextColor = ProjectResource.color_red
			};
			title.TextChanged += Entry_TextChanged;

			Label descriptionLabel = new Label
			{
				Text = "Description",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			description = new Editor
			{
				Text = "Add a description",
				TextColor = ProjectResource.color_red
			};
			description.TextChanged += Entry_TextChanged;

			Label yearLabel = new Label
			{
				Text = "Year",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};
			yearPicker = new Picker
			{
				Title = "Year",
				TextColor = ProjectResource.color_red
			};
			yearPicker.SelectedIndexChanged += Entry_TextChanged;

			int startYear = Int32.Parse(Session.activePerson.birthYear);
			for (int i = startYear; i < DateTime.Now.Year + 1; i++)
			{
				yearPicker.Items.Add(i.ToString());
			}

			Label area = new Label();
			switch (mediaType)
			{
				case ("Video"):
				case ("Sound"):
				case ("Song"):
					break;
				default:

					area = new Label
					{
						Text = "Location",
						TextColor = ProjectResource.color_dark,
						Margin = new Thickness(0, 10, 0, 2)
					};

					town_city = new Entry
					{
						Placeholder = "Town/City",
						TextColor = ProjectResource.color_red,
						Margin = new Thickness(0, 0, 0, 2)
					};
					town_city.TextChanged += Entry_TextChanged;

					break;
			}

			var periodLabel = new Label
			{
				Text = "What Period is this from?",
				TextColor = ProjectResource.color_dark,
				Margin = new Thickness(0, 10, 0, 2)
			};

			period = new Picker
			{
				Title = "What period of their life is this from?",
				TextColor = ProjectResource.color_red
			};

			var periodController = new PeriodController();
			periods = periodController.GetAllLocalPeriods();

			for (int i = 0; i < periods.Count; i++)
			{
				period.Items.Add(periods[i].text);
			}
			period.SelectedIndexChanged += Entry_TextChanged;

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
			foreach (string s in ProjectResource.groups)
			{
				access_level.Items.Add(s);
			}

			var headerStack = new StackLayout
			{
				Padding = new Thickness(10),
				BackgroundColor = ProjectResource.color_red,
				Children =
				{
					new Label
					{
						Text = "Details",
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						TextColor = ProjectResource.color_white,
					}
				}
			};

			StackLayout detailsStack;
			switch (mediaType)
			{
				case ("Picture"):
				case ("Photo"):
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
						periodLabel,
						period,
						area,
						town_city,
						accessLevelLabel,
						access_level,
					}
					};
					break;
				default:
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
						periodLabel,
						period,
						accessLevelLabel,
						access_level,
					}
					};
					break;
			};


			var buttonStack = new StackLayout
			{
				Spacing = 0,
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
					headerStack,
					detailsStack,
					buttonStack
				}
			};

			Content = stack;
		}

		/// <summary>
		/// Uploads the ticket.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private async void UploadTicket(object sender, EventArgs e)
		{

			saveButton.IsEnabled = false;

			var year = int.Parse(Session.activePerson.birthYear) + yearPicker.SelectedIndex;
			var ticket = new Ticket
			{
				title = title.Text,
				description = description.Text,
				mediaType = mediaType,
				year = year.ToString(),
				pathToFile = filePath,
				access_level = ProjectResource.groups[access_level.SelectedIndex],
				person_id = Session.activePerson.id,
			};

			//var area = new Area();
			if (mediaType.Equals("Picture"))
			{
				ticket.area = town_city.Text.Trim();
			}
			else
			{
				ticket.area = " ";
			}

			if (ticket.mediaType.Equals("YouTube"))
			{
				media = null;
			}
			else
			{
				if (media == null)
				{
					media = MediaController.ReadBytesFromFile(filePath);
				}
			}

			var selected_period = periods[period.SelectedIndex];

			var ticketController = new TicketController();
			var returned_ticket = await ticketController.AddTicketRemotely(ticket, media, selected_period);
			if (returned_ticket != null)
			{
				MessagingCenter.Send<NewTicketInfo, Ticket>(this, "ticket_added", returned_ticket);

				if (isInTutorial)
				{
					isInTutorial = false;
					Application.Current.MainPage = new FinishTutorialPage();
				}
				else
				{
					await Navigation.PopModalAsync();
				}
			}

			else
			{
				saveButton.IsEnabled = true;
			}
		}

		/// <summary>
		/// Entries the text changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Entry_TextChanged(object sender, EventArgs e)
		{
			var entriesNotNull = false;

			if (mediaType.Equals("Picture"))
			{
				entriesNotNull = (!string.IsNullOrEmpty(title.Text))
				&& (!string.IsNullOrEmpty(description.Text))
				&& (!string.IsNullOrEmpty(town_city.Text))
				&& (yearPicker.SelectedIndex != -1)
				&& (period.SelectedIndex != -1)
				&& (access_level.SelectedIndex != -1);
			}
			else
			{
				entriesNotNull = (!string.IsNullOrEmpty(title.Text))
				&& (!string.IsNullOrEmpty(description.Text))
				&& (yearPicker.SelectedIndex != -1)
				&& (period.SelectedIndex != -1)
				&& (access_level.SelectedIndex != -1);
			}

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
