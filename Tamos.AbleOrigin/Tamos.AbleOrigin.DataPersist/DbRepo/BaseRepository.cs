using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Tamos.AbleOrigin.DataPersist
{
    public abstract class BaseRepository : ServiceComponent
    {
        /// <summary>
        /// 创建DbCreater（主要用于子类调用）
        /// </summary>
        protected ShardingDbCreater<T> DbCreater<T>() where T : ShardingDbContext, new()
        {
            return GetComponent<ShardingDbCreater<T>>();
        }
        
        
    }
}