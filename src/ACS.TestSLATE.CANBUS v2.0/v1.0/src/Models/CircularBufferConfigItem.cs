// <copyright file="Channel.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class represents a Reflective Memory Configuration Item.
//          All Data Models should ideally implement the following:
//          IComparable - for sorting capabilities
//          IEquatable - to determinie equality (along with overriding HashCode())
//          IEditableObject to allow for transactional user interaction
//          INotifyPropertyChanged to allow the View, Presenter and Validators to respond to changes
//          ToString() should also be overriden to allow for textual data to be written out when debugging.
// </summary>

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;
using Jacobs.TestSLATE.DB;
using Jacobs.TestSLATE.PluginTemplate.Properties;
using TSChannel = Jacobs.TestSLATE.Cell.DomainLayer.Channels.ObjectClass;
using trfm = Jacobs.TestSLATE.Cell.DomainLayer.IObjectClass;
using Jacobs.TestSLATE.ExtensionDoc;

namespace Jacobs.TestSLATE.PluginTemplate.Models
{
    /// <summary>
    ///   An extension of the base Jacobs.TestVIEW.Cell.DomainLayer.Channels.ObjectClass with validation built in.
    /// </summary>
    [Serializable]
    public class CircularBufferConfigItem : IComparable<CircularBufferConfigItem>, IEquatable<CircularBufferConfigItem>, IEditableObject, INotifyPropertyChanged
    {

        ///<summary>
        ///</summary>
        public string _ItemName;

        ///<summary>
        ///</summary>
        public string _ItemValue;

        ///<summary>
        ///</summary>
        public int CellID;
        ///<summary>
        ///</summary>
        public int SourceID;

        internal bool DirtyFlag;
        /// <summary>
        ///   Backup of this instance.
        ///   This preserves the state of this model during user edits, before the changes are committed.
        /// </summary>
        private RFMConfigItem _backup;

        /// <summary>
        ///   Indicates that the user is still making changes and has not committed them.
        /// </summary>
        private bool _inTransaction;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RFMConfigItem" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        public CircularBufferConfigItem(string ItemName, string ItemValue)
        {
            this.Validator = new RFMConfigItemValidator(this);
            this._ItemName = ItemName;
            this._ItemValue = ItemValue;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RFMConfigItem" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        public CircularBufferConfigItem()
        {
            this.Validator = new RFMConfigItemValidator(this);
            this._ItemName = "";
            this._ItemValue = "";
        }

        /// <summary>
        ///   Gets or sets the Item Name.
        /// </summary>
        /// <value>The Item Name.</value>
        [XmlElement("ItemName")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string ItemName
        {
            get { return this._ItemName; }

            set
            {
                if (value == this._ItemName) return;
                this._ItemName = value;
                this.NotifyPropertyChanged("ItemName");
            }
        }

        /// <summary>
        ///   Gets or sets the Item Value.
        /// </summary>
        /// <value>The Item Value.</value>
        [XmlElement("ItemValue")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string ItemValue
        {
            get { return this._ItemValue; }

            set
            {
                if (value == this._ItemValue) return;
                this._ItemValue = value;
                this.NotifyPropertyChanged("ItemValue");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "RFMConfigItem" /> is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dirty; otherwise, <c>false</c>.
        /// </value>
        public bool Dirty
        {
            get { return this.DirtyFlag; }
            set { this.DirtyFlag = value; }
        }

        /// <summary>
        ///   The balance validator.
        /// </summary>
        internal RFMConfigItemValidator Validator { get; private set; }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        ///   Specifically this compares balances by BalanceOrder.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(RFMConfigItem other)
        {
            int thisaddr, otheraddr;
            thisaddr = System.Convert.ToInt32(this._ItemValue);
            otheraddr = System.Convert.ToInt32(other._ItemValue);
            return Math.Sign(thisaddr - otheraddr);
        }

        /// <summary>
        ///   Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if (this._inTransaction) return;
            this._backup = CreateNewCircularBufferConfigItem("", "0");

            this._backup._ItemName = this._ItemName;
            this._backup._ItemValue = this._ItemValue;
            this._inTransaction = true;
        }

        /// <summary>
        ///   Discards changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> call.
        /// </summary>
        public void CancelEdit()
        {
            if (!this._inTransaction) return;
            this._ItemName = this._backup._ItemName;
            this._ItemValue = this._backup._ItemValue;
            this._inTransaction = false;
            this.Dirty = false;
        }

        /// <summary>
        ///   Pushes changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> or <see cref = "M:System.ComponentModel.IBindingList.AddNew"></see> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            if (!this._inTransaction) return;
            this._backup = CreateNewCircularBufferConfigItem("", "0");
            this.Validator.Validate();
            this._inTransaction = false;
            this.Dirty = true;
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   True if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(RFMConfigItem other)
        {
            return this._ItemName == other._ItemName &&
                this._ItemValue == other._ItemValue;
        }

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        ///   This is the public factory method that returns the default object.
        /// </summary>
        /// <returns></returns>
        public static RFMConfigItem CreateNewCircularBufferConfigItem(string itemName, string itemAddr)
        {
            return new RFMConfigItem (itemName, itemAddr);
        }

        /// <summary>
        ///   Returns a <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String"></see> that represents the current <see cref = "T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("ID: " + this._ItemName + "\t");
            builder.Append(this._ItemValue);
            return builder.ToString();
        }

        /// <summary>
        ///   Notifies the property changed.
        /// </summary>
        /// <param name = "info">The info.</param>
        internal void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            this.Dirty = true;
        }

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        /// <param name = "cellId">The cell id.</param>
        /// <param name = "srcId">The config id.</param>
        public void Save(int cellId, int srcId)
        {
            this.EndEdit();
            this.CellID = cellId;
            this.SourceID = srcId;
            this.Dirty = false;
        }
    }
}