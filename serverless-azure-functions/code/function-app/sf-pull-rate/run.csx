using System;
using System.Net;
using Newtonsoft.Json;

public class Message
{
    public string Rate { get; set; }
}


public static string ParseRate (string resp) 
{ 
    var opening = "<Rate>";
    var closing = "</Rate>";
    
    var startIndex = resp.IndexOf(opening) + 6;
    var endIndex = resp.IndexOf(closing) - startIndex;

    return resp.Substring(startIndex,endIndex);
}

public static void Run(TimerInfo myTimer, out string outputQueueItem, TraceWriter log)
{
    var url = @"http://query.yahooapis.com/v1/public/yql?q=select * from yahoo.finance.xchange where pair in (""GBPUSD"")&env=store://datatables.org/alltableswithkeys";
    var client = new WebClient();
    var result = client.DownloadString(url);
    var rate = ParseRate(result);

    log.Info(rate);

    var msg = new Message();
    msg.Rate = rate;
    
    outputQueueItem = JsonConvert.SerializeObject(msg);
    
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");    
}