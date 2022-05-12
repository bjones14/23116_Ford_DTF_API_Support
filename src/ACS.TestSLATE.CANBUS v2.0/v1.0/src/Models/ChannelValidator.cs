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
    ///   This class provides all of the validation for a Channel.
    /// </summary>
    public class ChannelValidator : INotifyPropertyChanged
    {
        /// <summary>
        ///   The string representation of the Channel property.
        /// </summary>
        public const string PropertyNameChannelName = "ChannelName";

        /// <summary>
        ///   The string representation of the BalanceOrder property.
        /// </summary>
        public const string PropertyNameChannelDescr = "ChannelDescr";

        /// <summary>
        ///   The string representation of the ChannelType property.
        /// </summary>
        public const string PropertyNameChannelType = "ChannelType";

        /// <summary>
        ///   The string representation of the EUDType property.
        /// </summary>
        public const string PropertyNameEUDType = "EUDType";

        /// <summary>
        ///   The string representation of the ISORefChannelID property.
        /// </summary>
        public const string PropertyNameISORefChnlID = "ISORefChnlID";

        /// <summary>
        ///   The string representation of the BadCode property.
        /// </summary>
        public const string PropertyNameBadCode = "BadCode";

        /// <summary>
        ///   The string representation of the Active property.
        /// </summary>
        public const string PropertyNameActive = "Active";

        /// <summary>
        ///   The string representation of the LowRange property.
        /// </summary>
        public const string PropertyNameLowRange = "LowRange";

        /// <summary>
        ///   The string representation of the HighRange property.
        /// </summary>
        public const string PropertyNameHighRange = "HighRange";

        /// <summary>
        ///   The string representation of the Uncertainty property.
        /// </summary>
        public const string PropertyNameUncertainty = "Uncertainty";

        /// <summary>
        ///   The string representation of the Units property.
        /// </summary>
        public const string PropertyNameUnits = "Units";

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
        private readonly Channel _channel;

        /// <summary>
        ///   The collection of property errors.
        /// </summary>
        private readonly Hashtable _propertyErrors;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ChannelValidator" /> class.
        /// </summary>
        /// <param name = "channel">The channel.</param>
        public ChannelValidator(Channel channel)
        {
            this._channel = channel;
            this._channel.PropertyChanged += this.OnChannelPropertyChanged;
            this._propertyErrors = new Hashtable();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ChannelValidator" /> class.
        /// </summary>
        /// <param name = "channel">The channel.</param>
        /// <param name = "defaultInvalid">if set to <c>true</c> [default invalid].</param>
        public ChannelValidator(Channel channel, bool defaultInvalid)
            : this(channel)
        {
            if (!defaultInvalid) return;
            this._propertyErrors.Add(PropertyNameChannelName, PropertyNameChannelName);
            this._propertyErrors.Add(PropertyNameChannelDescr, PropertyNameChannelDescr);
            this._propertyErrors.Add(PropertyNameChannelType, PropertyNameChannelType);
            this._propertyErrors.Add(PropertyNameEUDType, PropertyNameEUDType);
            this._propertyErrors.Add(PropertyNameISORefChnlID, PropertyNameISORefChnlID);
            this._propertyErrors.Add(PropertyNameBadCode, PropertyNameBadCode);
            this._propertyErrors.Add(PropertyNameActive, PropertyNameActive);
            this._propertyErrors.Add(PropertyNameLowRange, PropertyNameLowRange);
            this._propertyErrors.Add(PropertyNameHighRange, PropertyNameHighRange);
            this._propertyErrors.Add(PropertyNameUncertainty, PropertyNameUncertainty);
            this._propertyErrors.Add(PropertyNameUnits, PropertyNameUnits);
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
                this._channel.NotifyPropertyChanged("Validator");
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
            get { return this._propertyErrors.ContainsKey(PropertyNameChannelName); }

            set
            {
                if (value)
                {
                    this.RegisterError(PropertyNameChannelName, "Channel name cannot be empty.");
                }
                else
                {
                    this.ClearError(PropertyNameChannelName);
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
                case PropertyNameChannelName:
                    this.ValidateName(this._channel.ChannelName);
                    break;
            }
        }

        /// <summary>
        ///   Validates this instance.
        /// </summary>
        public void Validate()
        {
            this.ValidateName(this._channel.ChannelName);
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