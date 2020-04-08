namespace Orc.NuGetExplorer.Converters
{
    using System;
    using Catel.MVVM.Converters;
    using Orc.NuGetExplorer;

    public class InvalidFeedToBoolConverter : ValueConverterBase<FeedVerificationResult, bool>
    {
        protected override object Convert(FeedVerificationResult value, Type targetType, object parameter)
        {
            return value == FeedVerificationResult.Invalid;
        }
    }
}
