using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTodo.Base
{
    public abstract class RepositoryModelBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty(PropertyName = "pk")]
        public string Pk { get; set; } = string.Empty;
        public abstract string ClassType { get; }

        public abstract void SetupPk();
    }
}
