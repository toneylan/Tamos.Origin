using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tamos.MetaCoding.GeneratorDesign
{
    /// <summary>
    /// 定义一份代码文件
    /// </summary>
    public class CodeFile
    {
        public string FilePath { get; set; }
        //public string UsingSection { get; set; }
        
        public List<DotNetType> TypeList { get; set; }

        public CodeFile(string path)
        {
            FilePath = path;
            TypeList = new List<DotNetType>();
        }

        public DotNetType AddType(DotNetType type)
        {
            TypeList.Add(type);
            return type;
        }

        /// <summary>
        /// 输出代码文件
        /// </summary>
        internal async Task WriteFile(IT4Template template, GrpcBuildConfig conf)
        {
            using (var writer = new CodeWriter(FilePath))
            {
                template.Session = new Dictionary<string, object>
                {
                    ["config"] = conf,
                    ["file"] = this
                };
                template.Initialize();

                await writer.WriteAsync(template.TransformText());
            }

            /*writer.WriteLine(UsingSection);
                //writer.WriteLine("namespace {0}", NameSpace);
                writer.BeginSubWrite();
                foreach (var dotNetType in TypeList)
                {
                    dotNetType.RenderCode(writer);
                }
                writer.EndSubWrite();
                writer.Flush();*/
        }
    }
}