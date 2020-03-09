using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randomous.ContentSystem
{
    public interface IContentProvider<T,B,S> where T : BaseSystemObject where B : BaseSystemObject
    {
        Task<List<B>> GetBasicAsync(S search);
        Task WriteAsync(IEnumerable<T> items);

        /// <summary>
        /// Expand basic items into more complex ones (only do this when necessary!)
        /// </summary>
        /// <remarks>
        /// May not be applicable to every provider!
        /// </remarks>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<List<T>> ExpandAsync(IEnumerable<B> items);

        //You'll probably need some kind of listen here... unless you want it per-provider.
    }

    public interface IUserProvider : IContentProvider<User, BasicUser, int> { }
    public interface IPermissionProvider : IContentProvider<BasicPermission, BasicPermission, int> { }
}