namespace Customer.TestSLATE.Mnemonic.Models
{
    using System.Collections.Specialized;
    using Jacobs.TestSLATE.Commands.Commands.Collection;

    /// <summary>
    /// This interface defines a common depiction of a model.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsDirty { get; set; }

        /// <summary>
        /// Occurs when [collection changed].
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;


        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        Jacobs.TestSLATE.Commands.Interfaces.UndoableCommand AddNew();

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        Jacobs.TestSLATE.Commands.Interfaces.UndoableCommand Insert(int index);

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        Jacobs.TestSLATE.Commands.Interfaces.UndoableCommand Remove(int index);

        /// <summary>
        /// Saves this instance.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        string GetErrorMessage(int index, string propertyName);

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Swaps the items.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="targetRow">The target row.</param>
        void SwapItems(int sourceRow, int targetRow);
    }
}