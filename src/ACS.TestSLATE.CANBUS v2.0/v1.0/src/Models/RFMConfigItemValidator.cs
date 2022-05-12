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
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    ///   This class provides all of the validation for a Channel.
    /// </summary>
    public class NamedConfigItemValidator : INotifyPropertyChanged
    {
        /// <summary>
        ///   The string representation of the ItemName property.
        /// </summary>
        public const string PropertyNameItemName = "ItemName";

        /// <summary>
        ///   The string representation of the ItemAddr property.
        /// </summary>
        public const string PropertyNameItemValue = "ItemValue";

        /// <summary>
        ///   The string representation of an Invalid property.
        /// </summary>
        public const string PropertyNameInvalid = "Invalid";

        /// <summary>
        ///   The string representation of the IsValid property.
        /// </summary>
        public const string PropertyNameIsvalid = "IsValid";

        /// <summary>
        ///   The channel the Validator is associated with.
        /// </summary>
        private readonly string _ItemName;

        /// <summary>
        ///   The string value for this item.
        /// </summary>
        private readonly string _StringValue;

        /// <summary>
        ///   The int value for this item.
        /// </summary>
        private readonly int _IntValue;

        /// <summary>
        ///   The float value for this item.
        /// </summary>
        private readonly float _FloatValue;

        /// <summary>
        ///   The collection of property errors.
        /// </summary>
        private readonly Hashtable _propertyErrors;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NamedConfigItemValidator" /> class.
        /// </summary>
        /// <param name = "cfgItem">The channel.</param>
        public NamedConfigItemValidator(NamedConfigItem cfgItem)
        {
            this._ItemName = cfgItem._ItemName;
            this._StringValue = cfgItem._StringValue;
            this._IntValue = cfgItem._IntValue;
            this._FloatValue = cfgItem._FloatValue;
            this.PropertyChanged += this.OnChannelPropertyChanged;
            this._propertyErrors = new Hashtable();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "NamedConfigItemValidator" /> class.
        /// </summary>
        /// <param name = "cfgItem">The channel.</param>
        /// <param name = "defaultInvalid">if set to <c>true</c> [default invalid].</param>
        public NamedConfigItemValidator(NamedConfigItem cfgItem, bool defaultInvalid)
            
        {
            if (!defaultInvalid) return;
            this._propertyErrors.Add(PropertyNameItemName, PropertyNameItemName);
            this._propertyErrors.Add(PropertyNameItemValue, PropertyNameItemValue);
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
                this.NotifyPropertyChanged("Validator");
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
            get { return this._propertyErrors.ContainsKey(PropertyNameItemName); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameItemName, "Item name cannot be empty.");
                }
                else
                {
                    this.ClearError(PropertyNameItemValue);
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
        private void OnChannelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNameIsvalid) return;
            switch (e.PropertyName)
            {
                case PropertyNameItemValue:
                    this.ValidateName(this._StringValue);
                    break;
            }
        }

        /// <summary>
        ///   Validates this instance.
        /// </summary>
        public void Validate()
        {
            this.ValidateName(this._StringValue);
        }

        /// <summary>
        ///   Validates the name.
        /// </summary>
        /// <param name = "name">The name.</param>
        public void ValidateName(string name)
        {
            this.InvalidName = string.IsNullOrWhiteSpace(name);
        }


    }
}