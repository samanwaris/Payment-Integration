using System.Text;

namespace payment_service.Helper
{
    public class LoggingHelper
    {
        public void AppendTextFunction(string Endpoint, string result)
        {
            string pathTxt = (@"D:/LogTxt/");

            string Tanggal = DateTime.Now.ToString("yyyyMMdd");
            string PathFile = pathTxt + Tanggal.ToString().Trim() + ".txt";

            using (StreamWriter sw = System.IO.File.AppendText(PathFile))
            {
                sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ": /" + Endpoint + " | " + result);
            }
        }

        public void CreateFullLog(Exception ex, string Endpoint)
        {
            var message = new StringBuilder();
            message.AppendLine(ex.Message);

            var innerException = ex.InnerException;
            while (innerException != null)
            {
                message.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            var fullMessage = message.ToString();

            AppendTextFunction(Endpoint, "Error " + "Error " + fullMessage);
        }
    }
}
