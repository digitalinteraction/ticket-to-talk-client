// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 04/04/2017
//
// LoadingPage.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	public class LoadingPage : ContentPage
	{

		public AbsoluteLayout layout = new AbsoluteLayout();
		public ProgressSpinner indicator;
		public ContentView stack = new ContentView();

		public LoadingPage()
		{
			indicator = new ProgressSpinner(this, ProjectResource.color_white_transparent, ProjectResource.color_dark);
			//indicator.SetBinding(ProgressSpinner.IsRunningProperty, "IsBusy");
			//indicator.BindingContext = this;

			var contentPlaceholder = new ContentView();
			contentPlaceholder.SetBinding(ContentView.ContentProperty, "Content");
			contentPlaceholder.BindingContext = stack;

			AbsoluteLayout.SetLayoutBounds(stack, new Rectangle(0.5, 0.5, 1.0, 1.0));
			AbsoluteLayout.SetLayoutFlags(stack, AbsoluteLayoutFlags.All);

			layout.Children.Add(stack);
			layout.Children.Add(indicator);

			Content = layout;
		}
	}
}

