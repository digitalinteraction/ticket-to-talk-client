﻿// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 12/09/2016
//
// ConversationHelp.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Conversation help.
	/// </summary>
	public class HelpPopup : ContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.ConversationHelp"/> class.
		/// </summary>
		public HelpPopup(string text, string iconName)
		{

			BackgroundColor = ProjectResource.color_red;

			var icon = new Image
			{
				Source = iconName,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HeightRequest = 50,
				WidthRequest = 50,
				Aspect = Aspect.AspectFill
			};

			var label = new Label
			{
				Text = text
			};
			label.setLabelStyleInversreCenter();

			var button = new Button
			{
				Text = "Dismiss"
			};
			button.setButtonStyle(ProjectResource.color_dark);
			button.Clicked += Button_Clicked;

			//var skipTutorialButton = new Button
			//{
			//	Text = "Skip Tutorial"
			//};
			//skipTutorialButton.setButtonStyle(ProjectResource.color_red);
			//skipTutorialButton.Clicked += SkipTutorialButton_Clicked;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Children = {
					icon,
					label,
					button,
					//skipTutorialButton
				}
			};
		}

		/// <summary>
		/// Buttons the clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void Button_Clicked(object sender, EventArgs e)
		{
			//var nav = new NavigationPage(new NewConversation());
			//nav.setNavHeaders();

			//Navigation.PushModalAsync(nav);

			Navigation.PopModalAsync();
		}

		///// <summary>
		///// Skips the tutorial button clicked.
		///// </summary>
		///// <param name="sender">Sender.</param>
		///// <param name="e">E.</param>
		//private void SkipTutorialButton_Clicked(object sender, EventArgs e)
		//{

		//}
	}
}


