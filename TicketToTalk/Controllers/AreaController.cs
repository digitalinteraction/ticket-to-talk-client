namespace TicketToTalk
{
	/// <summary>
	/// A Controller for the Area Model.
	/// </summary>
	public class AreaController
	{

		private AreaDB areaDB = new AreaDB();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TicketToTalk.AreaController"/> class.
		/// </summary>
		public AreaController()
		{
		}

		/// <summary>
		/// Gets the area from an ID value.
		/// </summary>
		/// <returns>The area.</returns>
		/// <param name="id">Identifier.</param>
		public Area GetArea(int id)
		{
			areaDB.Open();
			var area = areaDB.GetArea(id);
			areaDB.Close();
			return area;
		}

		/// <summary>
		/// Adds the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void AddAreaLocally(Area area)
		{
			areaDB.Open();
			areaDB.AddArea(area);
			areaDB.Close();
		}

		/// <summary>
		/// Deletes the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="id">Identifier.</param>
		public void DeleteAreaLocally(int id)
		{
			areaDB.Open();
			areaDB.DeleteArea(id);
			areaDB.Close();
		}

		/// <summary>
		/// Updates the area locally.
		/// </summary>
		/// <returns>The area locally.</returns>
		/// <param name="area">Area.</param>
		public void UpdateAreaLocally(Area area)
		{
			DeleteAreaLocally(area.id);
			AddAreaLocally(area);
		}
	}
}

