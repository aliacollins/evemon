using System;
using IO.Swagger.Api;
using IO.Swagger.Model;
using IO.Swagger.Client;

namespace EVEMon.Entities.ServerStatus
{
    public static class ServerStatus
    {

        private static int pilotsOnline { get; set; }
        private static bool serverOnline { get; set; }
        private static DateTime cacheExpires { get; set; }

        static ServerStatus()
        {
            cacheExpires = DateTime.UtcNow.AddSeconds(-1);
        }
            

        // invoked by events, updates it's internal model if appropriate, and then pushes to needed places
        public static void onEvent()
        {
            updateModelIfCacheExpired();
            PushStatusViewToMainWindowFooter();
        }

        private static void updateModelIfCacheExpired()
        {
            if (DateTime.UtcNow > cacheExpires){
                Tuple<int, bool, DateTime> status  = GetServerStatus();
                pilotsOnline = status.Item1;
                serverOnline = status.Item2;
                cacheExpires = status.Item3;
            }
        }

        // This shouldn't be here, it's the "viewmodel" from MVVM pattern, and should definitely be refactored elsewhere
        private static void PushStatusViewToMainWindowFooter()
        {
            
        }
        // this shouldn't be here, it should be under gateways
        private static Tuple<int, bool, DateTime> GetServerStatus()
        {
            StatusApi c = new StatusApi();
            try
            {                
                ApiResponse<GetStatusOk> s = c.GetStatusWithHttpInfo();
                string rawExpires = "";
                bool headerResult = s.Headers.TryGetValue("expires", out rawExpires);
                DateTime expires = DateTime.UtcNow.AddSeconds(-1);
                if (headerResult)
                {
                    DateTime Expires = DateTime.Parse(rawExpires);
                }
                int players = 0;
                if (s.Data.Players != null)
                {
                    players = s.Data.Players.Value;
                }
                return Tuple.Create(players, true, expires);
            }
            catch (IO.Swagger.Client.ApiException)
            {
                return Tuple.Create(0, false, DateTime.UtcNow);
            }
            
        }
    }
}
