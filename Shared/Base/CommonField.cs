
namespace BlazorTodo.Base
{
    public class CommonField
    {
        public List<int>? IntValues { get; set; }
        public List<double>? DoubleValues { get; set; }
        public List<string>? StringValues { get; set; }
        public List<DateTime>? DateTimeValues { get; set; }
        public List<bool>? BoolValues { get; set; }
        public CommonField()
        {

        }
        public CommonField(IEnumerable<DateTime> dateTimes)
        {
            this.DateTimeValues = dateTimes.ToList();
        }
        public CommonField(IEnumerable<double> doubles)
        {
            this.DoubleValues = doubles.ToList();
        }
        public CommonField(IEnumerable<string> strings)
        {
            this.StringValues = strings.ToList();
        }
        public CommonField(IEnumerable<int> ints)
        {
            this.IntValues = ints.ToList();
        }
        public CommonField(IEnumerable<bool> bools)
        {
            this.BoolValues = bools.ToList();
        }

        /*
        public string ToDisplayString(FieldDefinition fieldDefinition)
        {
            string? v = fieldDefinition.FieldType switch
            {
                FieldTypes.FieldInt => IntValues?.FirstOrDefault().ToString(),
                FieldTypes.FieldDouble => DoubleValues?.FirstOrDefault().ToString(),
                FieldTypes.FieldString => StringValues?.FirstOrDefault(),
                FieldTypes.FieldDateTime => DateTimeValues?.FirstOrDefault().ToString("yyyy-MM-dd"),
                FieldTypes.FieldBool => BoolValues?.FirstOrDefault().ToString(),
                _ => "",
            };

            string result = v!;

            return result;
        }*/
    }
}
