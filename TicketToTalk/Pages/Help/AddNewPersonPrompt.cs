﻿// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 09/09/2016
//
// AddNewPersonPrompt.cs
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Add new person prompt.
	/// </summary>
	public class AddNewPersonPrompt : TrackedContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AddNewPersonPrompt"/> class.
		/// </summary>
		public AddNewPersonPrompt(bool canSkip)
		{

            TrackedName = "Add New Conversation Prompt";

			Padding = new Thickness(20);
			BackgroundColor = ProjectResource.color_red;

			var icon = new Image
			{
				Source = "face_white_icon.png",
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill
			};

			var info = new Label
			{
				Text = "Let's make a profile for someone you'd like to collect tickets for!",
				//TextColor = ProjectResource.color_white,
				//HorizontalTextAlignment = TextAlignment.Center,
			};
            info.SetSubHeaderStyle();
            info.TextColor = ProjectResource.color_white;
            info.HorizontalTextAlignment = TextAlignment.Center;
            info.HorizontalOptions = LayoutOptions.CenterAndExpand;

			var button = new Button
			{
				Text = "Add a Person",
			};
            button.SetStyle();
            button.Margin = new Thickness(0, 0, 0, 0);
			button.SetButtonStyle(ProjectResource.color_dark);
			button.Clicked += Button_Clicked;

			var skipTutorialButton = new Button
			{
				Text = "Skip Tutorial"
			};
            skipTutorialButton.SetStyle();
            skipTutorialButton.Margin = new Thickness(0, 0, 0, 0);
			skipTutorialButton.SetButtonStyle(ProjectResource.color_red);
			skipTutorialButton.Clicked += SkipTutorialButton_Clicked;
			skipTutorialButton.IsVisible = canSkip;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Children = {
					icon,
					info,
					button,
					skipTutorialButton
				}
			};
		}

		/// <summary>
		/// On button press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Button_Clicked(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new AddPersonChoice());
			nav.SetNavHeaders();
			AddPerson.isInTutorial = true;

			//Application.Current.MainPage = nav;
			Navigation.PushModalAsync(nav);
		}

		/// <summary>
		/// Skips the tutorial button clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void SkipTutorialButton_Clicked(object sender, EventArgs e)
		{
			Application.Current.MainPage = new RootPage();
		}
	}
}