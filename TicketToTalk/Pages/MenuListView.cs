﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// Menu list view.
	/// </summary>
	public class MenuListView : ListView
	{
		/// <summary>
		/// Initializes a new instance of the menu list.
		/// </summary>
		public MenuListView()
		{
			List<NavMenuItem> data = new MenuListData();

			//ItemsSource = data;
			this.SetBinding(ListView.ItemsSourceProperty, new Binding("."));
			BindingContext = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = ProjectResource.color_white;
			//SeparatorColor = ProjectResource.color_grey;

			var cell = new DataTemplate(typeof(MenuCell));
			//cell.SetBinding(TextCell.TextProperty, "Title");
			//cell.SetBinding(ImageCell.ImageSourceProperty, "IconSource");

			//cell.SetValue(TextCell.TextColorProperty, ProjectResource.color_dark);

			HasUnevenRows = true;
			RowHeight = 60;

			ItemTemplate = cell;
			SeparatorColor = Color.Transparent;
		}
	}
}
