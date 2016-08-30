using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// User controller.
	/// </summary>
	public class UserController
	{

		UserDB userDB = new UserDB();
		NetworkController networkController = new NetworkController();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.UserController"/> class.
		/// </summary>
		public UserController()
		{
		}

		/// <summary>
		/// Gets the local user by identifier.
		/// </summary>
		/// <returns>The local user by identifier.</returns>
		/// <param name="id">Identifier.</param>
		public User getLocalUserByID(int id) 
		{
			userDB.open();
			var user = userDB.GetUser(id);
			userDB.close();

			return user;
		}

		/// <summary>
		/// Gets the local user by email.
		/// </summary>
		/// <returns>The local user by email.</returns>
		/// <param name="email">Email.</param>
		public User getLocalUserByEmail(string email) 
		{
			userDB.open();
			var user = userDB.getUserByEmail(email);
			userDB.close();
			return user;
		}

		/// <summary>
		/// Adds the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="user">User.</param>
		public void addUserLocally(User user) 
		{
			userDB.open();
			userDB.AddUser(user);
			userDB.close();
		}

		/// <summary>
		/// Deletes the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="id">Identifier.</param>
		public void deleteUserLocally(int id) 
		{
			userDB.open();
			userDB.DeleteUser(id);
			userDB.close();
		}

		/// <summary>
		/// Updates the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="user">User.</param>
		public void updateUserLocally(User user) 
		{
			deleteUserLocally(user.id);
			addUserLocally(user);
		}

		/// <summary>
		/// Authenticates the user.
		/// </summary>
		/// <returns>Is authenticated.</returns>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public async Task<bool> authenticateUser(string email, string password)
		{
			IDictionary<string, string> credentials = new Dictionary<string, string>();
			credentials["email"] = email;
			credentials["password"] = password;

			NetworkController net = new NetworkController();
			var jobject = await net.sendPostRequest("auth/login", credentials);
			if (jobject == null) return false;

			var jtoken = jobject.GetValue("code");
			var code = jtoken.ToObject<int>();

			if (code == 200)
			{
				Console.WriteLine("Success: Token Generated");
				jtoken = jobject.GetValue("token");
				var token = new Token
				{
					val = jtoken.ToObject<string>()
				};
				Session.Token = token;

				jtoken = jobject.GetValue("user");
				var returned_user = jtoken.ToObject<User>();

				var userController = new UserController();
				if (userController.getLocalUserByEmail(returned_user.email) == null)
				{
					userController.addUserLocally(returned_user);
					Session.activeUser = returned_user;
				}
				else 
				{
					Session.activeUser = returned_user;
					userController.updateUserLocally(returned_user);
				}
				Debug.WriteLine("UserController: Set active person to - " + Session.activeUser);
				return true;
			}
			else
			{
				jtoken = jobject.GetValue("message");
				Console.WriteLine(jtoken.ToObject<string>());
				return false;
			}
		}

		/// <summary>
		/// Registers the new user.
		/// </summary>
		/// <returns>The new user.</returns>
		/// <param name="user">User.</param>
		/// <param name="image">Image.</param>
		public async Task<bool> registerNewUser(User user, byte[] image)
		{
			// Json convert details.
			IDictionary<string, object> content = new Dictionary<string, object>();
			content["name"] = user.name;
			content["email"] = user.email.ToLower();
			content["password"] = user.password;
			content["image"] = image;

			// post to server.
			var net = new NetworkController();
			var jobject = await net.sendGenericPostRequest("auth/register", content);

			if (jobject != null)
			{
				var jToken = jobject.GetValue("token");
				Token token = new Token 
				{
					val = jToken.ToObject<string>()
				};
				Session.Token = token;

				jToken = jobject.GetValue("user");
				user = jToken.ToObject<User>();

				var fileName = "u_" + user.id + ".jpg";
				MediaController.writeImageToFile(fileName, image);

				if (image != null && image.Length > 0)
				{
					user.pathToPhoto = fileName;
				}
				else
				{
					user.pathToPhoto = "default_profile_pic.png";
				}
				Debug.WriteLine("Registered User: " + user);
				addUserLocally(user);

				Session.activeUser = user;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Sends the invite to person.
		/// </summary>
		/// <returns>The invite to person.</returns>
		/// <param name="email">Email.</param>
		/// <param name="group">Group.</param>
		/// <param name="person_id">Person identifier.</param>
		public async Task<bool> sendInviteToPerson(string email, string group, int person_id)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["email"] = email;
			parameters["group"] = group;
			parameters["person_id"] = person_id;
			parameters["token"] = Session.Token.val;

			var net = new NetworkController();
			var jobject = await net.sendGenericPostRequest("user/invitations/send", parameters);

			if (jobject != null) 
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the invitations.
		/// </summary>
		/// <returns>The invitations.</returns>
		public async Task<List<Invitation>> getInvitations() 
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var networkController = new NetworkController();
			var jobject = await networkController.sendGetRequest("user/invitations/get", parameters);

			if (jobject != null) 
			{
				var jtoken = jobject.GetValue("invites");
				var invitations = jtoken.ToObject<List<Invitation>>();
				return invitations;
			}
			return null;
		}

		/// <summary>
		/// Downloads the user profile picture.
		/// </summary>
		/// <returns>The user profile picture.</returns>
		/// <param name="user">User.</param>
		public string downloadUserProfilePicture(User user)
		{
			var download_finished = false;

			MessagingCenter.Subscribe<NetworkController, bool>(this, "download_image", (sender, finished) =>
			{
				Debug.WriteLine("Image Downloaded");
				download_finished = finished;
			});

			NetworkController net = new NetworkController();
			var fileName = "u_" + user.id + ".jpg";
			var task = Task.Run(() => net.downloadFile(user.pathToPhoto, fileName)).Result;

			user.pathToPhoto = fileName;

			while (!download_finished)
			{
			}

			updateUserLocally(user);

			MessagingCenter.Unsubscribe<NetworkController, bool>(this, "download_image");

			return fileName;
		}

		/// <summary>
		/// Accepts the invitation.
		/// </summary>
		/// <returns>The invitation.</returns>
		/// <param name="p">P.</param>
		public async Task<bool> acceptInvitation(Invitation i, string relation) 
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = i.person.id.ToString();
			parameters["relation"] = relation;
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.sendPostRequest("user/invitations/accept", parameters);

			if (jobject != null)
			{
				var pu = new PersonUser(i.person.id, Session.activeUser.id, i.group, relation);
				var puDB = new PersonUserDB();
				puDB.AddPersonUser(pu);
				puDB.close();

				return true;
			}
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// Rejects the invitation.
		/// </summary>
		/// <returns>The invitation.</returns>
		/// <param name="p">P.</param>
		public async Task<bool> rejectInvitation(Person p) 
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = p.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.sendPostRequest("user/invitations/reject", parameters);

			if (jobject != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}