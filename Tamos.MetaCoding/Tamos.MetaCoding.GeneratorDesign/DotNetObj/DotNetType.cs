using System.Collections.Generic;

namespace Tamos.MetaCoding.GeneratorDesign
{
    /// <summary>
	/// 代表一个.net类型，如class
	/// </summary>
    public class DotNetType
    {
        public string TypeName { get; set; }
        public List<BaseTypeMember> TypeMembers { get; set; }
        
        /// <summary>
        /// 继承和实现的类型列表
        /// </summary>
        public List<string> InheritAndImpls { get; set; }

        /// <summary>
        /// 追加的Ioc接口注册。可用于添加接口基类的注册。
        /// </summary>
        public List<string> AddIocServices { get; set; }

        public DotNetType(string name = null)
        {
            TypeName = name;
            TypeMembers = new List<BaseTypeMember>();
        }

        public T AddMember<T>(T member) where T : BaseTypeMember
        {
            TypeMembers.Add(member);
            return member;
        }
    }
}