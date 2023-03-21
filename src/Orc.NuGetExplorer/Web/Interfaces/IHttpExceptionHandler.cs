namespace Orc.NuGetExplorer.Web;

public interface IHttpExceptionHandler<T>
{
    FeedVerificationResult HandleException(T exception, string source);
}