using FacebookGroupArchiver.Interfaces;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FacebookGroupArchiver
{
    /// <summary>
    /// Retrieved Facebook authentication code
    /// </summary>
    public class HttpTokenRetriever
    {
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Starts an http listener on localhost to capture facebook's authentication code callback
        /// </summary>
        /// <param name="configManager">An instance of IConfigurationManager</param>
        /// <param name="toRunAfterServerStart">Code to run after server has been started</param>
        /// <returns>Authentication code</returns>
        public async Task<string> StartCodeListener(IConfigurationManager configManager, Action toRunAfterServerStart)
        {
            _tokenSource = new CancellationTokenSource();
            var cancellationToken = _tokenSource.Token;
            return await Task.Run(() =>
            {
                //Initialize and start the local listener
                HttpListener l = new HttpListener();
                l.Prefixes.Add(configManager.ReadSetting<string>("redirectUri"));
                l.Start();
                //Invoke the code to be run after the server ha started
                toRunAfterServerStart();
                //Capture the authentcation code
                string code = null;
                //Keep running till code has been captured
                while (string.IsNullOrEmpty(code))
                {
                    HttpListenerContext context = l.GetContext();
                    HttpListenerRequest request = context.Request;
                    code = request.QueryString["code"];
                    //Send an empty response to page
                    HttpListenerResponse res = context.Response;
                    string responseString = "";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    res.ContentLength64 = buffer.Length;
                    System.IO.Stream output = res.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    
                }
                return code;
            },cancellationToken);
           
            }

        /// <summary>
        /// Stops the listener
        /// </summary>
        public void StopTokenListener()
        {
            _tokenSource.Cancel();
        }
    }
}