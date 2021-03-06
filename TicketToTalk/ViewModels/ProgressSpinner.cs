﻿// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 03/04/2017
//
// ProgressSpinner.cs
using System;
using Xamarin.Forms;

namespace TicketToTalk
{
	public class ProgressSpinner : ActivityIndicator
	{
		public ProgressSpinner(object binding, Color backgroundColor)
		{
			this.Color = ProjectResource.color_white;
			this.BackgroundColor = backgroundColor;
			this.VerticalOptions = LayoutOptions.CenterAndExpand;
			this.HeightRequest = Session.ScreenHeight;

			this.VerticalOptions = LayoutOptions.CenterAndExpand;
			this.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			this.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			this.BindingContext = binding;

			AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.5, 0.5, Session.ScreenWidth, Session.ScreenHeight));
		}

		public ProgressSpinner(object binding, Color backgroundColor, Color spinnerColor)
		{
			this.Color = spinnerColor;
			this.VerticalOptions = LayoutOptions.CenterAndExpand;

#if __ANDROID__
            this.HeightRequest = Session.ScreenHeight * 0.1;
#else
            this.HeightRequest = Session.ScreenHeight;
            this.BackgroundColor = backgroundColor;
#endif
			this.VerticalOptions = LayoutOptions.CenterAndExpand;
			this.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
			this.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
			this.BindingContext = binding;

			AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.5, 0.5, Session.ScreenWidth, Session.ScreenHeight));
		}
	}
}
