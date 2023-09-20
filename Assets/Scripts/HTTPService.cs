using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Policy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class HTTPService
{
    private Uri ListenUri;
    private HttpListener listener;

    private bool isListening;
    private Dictionary<string, Action<HttpListenerContext>> HandlerMap = new Dictionary<string,Action<HttpListenerContext>>();
    private static HTTPService _httpService;
    private HTTPService(Uri listnerPrefix)
    {
        ListenUri = listnerPrefix;
        Listen();
    }

    public static HTTPService Initialize(Uri listnerPrefix)
    {
        if(_httpService==null)
            _httpService = new HTTPService(listnerPrefix);
        return _httpService;
    }
    private void Listen()
    {
        try
        {
            listener = new HttpListener();
            listener.Prefixes.Add(ListenUri.ToString());
            listener.Start();
            isListening = true;
            this.Run();
        }
        catch (HttpListenerException ex)
        {
            Debug.LogWarning("Could not start HTTPListener: " + ex.Message);
        }
    }

    private void Run()
    {
        var thread = new System.Threading.Thread(() =>
        {
            while (isListening)
            {
                try
                {
                    handleRequest(listener.GetContext());
                }
                catch (HttpListenerException e)
                {
                    Debug.LogWarning("NavMeshPath Service: HTTP Listener error: " + e.Message);
                }
            }

            isListening = false;
        });
        thread.Start();
    }

    private void handleRequest(HttpListenerContext context)
    {
        try
        {
            Action<HttpListenerContext> handler = findHandlerFor(context.Request.Url);
            if (handler == null)
            {
                foreach (string paths in HandlerMap.Keys)
                {
                    Debug.Log(paths);
                }
                Debug.Log("Handler Map:"+HandlerMap.Keys.ToString());
                context.Response.StatusCode = 404;
                context.Response.Close();
                return;
            }
            handler.Invoke(context);
            // output.Close();
            // context.Response.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Internal error:" + ex.Message);
            context.Response.Close();
            return;
        }
    }

    public void RegisterService(string path, Action<HttpListenerContext> handler)
    {
        Debug.Log("Added service");
        HandlerMap.Add(path, handler);
    }

    public void UnregisterService(string path)
    {
        HandlerMap.Remove(path);
    }
    public void SendResult(HttpListenerContext context, string message)
    {
        // JObject jObject = (JObject)joResponse["color"];
        context.Response.StatusCode = 200;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        context.Response.ContentLength64 = buffer.Length;
        System.IO.Stream output = context.Response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }

    private Action<HttpListenerContext> findHandlerFor(Uri url)
    {
        Debug.Log("URL:"+url.ToString());
        Debug.Log("ListenUri:"+ListenUri.ToString());
        string relativePath = url.ToString().Substring(ListenUri.ToString().Length);
        Debug.Log("RelativePath:"+relativePath);
        Action<HttpListenerContext> handler;
        if (HandlerMap.TryGetValue(relativePath, out handler))
        {
            return handler;
        }
        else
        {
            return null;
        }
    }

    public void Stop()
    {
        Debug.Log("Stopping HTTPService");
        if (isListening)
        {
            isListening = false;
            listener.Stop();
        }
    }    
}