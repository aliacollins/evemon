using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EVEMon.Services.Specification;

namespace EVEMon.Services.Factories
{
	public static class EVEAccountManagerFactory
	{
		public static IEVEAccountManager GetEVEAccountManager()
		{
			return new EVEAccountManager();
		}
	}
}
