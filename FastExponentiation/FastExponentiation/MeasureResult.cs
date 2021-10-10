using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastExponentiation {
    public class DataItem {
        public double Value { get; set; }
        public double Base { get; set; }
    }

    public class MeasuresResult {
        public Misc.PowerFunctionModel powerFunctionModel;
        public double maxError = double.MinValue;
        public double maxErrorNumber = 0;
        public double averageError = 0;
        public List<DataItem> dataItems;

        public MeasuresResult(Misc.PowerFunctionModel powerFunctionModel) {
            this.powerFunctionModel = powerFunctionModel;
            dataItems = new List<DataItem>();
        }
    }
}
