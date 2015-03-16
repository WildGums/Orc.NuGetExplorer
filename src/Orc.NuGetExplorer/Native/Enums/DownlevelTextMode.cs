// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownlevelTextMode.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    internal enum DownlevelTextMode
    {
        /// <summary>
        /// The text of the <see cref="CredentialDialog.MainInstruction"/> and <see cref="CredentialDialog.Content"/> properties is
        /// concatenated together, separated by an empty line.
        /// </summary>
        MainInstructionAndContent,

        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.MainInstruction"/> property is shown.
        /// </summary>
        MainInstructionOnly,

        /// <summary>
        /// Only the text of the <see cref="CredentialDialog.Content"/> property is shown.
        /// </summary>
        ContentOnly
    }
}