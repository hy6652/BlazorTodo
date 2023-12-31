﻿namespace BlazorTodo.Shared
{
    public class TodoItem : CosmosModelBase
    {
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public bool IsDone { get; set; } = false;
        public override string ClassType => "Todo";
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }
}
