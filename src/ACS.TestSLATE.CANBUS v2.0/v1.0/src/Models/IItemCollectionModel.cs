using System.Collections.Specialized;
using Customer.TestSLATE.Mnemonic.Presenters;
using Jacobs.TestSLATE.Commands.Interfaces;

namespace Customer.TestSLATE.Mnemonic.Models
{
    /// <summary>
    /// This interface defines a common depiction of a model.
    /// </summary>
    public interface IItemCollectionModel
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
        UndoableCommand AddNew (NamedConfigItem NamedConfigItem);

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="stringValue"></param>
        /// <param name="intValue"></param>
        /// <param name="floatValue"></param>
        /// <returns></returns>
        UndoableCommand UpdateValue(int index, string stringValue, int intValue, float floatValue);

        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        NamedConfigItem GetItemAt(int index);

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
    }
}