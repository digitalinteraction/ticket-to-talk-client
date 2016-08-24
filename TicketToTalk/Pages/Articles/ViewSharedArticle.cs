// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 24/08/2016
//
// ViewSharedArticle.cs
using System;

using Xamarin.Forms;

namespace TicketToTalk
{
	public class ViewSharedArticle : ContentPage
	{
		public ViewSharedArticle(Article a)
		{
			Content = new StackLayout
			{
				Children = {
					new Label { Text = "Hello ContentPage" }
				}
			};
		}
	}
}


