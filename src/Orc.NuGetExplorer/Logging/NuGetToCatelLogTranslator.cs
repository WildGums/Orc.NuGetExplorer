// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetToCatelLogTranstalor.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Logging;

    public class NuGetToCatelLogTranslator : PackageManagerLogListenerBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly string GetHttpRequestInfoPattern = "  GET https";
        private static readonly string SetHttpRequestInfoPattern = "  SET https";
        private static readonly string OkHttpRequestInfoPattern = "  OK https";
        private static readonly string NotFoundHttpRequestInfoPattern = "  NotFound https";
        private static readonly string CacheHttpRequestInfoPattern = "  CACHE https";
        #endregion

        #region Constructors
        public NuGetToCatelLogTranslator(INuGetLogListeningSevice nuGetLogListeningSevice)
            : base(nuGetLogListeningSevice)
        {
        }
        #endregion

        #region Methods
        protected override void OnInfo(object sender, NuGetLogRecordEventArgs e)
        {
            if (e.Message.StartsWith(GetHttpRequestInfoPattern) || e.Message.StartsWith(SetHttpRequestInfoPattern) 
                || e.Message.StartsWith(OkHttpRequestInfoPattern) || e.Message.StartsWith(NotFoundHttpRequestInfoPattern) || e.Message.StartsWith(CacheHttpRequestInfoPattern))
            {
                Log.Debug(e.Message);
                return;
            }
            Log.Info(e.Message);
        }

        protected override void OnDebug(object sender, NuGetLogRecordEventArgs e)
        {
            Log.Debug(e.Message);
        }

        protected override void OnWarning(object sender, NuGetLogRecordEventArgs e)
        {
            Log.Warning(e.Message);
        }

        protected override void OnError(object sender, NuGetLogRecordEventArgs e)
        {
            Log.Error(e.Message);
        }
        #endregion
    }
}
