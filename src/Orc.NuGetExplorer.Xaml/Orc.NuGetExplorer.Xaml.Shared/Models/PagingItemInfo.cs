// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingItemInfo.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Data;

    internal class PagingItemInfo : ModelBase
    {
        #region Constructors
        public PagingItemInfo(string header, int stepValue)
        {
            Argument.IsNotNullOrWhitespace(() => header);

            Header = header;
            StepValue = stepValue;
        }
        #endregion

        #region Properties
        public string Header { get; private set; }
        public int StepValue { get; private set; }
        #endregion
    }
}