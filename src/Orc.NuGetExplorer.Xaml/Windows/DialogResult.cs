namespace Orc.NuGetExplorer.Windows
{
    using System;
    using Catel;
    using Catel.Logging;

    internal class DialogResult<T> : DialogResult
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public T Result { get; private set; }

        public override void SetResult(IDialogOption result)
        {
            try
            {
                Argument.IsNotNull(() => result);

                if (result is IDialogOption<T>)
                {
                    Result = (result as IDialogOption<T>).DialogCallback();
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
            catch (InvalidCastException e)
            {
                Log.Error(e, $"Cannot set result value, expected type {typeof(T)} was {result.GetType()}");
            }
        }
    }

    internal class DialogResult
    {
        public virtual void SetResult(IDialogOption result)
        {
        }
    }
}
