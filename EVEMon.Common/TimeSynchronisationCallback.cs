using System;

namespace EVEMon.Common
{
    public delegate void TimeSynchronisationCallback(bool? isSynchronised, DateTime serverTime, DateTime localTime);
}
