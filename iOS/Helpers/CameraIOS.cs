// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 22/05/2017
//
// CameraIOS.cs

using System;
using AVFoundation;
using Foundation;
using UIKit;
using Xamarin.Forms;
using CoreGraphics;

[assembly: Dependency(typeof(TicketToTalk.iOS.CameraIOS))]
namespace TicketToTalk.iOS
{
	public class CameraIOS : CameraInterface
	{
		public CameraIOS()
		{
		}

		public async void BringUpCamera()
		{
			//Check if we have permission to use the camera
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			//If we don't have access, and have never asked before, prompt them
			if (authorizationStatus != AVAuthorizationStatus.Authorized)
			{
				var access = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);

				//If access was granted we can proceed, if not, you can add an else statement and implement an error message or something more helpful
				if (access)
				{
					GotAccessToCamera();
				}
			}
			else
			{
				//We've already been given access
				GotAccessToCamera();
			}
		}

		public void BringUpPhotoGallery()
		{
			var imagePicker = new UIImagePickerController { SourceType = UIImagePickerControllerSourceType.PhotoLibrary, MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary) };
			imagePicker.AllowsEditing = true;

			//Make sure we have the root view controller which will launch the photo gallery 
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}

			//Show the image gallery
			vc.PresentViewController(imagePicker, true, null);

			//call back for when a picture is selected and finished editing
			imagePicker.FinishedPickingMedia += (sender, e) =>
			{
				UIImage originalImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
				if (originalImage != null)
				{
					//Got the image now, convert it to byte array to send back up to the forms project
					var pngImage = originalImage.AsPNG();
					byte[] myByteArray = new byte[pngImage.Length];
					System.Runtime.InteropServices.Marshal.Copy(pngImage.Bytes, myByteArray, 0, Convert.ToInt32(pngImage.Length));

					MessagingCenter.Send<byte[]>(myByteArray, "ImageSelected");
				}

				//Close the image gallery on the UI thread
				Device.BeginInvokeOnMainThread(() =>
				{
					vc.DismissViewController(true, null);
				});
			};

			//Cancel button callback from the image gallery
			imagePicker.Canceled += (sender, e) => vc.DismissViewController(true, null);
		}

		private void GotAccessToCamera()
		{
			//Create an image picker object
			var imagePicker = new UIImagePickerController { SourceType = UIImagePickerControllerSourceType.Camera };

			//Make sure we can find the top most view controller to launch the camera
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}

			vc.PresentViewController(imagePicker, true, null);

			//Callback method for when the user has finished taking the picture
			imagePicker.FinishedPickingMedia += (sender, e) =>
			{
				//Grab the image
				UIImage image = (UIImage)e.Info.ObjectForKey(new NSString("UIImagePickerControllerOriginalImage"));

				//One thing you might notice is when you take a picture and throw it into an image view, 
				//it will be side ways, so we need to rotate it based it's orientation
				UIImage rotateImage = RotateImage(image, image.Orientation);

				//If you don't care about how big the file size is, 
				//or the image size is you can skip this step or adjust the amount of compression.
				rotateImage = rotateImage.Scale(new CGSize(rotateImage.Size.Width, rotateImage.Size.Height), 0.5f);

				var jpegImage = rotateImage.AsJPEG();

				//Converting the image to a byte array so I can send it to server via API
				//and will also use byte array to populate image view
				byte[] myByteArray = new byte[jpegImage.Length];
				System.Runtime.InteropServices.Marshal.Copy(jpegImage.Bytes, myByteArray, 0, Convert.ToInt32(jpegImage.Length));

				//Using messaging center to send the byte array back up to the UI
				MessagingCenter.Send<byte[]>(myByteArray, "ImageSelected");

				//Dismiss the camera view controller on UI thread
				Device.BeginInvokeOnMainThread(() =>
				{
					vc.DismissViewController(true, null);
				});
			};

			//Callback method for when the user cancels and doesn't take a picture
			imagePicker.Canceled += (sender, e) => vc.DismissViewController(true, null);
		}

		//Method that will take in a photo and rotate it based on the orientation that the image was taken in
		double radians(double degrees) { return degrees * Math.PI / 180; }

		private UIImage RotateImage(UIImage src, UIImageOrientation orientation)
		{
			UIGraphics.BeginImageContext(src.Size);

			if (orientation == UIImageOrientation.Right)
			{
				CGAffineTransform.MakeRotation((nfloat)radians(90));
			}
			else if (orientation == UIImageOrientation.Left)
			{
				CGAffineTransform.MakeRotation((nfloat)radians(-90));
			}
			else if (orientation == UIImageOrientation.Down)
			{
				// NOTHING
			}
			else if (orientation == UIImageOrientation.Up)
			{
				CGAffineTransform.MakeRotation((nfloat)radians(90));
			}

			src.Draw(new CGPoint(0, 0));
			UIImage image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return image;
		}
	}
}
