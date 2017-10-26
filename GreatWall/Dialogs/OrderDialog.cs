using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;

namespace GreatWall.Dialogs
{
    [Serializable]
    public class OrderDialog : IDialog<string>
    {
        string ServerUrl = "http://greatwallweb.azurewebsites.net/Images/";
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            var activity = await result as Activity;

            await context.PostAsync("메뉴를 선택해 주세요.");

            /* Black noodle
             * Create Image Object.
            */

            List<CardImage> menuimages = new List<CardImage>();
            menuimages.Add(new CardImage() { Url = this.ServerUrl + "menu1.JPG"});

            // create Button
            List<CardAction> menu1Buttons = new List<CardAction>();
            menu1Buttons.Add(new CardAction() { Title="짜장면", Value="짜장면"});

            HeroCard menu1Card = new HeroCard()
            {
                Title ="짜장면",
                Subtitle ="전통적인 짜장면 입니다.",
                Images = menuimages,
                Buttons = menu1Buttons

            };

            /* another noodle
             * create image object
             */

            List<CardImage> menu2images = new List<CardImage>();
            menu2images.Add(new CardImage() { Url = this.ServerUrl + "menu2.JPG"});

            //Create Button
            List<CardAction> menu2Buttons = new List<CardAction>();
            menu2Buttons.Add(new CardAction() { Title ="짬뽕", Value="짬뽕"});

            HeroCard menu2Card = new HeroCard()
            {
                Title = "짬뽕",
                Subtitle ="시원한 국물의 짬뽕입니다.",
                Images = menu2images,
                Buttons = menu2Buttons
            };

            /* another food
             * Create image object
             */
            List<CardImage> menu3images = new List<CardImage>();
            menu3images.Add(new CardImage() { Url = this.ServerUrl + "menu3.JPG"});

            //Create Button
            List<CardAction> menu3Buttons = new List<CardAction>();
            menu3Buttons.Add(new CardAction() { Title ="탕수육", Value="탕수육"});

            HeroCard menu3Card = new HeroCard()
            {
                Title ="탕수육",
                Subtitle ="부먹찍먹 모두 맛있는 탕수육 입니다.",
                Images = menu3images,
                Buttons = menu3Buttons
            };

           

            if (activity.Text.Trim() == "그만")
            {
                context.Done("주문 완료");
            }
            else
            {
                var message = context.MakeMessage();
                message.Attachments.Add(menu1Card.ToAttachment());
                message.Attachments.Add(menu2Card.ToAttachment());
                message.Attachments.Add(menu3Card.ToAttachment());

                await context.PostAsync(message);

                context.Wait(this.MessageReceivedAsync);

            }


            

        }



    }
}

