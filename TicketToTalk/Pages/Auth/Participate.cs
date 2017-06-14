// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/05/2017
//
// Participate.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
    public class Participate : TrackedContentPage
    {

        Button accept;
        Button reject;
        String hyperlink = "ticket-to-talk.com/participate";

        UserController userController;

        public Participate()
        {
            TrackedName = "Participate";

            userController = new UserController();

			NavigationPage.SetHasNavigationBar(this, false);

			this.Title = "Registration";
			this.Padding = 20;
			BackgroundColor = ProjectResource.color_red;

            var desc = new Label
            {
                Text = "Ticket to Talk was created by Newcastle University as part of an ongoing research study in intergenerational interactions within dementia.\n\nWould you like to take part by sharing your data with Newcastle University? Please tap the link below for more information before participating."
			};
            desc.SetBodyStyle();
            desc.SetLabelStyleInversreCenter();

            var link = new Label
            {
                Text = hyperlink,
            };
            link.SetSubHeaderStyle();
            link.SetLabelStyleInversreCenter();
            link.TextColor = ProjectResource.color_dark;

            // Add a tap gesuture recognizer to the link
			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) =>
			{
                Device.OpenUri(new Uri("http://" + hyperlink));
			};
			link.GestureRecognizers.Add(tapGestureRecognizer);

            accept = new Button
			{
				Text = "Accept"
			};
            accept.SetButtonStyle(ProjectResource.color_dark);
            accept.SetStyle();
			accept.WidthRequest = (Session.ScreenWidth * 0.4);
            accept.Clicked += Accept_Clicked;

            reject = new Button
			{
				Text = "Skip",
			};
			reject.SetButtonStyle(ProjectResource.color_dark);
            reject.SetStyle();
			reject.WidthRequest = (Session.ScreenWidth * 0.4);
            reject.Clicked += Reject_Clicked;

			var icon = new Image
			{
				Source = "clipboard_white_icon.png",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill
			};

			var infStack = new StackLayout
			{
				Spacing = 20,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
				{
					icon,
                    desc,
                    link
				}
			};

			var buttonStack = new StackLayout
			{
				Spacing = 20,
				VerticalOptions = LayoutOptions.End,
				Orientation = StackOrientation.Horizontal,
				Children =
				{
					reject,
					accept
				}
			};

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 5,
				Padding = 2,
				Children =
				{
					infStack,
					buttonStack
				}
			};
        }

        /// <summary>
        /// Accepts the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void Accept_Clicked(object sender, EventArgs e)
        {

            bool accepted = false;

            try
            {
                accepted = await userController.AcceptStudy();
            }
            catch (NoNetworkException ex)
            {
                Debug.WriteLine(ex);
                await DisplayAlert("No Network", ex.Message, "Dismiss");
            }

            if (accepted) 
            {
                NextScreen();
            }
            else 
            {
                await DisplayAlert("Participate", "Something went wrong, we were unable to include you in the study.", "Dismiss");
            }
        }

        /// <summary>
        /// Rejects the clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Reject_Clicked(object sender, EventArgs e)
        {
            NextScreen();
        }

        /// <summary>
        /// Progress to adding a new person.
        /// </summary>
        public void NextScreen() 
        {
			var t = new AddNewPersonPrompt(false);
			Application.Current.MainPage = t;
			AllProfiles.promptShown = true;
        }
    }
}

