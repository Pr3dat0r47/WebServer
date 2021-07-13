using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {

            //TODO: parse the receivedRequest using the \r\n delimeter   

            ParseRequestLine();
           
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length >= 3)
            {
                string[] temp = requestLines[0].Split(' ');
                if (temp.Length != 3)
                {
                    Console.WriteLine("DEBUG: This is bad Request");
                    return false;
                }

                // Get HTTP method
                SetHTTPMethod(temp[0]);
                Console.WriteLine(this.method);

                // GET Relative URI
                this.relativeURI = temp[1];
                Console.WriteLine(this.relativeURI);

                // GET HTTP Version
                SetHTTPVersion(temp[2]);
                Console.WriteLine(this.httpVersion);

                //validate blank line exist
                int indexofcontent = ValidateBlankLine();
                if (indexofcontent == 0)
                {
                    Console.WriteLine("DEBUG: This is bad Request 2");
                    return false;
                }

                // Get Header Lines & set contentline
                else
                {
                    bool headerline = LoadHeaderLines(indexofcontent);
                    if (this.method.Equals(RequestMethod.POST))
                    {
                        Setcontentlines(indexofcontent);
                    }
                }


          
            }

            return true;
           
        }

        private void ParseRequestLine()
        {
            requestLines = this.requestString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(int indexofcontent)
        {
            for (int i = 1; i < indexofcontent; i++)
            {
                this.headerLines[requestLines[i].Split(':')[0]] = requestLines[i].Split(':')[1].Trim();

            }
            return true;
        } //done

        private int ValidateBlankLine()
        {
            for (int i = 1; i < requestLines.Length; i++)
            {
                if (this.requestLines[i].Equals(""))
                {
                    return i;
                }
            }
            return 0;
        }//done

        private void SetHTTPVersion(string version)
        {
            if (version.Equals("HTTP/1.1"))
            {
                this.httpVersion = HTTPVersion.HTTP11;
            }
            else if (version.Equals("HTTP/1.0"))
            {
                this.httpVersion = HTTPVersion.HTTP10;
            }
            else if (version.Equals("HTTP/0.9"))
            {
                this.httpVersion = HTTPVersion.HTTP09;
            }
            else if (version.Equals("HTTP "))
            {
                this.httpVersion = HTTPVersion.HTTP09;
            }
        } //done

        private void SetHTTPMethod(string method)
        {
            if (method.Equals("GET"))
            {
                this.method = RequestMethod.GET;
            }
            else if (method.Equals("POST"))
            {
                this.method = RequestMethod.POST;
            }
            else if (method.Equals("HEAD"))
            {
                this.method = RequestMethod.HEAD;
            }
            
        }//done

        private void Setcontentlines(int indexofcontent)
        {
            if (this.method.Equals(RequestMethod.POST))
            {
                //we need the content
                int x = 0;
                for (int i = indexofcontent + 1; i < requestLines.Length; i++)
                {
                    if (requestLines[i].Equals("/0"))
                    {
                        break;
                    }
                    contentLines[x] = requestLines[i];
                    x++;
                }

            }
        }

        public string returnhttpversion()
        {
            return httpVersion.ToString();
        }
        public string returnuri()
        {
            //   if(this.relativeURI.ToString().Substring(1, relativeURI.Length - 1).Equals(null))
            try
            {
                return this.relativeURI.ToString().Substring(1, relativeURI.Length - 1);
            }
            catch(Exception ex) { 
            return "";
            }
        }

        public string Method()
        {
            return this.method.ToString();
        }
        public string returnlines()
        {
            string s;
            for (int i = 0; i < length; i++)
            {

            }

            return s;

        }
    }
}
