using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
    public class NewTicketInfo : ContentView
    {
        Picker access_level;
        Editor description;
        Picker period;
        List<Period> periods;
        Button saveButton;
        Entry title;
        Entry town_city;
        Picker yearPicker;
        
        string mediaType;
        string filePath;
        byte[] media;
        public NewTicketInfo(string mediaType, string filePath)
        {
            Debug.WriteLine("NewTicketInfo: constructor");
            
            this.mediaType = mediaType;
            this.filePath = filePath;
            
            Debug.WriteLine("NewTicketInfo: Setting new ticket info");
            
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
            saveButton.Clicked += uploadTicket;
            
            Debug.WriteLine("NewTicketInfo: Set save button");
            
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
            for (int i = startYear; i < DateTime.Now.Year; i++)
            {
                yearPicker.Items.Add(i.ToString());
            }
            
            Debug.WriteLine("NewTicketInfo: Set shared fields");
            
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
            };
            
            Debug.WriteLine("NewTicketInfo: Set location info");
            
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
            foreach (string s in ProjectResource.groups)
            {
                access_level.Items.Add(s);
            }
            
            Debug.WriteLine("NewTicketInfo: Set access control");
            
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
            
            Debug.WriteLine("NewTicketInfo: Set stack");
        }

		public NewTicketInfo(string mediaType, byte[] media)
		{
			this.media = media;
			Debug.WriteLine("NewTicketInfo: constructor");

			this.mediaType = mediaType;
			this.filePath = filePath;

			Debug.WriteLine("NewTicketInfo: Setting new ticket info");

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
			saveButton.Clicked += uploadTicket;

			Debug.WriteLine("NewTicketInfo: Set save button");

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
			for (int i = startYear; i < DateTime.Now.Year; i++)
			{
				yearPicker.Items.Add(i.ToString());
			}

			Debug.WriteLine("NewTicketInfo: Set shared fields");

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
			};

			Debug.WriteLine("NewTicketInfo: Set location info");

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
			foreach (string s in ProjectResource.groups)
			{
				access_level.Items.Add(s);
			}

			Debug.WriteLine("NewTicketInfo: Set access control");

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

			Debug.WriteLine("NewTicketInfo: Set stack");
		}
        
        /// <summary>
        /// Uploads the ticket.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void uploadTicket(object sender, EventArgs e)
        {
            var year = Int32.Parse(Session.activePerson.birthYear) + yearPicker.SelectedIndex;
            Ticket ticket = new Ticket
            {
                title = title.Text,
                description = description.Text,
                mediaType = mediaType,
                year = year.ToString(),
                pathToFile = filePath,
                access_level = ProjectResource.groups[access_level.SelectedIndex],
                person_id = Session.activePerson.id
            };
            
            var area = new Area();
            if (mediaType.Equals("Picture"))
            {
                area.townCity = town_city.Text;
            }
            else
            {
                area.townCity = " ";
            }
            
            //byte[] media;
            //if (mediaType.Equals("Picture"))
            //{
            //	//media = File.ReadAllBytes(filePath);
            //	media = MediaController.readBytesFromFile(filePath);
            //}
            //else 
            //{
            //	media = MediaController.readBytesFromFile(filePath);
            //}
            
            //using (MemoryStream ms = new MemoryStream())
            //{
            //	file.GetStream().CopyTo(ms);
            //	image = ms.ToArray();
            
            if (ticket.mediaType.Equals("YouTube"))
            {
                media = null;
            }
            else 
            {
				if (media == null)
				{
					media = MediaController.readBytesFromFile(filePath);
				}
            }
            
            var selected_period = periods[period.SelectedIndex];
            
            NetworkController net = new NetworkController();
            IDictionary<string, Object> parameters = new Dictionary<string, Object>();
            parameters["token"] = Session.Token.val;
            parameters["ticket"] = ticket;
            parameters["area"] = area;
            parameters["media"] = media;
            parameters["period"] = selected_period;
            
            var jobject = await net.sendGenericPostRequest("tickets/store", parameters);
            
            var jtoken = jobject.GetValue("ticket");
            var returned_ticket = jtoken.ToObject<Ticket>();
            var ticketController = new TicketController();
            
            string ext = String.Empty;
            switch (mediaType) 
            {
                case ("Picture"):
                returned_ticket.displayIcon = "photo_icon.png";
                ext = ".jpg";
                returned_ticket.pathToFile = "t_" + returned_ticket.id + ext;
                TicketsPicture.pictureTickets.Add(returned_ticket);
                break;
                case ("Sound"):
                returned_ticket.displayIcon = "audio_icon.png";
                ext = ".wav";
                returned_ticket.pathToFile = "t_" + returned_ticket.id + ext;
                TicketsSounds.soundTickets.Add(returned_ticket);
                break;
            }
            
            MediaController.writeImageToFile("t_" + returned_ticket.id + ext, media);
            
            ticketController.addTicketLocally(returned_ticket);
            
            // Add to view
            TicketsByPeriod.addTicket(returned_ticket);
            
            jtoken = jobject.GetValue("area");
            var returned_area = jtoken.ToObject<Area>();
            
            var areaController = new AreaController();
            var stored_area = areaController.getArea(returned_area.id);
            
            if (stored_area == null)
            {
                areaController.addAreaLocally(returned_area);
            }
            
            await Navigation.PopModalAsync();
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
                entriesNotNull = (!String.IsNullOrEmpty(title.Text))
                && (!String.IsNullOrEmpty(description.Text))
                && (!String.IsNullOrEmpty(town_city.Text))
                && (yearPicker.SelectedIndex != -1)
                && (period.SelectedIndex != -1)
                && (access_level.SelectedIndex != -1);
            }
            else 
            {
                entriesNotNull = (!String.IsNullOrEmpty(title.Text))
                && (!String.IsNullOrEmpty(description.Text))
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
