using BlazorTodo.Base;

namespace BlazorTodo.Server.Services.DIexampleWithCommonModel
{
    public interface ICommonRepository<T> where T : CommonModel
    {
        Task<T> AddAsync(T item);
        Task DeleteAsync(string id);
    }
}
