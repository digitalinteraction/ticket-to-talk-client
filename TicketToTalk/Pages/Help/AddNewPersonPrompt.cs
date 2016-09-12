// Author: Daniel Welsh - d.welsh@ncl.ac.uk
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
	public class AddNewPersonPrompt : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AddNewPersonPrompt"/> class.
		/// </summary>
		public AddNewPersonPrompt()
		{

			Padding = new Thickness(20);
			BackgroundColor = ProjectResource.color_blue;

			var info = new Label
			{
				Text = "Let's make a profile for someone you'd like to collect tickets for!",
				TextColor = ProjectResource.color_white,
				HorizontalTextAlignment = TextAlignment.Center,
				//VerticalOptions = LayoutOptions.Center
			};

			var button = new Button
			{
				Text = "Add a Person",
			};
			button.setButtonStyle(ProjectResource.color_dark);
			button.Clicked += Button_Clicked;

			var skipTutorialButton = new Button
			{
				Text = "Skip Tutorial"
			};
			skipTutorialButton.setButtonStyle(ProjectResource.color_blue);
			skipTutorialButton.Clicked += SkipTutorialButton_Clicked;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Children = {
					info,
					button,
					skipTutorialButton
				}
			};

			Debug.WriteLine("AddNewPersonPrompt: Content Set");
		}

		/// <summary>
		/// On button press.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Button_Clicked(object sender, EventArgs e)
		{
			var nav = new NavigationPage(new AddPerson(null));
			nav.setNavHeaders();

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