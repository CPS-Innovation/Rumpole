using Microsoft.Extensions.Primitives;

namespace RumpoleGateway.Helpers.Extension
{
    public static class StringValuesHelper
    {
        public static string ToJwtString(this StringValues values)
        {
            return values.ToString().Replace($"{Constants.Authentication.Bearer} ", string.Empty).Trim();
        }
    }
}
