﻿using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Help.
	/// </summary>
	public class Help : TrackedContentPage
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.Help"/> class.
		/// </summary>
		public Help()
		{

            TrackedName = "Help";

			Title = "Help";

			var title = new Label
			{
				Text = "Ticket to Talk",
				TextColor = ProjectResource.color_dark,
				FontSize = 22,
				FontAttributes = FontAttributes.Bold,
				HorizontalTextAlignment = TextAlignment.Center
			};

			var summary = new Label 
			{
				Text = "Help bridge gaps in conversation with Ticket to Talk.",
			};
            summary.SetSubHeaderStyle();
            summary.HorizontalTextAlignment = TextAlignment.Center;
            summary.HorizontalOptions = LayoutOptions.CenterAndExpand;

			var intro = new Label 
			{
				Text = 
					"You can use this app to collect information about someone through adding tickets. " +
					"Add tickets to a conversation which you can use when you talk to the person you've added tickets for. " +
					"",
				TextColor = ProjectResource.color_dark,
				FontSize = 14,
			};
            intro.SetBodyStyle();

			var ticketsTitle = new Label 
			{
				Text = "Tickets",
			};
            ticketsTitle.SetSubHeaderStyle();

			var ticketsAdding = new Label
			{
				Text =
					"Add tickets by navigating to the 'Tickets' page. " +
					"Select the add button in the top corner and select the type of ticket you want to create. " +
					"From here you can select or take a photo, record a sound, or add a link to a youtube video. " +
					"Once you have added the ticket fill out the information on the screen and click add!",
			};
            ticketsAdding.SetBodyStyle();

			var ticketsViewing = new Label
			{
				Text =
					"If you have added a ticket you can view it from the tickets page. " +
					"The tickets are sorted into categories of ticket type and the stages in life of the person your tickets are attached to. " +
					"Simply click on a ticket to view it.",
			};
            ticketsViewing.SetBodyStyle();

			var inspirationTitle = new Label
			{
				Text = "Inspiration",
			};
            inspirationTitle.SetSubHeaderStyle();

			var inspirationInf = new Label
			{
				Text =
					"If you need some inspiration about what ticket to add then try the inspirations feature. " +
					"Look through the provided inspiration and use these to add your tickets!",
			};
            inspirationInf.SetBodyStyle();

			var conversationsTitle = new Label 
			{
				Text = "Conversations",
			};
            conversationsTitle.SetSubHeaderStyle();

			var conversationsInf = new Label
			{
				Text =
					"Use your tickets by adding them to a conversation. " +
					"When viewing a ticket tap options, select add to conversation, and then select the conversation you want to add the ticket to. " +
					"Then when you are ready to start the conversation select it from the conversations menu and press start. " +
					"Dont forget to add some notes about the converation when you are finished for the other contributors to see!",
			};
            conversationsInf.SetBodyStyle();

			var usefulInformationTitle = new Label
			{
				Text = "Useful Information",
			};
            usefulInformationTitle.SetSubHeaderStyle();

			var usefulInformationInf = new Label
			{
				Text =
					"If you come across an article you like then add it to the useful information page. " +
					"You can use this to save pages you find useful. " +
					"If you think someone else might use the information you can share the article with them by selecting share from the options menu when viewing the infomation.",
			};
            usefulInformationInf.SetBodyStyle();

			var peopleTitle = new Label
			{
				Text = "People",
			};
            peopleTitle.SetSubHeaderStyle();

			var peopleInf = new Label
			{
				Text =
					"You can add contributors to a person from their profile. " +
					"Select send invitation, put in their email, and select the group you want to add them to.",
			};
            peopleInf.SetBodyStyle();

			var content = new StackLayout 
			{
				Padding = new Thickness(20),
				Spacing = 10,
				Children = 
				{
					title, 
					summary,
					intro,
					ticketsTitle,
					ticketsAdding,
					ticketsViewing,
					inspirationTitle,
					inspirationInf,
					conversationsTitle,
					conversationsInf,
					usefulInformationTitle,
					usefulInformationInf,
					peopleTitle,
					peopleInf
				}
			};

			Content = new ScrollView
			{
				Content = content
			};
		}
	}
}


