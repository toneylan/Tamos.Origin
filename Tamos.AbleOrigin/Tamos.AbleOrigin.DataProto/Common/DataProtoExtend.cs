using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tamos.AbleOrigin.Common;
using Tamos.AbleOrigin.Mapper;

namespace Tamos.AbleOrigin.DataProto
{
    public static class DataProtoExtend
    {
        #region Mapper

        /// <summary>
        /// Map为目标类型
        /// </summary>
        public static TDestination MapTo<TDestination>(this IGeneralEntity source)
        {
            return EntMapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Map为目标类型
        /// </summary>
        public static TDestination MapTo<TDestination>(this IReadOnlyCollection<IGeneralEntity> source)
        {
            return EntMapper.Map<TDestination>(source);
        }

        #endregion

        #region IGeneralEntity

        /// <summary>
        /// 按Id Find记录
        /// </summary>
        public static T ById<T>(this List<T> list, long id) where T : IGeneralEntity
        {
            return list == null ? default : list.Find(x => x.Id == id);
        }

        /// <summary>
        /// 依据列表数据的关联Id，获取关联的记录并填充到相应的属性值。
        /// </summary>
        public static void FillProperty<TSource, TDTO>(this IReadOnlyCollection<TSource> list, Expression<Func<TSource, TDTO>> expressionProp, 
            Func<TSource, long> funcPropId, Func<IReadOnlyCollection<long>, List<TDTO>> qryPropObj) where TDTO : IGeneralEntity
        {
            if (list.IsNullOrEmpty()) return;
            
            var objList = qryPropObj(list.Select(funcPropId).ToList());
            if (objList.IsNullOrEmpty()) return;

            var setter = TypeExtend.GetPropertySetter(expressionProp);
            foreach (var item in list)
            {
                var obj = objList.ById(funcPropId(item));
                if (obj != null) setter(item, obj);
            }
        }

        /// <summary>
        /// 检查对象的属性值是否有效，规则可通过PropMeta标记配置。
        /// </summary>
        public static bool Validate<T>(this T obj, out string error) where T : IGeneralEntity
        {
            error = EntityMeta.Get<T>().Validate(obj);
            return error.IsNull();
        }

        #endregion

        #region IGeneralResObj

        /// <summary>
        /// 设置错误信息，并返回当前对象
        /// </summary>
        public static T Error<T>(this T res, string msg) where T : IGeneralResObj
        {
            if (res.ErrorMsg == null) res.ErrorMsg = msg;
            else res.ErrorMsg += msg;
            return res;
        }

        /// <summary>
        /// ErrorMsg为空算成功。
        /// </summary>
        public static bool IsSuccess<T>(this T res) where T : IGeneralResObj
        {
            return res != null && string.IsNullOrEmpty(res.ErrorMsg);
        }

        #endregion
    }
}