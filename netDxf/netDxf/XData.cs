namespace netDxf
{
    using netDxf.Tables;
    using System;
    using System.Collections.Generic;

    public class XData : ICloneable
    {
        private netDxf.Tables.ApplicationRegistry appReg;
        private readonly List<netDxf.XDataRecord> xData;

        public XData(netDxf.Tables.ApplicationRegistry appReg)
        {
            this.appReg = appReg;
            this.xData = new List<netDxf.XDataRecord>();
        }

        public object Clone()
        {
            XData data = new XData((netDxf.Tables.ApplicationRegistry) this.appReg.Clone());
            foreach (netDxf.XDataRecord record in this.xData)
            {
                data.XDataRecord.Add(new netDxf.XDataRecord(record.Code, record.Value));
            }
            return data;
        }

        public override string ToString() => 
            this.ApplicationRegistry.Name;

        public netDxf.Tables.ApplicationRegistry ApplicationRegistry
        {
            get => 
                this.appReg;
            internal set => 
                (this.appReg = value);
        }

        public List<netDxf.XDataRecord> XDataRecord =>
            this.xData;
    }
}

