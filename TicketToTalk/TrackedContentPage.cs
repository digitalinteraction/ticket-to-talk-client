// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/05/2017
//
// TrackedContentPage.cs
using System;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace TicketToTalk
{
    public class TrackedContentPage : ContentPage
    {

        public string TrackedName;

        public TrackedContentPage()
        {
            
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView(TrackedName);
		}
    }
}
