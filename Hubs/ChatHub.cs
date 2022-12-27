using genetrix.Models;
using genetrix.Models.Fonctions;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace genetrix.Hubs
{
    public class ChatHub : Hub
    {
        readonly ApplicationDbContext db = new ApplicationDbContext();
        DateTime dateNow;

        public ChatHub()
        {
            dateNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "W. Central Africa Standard Time");
        }
        public void Send(string name, string message,string chatId,string userId,string imagePath,bool loading,string user1="",int? imageId=null)
        {
            string[] rk4 = null;
            var chat = db.Chats.Include("Destinataires").FirstOrDefault(c => c.ChatId.ToString() == chatId);
            rk4 = (from n in chat.Destinataires select n.Id).ToArray();
            if (!loading)
            {
                if (!string.IsNullOrEmpty(message) || imageId>0)
                {
                    if (chat.Contenu == null) chat.Contenu = new List<MessageItem>();
                    chat.DernierEcrit = (byte)(user1 == "client" ? 1 : 2);
                    if(imageId!=null && imageId>0)
                    {
                        try
                        {
                            imagePath = Fonctions.ImageBase64ImgSrc(db.GetImageChats.Find(imageId).Url);
                            imagePath = $"<a href=\"OpenImage?idImage={imageId}\" target='_blank'><img src='{imagePath}' alt='' height='max-height:150px' width='150px'/><a/>";
                            //imagePath = $"<img src='{imagePath}' onclick=\"window.open({imagePath}, '_blank')\" alt ='' height='max-height:150px' width='150px'/>";
                            //message = "image";
                        }
                        catch (Exception)
                        {}
                    }
                    chat.Contenu.Add(new MessageItem()
                    {
                        ChatId = chat.ChatId,
                        Date = dateNow,
                        EmetteurName = name,
                        Text = message,
                        LienImage = imagePath
                    });
                    db.SaveChanges();  
                }
            }
            
            Clients.All.addNewMessageToPage(name, message,"-1", rk4,"11",loading, imagePath);
        }
    }
}