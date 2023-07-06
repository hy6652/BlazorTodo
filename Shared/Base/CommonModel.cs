namespace BlazorTodo.Base
{
    public class CommonModel : RepositoryModelBase
    {
        public override string ClassType => nameof(CommonModel);
        public string Title { get; set; } = string.Empty;
        public string SchemaId { get; set; } = string.Empty;
        public string SchemaName { get; set; } = string.Empty;
        public Dictionary<string, CommonField> FieldDict { get; set; } = new();

        public string DefaultLabelField { get; set; } = string.Empty;
        public string DefaultValueField { get; set; } = string.Empty;

        public string GetLabel(string field = "")
        {
            if (string.IsNullOrEmpty(field))
            {
                if (string.IsNullOrEmpty(DefaultLabelField))
                {
                    return Title;
                }
                else
                {
                    field = DefaultLabelField;
                }
            }
            else if (FieldDict.ContainsKey(field))
            {
                return FieldDict[field].StringValues.FirstOrDefault();
            }
            else
            {
                field = DefaultLabelField;
            }
            return field;
        }

        public string GetValue(string field = "")
        {
            if (string.IsNullOrEmpty(field))
            {
                if (string.IsNullOrEmpty(DefaultValueField))
                {
                    return Id;
                }
                else
                {
                    field = DefaultValueField;
                }
            }

            return FieldDict[field].StringValues.FirstOrDefault();
        }

        public virtual CommonField? GetCommonField(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                if (string.IsNullOrEmpty(DefaultValueField))
                {
                    key = "Id";
                }
                else
                {
                    key = DefaultValueField;
                }
            }

            return FieldDict.GetValueOrDefault(key);
        }

        public override void SetupPk()
        {
            Pk = Id;
        }
    }
}
