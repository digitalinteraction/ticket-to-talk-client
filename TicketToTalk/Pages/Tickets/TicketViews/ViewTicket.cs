using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TicketToTalk
{
    /// <summary>
    /// View ticket.
    /// </summary>
    public partial class ViewTicket : ContentPage
    {
        public Ticket Ticket { get; set;}
        
        /// <summary>
        /// Initializes a view of the ticket content.
        /// </summary>
        /// <param name="ticket">Ticket.</param>
        public ViewTicket(Ticket ticket)
        {
            Ticket = ticket;
            this.Title = ticket.title;
            
            // Add button to navigation bar.
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "?",
                Icon = "info_icon.png",
                Order = ToolbarItemOrder.Primary,
                Command = new Command(displayInfo)
            });
            
            ContentView mediaContent = null;
            
            var hasPerms = Task.Run(() => checkStoragePerms()).Result;
            if (!hasPerms)
            {
                Navigation.PopAsync();
            }
            else
            {
                switch (ticket.mediaType)
                {
                    case ("Picture"):
                    mediaContent = new PictureLayout(ticket.pathToFile);
                    break;
                    case ("Sound"):
                    mediaContent = new AudioPlayerLayout(ticket);
                    break;
                    case ("Video"):
                    break;
                    case ("YouTube"):
                    mediaContent = new YouTubePlayer(ticket.pathToFile);
                    break;
                }
            }
            
            Debug.WriteLine("NewTicket: Set image view");
            
            var content = new StackLayout
            {
                Spacing = 12,
                Children =
                {
                    mediaContent,
                    new TicketInfo(ticket)
                }
            };
            
            Content = new ScrollView
            {
                Content = content
            };
        }
        
        /// <summary>
        /// Checks the storage perms.
        /// </summary>
        /// <returns>The storage perms.</returns>
        private async Task<bool> checkStoragePerms()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert("Storage", "Ticket to Talk needs access to storage to save tickets.", "OK");
                    }
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    status = results[Permission.Storage];
                }
                
                if (status == PermissionStatus.Granted)
                {
                    return true;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Storage Denied", "Cannot save tickets without access to audio.", "OK");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
            
            return false;
        }
        
        /// <summary>
        /// Display ticket info
        /// </summary>
        /// <returns>The info.</returns>
        public async void displayInfo()
        {
            var action = await DisplayActionSheet("Ticket Options", "Cancel", "Delete", "Display Information", "Add to Conversation");
            
            switch (action)
            {
                case ("Delete"):
                var ticketController = new TicketController();
                await Navigation.PopAsync();
                ticketController.destroyTicket(Ticket);
                break;
                case ("Display Information"):
                await Navigation.PushAsync(new DisplayTicketInfo(Ticket));
                break;
                case ("Add to Conversation"):
                await Navigation.PushModalAsync(new ConversationSelect(Ticket));
                break;
            }
        }
    }
}