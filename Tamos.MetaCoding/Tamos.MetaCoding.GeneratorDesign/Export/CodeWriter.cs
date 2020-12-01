using System.IO;
using System.Text;

namespace Tamos.MetaCoding.GeneratorDesign
{
    public class CodeWriter : StreamWriter
    {
        private const int BackUnit = 4;
        public int BackSpace { get; set; }

        public CodeWriter(string path) : base(path, false, Encoding.UTF8)
        {

        }

        #region Indent

        public void BeginSubWrite()
        {
            BackSpace += BackUnit;
            Write(string.Concat("{", NewLine, string.Empty.PadRight(BackSpace)));
        }
        public void EndSubWrite(bool blankLine = false)
        {
            Write(NewLine);
            BackSpace -= BackUnit;
            WriteLine("{0}}}{1}", string.Empty.PadRight(BackSpace), blankLine ? "\r\n" : null);
        }

        public void IncreaseIndent()
        {
            BackSpace += BackUnit;
        }
        public void DecreaseIndent()
        {
            BackSpace -= BackUnit;
        }

        #endregion

        public override void WriteLine()
        {
            base.WriteLine();
            Write(string.Empty.PadRight(BackSpace));
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            Write(string.Empty.PadRight(BackSpace));
        }

        public override void WriteLine(string format, params object[] arg)
        {
            base.WriteLine(format, arg);
            Write(string.Empty.PadRight(BackSpace));
        }
    }
}