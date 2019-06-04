using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using IOCLAndroidApp;

class clsNetwork
{
    TcpClient client;
    bool IS_CONNECTED = false;
    public string fnSendReceiveData(string strData)
    {
        string Response = string.Empty;
        try
        {
            client = new TcpClient();

            client.Connect(clsGlobal.mSockIp, Convert.ToInt32(clsGlobal.mSockPort));
           
            IS_CONNECTED = true;
            byte[] bData = Encoding.ASCII.GetBytes(strData);
            client.Client.Send(bData);
           
            byte[] bRes = new byte[500000];
            int iLen = client.Client.Receive(bRes);
            Response = Encoding.ASCII.GetString(bRes, 0, iLen);
            client.Client.Send(Encoding.ASCII.GetBytes("quit"));

            client.Close();
        }
        catch (Exception ex)
        {
            if (IS_CONNECTED)
                client.Close();
            Response = "NO_CONNECTION";
        }
        finally
        { client = null; }
        return Response;
    }
}

