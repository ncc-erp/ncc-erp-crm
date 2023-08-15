using System;
using Newtonsoft.Json;

namespace CRM.DynamicFilter
{
    public class ExpressionFilter
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public ComparisonOperator Comparison { get; set; }

        [JsonIgnore]
        public string ActualPropertyName { get; set; }
        [JsonIgnore]
        public object ActualValue { get; set; }
        [JsonIgnore]
        public Type PropertyType { get; set; }
    }

    public enum ComparisonOperator
    {
        Equal = 0,
        LessThan = 1,
        LessThanOrEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        NotEqual = 5,
        Contains = 6, //for strings  
        StartsWith = 7, //for strings  
        EndsWith = 8, //for strings  
        In = 9 // for list item
    }
}
