// <copyright file="IReportable.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This interface defines basic methods for providing reporting capabilities.</summary>

namespace Customer.TestSLATE.Mnemonic.UI
{
    /// <summary>
    ///   This interface indicates that this class is capable of printing and writing reports to file.
    /// </summary>
    public interface IReportable
    {
        /// <summary>
        ///   Prints the report.
        /// </summary>
        void PrintReport();

        /// <summary>
        ///   Writes the report to file.
        /// </summary>
        void WriteReportToFile();

        /// <summary>
        ///   Writes the report to file.
        /// </summary>
        /// <param name = "formatSpecifier">The format specifier.</param>
        /// <remarks>
        ///   Valid format specifiers are: 'csv' and 'pdf'
        /// </remarks>
        void WriteReportToFile(string formatSpecifier);
    }
}