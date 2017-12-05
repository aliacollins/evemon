using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEMon.XmlGenerator.Models
{
	public partial class industryBlueprints
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int typeID { get; set; }

		public int maxProductionLimit { get; set; }
	}
}
