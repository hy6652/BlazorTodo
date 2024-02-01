using BlazorTodo.Base;
using BlazorTodo.Shared;
using System.Diagnostics;

namespace BlazorTodo.Server.Services.DIexampleWithCommonModel
{
    public class CommonRepository<T> : ICommonRepository<T> where T : CommonModel
    {
        public async Task<T> AddAsync(T item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = Guid.NewGuid().ToString();
            }
            item.SetupPk();

            return item;
        }

        public async Task DeleteAsync(string id)
        {
            Debug.WriteLine($"Delete something : {id}");
        }
    }
}
