using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
          
            var logger = new AusCommonLibraries.Logging.RemoteLogger("http://remotelogger.com", "d0c96bd2-6ca1-484f-8332-609b3839c691");
            logger.Error("TestMessage");
            Console.WriteLine(logger.InternalLog);
            Console.ReadLine();
          
            WebRequest request = WebRequest.Create("http://localhost:61026/Log");
            request.Method = "POST"; // для отправки используется метод Post
                                     // данные для отправки
            /*var list = new List<>
            string data = "{\"ClientId\":1,\"Time\":\"2018 - 04 - 10T23: 28:00.24403 + 07:00\",\"Message\":\"TestMessage\",\"Status\":1}";
            // преобразуем данные в массив байтов
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
            // устанавливаем тип содержимого - параметр ContentType
            request.ContentType = "application/x-www-form-urlencoded";
            // Устанавливаем заголовок Content-Length запроса - свойство ContentLength
            request.ContentLength = byteArray.Length;

            //записываем данные в поток запроса
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
            response.Close();*/
        }
    }
}
