using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using IOCLAndroidApp;

namespace Honda_Device_Android
{
    internal class BiSocket
    {
        private string Eom = ">";
        internal void TcpClosed()
        {
            try
            {
                if (clsGlobal.mTcpClient != null)
                {
                    byte[] bData = Encoding.ASCII.GetBytes("QUIT" + Eom);
                    clsGlobal.mTcpClient.Client.Send(bData);
                    clsGlobal.mTcpClient.Close();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                clsGlobal.mTcpClient = null;
            }
        }
        
        
        internal string GetSetTcp(string sData)
        {
            string Response = string.Empty;
            try
            {
                if (clsGlobal.mTcpClient == null)
                {
                    clsGlobal.mTcpClient = new TcpClient();
                    clsGlobal.mTcpClient.Connect(clsGlobal.mSockIp.ToString().Trim(), clsGlobal.mSockPort);
                }                                 
                byte[] bData = Encoding.ASCII.GetBytes(sData);
                clsGlobal.mTcpClient.Client.Send(bData);
                // System.Threading.Thread.Sleep(300);
                byte[] bRes = new byte[60000];
                int iLen = clsGlobal.mTcpClient.Client.Receive(bRes);
                Response = Encoding.ASCII.GetString(bRes, 0, iLen);
              //  Response = Response.Replace(Eom, "");
                string sReturn = "";
                while (!Response.EndsWith(Eom))
                {
                    sReturn += Response;
                    //byte[] bRes2 = new byte[104857600];
                    byte[] bRes2 = new byte[60000];
                    int iLen2 = clsGlobal.mTcpClient.Client.Receive(bRes2);
                    Response = Encoding.ASCII.GetString(bRes2, 0, iLen2);
                    if (Response.EndsWith("}"))
                    {
                        Response = Response.Replace(Eom, "");
                        break;
                    }
                }
                Response = Response.Replace(Eom, "");
                sReturn += Response;
                Response = sReturn.Replace(Eom, "");
            }
            catch (Exception ex)
            {
                clsGlobal.mTcpClient = null;
                Response = "NO_CONNECTION~" + ex.Message;
            }
            return Response;
        }
       
    }
}
