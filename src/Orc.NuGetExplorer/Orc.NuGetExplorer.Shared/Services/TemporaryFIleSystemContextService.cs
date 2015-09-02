// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFIleSystemContextService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using Catel.IoC;

    internal class TemporaryFIleSystemContextService : ITemporaryFIleSystemContextService
    {
        #region Fields
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public TemporaryFIleSystemContextService(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }
        #endregion

        #region Properties
        public ITemporaryFileSystemContext Context { get; private set; }
        #endregion

        #region Methods
        public IDisposable UseTemporaryFIleSystemContext()
        {
            var context = _typeFactory.CreateInstance<TemporaryFileSystemContext>();
            return new DisposableToken<ITemporaryFileSystemContext>(context, token => { }, token => { });
        }
        #endregion
    }
}