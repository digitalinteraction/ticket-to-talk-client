// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 26/08/2016
//
// MenuCell.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	public class MenuCell : ViewCell
	{
		public MenuCell()
		{
			var image = new Image
			{
				HeightRequest = 30,
				WidthRequest = 30,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			image.SetBinding(Image.SourceProperty, "IconSource");

			var label = new Label
			{
				TextColor = ProjectResource.color_dark,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			label.SetBinding(Label.TextProperty, "Title");

			var content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 5,
				Children = 
				{
					image,
					label
				}
			};

			this.View = content;
		}
	}
}


