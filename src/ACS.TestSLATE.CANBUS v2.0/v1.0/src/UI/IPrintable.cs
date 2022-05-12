// <copyright file="IPrintable.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This interface defines basic methods for providing printing capabilities.</summary>

namespace Customer.TestSLATE.Mnemonic.UI
{
    /// <summary>
    ///   This interface indicates that this class is capable of printing.
    /// </summary>
    public interface IPrintable
    {
        /// <summary>
        ///   Prints this instance.
        /// </summary>
        void Print();
    }
}