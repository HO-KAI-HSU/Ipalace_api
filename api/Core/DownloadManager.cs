using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public class DefaultDownloader : Downloader
{
    protected static DefaultDownloader _instance = new DefaultDownloader();

    public static DefaultDownloader Instance
    {
        get
        {
            return _instance;
        }
    }
}

public class Downloader
{
    public string ContentType = string.Empty;

    public CookieContainer Cookies = null;

    public string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";

    public bool AutoDecompressGZip = true;

    public string Accept;

    public WebProxy Proxy = null;

    public int DownloadFile(String url, string filePath, Encoding enc = null, string postData = null, string referer = null, IDictionary<string, string> headers = null)
    {
        using (FileStream localStream = File.Create(filePath))
        {
            if (enc == null)
            {
                enc = System.Text.Encoding.UTF8;
            }

            if (postData == null)
            {
                return Download(url, localStream, null, referer, headers);
            }
            else
            {
                return Download(url, localStream, enc.GetBytes(postData), referer, headers);
            }
        }
    }

    public byte[] DownloadBinary(String url, Encoding enc = null, string postData = null, string referer = null, IDictionary<string, string> headers = null)
    {
        using (MemoryStream localStream = new MemoryStream(1024))
        {
            if (enc == null)
            {
                enc = System.Text.Encoding.UTF8;
            }

            if (postData == null)
            {
                Download(url, localStream, null, referer, headers);
            }
            else
            {
                Download(url, localStream, enc.GetBytes(postData), referer, headers);
            }

            return localStream.ToArray();
        }
    }

    public string DownloadString(String url, Encoding enc = null, string postData = null, string referer = null, IDictionary<string, string> headers = null, string method = "GET")
    {
        // System.Diagnostics.Debug.Write(url);
        if (enc == null)
        {
            enc = System.Text.Encoding.UTF8;
        }

        MemoryStream localStream = new MemoryStream();
        if (postData == null)
        {
            Download(url, localStream, null, referer, headers, method);
        }
        else
        {
            Download(url, localStream, enc.GetBytes(postData), referer, headers, method);
        }

        return enc.GetString(localStream.GetBuffer());
    }

    private int Download(String url, Stream localStream, byte[] postData, string referer, IDictionary<string, string> headers, string method = "GET")
    {
        // Function will return the number of bytes processed
        // to the caller. Initialize to 0 here.
        int bytesProcessed = 0;

        // Assign values to these objects here so that they can
        // be referenced in the finally block
        Stream remoteStream = null;
        WebResponse response = null;

        // Use a try/catch/finally block as both the WebRequest and Stream
        // classes throw exceptions upon error
        try
        {
            // Create a request for the specified remote file name
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ServicePoint.Expect100Continue = false;
            if (AutoDecompressGZip)
            {
                request.AutomaticDecompression = DecompressionMethods.GZip;
            }

            if (request != null)
            {
                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        request.Headers.Add(h.Key, h.Value);
                    }
                }

                request.Proxy = Proxy;

                if (referer != null)
                {
                    request.Referer = referer;
                }

                if (Cookies != null)
                {
                    request.CookieContainer = Cookies;
                }

                ((HttpWebRequest)request).UserAgent = UserAgent;

                if (!string.IsNullOrEmpty(Accept))
                {
                    ((HttpWebRequest)request).Accept = Accept;
                }

                if (postData != null)
                {
                    if (method == "GET")
                    {
                        method = "POST";
                    }

                    request.Method = method;
                    if (string.IsNullOrEmpty(ContentType))
                    {
                        ContentType = "application/json";
                    }

                    request.ContentType = ContentType;
                    request.ContentLength = postData.Length;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postData, 0, postData.Length);
                        requestStream.Close();
                    }
                }
                else
                {
                    request.Method = method;
                }

                // Send the request to the server and retrieve the
                // WebResponse object
                response = request.GetResponse();
                if (response != null)
                {
                    if (OnStartDownload != null)
                    {
                        OnStartDownload.Invoke(url, response.ContentLength);
                    }

                    // ContentType = response.ContentType;

                    // Once the WebResponse object has been retrieved,
                    // get the stream object associated with the response's data
                    remoteStream = response.GetResponseStream();

                    // Allocate a 1k buffer
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    // Simple do/while loop to read from stream until
                    // no bytes are returned
                    do
                    {
                        // Read data (up to 1k) from the stream
                        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                        // Write the data to the local file
                        localStream.Write(buffer, 0, bytesRead);

                        // Increment total bytes processed
                        bytesProcessed += bytesRead;

                        if (OnProgress != null)
                        {
                            OnProgress.Invoke(url, bytesProcessed);
                        }
                    }
                    while (bytesRead > 0);
                }

                if (OnBeforeFinish != null)
                {
                    OnBeforeFinish.Invoke(url, response);
                }

                if (OnFinish != null)
                {
                    OnFinish.Invoke(url);
                }
            }
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                try
                {
                    var resp = ex.Response.GetResponseStream();
                    byte[] data = new byte[resp.Length];
                    var task = resp.ReadAsync(data, 0, (int)resp.Length);
                    task.Wait();
                    var rs = System.Text.Encoding.UTF8.GetString(data);

                    if (OnError != null)
                    {
                        OnError.Invoke(url, new Exception(rs));
                    }
                }
                catch (Exception)
                {
                    OnError?.Invoke(url, ex);
                }
            }
            else
            {
                OnError?.Invoke(url, ex);
            }
        }
        catch (Exception e)
        {
            if (OnError != null)
            {
                OnError.Invoke(url, e);
            }
        }
        finally
        {
            // Close the response and streams objects here
            // to make sure they're closed even if an exception
            // is thrown at some point
            if (response != null)
            {
                response.Close();
            }

            if (remoteStream != null)
            {
                remoteStream.Close();
            }
        }

        return bytesProcessed;
    }

    #region Event

    public delegate void OnErrorHandler(string url, Exception ex);

    public event OnErrorHandler OnError;

    public delegate void OnStartDownloadHandler(string url, long totalSize);

    public event OnStartDownloadHandler OnStartDownload;

    public delegate void OnProgressHandler(string url, int bytesRead);

    public event OnProgressHandler OnProgress;

    public delegate void OnFinishHandler(string url);

    public event OnFinishHandler OnFinish;

    public delegate void OnBeforeFinishHandler(string url, WebResponse response);

    public event OnBeforeFinishHandler OnBeforeFinish;

    #endregion
}
