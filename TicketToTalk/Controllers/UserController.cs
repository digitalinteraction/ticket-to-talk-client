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
		/// Updates the user remotely.
		/// </summary>
		/// <param name="user">User.</param>
		public async Task<User> updateUserRemotely(User user, byte[] image)
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

			var jobject = await networkController.sendGenericPostRequest("user/update", parameters);
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
					MediaController.writeImageToFile(Session.activeUser.pathToPhoto, image);
				}

				updateUserLocally(user);

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
		public async Task<bool> authenticateUser(string email, string password)
		{
			IDictionary<string, string> credentials = new Dictionary<string, string>();
			credentials["email"] = email;
			credentials["password"] = password.HashString();

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
				var local_user = userController.getLocalUserByEmail(returned_user.email);
				if (local_user == null)
				{
					returned_user.firstLogin = true;
					userController.addUserLocally(returned_user);
					Session.activeUser = returned_user;
				}
				else
				{
					returned_user.firstLogin = false;

					if (returned_user.imageHash != null)
					{
						if (local_user.imageHash == null)
						{
							await downloadUserProfilePicture();
						}
						else if (!returned_user.imageHash.Equals(local_user.imageHash))
						{
							await downloadUserProfilePicture();
						}
						local_user.pathToPhoto = "u_" + local_user.id + ".jpg";
					}

					returned_user.pathToPhoto = local_user.pathToPhoto;
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
			SHA256 sha = new SHA256Managed();
			byte[] passBytes = Encoding.UTF8.GetBytes(user.password);
			byte[] hash = sha.ComputeHash(passBytes);
			user.password = byteToHex(hash);

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

				if (image != null && image.Length > 0)
				{
					var fileName = "u_" + user.id + ".jpg";
					MediaController.writeImageToFile(fileName, image);
					user.pathToPhoto = fileName;
				}
				else
				{
					Debug.WriteLine("UserController: Set to default photo");
					user.pathToPhoto = "default_profile.png";
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

			var jobject = await networkController.sendGenericPostRequest("user/invitations/send", parameters);

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
		/// Gets the user profile picture.
		/// </summary>
		/// <returns>The user profile picture.</returns>
		/// <param name="user">User.</param>
		public async Task<ImageSource> getUserProfilePicture()
		{
			var user = Session.activeUser;
			ImageSource imageSource;
			if (user.pathToPhoto.Equals("default_profile.png"))
			{
				Debug.WriteLine("UserController: Getting default image.");
				imageSource = ImageSource.FromFile(user.pathToPhoto);
			}
			else if (user.pathToPhoto.StartsWith("storage", StringComparison.Ordinal))
			{
				Debug.WriteLine("UserController: Getting image from server.");
				var imgBytes = await downloadUserProfilePicture();
				imageSource = ImageSource.FromStream(() => new MemoryStream(imgBytes));
			}
			else
			{
				Debug.WriteLine("UserController: Getting image from storage");
				var rawBytes = MediaController.readBytesFromFile(user.pathToPhoto);
				Debug.WriteLine("PersonController: fileSize " + rawBytes.Length);
				imageSource = ImageSource.FromStream(() => new MemoryStream(rawBytes));
			}
			return imageSource;
		}

		/// <summary>
		/// Downloads the user profile picture.
		/// </summary>
		/// <returns>The user profile picture.</returns>
		/// <param name="user">User.</param>
		public async Task<byte[]> downloadUserProfilePicture()
		{
			var user = Session.activeUser;

			var fileName = "u_" + user.id + ".jpg";
			Debug.WriteLine("UserController: Downloading profile picture with path - " + user.pathToPhoto);
			var task = await networkController.downloadFile(user.pathToPhoto, fileName);

			user.pathToPhoto = fileName;

			updateUserLocally(user);

			return MediaController.readBytesFromFile(user.pathToPhoto);
		}

		/// <summary>
		/// Accepts the invitation.
		/// </summary>
		/// <returns>The invitation.</returns>
		/// <param name="i">The index.</param>
		/// <param name="relation">Relation.</param>
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

		/// <summary>
		/// Bytes to hex.
		/// Source: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
		/// </summary>
		/// <returns>The to hex.</returns>
		/// <param name="ba">Ba.</param>
		private string byteToHex(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				hex.AppendFormat("{0:X2}", b);
			}
			return hex.ToString();
		}
	}
}