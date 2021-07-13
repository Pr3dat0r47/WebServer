using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        int port;
        Response res;
        Dictionary<string, string> RedirectionRules_dic = new Dictionary<string, string>();

        public Server(int portNumber, string redirectionMatrixPath)
        {
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket
            this.port = portNumber;
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, this.port);
            this.serverSocket.Bind(hostEndPoint);
        }
        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New Client Accepted: {0}", clientSocket.RemoteEndPoint);

                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));

                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            //string welcome = "Welcome To HTTP Server\n";
            //clientSocket.Send(data);
            //clientSocket.Send(data);
            //data = Encoding.ASCII.GetBytes(welcome);
            Socket clientSocket = (Socket)obj;
            clientSocket.ReceiveTimeout = 0;
            byte[] data = new byte[1024];
            //string welcome = "this is server";
            //data = Encoding.ASCII.GetBytes(welcome);
            //clientSocket.Send(data);
            int recvdLen;


            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    // TODO: break the while loop if receivedLen==0
                    // TODO: Create a Request object using received request string
                    data = new byte[1024];
                    recvdLen = clientSocket.Receive(data);
                    if (recvdLen == 0)
                    {
                        Console.WriteLine("Client {0} : ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }                                                            ///////////////////////////////////////
                    string requestString = Encoding.ASCII.GetString(data);

                    Request req = new Request(requestString);

                    // TODO: Call HandleRequest Method that returns the response
                    // TODO: Send Response back to client
                    string res = HandleRequest(req);
                    //  string html_page = File.ReadAllText(Configuration.RootPath + "\\aboutus.html");
                    // Create OK response
                    // Response x = new Response(StatusCode.OK, "text/html", html_page, "", "HTTP11");
                    //   Console.WriteLine(x.responseString);
                    data = Encoding.ASCII.GetBytes(res);
                    clientSocket.Send(data);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    //Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }


        string HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            string version = "";
            try
            {
                bool r = request.ParseRequest();
                version = request.returnhttpversion();
                string filePath = "";
                string uri = request.returnuri();
                string method = request.Method();
                //throw new Exception();

                LoadRedirectionRules("D:\\FCIS\\Second term\\Project_Template and Required Files\\Template\\HTTPServer\\bin\\Debug\\redirectionRules.txt");

                //TODO: check for bad request 
                if (r == false)
                {

                    //filePath = Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName);
                    filePath = @"D:\\FCIS\\Second term\\Project_Template and Required Files\\inetpub\\wwwroot\\fcis1\\BadRequest.html";
                    string http_file = File.ReadAllText(filePath);
                    res = new Response(StatusCode.BadRequest, "text/html", http_file, "", version);


                }
                else
                {
                    if (method == "GET")
                    {
                        string redirect = GetRedirectionPagePathIFExist(uri);
                        //TODO: map the relativeURI in request to get the physical path of the resource.
                        //TODO: check for redirect
                        //TODO: check file exists
                        if (redirect.Length != 0)
                        {

                            // file redirection 
                            filePath = Path.Combine(Configuration.RootPath, redirect);
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.Redirect, "text/html", http_file, redirect, version);
                        }
                        else
                        {
                            //redirection 
                            string fileexist = Path.Combine(Configuration.RootPath, uri);
                            if (!File.Exists(fileexist))
                            {
                                filePath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                                string http_file = File.ReadAllText(filePath);
                                res = new Response(StatusCode.NotFound, "text/html", http_file, "", version);
                            }
                            else
                            {

                                string http_file = File.ReadAllText(fileexist);
                                res = new Response(StatusCode.OK, "text/html", http_file, "", version);
                            }
                        }
                        // Default Page
                        if (uri.Length == 0)
                        {
                            filePath = @"D:\\FCIS\\Second term\\Project_Template and Required Files\\inetpub\\wwwroot\\fcis1\\main.html";
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.OK, "text/html", http_file, "", version);
                        }

                        //TODO: read the physical file
                        string html_page = File.ReadAllText(Configuration.RootPath + "\\main.html");
                        // Create OK response
                        Response x = new Response(StatusCode.OK, "text/html", html_page, "", "HTTP11");
                    }
                    else if (method == "HEAD")
                    {
                        string redirect = GetRedirectionPagePathIFExist(uri);
                        //TODO: map the relativeURI in request to get the physical path of the resource.
                        //TODO: check for redirect
                        //TODO: check file exists
                        if (redirect.Length != 0)
                        {

                            // file redirection 
                            filePath = Path.Combine(Configuration.RootPath, redirect);
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.Redirect, "text/html", "", "", version);
                        }
                        else
                        {
                            //redirection 
                            string fileexist = Path.Combine(Configuration.RootPath, uri);
                            if (!File.Exists(fileexist))
                            {
                                filePath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                                string http_file = File.ReadAllText(filePath);
                                res = new Response(StatusCode.NotFound, "text/html", "", "", version);
                            }
                            else
                            {

                                string http_file = File.ReadAllText(fileexist);
                                res = new Response(StatusCode.OK, "text/html", "", "", version);
                            }
                        }
                        // Default Page
                        if (uri.Length == 0)
                        {
                            filePath = @"D:\\FCIS\\Second term\\Project_Template and Required Files\\inetpub\\wwwroot\\fcis1\\main.html";
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.OK, "text/html", "", "", version);
                        }

                        //TODO: read the physical file
                        string html_page = File.ReadAllText(Configuration.RootPath + "\\main.html");
                        // Create OK response
                        Response x = new Response(StatusCode.OK, "text/html", "", "", "HTTP11");
                    }
                    else if (method == "POST")
                    {
                        //////////////////////////////////////////////////////////////////////////////
                        string redirect = GetRedirectionPagePathIFExist(uri);
                        //TODO: map the relativeURI in request to get the physical path of the resource.
                        //TODO: check for redirect
                        //TODO: check file exists
                        if (redirect.Length != 0)
                        {

                            // file redirection 
                            filePath = Path.Combine(Configuration.RootPath, redirect);
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.Redirect, "text/html", http_file, redirect, version);
                        }
                        else
                        {
                            //redirection 
                            string fileexist = Path.Combine(Configuration.RootPath, uri);
                            if (!File.Exists(fileexist))
                            {
                                filePath = Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName);
                                string http_file = File.ReadAllText(filePath);
                                res = new Response(StatusCode.NotFound, "text/html", http_file, "", version);
                            }
                            else
                            {

                                string http_file = File.ReadAllText(fileexist);
                                res = new Response(StatusCode.OK, "text/html", http_file, "", version);
                            }
                        }
                        // Default Page
                        if (uri.Length == 0)
                        {
                            filePath = @"D:\\FCIS\\Second term\\Project_Template and Required Files\\inetpub\\wwwroot\\fcis1\\main.html";
                            string http_file = File.ReadAllText(filePath);
                            res = new Response(StatusCode.OK, "text/html", http_file, "", version);
                        }

                        //TODO: read the physical file
                        string html_page = File.ReadAllText(Configuration.RootPath + "\\main.html");
                        // Create OK response
                        Response x = new Response(StatusCode.OK, "text/html", html_page, "", "HTTP11");


                        /////////////////////////////
                        request.
                        //////////////////////////////////////////////////////////////////////////
                    }
                }

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
              
               

                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                string internalerror_File = Path.Combine( Configuration.RootPath , Configuration.InternalErrorDefaultPageName);
                string http_file = File.ReadAllText(internalerror_File);
                res = new Response(StatusCode.InternalServerError, "text/html", http_file, "", version);
            }
            Console.WriteLine(res.responseString);
            return res.responseString;
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            string redirected_page = "";

            if (RedirectionRules_dic.ContainsKey(relativePath))
            {
                redirected_page = RedirectionRules_dic[relativePath];
                return redirected_page;
            }
            else
            {
                return string.Empty;
            }

        }

        private string LoadDefaultPage(string defaultPageName)  //Done
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            string html = "";
            try
            {
                if (File.Exists(defaultPageName))
                {
                    html = File.ReadAllText(filePath);
                    return html;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 

                string Redirectionrules = "";
                string[] RedirectionruleS_AF;

                if (File.Exists(filePath))
                {
                    Redirectionrules = File.ReadAllText(filePath);
                    this.RedirectionRules_dic[Redirectionrules.Split(',')[0]] = Redirectionrules.Split(',')[1].Trim();
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
