namespace Lightweight.Business.Helpers
{
    public class UrlHelper
    {
        public static string CombineUrl(string baseUrl, string relativeUrl)
        {
            baseUrl = baseUrl.TrimEnd('/');
            relativeUrl = string.IsNullOrEmpty(relativeUrl) ? string.Empty : relativeUrl.TrimStart('/');
            return string.Format("{0}/{1}", baseUrl, relativeUrl);
        }
    }
}
