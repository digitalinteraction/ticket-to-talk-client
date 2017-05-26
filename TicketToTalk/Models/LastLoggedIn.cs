// Author: Daniel Welsh - d.welsh@ncl.ac.uk
// Created on: 26/05/2017
//
// LastLoggedIn.cs
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TicketToTalk
{
    public class LastLoggedIn : INotifyPropertyChanged, IComparable
    {

        private string _email;
        private DateTime _date;
        private object now;

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string email 
        {
            get 
            {
                return _email;
            }

            set 
            {
                if (_email != value) 
                {
                    this._email = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime date 
        {
            get 
            {
                return _date;
            }
            set 
            {
                if (value != _date) 
                {
                    this._date = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LastLoggedIn()
        {
        }

        public LastLoggedIn(string email, DateTime date)
        {
            this.email = email;
            this.date = date;
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="obj">Object.</param>
        public int CompareTo(object obj)
        {
            var rhs = obj as LastLoggedIn;

			var comp = string.Compare(email, rhs.email, StringComparison.Ordinal);

            return comp;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.LastLoggedIn"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:TicketToTalk.LastLoggedIn"/>.</returns>
        public override string ToString()
        {
            return string.Format("[LastLoggedIn: email={0}, date={1}]", email, date);
        }
    }
}
