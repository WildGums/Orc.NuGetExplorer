// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextSpy.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Windows;

#if DEBUG
    using System.ComponentModel;
    using System.Windows.Data;
#endif

    // this code has been found at http://www.codeproject.com/Articles/27432/Artificial-Inheritance-Contexts-in-WPF
    public class DataContextSpy
        : Freezable // Enable ElementName and DataContext bindings
    {
#if DEBUG
        #region Constructors
        public DataContextSpy()
        {
            // This binding allows the spy to inherit a DataContext.
            BindingOperations.SetBinding(this, DataContextProperty, new Binding());

            this.IsSynchronizedWithCurrentItem = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets whether the spy will return the CurrentItem of the 
        /// ICollectionView that wraps the data context, assuming it is
        /// a collection of some sort. If the data context is not a 
        /// collection, this property has no effect. 
        /// The default value is true.
        /// </summary>
        public bool IsSynchronizedWithCurrentItem { get; set; }

        public object DataContext
        {
            get { return (object) GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }
        #endregion

        #region Methods
        private static object OnCoerceDataContext(DependencyObject depObj, object value)
        {
            DataContextSpy spy = depObj as DataContextSpy;
            if (spy == null)
            {
                return value;
            }

            if (spy.IsSynchronizedWithCurrentItem)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(value);
                if (view != null)
                {
                    return view.CurrentItem;
                }
            }

            return value;
        }
        #endregion

        // Borrow the DataContext dependency property from FrameworkElement.
        public static readonly DependencyProperty DataContextProperty = FrameworkElement.DataContextProperty.AddOwner(typeof (DataContextSpy), new PropertyMetadata(null, null, OnCoerceDataContext));
#endif

        protected override Freezable CreateInstanceCore()
        {
            // We are required to override this abstract method.
            throw new NotImplementedException();
        }
    }
}