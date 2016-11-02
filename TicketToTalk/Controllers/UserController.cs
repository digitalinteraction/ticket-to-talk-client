using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TicketToTalk
{
	/// <summary>
	/// User controller.
	/// </summary>
	public class UserController
	{

		private UserDB userDB = new UserDB();
		private NetworkController networkController = new NetworkController();

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
		public User GetLocalUserByID(int id)
		{
			userDB.Open();
			var user = userDB.GetUser(id);
			userDB.Close();

			return user;
		}

		/// <summary>
		/// Gets the local user by email.
		/// </summary>
		/// <returns>The local user by email.</returns>
		/// <param name="email">Email.</param>
		public User GetLocalUserByEmail(string email)
		{
			userDB.Open();
			var user = userDB.GetUserByEmail(email);
			userDB.Close();
			return user;
		}

		/// <summary>
		/// Adds the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="user">User.</param>
		public void AddUserLocally(User user)
		{
			userDB.Open();
			userDB.AddUser(user);
			userDB.Close();
		}

		/// <summary>
		/// Deletes the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteUserLocally(int id)
		{
			userDB.Open();
			userDB.DeleteUser(id);
			userDB.Close();
		}

		/// <summary>
		/// Updates the user locally.
		/// </summary>
		/// <returns>The user locally.</returns>
		/// <param name="user">User.</param>
		public void UpdateUserLocally(User user)
		{
			DeleteUserLocally(user.id);
			AddUserLocally(user);
		}

		/// <summary>
		/// Updates the user remotely.
		/// </summary>
		/// <param name="user">User.</param>
		public async Task<User> UpdateUserRemotely(User user, byte[] image)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["name"] = user.name;
			parameters["email"] = user.email;
			parameters["password"] = user.password.HashString();
			parameters["image"] = null;
			parameters["imageHash"] = null;
			parameters["token"] = Session.Token.val;

			if (image != null)
			{
				parameters["image"] = image;
				parameters["imageHash"] = image.HashArray();
			}

			var jobject = await networkController.SendGenericPostRequest("user/update", parameters);
			if (jobject != null)
			{
				var jtoken = jobject.GetValue("User");
				var returned = jtoken.ToObject<User>();

				Session.activeUser.name = returned.name;
				Session.activeUser.email = returned.email;

				if (Session.activeUser.imageHash == null)
				{
					user.pathToPhoto = "default_profile.png";
				}

				if (image != null)
				{
					Session.activeUser.imageSource = ImageSource.FromStream(() => new MemoryStream(image));
					Session.activeUser.imageHash = image.HashArray();
					MediaController.WriteImageToFile(Session.activeUser.pathToPhoto, image);
				}

				UpdateUserLocally(user);

				return returned;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Authenticates the user.
		/// </summary>
		/// <returns>Is authenticated.</returns>
		/// <param name="email">Email.</param>
		/// <param name="password">Password.</param>
		public async Task<bool> AuthenticateUser(string email, string password)
		{
			IDictionary<string, string> credentials = new Dictionary<string, string>();
			credentials["email"] = email;
			credentials["password"] = password.HashString();

			var userController = new UserController();
			var user = userController.GetLocalUserByEmail(email.ToLower());

			// first time login
			if (user == null)
			{
				Debug.WriteLine("First time login");
				Session.activeUser = null;
			}
			else 
			{
				Session.activeUser = user;
			}

			var net = new NetworkController();
			var jobject = await net.SendPostRequest("auth/login", credentials);

			// fail if null response
			if (jobject == null) return false;

			Debug.WriteLine(jobject);

			var status = jobject.GetValue("status");
			//var jtoken = status.
			var jcode = status["code"];
			var code = jcode.ToObject<int>();

			// if success.
			if (code == 200)
			{
				var data = jobject.GetValue("data");
				var jtoken = data["token"];
				var token = new Token
				{
					val = jtoken.ToObject<string>()
				};
				Session.Token = token;

				jtoken = data["user"];
				var returned_user = jtoken.ToObject<User>();

				var local_user = userController.GetLocalUserByEmail(returned_user.email);
				if (local_user == null)
				{
					returned_user.firstLogin = true;
					jtoken = data["api_key"];
					returned_user.api_key = jtoken.ToObject<string>();
					userController.AddUserLocally(returned_user);
					Session.activeUser = returned_user;
				}
				else
				{
					returned_user.firstLogin = false;

					if (returned_user.imageHash != null)
					{
						if (local_user.imageHash == null)
						{
							await DownloadUserProfilePicture();
						}
						else if (!returned_user.imageHash.Equals(local_user.imageHash))
						{
							await DownloadUserProfilePicture();
						}
						local_user.pathToPhoto = "u_" + local_user.id + ".jpg";
					}

					returned_user.pathToPhoto = local_user.pathToPhoto;

					Session.activeUser = returned_user;
					Session.activeUser.api_key = user.api_key;
					userController.UpdateUserLocally(returned_user);
				}
				return true;
			}
			else
			{
				var errors = jobject.GetValue("errors");
				var jerrors = errors["message"];
				Console.WriteLine(jerrors.ToObject<string>());
				return false;
			}
		}

		/// <summary>
		/// Registers the new user.
		/// </summary>
		/// <returns>The new user.</returns>
		/// <param name="user">User.</param>
		/// <param name="image">Image.</param>
		public async Task<bool> RegisterNewUser(User user, byte[] image)
		{
			SHA256 sha = new SHA256Managed();
			byte[] passBytes = Encoding.UTF8.GetBytes(user.password);
			byte[] hash = sha.ComputeHash(passBytes);
			user.password = ByteToHex(hash);

			// Json convert details.
			IDictionary<string, object> content = new Dictionary<string, object>();
			content["name"] = user.name;
			content["email"] = user.email.ToLower();
			content["password"] = user.password;
			content["pathToPhoto"] = null;
			content["image"] = image;
			content["imageHash"] = null;

			if (image == null)
			{
				content["pathToPhoto"] = "default_profile.png";
			}
			else
			{
				user.imageHash = image.HashArray();
				content["imageHash"] = user.imageHash;
			}

			// post to server.
			var net = new NetworkController();
			var jobject = await net.SendGenericPostRequest("auth/register", content);

			if (jobject != null)
			{
				var data = jobject["data"];
				var jtoken = data["token"];
				var token = new Token
				{
					val = jtoken.ToObject<string>()
				};
				Session.Token = token;

				var juser = data["user"];
				user = juser.ToObject<User>();
				user.firstLogin = true;

				if (image != null && image.Length > 0)
				{
					var fileName = "u_" + user.id + ".jpg";
					MediaController.WriteImageToFile(fileName, image);
					user.pathToPhoto = fileName;
				}
				else
				{
					Debug.WriteLine("UserController: Set to default photo");
					user.pathToPhoto = "default_profile.png";
				}
				Debug.WriteLine("Registered User: " + user);

				// Store api-key
				var japi_key = data["api_key"];
				user.api_key = japi_key.ToObject<string>();

				AddUserLocally(user);

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
		public async Task<bool> SendInviteToPerson(string email, string group, int person_id)
		{
			IDictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["email"] = email;
			parameters["group"] = group;
			parameters["person_id"] = person_id;
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.SendGenericPostRequest("user/invitations/send", parameters);

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
		public async Task<List<Invitation>> GetInvitations()
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.SendGetRequest("user/invitations/get", parameters);

			if (jobject != null)
			{
				var jtoken = jobject.GetValue("invites");
				var invitations = jtoken.ToObject<List<Invitation>>();

				var personController = new PersonController();

				foreach (Invitation i in invitations)
				{
					i.imageSource = await personController.GetPersonProfilePictureForInvite(i.person);
				}

				return invitations;
			}
			return null;
		}

		/// <summary>
		/// Gets the user profile picture.
		/// </summary>
		/// <returns>The user profile picture.</returns>
		public async Task<ImageSource> GetUserProfilePicture()
		{
			var user = Session.activeUser;
			ImageSource imageSource;
			if (user.pathToPhoto.Equals("default_profile.png"))
			{
				Debug.WriteLine("UserController: Getting default image.");
				imageSource = ImageSource.FromFile(user.pathToPhoto);
			}
			else if (user.pathToPhoto.StartsWith("ticket_to_talk", StringComparison.Ordinal))
			{
				Debug.WriteLine("UserController: Getting image from server.");
				var imgBytes = await DownloadUserProfilePicture();
				imageSource = ImageSource.FromStream(() => new MemoryStream(imgBytes));
			}
			else
			{
				Debug.WriteLine("UserController: Getting image from storage");
				var rawBytes = MediaController.ReadBytesFromFile(user.pathToPhoto);
				Debug.WriteLine("PersonController: fileSize " + rawBytes.Length);
				imageSource = ImageSource.FromStream(() => new MemoryStream(rawBytes));
			}
			return imageSource;
		}

		/// <summary>
		/// Downloads the user profile picture.
		/// </summary>
		/// <returns>The user profile picture.</returns>
		public async Task<byte[]> DownloadUserProfilePicture()
		{
			var user = Session.activeUser;

			var fileName = "u_" + user.id + ".jpg";
			Debug.WriteLine("UserController: Downloading profile picture with path - " + user.pathToPhoto);
			await networkController.DownloadFile(user.pathToPhoto, fileName);

			user.pathToPhoto = fileName;

			UpdateUserLocally(user);

			return MediaController.ReadBytesFromFile(user.pathToPhoto);
		}

		/// <summary>
		/// Accepts the invitation.
		/// </summary>
		/// <returns>The invitation.</returns>
		/// <param name="i">The index.</param>
		/// <param name="relation">Relation.</param>
		public async Task<bool> AcceptInvitation(Invitation i, string relation)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = i.person.id.ToString();
			parameters["relation"] = relation;
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.SendPostRequest("user/invitations/accept", parameters);

			if (jobject != null)
			{
				var pu = new PersonUser(i.person.id, Session.activeUser.id, i.group, relation);
				var puDB = new PersonUserDB();
				puDB.AddPersonUser(pu);
				puDB.Close();

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
		public async Task<bool> RejectInvitation(Person p)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["person_id"] = p.id.ToString();
			parameters["token"] = Session.Token.val;

			var jobject = await networkController.SendPostRequest("user/invitations/reject", parameters);

			if (jobject != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Bytes to hex.
		/// Source: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
		/// </summary>
		/// <returns>The to hex.</returns>
		/// <param name="ba">Ba.</param>
		private string ByteToHex(byte[] ba)
		{
			var hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				hex.AppendFormat("{0:X2}", b);
			}
			return hex.ToString();
		}
	}
}