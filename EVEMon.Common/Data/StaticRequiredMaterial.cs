using System;
using EVEMon.Common.Serialization.Datafiles;

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
        internal StaticRequiredMaterial(SerializableRequiredMaterial src)
            : base(src.ID,  GetName(src.ID))
        {
            Quantity = src.Quantity;
            DamagePerJob = src.DamagePerJob;
            Activity = (BlueprintActivity)Enum.ToObject(typeof(BlueprintActivity), src.Activity);
            WasteAffected = Convert.ToBoolean(src.WasteAffected);
        }

        #endregion


        #region Private Finders

        /// <summary>
        /// Gets the material's name.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private static string GetName(int id)
        {
            Item item = StaticItems.GetItemByID(id);

            return (item != null ? item.Name : String.Empty);
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

        /// <summary>
        /// Gets or sets if waste affected.
        /// </summary>
        public bool WasteAffected
        {
            get;
            set;
        }

        #endregion

    }
}