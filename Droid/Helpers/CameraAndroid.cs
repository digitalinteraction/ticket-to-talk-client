// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/05/2017
//
// CameraAndroid.cs
using System;
using Android.App;
using Android.Content;
using Android.Provider;
using Xamarin.Forms;

[assembly: Dependency(typeof(TicketToTalk.Droid.CameraAndroid))]
namespace TicketToTalk.Droid
{
	public class CameraAndroid : CameraInterface
	{
		public CameraAndroid()
		{
		}

		public void BringUpCamera()
		{
		var intent = new Intent(MediaStore.ActionImageCapture);
		((Activity)Forms.Context).StartActivityForResult(intent, 1);
		}

		public void BringUpPhotoGallery()
		{
			var imageIntent = new Intent();
			imageIntent.SetType("image/*");
			imageIntent.SetAction(Intent.ActionGetContent);

			((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
		}
	}
}