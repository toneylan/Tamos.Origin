using Microsoft.EntityFrameworkCore;
using Tamos.AbleOrigin.DataPersist;

namespace UnitTest.AbleOrigin
{
    public class LiveDbContext : BaseDbContext
    {
        #region Ctor

        //private static readonly string ConnectionString = ServiceAddressConfig.GetExternalSrvSet("DB_aotran_fancylive");

        public LiveDbContext() : base("Server=host.docker.internal; Database=aotran_fancylive; Uid=root; Pwd=pwd80ding; charset=utf8;")
        {
        }

        #endregion

        public DbSet<ImMessageAt> ImMessageAt { get; set; }

        public EntityDbMap GetMap()
        {
            return GetDbMap<ImMessageAt>();
        }
    }
}