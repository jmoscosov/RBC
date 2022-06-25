using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;

namespace ByPass
{
    public class SendStatusNotSolicited
    {
        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\NCR\Advance NDC\ByPassRBC");
        string RBCMessageLetter;
        public void send()
        {
            
            Logger.Log($"JM185384 entry to Thread");
            int intTimer03 = NDCMessage.GetIntVal(3129);
            Logger.Log($"Timer03 Value :{intTimer03}");
            bool txReplyMessage = false;
            if (key != null)
            {
                RBCMessageLetter = (string)key.GetValue("ByPassRBCID");
            }
            else
            {
                RBCMessageLetter = "R";
            }
            for (int i = 0; i < intTimer03 ; i++)
            {
                if (NDCMessage.GetIntValUCDI("ByPassUCDI") == 0)
                {
                    txReplyMessage = true;
                    break;
                }
                Thread.Sleep(1000);
            }
            if (!txReplyMessage)
            {

                Logger.Log($"JM185384 Sending Message not solicited with ID : {RBCMessageLetter}");
                NDCMessage.SendStatus(RBCMessageLetter, false, false);
            }
            
            
            Logger.Log($"exit to Thread");
        }
    }
}
