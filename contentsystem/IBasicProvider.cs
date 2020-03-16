using System.Collections.Generic;
using System.Threading.Tasks;

namespace Randomous.ContentSystem
{
    public interface IBasicProvider<T,B,S> where T : BaseSystemObject where B : BaseSystemObject where S : BaseSearch
    {
        Task<List<B>> GetBasicAsync(S search);
        Task WriteAsync(IEnumerable<T> items, bool writeIds = true);

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

    public interface IUserProvider : IBasicProvider<User, BasicUser, UserSearch> { }
    public interface IContentProvider : IBasicProvider<Content, BasicContent, ContentSearch> { }
    //public interface IPermissionProvider : IBasicProvider<BasicPermission, BasicPermission, PermissionSearch> { }
    public interface IRelationProvider : IBasicProvider<BasicRelation, BasicRelation, RelationSearch> { }
    public interface IValueProvider : IBasicProvider<BasicValue, BasicValue, ValueSearch> { }
}