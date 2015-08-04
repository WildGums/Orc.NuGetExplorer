// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetToCatelLogTranstalor.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Logging;

    public class NuGetToCatelLogTranslator : PackageManagerLogListenerBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
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