using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastExponentiation {
	public class WideMeasuresResult : MeasuresResult {
		public double MaxErrorExp = 0d;
		public int IterationsCount = 0;

		public WideMeasuresResult(Misc.PowerFunctionModel powerFunctionModel) : base(powerFunctionModel) { }
	}
}
