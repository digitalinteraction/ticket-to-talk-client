﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class NewSoundTicket : ContentPage
	{
		string fileName;

		List<Tag> selectTags = new List<Tag>();

		Entry title;
		Editor description;
		Picker yearPicker;
		Entry town_city;
		Picker access_level;

		string[] accessLevels = ProjectResource.groups;
		Button saveButton;
		Picker period;
		List<Period> periods;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.NewPhotoTicket"/> class.
		/// </summary>
		/// <param name="file">File.</param>
		public NewSoundTicket(string fileName)
		{
			this.fileName = fileName;
			// Wait for tags to be sent from modal activity.
			//MessagingCenter.Subscribe<EditTags, List<Tag>>(this, "selected_tags", (sender, tags) =>
			//{
			//	Debug.WriteLine("These tags have been selected:");
			//	foreach (Tag t in tags)
			//	{
			//		Debug.WriteLine(t);
			//	}

			//	selectTags = tags;
			//});

			Title = "New Ticket";

			// Set save button
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "Cancel",
				Order = ToolbarItemOrder.Primary,
				Command = new Command(cancel)
			});

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
			saveButton.Clicked += testSave;

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

			Label area = new Label
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

			// Add years to picker from 1890 to now.
			int startYear = Int32.Parse(Session.activePerson.birthYear);
			for (int i = startYear; i < DateTime.Now.Year; i++)
			{
				yearPicker.Items.Add(i.ToString());
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
			periods = periodController.getAllLocalPeriods();

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
			foreach (string s in accessLevels)
			{
				access_level.Items.Add(s);
			}

			//Button editTags = new Button
			//{
			//	Text = "Edit Tags",
			//	TextColor = ProjectResource.color_white,
			//	BackgroundColor = ProjectResource.color_dark,
			//	HorizontalOptions = LayoutOptions.Fill,
			//	BorderRadius = 0
			//};
			//editTags.Clicked += launchEditTagsView;

			//TagDB tagDB = new TagDB();
			var tagController = new TagController();
			foreach (Tag t in tagController.getTags())
			{
				var cell = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
				};

				var text = new Label
				{
					Text = t.text,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};

				var toggle = new ToggleSwitch
				{
					HorizontalOptions = LayoutOptions.End,
					tag = t
				};
				toggle.Toggled += tagToggleEvent;

				cell.Children.Add(text);
				cell.Children.Add(toggle);
				//toggles.Children.Add(cell);
			}
			//tagDB.close();

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

			var detailsStack = new StackLayout
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
					area,
					town_city,
					periodLabel,
					period,
					accessLevelLabel,
					access_level,
				}
			};

			//var EditTagsStack = new StackLayout
			//{
			//	Spacing = 0,
			//	BackgroundColor = ProjectResource.color_blue,
			//	Children =
			//	{
			//		editTags
			//	}
			//};

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
					new AudioPlayerLayout(new Ticket{ pathToFile = fileName }),
					headerStack,
					detailsStack,
					//EditTagsStack,
					buttonStack
				}
			};

			Content = new ScrollView
			{
				Content = stack
			};
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
				&& (access_level.SelectedIndex != -1)
				&& (period.SelectedIndex != -1);

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

		public async void testSave(object sender, EventArgs e)
		{
			// Check pickers are selected.
			if (yearPicker.SelectedIndex == -1)
			{
				cancel();
			}
			var year = Int32.Parse(Session.activePerson.birthYear) + yearPicker.SelectedIndex;

			//List<Tag> tags = new List<Tag>();
			//Debug.WriteLine("Printing tags to add");
			foreach (Tag t in selectTags)
			{
				Debug.WriteLine("Tag in tags to add" + t);
			}

			Ticket ticket = new Ticket
			{
				title = title.Text,
				description = description.Text,
				mediaType = "Song",
				year = year.ToString(),
				pathToFile = fileName,
				access_level = accessLevels[access_level.SelectedIndex],
				person_id = Session.activePerson.id
			};

			Area area = new Area
			{
				townCity = town_city.Text,
			};

			var selected_period = periods[period.SelectedIndex];

			NetworkController net = new NetworkController();
			IDictionary<string, Object> parameters = new Dictionary<string, Object>();
			parameters["token"] = Session.Token.val;
			parameters["tags"] = selectTags.ToArray();
			parameters["ticket"] = ticket;
			parameters["area"] = area;
			parameters["audio"] = MediaController.readBytesFromFile(fileName);
			parameters["period"] = selected_period;

			var jobject = await net.sendGenericPostRequest("tickets/store", parameters);
			Debug.WriteLine(jobject);

			var jtoken = jobject.GetValue("ticket");
			var returned_ticket = jtoken.ToObject<Ticket>();

			returned_ticket.pathToFile = fileName;
			var ticketController = new TicketController();
			returned_ticket.displayIcon = "audio_icon.png";

			ticketController.addTicketLocally(returned_ticket);
			TicketsSounds.soundTickets.Add(returned_ticket);
			TicketsByPeriod.addTicket(returned_ticket);

			Debug.WriteLine("Saving area.");
			jtoken = jobject.GetValue("area");
			var returned_area = jtoken.ToObject<Area>();

			var areaController = new AreaController();
			var stored_area = areaController.getArea(returned_area.id);

			if (stored_area == null)
			{
				areaController.addAreaLocally(returned_area);
			}

			ticketController.addTagRelationsLocally(selectTags, returned_ticket);

			//MessagingCenter.Unsubscribe<EditTags, List<Tag>>(this, "selected_tags");
			await Navigation.PopAsync();
		}

		/// <summary>
		/// Cancel this instance.
		/// </summary>
		void cancel()
		{
			//MessagingCenter.Unsubscribe<EditTags, List<Tag>>(this, "selected_tags");
			Navigation.PopAsync();
		}

		/// <summary>
		/// On a tag toggle, edit the relationship
		/// </summary>
		/// <returns>The toggle event.</returns>
		/// <param name="sender">Sender: Toggle</param>
		/// <param name="ea">Ea: EventArgs</param>
		public void tagToggleEvent(Object sender, EventArgs ea)
		{
			var toggle = (ToggleSwitch)sender;
			Tag t = toggle.tag;

			if (toggle.IsToggled)
			{
				selectTags.Add(t);
			}
			else
			{
				selectTags.Remove(t);
			}
		}

		//public async void launchEditTagsView(Object sender, EventArgs e)
		//{
		//	var ticket_tags = new List<Tag>();
		//	EditTags editTagsPage = new EditTags(ticket_tags);
		//	await Navigation.PushModalAsync(editTagsPage);
		//}
	}
}

