// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 12/09/2016
//
// FinishTutorialPage.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{

	/// <summary>
	/// Finish tutorial page.
	/// </summary>
	public class FinishTutorialPage : TrackedContentPage
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.FinishTutorialPage"/> class.
		/// </summary>
		public FinishTutorialPage()
		{

            TrackedName = "Finish Tutorial Page";

			BackgroundColor = ProjectResource.color_red;

			var label = new Label
			{
				Text = "You've added a person and a photo ticket! Look for the ticket option in the menu to view it."
			};
            label.SetSubHeaderStyle();
			label.SetLabelStyleInversreCenter();

			var button = new Button
			{
				Text = "Finish Tutorial"
			};
            button.SetStyle();
            button.Margin = new Thickness(0, 0, 0, 0);
			button.SetButtonStyle(ProjectResource.color_dark);
			button.Clicked += Button_Clicked;

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 20,
				Children = {
					label,
					button
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
			Application.Current.MainPage = new RootPage();
		}
	}
}


