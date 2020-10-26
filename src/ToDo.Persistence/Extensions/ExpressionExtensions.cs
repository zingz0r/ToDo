using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ToDo.Persistence.Extensions
{
    public static class ExpressionExtensions
    {
        public static string ToReadableString(this Expression expression)
        {
            var regex = new Regex(@"value\(\S*\)\.");
            return regex.Replace(expression.ToString(), "");
        }
    }
}