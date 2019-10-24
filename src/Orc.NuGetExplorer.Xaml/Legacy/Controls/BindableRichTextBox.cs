// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindableRichTextBox.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    internal class BindableRichTextBox : RichTextBox
    {
        #region Properties
        public FlowDocument BindableDocument
        {
            get { return (FlowDocument) GetValue(BindableDocumentProperty); }
            set { SetValue(BindableDocumentProperty, value); }
        }

        public static readonly DependencyProperty BindableDocumentProperty =
            DependencyProperty.Register(nameof(BindableDocument), typeof(FlowDocument),
                typeof(BindableRichTextBox), new PropertyMetadata(OnBindableDocumentChanged));
        #endregion

        #region Methods
        private static void OnBindableDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisControl = (BindableRichTextBox) d;

            thisControl.Document = (e.NewValue == null) ? new FlowDocument() : (FlowDocument) e.NewValue;
        }
        #endregion        
    }
}