using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models.Collections
{
    /// <summary>
    /// Represents the character's list of plans.
    /// </summary>
    public sealed class PlanCollection : BaseList<Plan>
    {
        private readonly Character m_owner;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal PlanCollection(Character owner)
        {
            m_owner = owner;
        }

        /// <summary>
        /// Gets the plan with the given name, null when not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Plan this[string name] => Items.FirstOrDefault(plan => plan.Name == name);

        /// <summary>
        /// When we add a plan, we may have to clone it (and maybe changed the character it is bound to) and connects it.
        /// </summary>
        /// <param name="item"></param>
        protected override void OnAdding(ref Plan item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Character != m_owner)
                item = item.Clone(m_owner);
            else if (Contains(item))
                item = item.Clone();

            item.IsConnected = true;
        }

        /// <summary>
        /// When removing a plan, we need to disconnect it.
        /// </summary>
        /// <param name="oldItem"></param>
        protected override void OnRemoving(Plan oldItem)
        {
            if (oldItem == null)
                throw new ArgumentNullException("oldItem");

            oldItem.IsConnected = false;
        }

        /// <summary>
        /// When the collection changed, the global event is fired.
        /// </summary>
        protected override void OnChanged()
        {
            EveMonClient.OnCharacterPlanCollectionChanged(m_owner);
        }

        /// <summary>
        /// Imports data from the given deserialization object.
        /// </summary>
        /// <param name="plans"></param>
        internal void Import(IEnumerable<SerializablePlan> plans)
        {
            // Filter plans which belong to this owner
            List<Plan> newPlanList = plans.Where(plan => plan.Owner == m_owner.Guid).Select(
                serialPlan => new Plan(m_owner, serialPlan) { IsConnected = true }).ToList();

            // We now add the plans
            SetItems(newPlanList);

            // Fire the global event
            EveMonClient.OnCharacterPlanCollectionChanged(m_owner);
        }
    }
}