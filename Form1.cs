using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Growl.Connector;
using SKYPE4COMLib;
/*Copyright: Barry O'Gorman ( bogorman@gmail.com ) under the 
 * Appache License http://www.apache.org/licenses/LICENSE-2.0

Modified By LordGregGreg to work with x64 and to have a icon.

Description: Send's Notifications from Skype to Growl for Windows.
 * */
namespace SkypeToGrowl
{
    public partial class Form1 : Form
    {
        private GrowlConnector growl;
        private NotificationType voiceMailNotificationType;
        private NotificationType externalCallNotificationType;
        private NotificationType internalCallNotificationType;
        private NotificationType messageCallNotificationType;
        public static Skype oSkype = new Skype();
        private Growl.Connector.Application application;

        private string vmNotificationType = "VM_NOTIFICATION";
        private string extNotificationType = "EXT_NOTIFICATION";
        private string intNotificationType = "INT_NOTIFICATION";
        private string msgNotificationType = "MSG_NOTIFICATION";

        public Form1()
        {
            InitializeComponent();
            //oSkype = new Skype();
            try
            {
                if (!oSkype.Client.IsRunning)
                    throw new Exception("Client not running");

                this.voiceMailNotificationType = new NotificationType(vmNotificationType, "VoiceMail Notification");
                this.externalCallNotificationType = new NotificationType(extNotificationType, "External Call Notification");
                this.internalCallNotificationType = new NotificationType(intNotificationType, "Skype Call Notification");
                this.messageCallNotificationType = new NotificationType(msgNotificationType, "Message Notification");
                
                this.growl = new GrowlConnector();

                this.growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);

                this.growl.EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText;

                this.application = new Growl.Connector.Application("Skype");


                this.application.Icon = (Image)SkypeToGrowl.Properties.Resources.skype_icon;

                NotificationType[] types = new NotificationType[4];
                types[0] = voiceMailNotificationType;
                types[1] = externalCallNotificationType;
                types[2] = internalCallNotificationType;
                types[3] = messageCallNotificationType;

                this.growl.Register(application, types);

                oSkype.CallStatus += Skype_CallStatus;
                oSkype.VoicemailStatus += Skype_VoicemailStatus;
                oSkype.MessageStatus += Skype_MessageStatus;
                oSkype.Attach(5, true);

                return;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Please verify Skype and Growl are running\n"+ex);
            }
            Close();

        }

        void growl_NotificationCallback(Response response, CallbackData callbackData)
        {
            //string text = String.Format("Response Type: {0}\r\nNotification ID: {1}\r\nCallback Data: {2}\r\nCallback Data Type: {3}\r\n", callbackData.Result, callbackData.NotificationID, callbackData.Data, callbackData.Type);
            //MessageBox.Show(text, "Callback received", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        void Skype_CallStatus(Call call, TCallStatus status)
        {
            //Console.WriteLine(">Call " + call.Id + " status: " + oConvert.CallStatusToText(status));
            if (status != TCallStatus.clsRinging)
                return;
            
            if (call.Type == TCallType.cltIncomingP2P)
            {
                SendGrowlMessage(this.externalCallNotificationType, "Skype Call", "Incoming Call from " + call.PartnerDisplayName);
            }
            else if (call.Type == TCallType.cltIncomingPSTN)
            {
                SendGrowlMessage(this.internalCallNotificationType, "External Call", "Incoming Call from " + call.PartnerDisplayName);
            }
        }

        void Skype_VoicemailStatus(Voicemail voicemail, TVoicemailStatus status)
        {
            //Console.WriteLine(">Voicemail status: " + oConvert.VoicemailStatusToText(status));
            if (status == TVoicemailStatus.vmsNotDownloaded)
            {
                //Console.WriteLine("Sending SMS to " + sMyMobileNumber);
                //SmsMessage oMessage = oSkype.SendSms(sMyMobileNumber, "You have new voicemail from " + voicemail.PartnerDisplayName, "");
                SendGrowlMessage(this.voiceMailNotificationType, "Skype Voicemail", "You have new voicemail from " + voicemail.PartnerDisplayName);
            }
        }

        void Skype_MessageStatus(ChatMessage pMessage, TChatMessageStatus status)
        {
            //Console.WriteLine(">Voicemail status: " + oConvert.VoicemailStatusToText(status));
            if (status == TChatMessageStatus.cmsReceived)
            {
                //Console.WriteLine("Sending SMS to " + sMyMobileNumber);
                //SmsMessage oMessage = oSkype.SendSms(sMyMobileNumber, "You have new voicemail from " + voicemail.PartnerDisplayName, "");
                SendGrowlMessage(this.messageCallNotificationType, "Skype IM", "IM from " + pMessage.FromDisplayName + ". " + pMessage.Body);
            }
        }

        void SendGrowlMessage(NotificationType inNotifyType, string inNotifyDisplayMessageType, string inNotifyMessage)
        {
            CallbackContext callbackContext = new CallbackContext("Skype Data", "Skype Type");
            
            Notification notification = new Notification(this.application.Name, inNotifyType.Name, DateTime.Now.Ticks.ToString(), inNotifyDisplayMessageType, inNotifyMessage);
            this.growl.Notify(notification, callbackContext);

        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
