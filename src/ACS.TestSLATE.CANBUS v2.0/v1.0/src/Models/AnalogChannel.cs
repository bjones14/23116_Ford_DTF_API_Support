// <copyright file="AnalogChannel.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>This class represents an Analog Channel. It is built around (composition) the class Channel.
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
using Customer.TestSLATE.Mnemonic.Properties;
using TSChannel = Jacobs.TestSLATE.Cell.DomainLayer.Channels.ObjectClass;

namespace Customer.TestSLATE.Mnemonic.Models
{
    /// <summary>
    ///   An extension of the base Jacobs.TestVIEW.Cell.DomainLayer.Channels.ObjectClass with validation built in.
    /// </summary>
    [Serializable]
    public sealed class AnalogChannel : Channel, IComparable<Channel>, IEquatable<Channel>, IEditableObject, INotifyPropertyChanged
    {
// Uncomment this if you need ChannelNumber.
/*        /// <summary>
        ///   Gets or sets the ChannelNumber.
        /// </summary>
        /// <value>The Channel Number.</value>
        [XmlElement("ChannelNumber")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public new int ChannelNumber
        {
            get { return this._channel.ChannelNumber; }

            set
            {
                if (value == this._channel.ChannelNumber) return;
                this._channel.ChannelNumber = value;
                this.NotifyPropertyChanged("ChannelNumber");
            }
        }
*/
        /// <summary>
        ///   Gets or sets the Bad Code.
        /// </summary>
        /// <value>The Bad Code.</value>
        [XmlElement("BadCode")]
        public bool BadCode
        {
            get { return this._channel.BadCode; }

            set
            {
                if (value == this._channel.BadCode) return;
                this._channel.BadCode = value;
                this.NotifyPropertyChanged("BadCode");
            }
        }

        /// <summary>
        ///   Gets or sets the name of the channel.
        /// </summary>
        /// <value>The name of the channel.</value>
        [XmlElement("ChannelName")]
        public new string ChannelName
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
        /// Gets or sets the channel descr.
        /// </summary>
        /// <value>
        /// The channel descr.
        /// </value>
        [XmlElement("ChannelDescr")]
        public new string ChannelDescr
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
        public string Units
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
        ///   Gets or sets the EUD Type.
        /// </summary>
        /// <value>The EUD Type.</value>
        [XmlElement("EUD Type")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public new int EUDType
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
        ///   Gets or sets the Active flag.
        /// </summary>
        /// <value>The Active flag.</value>
        [XmlElement("Active")]
        private bool Active
        {
            get { return this._channel.Active; }

            set
            {
                if (value == this._channel.Active) return;
                this._channel.Active = value;
                this.NotifyPropertyChanged("Active");
            }
        }

        /// <summary>
        ///   Gets or sets the LowRange.
        /// </summary>
        /// <value>The Low Range.</value>
        [XmlElement("LowRange")]
        public float LowRange
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
        ///   Gets or sets the High Range.
        /// </summary>
        /// <value>The High Range.</value>
        [XmlElement("HighRange")]
        public float HighRange
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
        ///   This is the public factory method that returns the default object.
        /// </summary>
        /// <returns></returns>
        public new static AnalogChannel CreateNewChannel(String channelType, String channelDescription, String channelName, String channelUnits,
            float channelLoRange, float channelHiRange, int channelNumber)
        {
            return new AnalogChannel(channelType, channelDescription, channelName, channelUnits, channelLoRange, channelHiRange, channelNumber);
        }

         /// <summary>
        ///   Initializes a new instance of the <see cref = "AnalogChannel" /> class.
        ///   This is a private constructor that generates a default object.
        /// </summary>
        private AnalogChannel(String channelType, String channelDescription, String channelName, String channelUnits, float channelLoRange, 
            float channelHiRange, int channelNumber)
        {
             this._channel = new TSChannel(EnvironmentFactory.CellEnvironment())
                                 {
                                     ChannelDescr = channelDescription,
                                     ChannelName = channelName,
                                     ChannelType = channelType,
                                     ChannelNumber = channelNumber,
                                    EUDType = 0,
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
        ///   Creates a new instance of the <see cref = "Channel" /> class from a TSChannel.
        ///   Copy Constructor.
        /// </summary>
        /// <param name = "channel">The channel.</param>
        public AnalogChannel(TSChannel channel)
        {
            this._channel = channel;
//            this.Validator = new ChannelValidator(this);
        }

    }

}