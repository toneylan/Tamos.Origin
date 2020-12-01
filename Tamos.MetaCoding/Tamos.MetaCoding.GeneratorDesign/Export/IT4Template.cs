using System.Collections.Generic;

namespace Tamos.MetaCoding.GeneratorDesign
{
    /*public interface ICodeRender
    {
        void RenderCode(CodeWriter writer);
    }*/

    internal interface IT4Template
    {
        void Write(string textToAppend);

        void WriteLine(string textToAppend);

        void PushIndent(string indent);

        string PopIndent();

        /// <summary>
        /// Initialize the template
        /// </summary>
        void Initialize();

        /// <summary>
        /// Create the template output
        /// </summary>
        string TransformText();

        /// <summary>
        /// Current transformation session
        /// </summary>
        IDictionary<string, object> Session { get; set; }
    }
}