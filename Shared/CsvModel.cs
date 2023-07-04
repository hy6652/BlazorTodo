﻿using CsvHelper.Configuration.Attributes;

namespace BlazorTodo.Shared
{
    public class CsvModel
    {
        [Name("id")]
        public int Id { get; set; }
        [Name("movie")]
        public string Movie { get; set; } = string.Empty;
        [Name("rating")]
        public double Rating { get; set; }
    }
}
