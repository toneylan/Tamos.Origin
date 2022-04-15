using System.IO;
using System.Text;

namespace Tamos.AbleOrigin.Booster
{
    /*internal class LogTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        #region Overrides of TextWriter

        public override void Write(char value)
        {
            if (value == '\r' || value == '\n') return; //忽略单独的换行
            LogService.Debug(value.ToString());
        }

        public override void Write(string value)
        {
            if (value == "\r\n") return; //忽略单独的换行
            if (value.Length == 1)
            {
                Write(value[0]);
                return;
            }
            LogService.Debug(value);
        }

        public override void WriteLine()
        {
            //base.WriteLine(); //忽略单独的换行
        }

        #endregion
    }*/
}