using System.Collections.Generic;
using System.Linq;
using NUnitLite;

namespace Streams.Resources
{
	class Program
	{
		static void Main(string[] args)
		{
			ResourceStream_should rs = new ResourceStream_should();
			rs.ReadsCorrectly_WhenZeroValueInTheValue();
		}
	}
}
