using System;
using System.Collections.Generic;
using System.Text;

using EVEMon.Common.Serialization.Datafiles;
using EVEMon.Common.Collections;

namespace EVEMon.Common.Data
{
    public class StaticRequiredMaterial : Item
    {

        #region Constructors

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <param name="dmgPerJob"></param>
        /// <param name="activityId"></param>
        internal StaticRequiredMaterial(int id, int quantity, double dmgPerJob, int activityId)
            : base(id, StaticItems.GetItemByID(id).Name)
        {
            this.Quantity = quantity;
            this.DamagePerJob = dmgPerJob;
            this.Activity = (BlueprintActivity)Enum.ToObject(typeof(BlueprintActivity), activityId);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the damage per job.
        /// </summary>
        public double DamagePerJob
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        public BlueprintActivity Activity
        {
            get;
            set;
        }

        #endregion

    }
}