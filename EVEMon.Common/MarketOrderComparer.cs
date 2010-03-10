using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common
{
    public sealed class MarketOrderComparer : Comparer<MarketOrder>
    {
        private MarketOrderColumn m_column;
        private bool m_isAscending;

        public MarketOrderComparer(MarketOrderColumn column, bool isAscending)
        {
            m_column = column;
            m_isAscending = isAscending;
        }

        public override int Compare(MarketOrder x, MarketOrder y)
        {
            if (m_isAscending)
                return CompareCore(x, y);

            return -CompareCore(x, y);
        }

        public int CompareCore(MarketOrder x, MarketOrder y)
        {
            switch (m_column)
            {
                case MarketOrderColumn.Duration:
                    return x.Duration.CompareTo(y.Duration);

                case MarketOrderColumn.Expiration:
                    return x.Expiration.CompareTo(y.Expiration);

                case MarketOrderColumn.InitialVolume:
                    return x.InitialVolume.CompareTo(y.InitialVolume);

                case MarketOrderColumn.Issued:
                    return x.Issued.CompareTo(y.Issued);

                case MarketOrderColumn.Item:
                    return x.Item.Name.CompareTo(y.Item.Name);

                case MarketOrderColumn.ItemType:
                    return x.Item.MarketGroup.Name.CompareTo(y.Item.MarketGroup.Name);

                case MarketOrderColumn.Location:
                    return x.Station.CompareTo(y.Station);

                case MarketOrderColumn.MinimumVolume:
                    return x.MinVolume.CompareTo(y.MinVolume);

                case MarketOrderColumn.Region:
                    return x.Station.SolarSystem.Constellation.Region.CompareTo(y.Station.SolarSystem.Constellation.Region);

                case MarketOrderColumn.RemainingVolume:
                    return x.RemainingVolume.CompareTo(y.RemainingVolume);

                case MarketOrderColumn.SolarSystem:
                    return x.Station.SolarSystem.CompareTo(y.Station.SolarSystem);

                case MarketOrderColumn.Station:
                    return x.Station.CompareTo(y.Station);

                case MarketOrderColumn.TotalPrice:
                    return x.TotalPrice.CompareTo(y.TotalPrice);

                case MarketOrderColumn.UnitaryPrice:
                    return x.UnitaryPrice.CompareTo(y.UnitaryPrice);

                case MarketOrderColumn.Volume:
                    // Compare the percent left
                    return (x.InitialVolume * y.RemainingVolume - x.RemainingVolume * y.InitialVolume);

                case MarketOrderColumn.LastStateChange:
                    return x.LastStateChange.CompareTo(y.LastStateChange);

                case MarketOrderColumn.OrderRange:
                    // Compare applies only to BuyOrder 
                    return (x is BuyOrder ? ((BuyOrder)x).Range.CompareTo(((BuyOrder)y).Range) : 0);

                case MarketOrderColumn.Escrow:
                    // Compare applies only to BuyOrder 
                    return (x is BuyOrder ? ((BuyOrder)x).Escrow.CompareTo(((BuyOrder)y).Escrow) : 0);

                default:
                    return 0;
            }
        }
    }
}
