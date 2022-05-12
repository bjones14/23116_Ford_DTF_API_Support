// <copyright file="IExportable.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This interface defines basic methods for providing export capabilities.</summary>

namespace Customer.TestSLATE.Mnemonic.UI
{
    /// <summary>
    ///   This interface indicates that this class is capable of exporting data to files.
    /// </summary>
    public interface IExportable
    {
        /// <summary>
        ///   Exports this instance to file.
        /// </summary>
        void Export();

        /// <summary>
        ///   Exports this instance to file using the specified format specifier.
        /// </summary>
        /// <param name = "formatSpecifier">The format specifier.</param>
        /// <remarks>
        ///   Valid format specifiers are: 'xlsx' and 'pdf'
        /// </remarks>
        void Export(string formatSpecifier);
    }
}