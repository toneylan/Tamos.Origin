using System;
using System.Text;

namespace Tamos.MetaCoding.GeneratorDesign
{
	public class MemberPlainCode : BaseTypeMember
	{
		private StringBuilder _codeBuilder;
        const int IndentUnit = 4;

        /// <summary>
        /// 缩进值
        /// </summary>
        private int IndentNum { get; set; }

		public MemberPlainCode()
		{
			_codeBuilder = new StringBuilder();
		}

	    private string GetIndentValue()
	    {
	        return IndentNum <= 0 ? null : " ".PadRight(IndentNum);
	    }

	    public void WriteCode(string code)
	    {
		    if (string.IsNullOrEmpty(code)) return;
	        _codeBuilder.Append(string.Concat(GetIndentValue(), code, Environment.NewLine));
	    }

		public void WriteCode(string format, params object[] args)
		{
		    if (IndentNum > 0) _codeBuilder.Append(GetIndentValue());
			_codeBuilder.AppendFormat(format, args);
            NewLine();
		}

	    public void NewLine(int indentInc = 0)
	    {
            _codeBuilder.Append(Environment.NewLine);
            IndentNum += indentInc;
	    }

        #region Template write

		public MemberPlainCode StartBlock(int indentInc = 0)
	    {
            IndentNum += indentInc;
            WriteCode("{");
            IndentNum += IndentUnit;
			return this;
	    }

		public MemberPlainCode EndBlock()
        {
            IndentNum -= IndentUnit;
            WriteCode("}");
			return this;
        }

        // try block
	    public void StartTry()
	    {
            WriteCode("try");
            WriteCode("{");
            IndentNum += IndentUnit;
	    }

        public void StartCatch()
        {
            IndentNum -= IndentUnit;
            WriteCode("}");
            WriteCode("catch (Exception e)");
            WriteCode("{");
            IndentNum += IndentUnit;
        }

        #endregion

        /*public override void RenderCode(CodeWriter writer)
		{
            foreach (var line in _codeBuilder.ToString().Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                writer.WriteLine(line);
            }
		}*/
	}
}