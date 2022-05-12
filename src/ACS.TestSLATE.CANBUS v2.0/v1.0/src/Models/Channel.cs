// <copyright file="Channel.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class represents a channel data model. It is built around (composition) a TestSLATE domain class (Channels).
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
using System.Windows.Forms;
using System.Text;
using System.Xml.Serialization;
using Customer.TestSLATE.Mnemonic.Models;
using Customer.TestSLATE.Mnemonic.Presenters;
using Jacobs.TestSLATE.DB;
using Customer.TestSLATE.Mnemonic.Properties;
using TSChannel = Jacobs.TestSLATE.Cell.DomainLayer.Channels.ObjectClass;

namespace Customer.TestSLATE.Mnemonic.Models
{
    /// <summary>
    ///   An extension of the base Jacobs.TestVIEW.Cell.DomainLayer.Channels.ObjectClass with validation built in.
    /// </summary>
    [Serializable]
    public class Channel : IComparable<Channel>, IEquatable<Channel>, IEditableObject, INotifyPropertyChanged
    {
        /// <summary>
        ///   The Test SLATE DomainLayer Channel.
        /// </summary>
        public TSChannel _channel;

        ///<summary>
        ///</summary>
        public ChannelPresenter _presenter;

        /// <summary>
        ///   Backup of this instance.
        ///   This preserves the state of this model during user edits, before the changes are committed.
        /// </summary>
        private Channel _backup;

        /// <summary>
        ///   Indicates that the user is still making changes and has not committed them.
        /// </summary>
        private bool _inTransaction;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Channel" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        private Channel(String channelType, String channelDescription, String channelName, String channelUnits, float channelLoRange, 
            float channelHiRange, int EUDType)
        {
            this.Validator = new ChannelValidator(this);
            this._channel = new TSChannel(EnvironmentFactory.CellEnvironment())
                                {
                                    ChannelDescr = channelDescription,
                                    ChannelName = channelName,
                                    ChannelType = channelType,
                                    EUDType = EUDType,
                                    ISORefChannelID = 0,
                                    BadCode = false,
                                    Active = true,
                                    LowRange = channelLoRange,
                                    HighRange = channelHiRange,
                                    Uncertainty = (float)0.0,
                                    Units = channelUnits
                                };
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Channel" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        public Channel()
        {
            this.Validator = new ChannelValidator(this);
            this._channel = new TSChannel(EnvironmentFactory.CellEnvironment())
            {
                ChannelDescr = Resources.DefaultAIDescription,
                ChannelName = Resources.DefaultAIName,
                ChannelType = "AI",
                EUDType = 0,
                ISORefChannelID = 0,
                BadCode = true,
                Active = true,
                LowRange = (float)-999999.99,
                HighRange = (float)999999.99,
                Uncertainty = (float)0.0,
                Units = ""
            };
        }

        /// <summary>
        ///   Creates a new instance of the <see cref = "Channel" /> class from a TSChannel.
        ///   Copy Constructor.
        /// </summary>
        /// <param name = "channel">The channel.</param>
        public Channel(TSChannel channel)
        {
            this._channel = channel;
            this.Validator = new ChannelValidator(this);
        }

        /// <summary>
        ///   Gets or sets the ChannelNumber.
        /// </summary>
        /// <value>The Channel Number.</value>
        [XmlElement("ChannelNumber")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        private int ChannelNumber
        {
            get { return this._channel.ChannelNumber; }

            set
            {
                if (value == this._channel.ChannelNumber) return;
                this._channel.ChannelNumber = value;
                this.NotifyPropertyChanged("ChannelNumber");
            }
        }

        /// <summary>
        ///   Gets or sets the name of the channel.
        /// </summary>
        /// <value>The name of the channel.</value>
        [XmlElement("ChannelName")]
        public string ChannelName
        {
            get { return this._channel.ChannelName; }

            set
            {
                if (value == this._channel.ChannelName) return;
                this._channel.ChannelName = value;
                this.NotifyPropertyChanged("ChannelName");
            }
        }

        /// <summary>
        ///   Gets or sets the EUD Type.
        /// </summary>
        /// <value>The EUD Type.</value>
        [XmlElement("EUD Type")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public int EUDType
        {
            get { return this._channel.EUDType; }

            set
            {
                if (value == this._channel.EUDType) return;
                this._channel.EUDType = value;
                this.NotifyPropertyChanged("EUDType");
            }
            
        }
        /// <summary>
        ///   Gets or sets the ISORefChannelID.
        /// </summary>
        /// <value>The ISORefChannelID.</value>
        [XmlElement("ISORefChannelID")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        private int ISORefChannelID
        {
            get { return this._channel.ISORefChannelID; }

            set
            {
                if (value == this._channel.ISORefChannelID) return;
                this._channel.ISORefChannelID = value;
                this.NotifyPropertyChanged("ISORefChannelID");
            }
        }

        /// <summary>
        ///   Gets or sets the High Range.
        /// </summary>
        /// <value>The High Range.</value>
        [XmlElement("HighRange")]
        private float HighRange
        {
            get { return this._channel.HighRange; }

            set
            {
                if (value == this._channel.HighRange) return;
                this._channel.HighRange = value;
                this.NotifyPropertyChanged("HighRange");
            }
        }

        /// <summary>
        ///   Gets or sets the High Range.
        /// </summary>
        /// <value>The High Range.</value>
        [XmlElement("LowRange")]
        private float LowRange
        {
            get { return this._channel.LowRange; }

            set
            {
                if (value == this._channel.LowRange) return;
                this._channel.LowRange = value;
                this.NotifyPropertyChanged("LowRange");
            }
        }

        /// <summary>
        /// Gets or sets the channel descr.
        /// </summary>
        /// <value>
        /// The channel descr.
        /// </value>
        [XmlElement("ChannelDescr")]
        public string ChannelDescr
        {
            get { return this._channel.ChannelDescr; }
            set
            {
                if (value == this._channel.ChannelDescr) return;
                this._channel.ChannelDescr = value;
                this.NotifyPropertyChanged("ChannelDescr");
            }
        }

        /// <summary>
        ///   Gets or sets the channel type.
        /// </summary>
        /// <value>The channel type.</value>
        [XmlElement("ChannelType")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        private string ChannelType
        {
            get { return this._channel.ChannelType; }

            set
            {
                if (value == this._channel.ChannelType) return;
                this._channel.ChannelType = value;
                this.NotifyPropertyChanged("ChannelType");
            }
        }

        /// <summary>
        ///   Gets or sets the Units.
        /// </summary>
        /// <value>The Units.</value>
        [XmlElement("Units")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        private string Units
        {
            get { return this._channel.Units; }

            set
            {
                if (value == this._channel.Units) return;
                this._channel.Units = value;
                this.NotifyPropertyChanged("Units");
            }
        }

        /// <summary>
        ///   Gets or sets the Uncertainty.
        /// </summary>
        /// <value>The Uncertainty.</value>
        [XmlElement("Uncertainty")]
        private float Uncertainty
        {
            get { return this._channel.Uncertainty; }

            set
            {
                if (value == this._channel.Uncertainty) return;
                this._channel.Uncertainty = value;
                this.NotifyPropertyChanged("Uncertainty");
            }
        }
        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "Channel" /> is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dirty; otherwise, <c>false</c>.
        /// </value>
        internal bool Dirty
        {
            get { return this._channel.Dirty; }
            set { this._channel.Dirty = value; }
        }

        /// <summary>
        ///   The balance validator.
        /// </summary>
        internal ChannelValidator Validator { get; private set; }

        /// <summary>
        ///   Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        internal Channels<Channel> Parent { get; set; }

        /// <summary>
        ///   Compares the current object with another object of the same type.
        ///   Specifically this compares balances by BalanceOrder.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(Channel other)
        {
            return Math.Sign(this.ChannelNumber - other.ChannelNumber);
        }

        /// <summary>
        ///   Begins an edit on an object.
        /// </summary>
        public void BeginEdit()
        {
            if (this._inTransaction)
            {
                return;
            }
            this._backup = CreateNewChannel("BU","BU","BU","", (float)0.0, (float)0.0, 0);
            this._backup.ChannelDescr = this.ChannelDescr;
            this._backup.ChannelName = this.ChannelName;
            this._backup.ChannelType = this.ChannelType;
            this._backup.EUDType = this.EUDType;
            this._backup.HighRange = this.HighRange;
            this._backup.ISORefChannelID = this.ISORefChannelID;
            this._backup.LowRange = this.LowRange;
            this._backup.Uncertainty = this.Uncertainty;
            this._backup.Units = this.Units;
            this._inTransaction = true;
            this._presenter.CellChanged = false;
        }

        /// <summary>
        ///   Discards changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> call.
        /// </summary>
        public void CancelEdit()
        {
            if (!this._inTransaction)
            {
                return;
            }
            this._channel.ChannelDescr = this._backup._channel.ChannelDescr;
            this._channel.ChannelName = this._backup._channel.ChannelName;
            this._channel.ChannelNumber = this._backup._channel.ChannelNumber;
            this._channel.ChannelType = this._backup._channel.ChannelType;
            this._channel.EUDType = this._backup._channel.EUDType;
            this._channel.HighRange = this._backup._channel.HighRange;
            this._channel.ISORefChannelID = this._backup._channel.ISORefChannelID;
            this._channel.LowRange = this._backup._channel.LowRange;
            this._channel.Uncertainty = this._backup._channel.Uncertainty;
            this._channel.Units = this._backup._channel.Units;
            this._inTransaction = false;
        }

        /// <summary>
        ///   Pushes changes since the last <see cref = "M:System.ComponentModel.IEditableObject.BeginEdit"></see> or <see cref = "M:System.ComponentModel.IBindingList.AddNew"></see> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            bool nFound;
            int numfound;
            if (!this._inTransaction)
            {
                return;
            }
            var DefaultChannels = new DefaultChannels();
            if (DefaultChannels.GetCount() > 0)
            {
                nFound = false;
                foreach (var stdChannel in DefaultChannels)
                {
                    if (this._backup.ChannelName == stdChannel.ChannelName)
                        nFound = true;
                }
                if (nFound)
                {
                    if (this._channel.ChannelName != this._backup.ChannelName)
                    {
                        MessageBox.Show("Standard Channel Names cannot be modified. Reverted to previous value.",
                                        "Standard Channel", MessageBoxButtons.OK);
                        this._channel.ChannelName = this._backup.ChannelName;
                    }
                    if (this._channel.BadCode != this._backup._channel.BadCode)
                    {
                        MessageBox.Show("Standard Channel Bad Codes cannot be modified. Reverted to previous value.",
                                        "Standard Channel", MessageBoxButtons.OK);
                        this._channel.BadCode = this._backup._channel.BadCode;

                    }
                    if (this._channel.Units != this._backup._channel.Units)
                    {
                        MessageBox.Show("Standard Channel Units cannot be modified. Reverted to previous value.",
                                        "Standard Channel", MessageBoxButtons.OK);
                        this._channel.Units = this._backup._channel.Units;

                    }
                    if (this._channel.EUDType != this._backup._channel.EUDType)
                    {
                        MessageBox.Show("Standard Channel EUD Types cannot be modified. Reverted to previous value.",
                                        "Standard Channel", MessageBoxButtons.OK);
                        this._channel.EUDType = this._backup._channel.EUDType;

                    }
                }
            }
            numfound = 0;
            for (int j=0;j<4;j++)
            {
                var nCount = this._presenter.ChnlModels[j].GetCount();
                for (int i = 0; i < nCount; i++)
                {
                    if (this._channel.ChannelName == this._presenter.ChnlModels[j].GetNameAt(i))
                        numfound++;
                }
            }
            if (numfound > 1)
            {
                MessageBox.Show("Cannot have duplicate channel names. Reverted to previous value", "Channel Validation", MessageBoxButtons.OK);
                this._channel.ChannelName = this._backup._channel.ChannelName;
            }
            this._backup = CreateNewChannel("", "", "", "", (float)0.0, (float)0.0, 0);
            this.Validator.Validate();
            this._inTransaction = false;
            if (this._presenter.CellChanged)
            {
                this.Dirty = true;
                this._channel.Dirty = true;
            }
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   True if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Channel other)
        {
            return this._channel.ChannelNumber == other.ChannelNumber &&
                   this._channel.ChannelName == other.ChannelName;
        }

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        ///   This is the public factory method that returns the default object.
        /// </summary>
        /// <returns></returns>
        public static Channel CreateNewChannel(String channelType, String channelDescription, String channelName, String channelUnits, float channelLoRange,
            float channelHiRange, int EUDType)
        {
            return new Channel(channelType, channelDescription, channelName, channelUnits, channelLoRange, channelHiRange, EUDType);
        }

        /// <summary>
        ///   This is the public factory method that returns the default object.
        /// </summary>
        /// <returns></returns>
        public static Channel CreateNewChannel(Channel srcChannel)
        {
            return new Channel(srcChannel.ChannelType, srcChannel.ChannelDescr, srcChannel.ChannelName, srcChannel.Units, srcChannel.LowRange, srcChannel.HighRange, 
                srcChannel.EUDType);
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
            builder.Append("ID: " + this._channel.ChannelNumber + "\t");
            builder.Append(this._channel.ChannelName + "\t");
            builder.Append(this._channel.ChannelDescr + "\t");
            builder.Append(this._channel.ChannelType + "\t");
            builder.Append(this._channel.EUDType + "\t");
            builder.Append(this._channel.ISORefChannelID + "\t");
            builder.Append(this._channel.BadCode + "\t");
            builder.Append(this._channel.Active + "\t");
            builder.Append(this._channel.LowRange + "\t");
            builder.Append(this._channel.HighRange + "\t");
            builder.Append(this._channel.Uncertainty + "\t");
            builder.Append(this._channel.Units);
            return builder.ToString();
        }

        /// <summary>
        ///   Notifies the property changed.
        /// </summary>
        /// <param name = "info">The info.</param>
        internal void NotifyPropertyChanged(string info)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(info));
//            this._channel.Dirty = true;
        }

        /// <summary>
        ///   Saves this instance.
        /// </summary>
        /// <param name = "cellId">The cell id.</param>
        /// <param name = "srcId">The config id.</param>
        public void Save(int cellId, int srcId)
        {
            this.EndEdit();
            this._channel.CellID = cellId;
            this._channel.SourceID = srcId;
            this._channel.Save();
            this._channel.Dirty = false;
        }
    }
}