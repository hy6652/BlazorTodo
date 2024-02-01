using BlazorTodo.Base;

namespace BlazorTodo.Shared
{
    public class User
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UserWithCommonModel : CommonModel
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
