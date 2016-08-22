using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TicketToTalk
{
	/// <summary>
	/// Tag controller.
	/// </summary>
	public class TagController
	{
		TagDB tagDB = new TagDB();
		public TagController()
		{
		}

		/// <summary>
		/// Adds the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="tag">Tag.</param>
		public void addTagLocally(Tag tag) 
		{
			tagDB.open();
			tagDB.AddTag(tag);
			tagDB.close();
		}

		/// <summary>
		/// Deletes the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="id">Identifier.</param>
		public void deleteTagLocally(int id) 
		{
			tagDB.open();
			tagDB.DeleteTag(id);
			tagDB.close();
		}

		/// <summary>
		/// Updates the tag locally.
		/// </summary>
		/// <returns>The tag locally.</returns>
		/// <param name="t">T.</param>
		public void updateTagLocally(Tag t) 
		{
			deleteTagLocally(t.id);
			addTagLocally(t);
		}

		/// <summary>
		/// Gets the tag.
		/// </summary>
		/// <returns>The tag.</returns>
		/// <param name="id">Identifier.</param>
		public Tag getTag(int id) 
		{
			tagDB.open();
			var tag = tagDB.GetTag(id);
			tagDB.close();
			return tag;
		}

		public List<Tag> getTags() 
		{
			tagDB.open();
			var tags = tagDB.GetTags();
			tagDB.close();

			var list = new List<Tag>();
			foreach (Tag t in tags) 
			{
				list.Add(t);
			}
			return list;
		}

		/// <summary>
		/// Adds the tag to server.
		/// </summary>
		/// <returns>if new tag</returns>
		/// <param name="tag">Tag.</param>
		public async Task<bool> addTagToServer(Tag tag)
		{
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["text"] = tag.text;
			parameters["token"] = Session.Token.val;

			NetworkController net = new NetworkController();
			var jobject = await net.sendPostRequest("tags/store", parameters);
			var jtoken = jobject.GetValue("tag");
			var returned_tag = jtoken.ToObject<Tag>();

			Console.WriteLine(tag);

			// Check tag already exists.
			var stored = getTag(returned_tag.id);

			bool exists = false;
			if (stored != null)
			{
				exists = true;
			}
			else 
			{
				addTagLocally(tag);
			}

			return exists;
		}
	}
}

