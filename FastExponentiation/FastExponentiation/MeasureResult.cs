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
        public Misc.PowerFunctionModel PowerFunctionModel;
        public double MaxError = double.MinValue;
        public double MaxErrorBase = 0;
        public double AverageError = 0d;
        public List<DataItem> DataItems;

        public MeasuresResult(Misc.PowerFunctionModel powerFunctionModel) {
            PowerFunctionModel = powerFunctionModel;
            DataItems = new List<DataItem>();
        }
    }
}
