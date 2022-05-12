// <copyright file="BalanceValidator.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This class implements INotifyPropertyChanged and registers with a Balance for PropertyChanged events.
//           When a balance reports that a property has changed, the event will fire and this class will automatically validate the property.
//           It will register the result of the validation so that the error is available.
//           This class also contains independent methods of validation not tied to specific class implementations so that if a table was loaded from 
//           a database and the raw data needed to be validated it could be without the overhead of having to instantiate Balance objects.
// </summary>

namespace Customer.TestSLATE.Mnemonic.Models
{
    using System.Collections;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    ///   This class provides all of the validation for a Balance.
    /// </summary>
    public class BalanceValidator : INotifyPropertyChanged
    {
        /// <summary>
        ///   The string representation of the BalanceName property.
        /// </summary>
        public const string PropertyNameBalname = "BalanceName";

        /// <summary>
        ///   The string representation of the BalanceOrder property.
        /// </summary>
        public const string PropertyNameOrder = "BalanceOrder";

        /// <summary>
        ///   The string representation of the BalanceReference property.
        /// </summary>
        public const string PropertyNameReference = "BalanceReference";

        /// <summary>
        ///   The string representation of the NumberOfReadings property.
        /// </summary>
        public const string PropertyNameReadings = "NumberOfReadings";

        /// <summary>
        ///   The string representation of the AeroTareFilePath property.
        /// </summary>
        public const string PropertyNameAeropath = "AeroTareFilePath";

        /// <summary>
        ///   The string representation of the WeightTareFilePath property.
        /// </summary>
        public const string PropertyNameWtpath = "WeightTareFilePath";

        /// <summary>
        ///   The string representation of the CalibrationFilePath property.
        /// </summary>
        public const string PropertyNameCalpath = "CalibrationFilePath";

        /// <summary>
        ///   The string representation of an Invalid property.
        /// </summary>
        public const string PropertyNameInvalid = "Invalid";

        /// <summary>
        ///   The string representation of the IsValid property.
        /// </summary>
        public const string PropertyNameIsvalid = "IsValid";

        /// <summary>
        ///   The balance the Validator is associated with.
        /// </summary>
        private readonly Balance _balance;

        /// <summary>
        ///   The collection of property errors.
        /// </summary>
        private readonly Hashtable _propertyErrors;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BalanceValidator" /> class.
        /// </summary>
        /// <param name = "balance">The balance.</param>
        public BalanceValidator(Balance balance)
        {
            this._balance = balance;
            this._balance.PropertyChanged += this.OnBalancePropertyChanged;
            this._propertyErrors = new Hashtable();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BalanceValidator" /> class.
        /// </summary>
        /// <param name = "balance">The balance.</param>
        /// <param name = "defaultInvalid">if set to <c>true</c> [default invalid].</param>
        public BalanceValidator(Balance balance, bool defaultInvalid) : this(balance)
        {
            if (!defaultInvalid) return;
            this._propertyErrors.Add(PropertyNameAeropath, PropertyNameAeropath);
            this._propertyErrors.Add(PropertyNameBalname, PropertyNameBalname);
            this._propertyErrors.Add(PropertyNameCalpath, PropertyNameCalpath);
            this._propertyErrors.Add(PropertyNameOrder, PropertyNameOrder);
            this._propertyErrors.Add(PropertyNameReadings, PropertyNameReadings);
            this._propertyErrors.Add(PropertyNameReference, PropertyNameReference);
            this._propertyErrors.Add(PropertyNameWtpath, PropertyNameWtpath);
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return (this._propertyErrors.Keys.Count == 0); }

            set
            {
                this.NotifyPropertyChanged(PropertyNameIsvalid);
                this._balance.NotifyPropertyChanged("Validator");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid order].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid order]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidOrder
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameOrder); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameOrder, "Order must be greater than 0.");
                }
                else
                {
                    this.ClearError(PropertyNameOrder);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid name].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid name]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidName
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameBalname); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameBalname, "Balance name cannot be empty.");
                }
                else
                {
                    this.ClearError(PropertyNameBalname);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid reference].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid reference]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidReference
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameReference); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameReference, "Balance Reference must be less than Balance Order.");
                }
                else
                {
                    this.ClearError(PropertyNameReference);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid number of readings].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid number of readings]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidNumberOfReadings
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameReadings); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameReadings, "Balances must have between 1 and 14 Readings.");
                }
                else
                {
                    this.ClearError(PropertyNameReadings);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid aero tare path].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid aero tare path]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidAeroTarePath
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameAeropath); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameAeropath, "Aero Tare File Path cannot be empty or the File does not Exist.");
                }
                else
                {
                    this.ClearError(PropertyNameAeropath);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid weight tare path].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid weight tare path]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidWeightTarePath
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameWtpath); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameWtpath, "Weight Tare File Path cannot be empty or the File does not Exist.");
                }
                else
                {
                    this.ClearError(PropertyNameWtpath);
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether [invalid calibration file path].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invalid calibration file path]; otherwise, <c>false</c>.
        /// </value>
        public bool InvalidCalibrationFilePath
        {
            get { return this._propertyErrors.ContainsKey(PropertyNameCalpath); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameCalpath, "Calibration File Path cannot be empty or the File does not Exist.");
                }
                else
                {
                    this.ClearError(PropertyNameCalpath);
                }
            }
        }

        /// <summary>
        ///   Gets the <see cref = "System.String" /> with the specified property name.
        /// </summary>
        public string this[string propertyName]
        {
            get
            {
                return this._propertyErrors.ContainsKey(propertyName)
                           ? this._propertyErrors[propertyName].ToString()
                           : string.Empty;
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        /// <summary>
        ///   Registers the error.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <param name = "message">The message.</param>
        public void RegisterError(string propertyName, string message)
        {
            if (this._propertyErrors.ContainsKey(propertyName))
            {
                this._propertyErrors[propertyName] = message;
            }
            else
            {
                this._propertyErrors.Add(propertyName, message);
            }

            this.NotifyPropertyChanged(PropertyNameInvalid + propertyName);
            this.IsValid = false;
        }

        /// <summary>
        ///   Clears the error.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public void ClearError(string propertyName)
        {
            if (this._propertyErrors.ContainsKey(propertyName))
            {
                this._propertyErrors.Remove(propertyName);
            }

            this.NotifyPropertyChanged(PropertyNameInvalid + propertyName);
            this.IsValid = true;
        }

        /// <summary>
        ///   Notifies the property changed.
        /// </summary>
        /// <param name = "info">The info.</param>
        private void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        ///   Called when [balance property changed].
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnBalancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNameIsvalid) return;
            switch (e.PropertyName)
            {
                case PropertyNameBalname:
                    this.ValidateName(this._balance.BalanceName);
                    break;
                case PropertyNameOrder:
                    this.ValidateOrder(this._balance.BalanceOrder);
                    break;
                case PropertyNameReadings:
                    this.ValidateReadings(this._balance.NumberOfReadings);
                    break;
                case PropertyNameReference:
                    this.ValidateReference(this._balance.BalanceReference, this._balance.BalanceOrder);
                    break;
                case PropertyNameAeropath:
                    this.ValidateAeroPath(this._balance.AeroTareFilePath);
                    break;
                case PropertyNameWtpath:
                    this.ValidateWeightPath(this._balance.WeightTareFilePath);
                    break;
                case PropertyNameCalpath:
                    this.ValidateCalPath(this._balance.CalibrationFilePath);
                    break;
            }
        }

        /// <summary>
        ///   Validates this instance.
        /// </summary>
        public void Validate()
        {
            this.ValidateName(this._balance.BalanceName);
            this.ValidateOrder(this._balance.BalanceOrder);
            this.ValidateReadings(this._balance.NumberOfReadings);
            this.ValidateReference(this._balance.BalanceReference, this._balance.BalanceOrder);
            this.ValidateAeroPath(this._balance.AeroTareFilePath);
            this.ValidateWeightPath(this._balance.WeightTareFilePath);
            this.ValidateCalPath(this._balance.CalibrationFilePath);
        }

        /// <summary>
        ///   Validates the name.
        /// </summary>
        /// <param name = "name">The name.</param>
        public void ValidateName(string name)
        {
            this.InvalidName = string.IsNullOrWhiteSpace(name);
        }

        /// <summary>
        ///   Validates the aero path.
        /// </summary>
        /// <param name = "path">The path.</param>
        public void ValidateAeroPath(string path)
        {
            this.InvalidAeroTarePath = string.IsNullOrWhiteSpace(path) || !File.Exists(path);
        }

        /// <summary>
        ///   Validates the weight path.
        /// </summary>
        /// <param name = "path">The path.</param>
        public void ValidateWeightPath(string path)
        {
            this.InvalidWeightTarePath = string.IsNullOrWhiteSpace(path) || !File.Exists(path);
        }

        /// <summary>
        ///   Validates the cal path.
        /// </summary>
        /// <param name = "path">The path.</param>
        public void ValidateCalPath(string path)
        {
            this.InvalidCalibrationFilePath = string.IsNullOrWhiteSpace(path) || !File.Exists(path);
        }

        /// <summary>
        ///   Validates the order.
        /// </summary>
        /// <param name = "order">The order.</param>
        public void ValidateOrder(int order)
        {
            this.InvalidOrder = order < 0;
        }

        /// <summary>
        ///   Validates the readings.
        /// </summary>
        /// <param name = "readings">The readings.</param>
        public void ValidateReadings(int readings)
        {
            this.InvalidNumberOfReadings = readings < 1 || readings > 14;
        }

        /// <summary>
        ///   Validates the reference.
        /// </summary>
        /// <param name = "reference">The reference.</param>
        /// <param name = "order">The order.</param>
        public void ValidateReference(int reference, int order)
        {
            this.InvalidReference = reference > order;
        }
    }
}