using System.Threading.Tasks;

namespace OnionSeed.Data
{
	/// <summary>
	/// Defines an atomic transaction whose changes are to be asynchronously committed as a collective whole.
	/// </summary>
	public interface IAsyncUnitOfWork
	{
		/// <summary>
		/// Commits any pending changes.
		/// </summary>
		/// <returns>A task representing the operation.</returns>
		Task CommitAsync();
	}
}
