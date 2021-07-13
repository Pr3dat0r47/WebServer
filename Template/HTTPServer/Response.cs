using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        public string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;

        List<string> headerLines = new List<string>();

        public Response(StatusCode code, string contentType, string content, string redirectoinPath, string httpversion)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            ////////////////////////////
            ////////////////////////////
            // TODO: Create the response string
            //  responseString = StatusCode.+"";

            string statusline = GetStatusLine(code, httpversion);

            bool header = GetHeaders(code, contentType, content, redirectoinPath);

            if (redirectoinPath != "")
            {
                responseString = statusline + "\r\n" + headerLines[0] + "\r\n" + headerLines[1] + "\r\n" + headerLines[2] + "\r\n" + headerLines[3] + "\r\n\r\n\r\n" + content;
            }
            else
            {
                responseString = statusline + "\r\n" + headerLines[0] + "\r\n" + headerLines[1] + "\r\n" + headerLines[2] + "\r\n\r\n\r\n" + content;
            }
        
        }

        private string GetStatusLine(StatusCode code, string httpversion)
        {
            // TODO: Create the response status line and return it
            string tempversion = "";
            if (httpversion == "HTTP10")
            {
                tempversion = "HTTP/1.0";
            }
            else if (httpversion == "HTTP09")
            {
                tempversion = "HTTP/0.9";
            }
            else if (httpversion == "HTTP11")
            {
                tempversion = "HTTP/1.1";
            }

            string statusLine = string.Empty;

            if (code.Equals(StatusCode.OK))
            {
                statusLine += tempversion + " OK";
            }
            else if (code.Equals(StatusCode.NotFound))
            {
                statusLine += tempversion + " 404 ERROR";
            }
            else if (code.Equals(StatusCode.InternalServerError))
            {
                statusLine += tempversion + "  Server Error";
            }
            else if (code.Equals(StatusCode.BadRequest))
            {
                statusLine += tempversion + code + " Bad Request";
            }
            else if (code.Equals(StatusCode.Redirect))
            {
                statusLine += tempversion + code + " Redirecting";
            }

            return statusLine;
        }

        private bool GetHeaders(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //if redirect return true
            headerLines.Add("Content-Type:" + contentType);
            headerLines.Add("Content-Length:" + content.Length);
            DateTime dateTime = DateTime.Now;
            headerLines.Add("Date:" + dateTime.ToString());
            //!redirectoinPath.Equals("")
            if (code.Equals(StatusCode.Redirect))
            {
                headerLines.Add("Location:" + redirectoinPath);
                return true;
            }
            return false;
        }
        
    }
}
