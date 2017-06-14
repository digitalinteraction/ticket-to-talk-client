// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 13/06/2017
//
// T3ImageCell.cs
using System;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
    public class T3ImageCell : ViewCell
    {

        public string Text;
        public string SubText;
        public ImageSource Img;

        public T3ImageCell(string Text, string SubText, ImageSource Img)
        {

            this.Text = Text;
            this.SubText = SubText;
            this.Img = Img;

            var _master = new Label();
            var _detail = new Label();

            _master.Text = Text;
            _master.SetSubHeaderStyle();
            _master.VerticalOptions = LayoutOptions.Start;
            _master.Margin = new Thickness(0, 5, 0, 0);

            _detail.Text = SubText;
            _detail.SetBodyStyle();
            _detail.VerticalOptions = LayoutOptions.Start;
            _detail.TextColor = ProjectResource.color_red;

            var detailStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
                Children = 
                {
                    _master,
                    _detail
                }
            };

            var img = new Image
            {
                HeightRequest = 50.0,
                WidthRequest = 50.0,
                Margin = new Thickness(5, 0, 0, 0)
            };
            img.Source = Img;

            var stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = 
                {
                    img, 
                    detailStack
                }
            };

            this.View = stack;
        }
    }
}

