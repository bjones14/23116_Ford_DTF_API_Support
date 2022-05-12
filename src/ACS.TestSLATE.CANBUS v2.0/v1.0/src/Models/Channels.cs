// <copyright file="Channels.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary> This class implements a collection of Channels.
// Collections utililzing .NET 4.0 can inherit from ObservableCollection, if the dll has to use .NET 3.5, then
//          collections will need to inherit from CollectionBase and implement IBindingList.
//          Doing so will make collection's changes obseverable by the grid through event notifications.
// </summary>

namespace Customer.TestSLATE.Mnemonic.Models
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.ComponentModel;
    using Jacobs.TestSLATE.Commands.Commands.Collection;
    using Jacobs.TestSLATE.Commands.Interfaces;
    using Jacobs.TestSLATE.DB;
    using System;
    using Customer.TestSLATE.Mnemonic.Properties;
    using Customer.TestSLATE.Mnemonic.Presenters;
    using System.Xml.Serialization;

    /// <summary>
    /// A collection of Channels
    /// </summary>
    [Serializable]
    [XmlRoot]
    public class Channels<T> : ObservableCollection<T>, IChannelModel where T : Channel
    {
        /// <summary>
        /// The Cell Id.
        /// </summary>
        private readonly int _cellId;

        /// <summary>
        /// The Source Id.
        /// </summary>
        private readonly int _srcId;

        private bool _Dirty;

        /// <summary>
        /// The Channel Type for this model.
        /// </summary>
       public String _defChannelType;

       /// <summary>
       /// The Default Channel Description for this model.
       /// </summary>
       public String _defChannelDescription;

       /// <summary>
       /// The Default Channel Name for this model.
       /// </summary>
       public String _defChannelName;

        private ChannelPresenter _presenter;

       // Declare the list that will contain the channels that have been tagged for deletion
       private System.Collections.ObjectModel.Collection<T> deleteChansList = new System.Collections.ObjectModel.Collection<T>();

       ///<summary>
       ///</summary>
       ///<param name="index"></param>
       ///<returns></returns>
       public string GetNameAt(int index)
       {

           return this.ElementAt(index).ChannelName;
       }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public int GetCount ()
        {
            return this.Count();
        }

       /// <summary>
       /// Initializes a new instance of the <see cref="Channels&lt;T&gt;"/> class.
       /// </summary>
       /// <param name="cellId">The cell id.</param>
       /// <param name="srcId">The Source id.</param>
       /// <param name="channelType">The Channel Type for this model.</param>
        public Channels(int cellId, int srcId, String channelType)
       {
           this._cellId = cellId;
           this._srcId = srcId;
           this._defChannelType = channelType;
           this._Dirty = false;
           switch (channelType)
           {
               case "AI":
                   _defChannelDescription = Properties.Resources.DefaultAIDescription;
                   _defChannelName = Properties.Resources.DefaultAIName;
                   break;
               case "AO":
                   _defChannelDescription = Properties.Resources.DefaultAODescription;
                   _defChannelName = Properties.Resources.DefaultAOName;
                   break;
               case "DI":
                   _defChannelDescription = Properties.Resources.DefaultDIDescription;
                   _defChannelName = Properties.Resources.DefaultDIName;
                   break;
               case "DO":
                   _defChannelDescription = Properties.Resources.DefaultDODescription;
                   _defChannelName = Properties.Resources.DefaultDOName;
                   break;
           }
    }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError
        {
            get { return this.Any(channel => !channel.Validator.IsValid); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get { return this.Any(channel => channel.Dirty) | this._Dirty; }
            set { this._Dirty = value; }
        }

        /// <summary>
        /// Adds the new.
        /// </summary>
        /// <returns></returns>
        public UndoableCommand AddNew()
        {
            Channel DigitalChannel;
            AnalogChannel AnalogChannel;
            int i;
            bool nFound;
            string NewChannelName;

            i = 1;
            NewChannelName = _defChannelName;
            nFound = true;
            while (nFound)
            {
                NewChannelName = _defChannelName + "_" + i.ToString().PadLeft(3, '0');
                nFound = false;
                foreach (var ch in this)
                {
                    if (ch.ChannelName == NewChannelName)
                        nFound = true;
                }
                i++;
            }
            Jacobs.TestSLATE.Commands.Commands.Collection.AddItem addItem;
            if ((_defChannelType == "AI") || (_defChannelType == "AO"))
            {
                AnalogChannel = AnalogChannel.CreateNewChannel(_defChannelType, _defChannelDescription,
                                                                   NewChannelName, "", (float)-10.0, (float)10.0, 0);
                AnalogChannel._presenter = this._presenter;
                addItem = new AddItem(this, AnalogChannel);
            }
            else
            {
                DigitalChannel = Channel.CreateNewChannel(_defChannelType, _defChannelDescription, NewChannelName, "", (float)0.0, (float)0.0, 0);
                DigitalChannel._presenter = this._presenter;
                addItem = new AddItem(this, DigitalChannel);
            }
            addItem.Execute();
            return addItem;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UndoableCommand Insert(int index)
        {
            Channel DigitalChannel;
            AnalogChannel AnalogChannel;
            Jacobs.TestSLATE.Commands.Commands.Collection.InsertItem insertItem;
            if ((_defChannelType == "AI") || (_defChannelType =="AO"))
            {
                AnalogChannel = AnalogChannel.CreateNewChannel(_defChannelType, _defChannelDescription,
                                                                   _defChannelName, "", (float) -10.0, (float) 10.0, index + 1);
                AnalogChannel._presenter = this._presenter;
                insertItem = new InsertItem(this, AnalogChannel, index);
            }
            else
            {
                DigitalChannel = Channel.CreateNewChannel(_defChannelType, _defChannelDescription, _defChannelName, "", (float)-9.99e-20, (float)9.99e+20,index + 1);
                DigitalChannel._presenter = this._presenter;
                insertItem = new InsertItem(this, DigitalChannel, index);
            }
            insertItem.Execute();
            return insertItem;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UndoableCommand Remove(int index)
        {
            deleteChansList.Add(this[index]);
            var removeItem = new RemoveItem(this, this[index]);
            removeItem.Execute();
            this._Dirty = true;
            return removeItem;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <returns></returns>
        public UndoableCommand Remove()
        {
            return null;
        }

        ///<summary>
        ///</summary>
        public void UpdateChannelNumbers()
        {
            int i;
            i = 0;
            foreach (var b in this)
            {
//                b.ChannelNumber = i;
                i++;
            }
        }

        ///<summary>
        ///</summary>
        public void SetPresenter(ChannelPresenter presenter)
        {
            _presenter = presenter;
            foreach (var b in this)
            {
                b._presenter = presenter;
            }
        }

        /// <summary>
        /// Saves the specified cell id.
        /// </summary>
        public void Save()
        {
            var environment = EnvironmentFactory.CellEnvironment();
            foreach (var b in this)
            {
                b.Save(this._cellId, this._srcId);
            }
            var tsChannels = environment.Channels.SelectByCellIDAndSourceID(this._cellId, this._srcId);
            foreach (var chnl in tsChannels)
            {
                foreach (var delchnl in this.deleteChansList)
                {
                    if (chnl.ChannelName == delchnl.ChannelName)
                    {
                        chnl.Delete();
                    }
                }
            }
            deleteChansList.Clear ();
            this._Dirty = false;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetErrorMessage(int index, string propertyName)
        {
            return this[index].Validator[propertyName];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return this.All(channel => channel.Validator.IsValid); }
        }

        /// <summary>
        /// Validates the Collection of Channels
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return this.All(channel => channel.Validator.IsValid);
        }
        /// <summary>
        /// Swaps the positions of two balances.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="targetRow">The target row.</param>
        public void SwapItems(int sourceRow, int targetRow)
        {
            var tmp = this[sourceRow];
            this[sourceRow] = this[targetRow];
            this[targetRow] = tmp;
        }
    }
}
