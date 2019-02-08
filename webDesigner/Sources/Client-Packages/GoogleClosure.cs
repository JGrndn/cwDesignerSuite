using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Text;
using System;


namespace Casewise.GraphAPI.Operations.Web
{
    /// <summary>
    /// A C# wrapper around the Google Closure Compiler web service.
    /// </summary>
    public class GoogleClosure
    {
        //private const string PostData = "js_code={0}&output_format=xml&output_info=compiled_code&compilation_level=ADVANCED_OPTIMIZATIONS&output_info=warnings&output_info=errors";
        private const string PostData = "output_format=xml&output_info=compiled_code&output_info=warnings&output_info=errors&output_info=statistics&compilation_level=SIMPLE_OPTIMIZATIONS&warning_level=default&output_file_name=default.js&js_code={0}";
        private const string ApiEndpoint = "http://closure-compiler.appspot.com/compile";

        /// <summary>
        /// Compresses the specified file using Google's Closure Compiler algorithm.
        /// <remarks>
        /// The file to compress must be smaller than 200 kilobytes.
        /// </remarks>
        /// </summary>
        /// <param name="file">The absolute file path to the javascript file to compress.</param>
        /// <param name="mimify">if set to <c>true</c> [mimify].</param>
        /// <returns>
        /// A compressed version of the specified JavaScript file.
        /// </returns>
        public string Compress(string file, bool mimify)
        {
            string source = File.ReadAllText(file);
            XmlDocument xml = CallApi(source, mimify);
            return xml.SelectSingleNode("//compiledCode").InnerText;
        }

        /// <summary>
        /// Compresses the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="mimify">if set to <c>true</c> [mimify].</param>
        /// <returns></returns>
        public string Compress(StringBuilder content, bool mimify)
        {
            XmlDocument xml = CallApi(content.ToString(), mimify);
            XmlNodeList errorNodes = xml.SelectNodes("//error");
            foreach (XmlNode errorNode in errorNodes)
            {
                throw new ApplicationException(errorNode.OuterXml);
            }
            return xml.SelectSingleNode("//compiledCode").InnerText;
        }

        /// <summary>
        /// Calls the API with the source file as post data.
        /// </summary>
        /// <param name="source">The content of the source file.</param>
        /// <param name="mimify">if set to <c>true</c> [mimify].</param>
        /// <returns>
        /// The Xml response from the Google API.
        /// </returns>
        private static XmlDocument CallApi(string source, bool mimify)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("content-type", "application/x-www-form-urlencoded");

                string data = string.Format(PostData, HttpUtility.UrlEncode(source));
                if (!mimify)
                {
                    data += "&formatting=pretty_print";
                }

                string result = client.UploadString(ApiEndpoint, data);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                return doc;
            }
        }
    }
}
