// <copyright file="Balance.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class represents a sample data model. It is built around (composition) a TestSLATE domain class (Balance).
//          All Data Models should ideally implement the following:
//          IComparable - for sorting capabilities
//          IEquatable - to determinie equality (along with overriding HashCode())
//          IEditableObject to allow for transactional user interaction
//          INotifyPropertyChanged to allow the View, Presenter and Validators to respond to changes
//          ToString() should also be overriden to allow for textual data to be written out when debugging.
// </summary>

namespace Customer.TestSLATE.Mnemonic.Models
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Text;
    using System.Xml.Serialization;
    using Jacobs.TestSLATE.DB;
    using Customer.TestSLATE.Mnemonic.Properties;
    using TSBalance = Jacobs.TestSLATE.Cell.DomainLayer.Balances.ObjectClass;

    /// <summary>
    ///   An extension of the base Jacobs.TestVIEW.Cell.DomainLayer.Balances.ObjectClass with validation built in.
    /// </summary>
    [Serializable]
    public sealed class Balance : IComparable<Balance>, IEquatable<Balance>, IEditableObject, INotifyPropertyChanged
    {
        /// <summary>
        ///   The Test SLATE DomainLayer Balance.
        /// </summary>
        private readonly TSBalance _balance;

        /// <summary>
        ///   Backup of this instance.
        ///   This preserves the state of this model during user edits, before the changes are committed.
        /// </summary>
        private Balance _backup;

        /// <summary>
        ///   Indicates that the user is still making changes and has not committed them.
        /// </summary>
        private bool _inTransaction;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Balance" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        private Balance()
        {
            this.Validator = new BalanceValidator(this);
            this._balance = new TSBalance(EnvironmentFactory.CellEnvironment())
                                {
                                    AeroTareFilePath = Resources.DefaultFilePath,
                                    CalibrationFilePath = Resources.DefaultFilePath,
                                    WeightTareFilePath = Resources.DefaultFilePath,
                                    BalanceDescr = Resources.DefaultDescription,
                                    BalanceName = Resources.DefaultBalName,
                                    BalanceOrder = 0,
                                    BalanceReference = 0,
                                    NumberOfReadings = 6,
                                    BalanceID = -1
                                };
        }

        /// <summary>
        ///   Creates a new instance of the <see cref = "Balance" /> class from a TSBalance.
        ///   Copy Constructor.
        /// </summary>
        /// <param name = "balance">The balance.</param>
        public Balance(TSBalance balance)
        {
            this._balance = balance;
            this.Validator = new BalanceValidator(this);
        }

        /// <summary>
        ///   Gets or sets the name of the balance.
        /// </summary>
        /// <value>The name of the balance.</value>
        [XmlElement("BalanceName")]
        public string BalanceName
        {
            get { return this._balance.BalanceName; }

            set
            {
                if (value == this._balance.BalanceName) return;
                this._balance.BalanceName = value;
                this.NotifyPropertyChanged("BalanceName");
            }
        }

        /// <summary>
        /// Gets or sets the balance descr.
        /// </summary>
        /// <value>
        /// The balance descr.
        /// </value>
        [XmlElement("BalanceDescr")]
        public string BalanceDescr
        {
            get { return this._balance.BalanceDescr; }
            set 
            {
                if (value == this._balance.BalanceDescr) return;
                this._balance.BalanceDescr = value;
                this.NotifyPropertyChanged("BalanceDescr");
            }
        }

        /// <summary>
        ///   Gets or sets the aero tare file path.
        /// </summary>
        /// <value>The aero tare file path.</value>
        [XmlElement("AeroTareFilePath")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string AeroTareFilePath
        {
            get { return this._balance.AeroTareFilePath; }

            set
            {
                if (value == this._balance.AeroTareFilePath) return;
                this._balance.AeroTareFilePath = value;
                this.NotifyPropertyChanged("AeroTareFilePath");
            }
        }

        /// <summary>
        ///   Gets or sets the calibration file path.
        /// </summary>
        /// <value>The calibration file path.</value>
        [XmlElement("CalibrationFilePath")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string CalibrationFilePath
        {
            get { return this._balance.CalibrationFilePath; }

            set
            {
                if (value == this._balance.CalibrationFilePath) return;
                this._balance.CalibrationFilePath = value;
                this.NotifyPropertyChanged("CalibrationFilePath");
            }
        }

        /// <summary>
        ///   Gets or sets the weight tare file path.
        /// </summary>
        /// <value>The weight tare file path.</value>
        [XmlElement("WeightTareFilePath")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string WeightTareFilePath
        {
            get { return this._balance.WeightTareFilePath; }

            set
            {
                if (value == this._balance.WeightTareFilePath) return;
                this._balance.WeightTareFilePath = value;
                this.NotifyPropertyChanged("WeightTareFilePath");
            }
        }

        /// <summary>
        ///   Gets or sets the balance order.
        /// </summary>
        /// <value>The balance order.</value>
        [XmlElement("BalanceOrder")]
        public int BalanceOrder
        {
            get { return this._balance.BalanceOrder; }

            set
            {
                if (value == this._balance.BalanceOrder) return;
                this._balance.BalanceOrder = value;
                this.NotifyPropertyChanged("BalanceOrder");
            }
        }

        /// <summary>
        ///   Gets or sets the number of readings.
        /// </summary>
        /// <value>The number of readings.</value>
        [XmlElement("NumberOfReadings")]
        public int NumberOfReadings
        {
            get { return this._balance.NumberOfReadings; }

            set
            {
                if (value == this._balance.NumberOfReadings) return;
                this._balance.NumberOfReadings = value;
                this.NotifyPropertyChanged("NumberOfReadings");
            }
        }

        /// <summary>
        ///   Gets or sets the balance reference.
        /// </summary>
        /// <value>The balance reference.</value>
        [XmlElement("BalanceReference")]
        public int BalanceReference
        {
            get { return this._balance.BalanceReference; }

            set
            {
                if (value == this._balance.BalanceReference) return;
                this._balance.BalanceReference = value;
                this.NotifyPropertyChanged("BalanceReference");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "Balance" /> is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dirty; otherwise, <c>false</c>.
        /// </value>
        internal bool Dirty
        {
            get { return this._balance.Dirty; }
            set { this._balance.Dirty = value; }
        }

        /// <summary>
        ///   The balance validator.
        /// </summary>
        internal BalanceValidator Validator { get; private set; }

        /// <summary>
        ///   Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal Balances Parent { get; set; }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        ///   Specifically this compares balances by BalanceOrder.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(Balance other)
        {
            return Math.Sign(this.BalanceOrder - other.BalanceOrder);
        }

        /// <summary>
        ///   Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if (this._inTransaction) return;
            this._backup = CreateNewBalance();
            this._backup.AeroTareFilePath = this.AeroTareFilePath;
            this._backup.WeightTareFilePath = this.WeightTareFilePath;
            this._backup.CalibrationFilePath = this.CalibrationFilePath;
            this._backup.NumberOfReadings = this.NumberOfReadings;
            this._backup.BalanceName = this.BalanceName;
            this._backup.BalanceOrder = this.BalanceOrder;
            this._backup.BalanceReference = this.BalanceReference;
            this._inTransaction = true;
        }

        /// <summary>
        ///   Discards changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> call.
        /// </summary>
        public void CancelEdit()
        {
            if (!this._inTransaction) return;
            this._balance.AeroTareFilePath = this._backup._balance.AeroTareFilePath;
            this._balance.WeightTareFilePath = this._backup._balance.WeightTareFilePath;
            this._balance.CalibrationFilePath = this._backup._balance.CalibrationFilePath;
            this._balance.BalanceName = this._backup._balance.BalanceName;
            this._balance.BalanceOrder = this._backup._balance.BalanceOrder;
            this._balance.BalanceReference = this._backup._balance.BalanceReference;
            this._balance.NumberOfReadings = this._backup._balance.NumberOfReadings;
            this._inTransaction = false;
            this._balance.Dirty = false;
        }

        /// <summary>
        ///   Pushes changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> or <see cref = "M:System.ComponentModel.IBindingList.AddNew"></see> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            if (!this._inTransaction) return;
            this._backup = CreateNewBalance();
            this.Validator.Validate();
            this._inTransaction = false;
            this._balance.Dirty = true;
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   True if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Balance other)
        {
            return this._balance.BalanceID == other._balance.BalanceID &&
                   this._balance.CellID == other._balance.CellID &&
                   this._balance.CfgID == other._balance.CfgID;
        }

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        ///   This is the public factory method that returns the default object.
        /// </summary>
        /// <returns></returns>
        public static Balance CreateNewBalance()
        {
            return new Balance();
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
            builder.Append("ID: " + this._balance.BalanceID + "\t");
            builder.Append(this._balance.BalanceOrder + "\t");
            builder.Append(this._balance.BalanceName + "\t");
            builder.Append(this._balance.BalanceReference + "\t");
            builder.Append(this._balance.NumberOfReadings + "\t");
            builder.Append(this._balance.BalanceDescr + "\t");
            builder.Append(this._balance.CalibrationFilePath + "\t");
            builder.Append(this._balance.AeroTareFilePath + "\t");
            builder.Append(this._balance.WeightTareFilePath);
            return builder.ToString();
        }

        /// <summary>
        ///   Notifies the property changed.
        /// </summary>
        /// <param name = "info">The info.</param>
        internal void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            this._balance.Dirty = true;
        }

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        /// <param name = "cellId">The cell id.</param>
        /// <param name = "configId">The config id.</param>
        public void Save(int cellId, int configId)
        {
            this.EndEdit();
            this._balance.CellID = cellId;
            this._balance.CfgID = configId;
            this._balance.Save();
            this._balance.Dirty = false;
        }
    }
}