// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 14/03/2017
//
// Verification.cs

using System;
using System.Collections.Generic;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class Verification : TrackedContentPage
	{

		Entry code;
		Button resend;
		Button send;
		bool firstLogin;

		public Verification(bool firstLogin)
		{
            TrackedName = "Verfication";

			this.firstLogin = firstLogin;

			NavigationPage.SetHasNavigationBar(this, false);

			this.Title = "Registration";
			this.Padding = 20;
			BackgroundColor = ProjectResource.color_red;

			var label = new Label
			{
				Text = "You're nearly registered!\n\nPlease verify your email address by entering the code emailed to you."
			};
            label.SetBodyStyle();
			label.SetLabelStyleInversreCenter();

			code = new Entry
			{
				TextColor = ProjectResource.color_white,
				WidthRequest = (Session.ScreenWidth * 0.5),
				BackgroundColor = ProjectResource.color_red,
			};
			code.Focus();
            code.SetStyle();
			code.TextChanged += Entry_TextChanged;

			send = new Button 
			{
				Text = "Verify"
			};
            send.SetStyle();
			send.SetButtonStyle(ProjectResource.color_grey);
			send.WidthRequest = (Session.ScreenWidth * 0.4);
			send.IsEnabled = false;
			send.Clicked += CheckVerification;

			resend = new Button
			{
				Text = "Resend Email",
			};
            resend.SetStyle();
			resend.SetButtonStyle(ProjectResource.color_dark);
			resend.WidthRequest = (Session.ScreenWidth * 0.4);
			resend.Clicked += Resend_Clicked;

			var icon = new Image
			{
				Source = "face_white_icon.png",
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
					label,
					code,
				}
			};

			var buttonStack = new StackLayout 
			{
				Spacing = 20,
				VerticalOptions = LayoutOptions.End,
				Orientation = StackOrientation.Horizontal,
				Children = 
				{
					resend,
					send
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
		/// Checks the verification.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void CheckVerification(object sender, EventArgs e)
		{
			send.IsEnabled = false;
			var c = code.Text.ToUpper().Trim();
			var userController = new UserController();

			bool verified = false;

			try
			{
				verified = await userController.VerifyUser(c);
			}
			catch (Exception ex)
			{
				await DisplayAlert("No Network", ex.Message, "Dismiss");
				send.IsEnabled = true;
			}

			if (verified)
			{
				// Launch next screen.
				if (firstLogin) 
				{
                    var t = new Participate();
					Application.Current.MainPage = t;

					//await Navigation.PushAsync(new AllProfiles());
					//Navigation.RemovePage(this);
				}
				else 
				{
					await Navigation.PushAsync(new SelectActivePerson());
					Navigation.RemovePage(this);
				}
			}
			else 
			{
				// Notify
				await DisplayAlert("Verification", "Your account could not be verified, please retry the code or request another email.", "OK");
				send.IsEnabled = true;
			}

		}

		/// <summary>
		/// Entries the text changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public void Entry_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(code.Text) && code.Text.Length == 6)
			{
				send.BackgroundColor = ProjectResource.color_dark;
				send.IsEnabled = true;
			}
			else 
			{
				send.IsEnabled = false;
				send.BackgroundColor = ProjectResource.color_grey;
			}
		}

		/// <summary>
		/// Resend an email when clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void Resend_Clicked(object sender, EventArgs e)
		{
			var userController = new UserController();
			bool sent = false;

			try
			{
				sent = await userController.resendEmail();
			}
			catch (NoNetworkException ex)
			{
				await DisplayAlert("No Network", ex.Message, "Dismiss");
			}

			if (sent) 
			{
				await DisplayAlert("Verification", "A new verification email has been sent to your email account.", "OK");
			}
		}
	}
}

