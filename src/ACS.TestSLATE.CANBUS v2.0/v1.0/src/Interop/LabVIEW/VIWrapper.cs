// <copyright file="VIWrapper.cs" company="Jacobs Technology, Inc.">
//     Copyright (c) Jacobs Technology, Inc. All rights reserved.
// </copyright>
// <summary>
//          This class is a generic VI Wrapper for interacting with a VI. As a requirement, this project must be built
//          with it's language features set to .NET 3.5 or lower. It cannot be .NET 4.0 as LabVIEW doesn't support those dlls.
//
//          IMPORTANT :: You will need one VI Wrapper for each VI you plan to use in your plugin.
//
// </summary>


namespace Customer.TestSLATE.Mnemonic.Interop.LabVIEW
{
    using System;
    using Jacobs.TestSLATE.LabVIEWInterop.VIWrappers;
    using Customer.TestSLATE.Mnemonic.Properties;

    /// <summary>
    ///   This class is a generic VI Wrapper for interacting with a VI. As a requirement, this project must be built
    ///   with it's language features set to .NET 3.5 or lower. It cannot be .NET 4.0 as LabVIEW doesn't support those dlls.
    /// </summary>
    public sealed class VIWrapper : WrapperBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "VIWrapper" /> class.
        /// </summary>
        public VIWrapper()
        {
            this.viName = Resources.VIName; // The name of the VI
            this.viRelativeSrcDir = Resources.VIRelativeSourceDir; // The path of the VI (after \lv\)
            this.viPluginName = Resources.VILLBName; // The .llb the VI is located in
            this.GetReference();
        }

        // Implement properties that reference the controls or indicators on your LabVIEW VI Front Panel like so:
        // The names of the ControlValues must match exactly the names of the controls or indicators on the Front Panel.
        // These sample properties highlight a few of the data types you're likely to see being passed between .NET and LabVIEW.

        /// <summary>
        /// Gets or sets the cell id.
        /// </summary>
        /// <value>
        /// The cell id.
        /// </value>
        public Int32 CellId
        {
            get { return (Int32) (this.GetControlValue(@"CellId")); }
            set { this.SetControlValue(@"CellId", value); }
        }

        /// <summary>
        /// Gets or sets the CFG id.
        /// </summary>
        /// <value>
        /// The CFG id.
        /// </value>
        public Int32 CfgId
        {
            get { return (Int32)(this.GetControlValue(@"CfgId")); }
            set { this.SetControlValue(@"CfgId", value); }
        }

        //public Single[] AiCounts
        //{
        //    get { return (Single[])(GetControlValue(@"AI Counts")); }
        //    set { SetControlValue(@"AI Counts", value); }
        //}

        //public Int32[] AllAlarmValues
        //{
        //    get { return (Int32[])(GetControlValue(@"All ALARM VALUES")); }
        //    set { SetControlValue(@"All ALARM VALUES", value); }
        //}

        //public String[] Names
        //{
        //    get { return (String[])(GetControlValue(@"Names")); }
        //    set { SetControlValue(@"Names", value); }
        //}

        //public Int16[] DisplayPrecision
        //{
        //    get { return (Int16[])(GetControlValue(@"Display Precision")); }
        //    set { SetControlValue(@"Display Precision", value); }
        //}

        //public Double[] LoRangeValue
        //{
        //    get { return (Double[])(GetControlValue(@"Lo Range Value")); }
        //    set { SetControlValue(@"Lo Range Value", value); }
        //}

        //public UInt32[] SystemColors
        //{
        //    get { return (UInt32[])(GetControlValue(@"System Colors")); }
        //    set { SetControlValue(@"System Colors", value); }
        //}

        //public Boolean[] BadCoded_p
        //{
        //    get { return (Boolean[])(GetControlValue(@"Bad Coded?")); }
        //    set { SetControlValue(@"Bad Coded?", value); }
        //}

        // The following is an example of how to define enums and structs for unpacking and packing Clusters that some VIs will use
        //--------------------------------------------------------------------------------------------------------------------------

        //public enum FileTypes : ushort
        //{
        //    Log = 0,
        //    D1 = 1,
        //    D2 = 2,
        //    Burst = 3,
        //    BBox = 4
        //}

        //[Serializable]
        //public struct CurrentStorage
        //{
        //    public double Average;
        //    public double[,] AverageAry;
        //    public UInt32 AverageCount;
        //    public Int32 DataFileID;
        //    public String DataFileName;
        //    public double Duration;
        //    public FileTypes FileType;
        //    public double Interval;
        //    public Int32 SampleNum;
        //}

        //public CurrentStorage[] CurrentStorage
        //{
        //    get
        //    {
        //        CurrentStorage[] ret;

        //        var tmp = (Object[])GetControlValue(@"Current Storage");

        //        var csl = new List<CurrentStorage>();

        //        foreach (Object t in tmp)
        //        {
        //            var foo = (Object[])t;
        //            var cs = new CurrentStorage();

        //            if (foo.Length == 9)
        //            {
        //                cs.DataFileID = (Int32)foo[0];
        //                cs.FileType = (FileTypes)foo[1];
        //                cs.DataFileName = (String)foo[2];
        //                cs.SampleNum = (Int32)foo[3];
        //                cs.Interval = (double)foo[4];
        //                cs.Duration = (double)foo[5];
        //                cs.Average = (double)foo[6];
        //                cs.AverageCount = (UInt32)foo[7];
        //                cs.AverageAry = (double[,])foo[8];
        //            }
        //            csl.Add(cs);
        //        }

        //        ret = csl.ToArray();
        //        return ret;
        //    }
        //    set
        //    {
        //        var tmp = new List<Object>();

        //        foreach (CurrentStorage cs in value)
        //        {
        //            var foo = new List<Object>();
        //            foo.Add(cs.DataFileID);
        //            foo.Add(cs.FileType);
        //            foo.Add(cs.DataFileName);
        //            foo.Add(cs.SampleNum);
        //            foo.Add(cs.Interval);
        //            foo.Add(cs.Duration);
        //            foo.Add(cs.Average);
        //            foo.Add(cs.AverageCount);
        //            foo.Add(cs.AverageAry);

        //            tmp.Add((foo.ToArray()));
        //        }


        //        SetControlValue(@"Current Storage", (tmp.ToArray()));
        //    }
        //}
    }
}