// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchHelper.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public static class DispatchHelper
    {
        private static Dispatcher Dispatcher = Application.Current.Dispatcher;

        public static void DispatchIfNecessary(Action action)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}