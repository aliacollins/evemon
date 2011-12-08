using System;
using System.Linq;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class ResearchPoint
    {
        #region Fields

        private readonly float m_remainderPoints;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal ResearchPoint(SerializableResearchListItem src)
        {
            GetAgentInfoByID(src.AgentID);

            AgentID = src.AgentID;
            Field = StaticSkills.GetSkillName(src.SkillID);
            StartDate = src.ResearchStartDate;
            PointsPerDay = src.PointsPerDay;
            m_remainderPoints = src.RemainderPoints;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the agents ID.
        /// </summary>
        public int AgentID { get; private set; }

        /// <summary>
        /// Gets the agents name.
        /// </summary>
        public string AgentName { get; private set; }

        /// <summary>
        /// Gets the agents level.
        /// </summary>
        public int AgentLevel { get; private set; }

        /// <summary>
        /// Gets the agents field of research.
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Gets the research points per day.
        /// </summary>
        public double PointsPerDay { get; private set; }

        /// <summary>
        /// Cets the current accumulated research points.
        /// </summary>
        public double CurrentRP
        {
            get { return (m_remainderPoints + (PointsPerDay * DateTime.UtcNow.Subtract(StartDate).TotalDays)); }
        }

        /// <summary>
        /// Gets the date the research was started.
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the station where the agent is.
        /// </summary>
        public Station Station { get; private set; }

        #endregion


        #region Private Finders

        /// <summary>
        /// Assigns the supplemental info of an agent by its ID.
        /// </summary>
        /// <param name="id"></param>
        private void GetAgentInfoByID(int id)
        {
            Agent agent = StaticGeography.AllAgents.FirstOrDefault(x => x.ID == id);
            if (agent == null)
                return;

            AgentName = agent.Name;
            AgentLevel = agent.Level;

            if (agent.Station != null)
                Station = StaticGeography.GetStationByID(agent.Station.ID);
        }

        #endregion
    }
}