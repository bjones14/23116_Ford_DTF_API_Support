using System.Collections.Generic;
using Customer.TestSLATE.Mnemonic.Properties;

namespace Customer.TestSLATE.Mnemonic.Models
{
    /// <summary>
    ///   An extension of the base Jacobs.TestVIEW.Cell.DomainLayer.Channels.ObjectClass with validation built in.
    /// </summary>
    public class DefaultChannels : List<Channel>
    {
        ///<summary>
        ///</summary>
        public DefaultChannels()
        {
            string channelName;
            channelName = "Date_" + Resources.SourceName;
            this.Add(Channel.CreateNewChannel("AI", "TS Date", channelName, "TSDate", (float)0.0, (float)1.0e+6, 2));
            channelName = "Time_" + Resources.SourceName;
            this.Add(Channel.CreateNewChannel("AI", "TS Time", channelName, "TSTime", (float)0.0, (float)1.0e+6, 2));
            channelName = "Milliseconds_" + Resources.SourceName;
            // FB 3585: 2012-03-06 change units from "TSMillisecs" to "TSMillisec", change hirange from 0 to 1000
            this.Add(Channel.CreateNewChannel("AI", "TS Millisecs", channelName, "TSMillisec", (float)0.0, (float)1000.0, 2));
            
        }
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public int GetCount ()
        {
            return this.Count;
        }

        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        public Channel GetAt (int index)
        {
            if (index >= this.Count)
                return null;
            else
            {
                return this.GetAt(index);
            }
        }
    }
}
