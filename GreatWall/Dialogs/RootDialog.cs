using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace GreatWall.Dialogs
{
    
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string welcomeMessage = "안녕하세요 만리장성 봇 입니다. 1.주문, 2. FAQ 중에 선택하세요";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
           
            await context.PostAsync(welcomeMessage);

            context.Wait(SendWelcomeMessageAsync);
        }


        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //erase space text
            string selected = activity.Text.Trim();
            
            if(selected =="1")
            {
                await context.PostAsync("음식 주문 메뉴 입니다. 원하시는 음식을 입력해 주십시오.");
                
                //other dialog call 
                context.Call(new OrderDialog(), DialogResumeAfter);
            }else if(selected == "2")
            {
                await context.PostAsync("FAQ서비스 입니다. 질문을 입력해 주십시오.");
                context.Call(new FAQDialog(), DialogResumeAfter);

            }
            else
            {
                await context.PostAsync("잘못 선택 하셨습니다. 다시 선택해 주십시오.");
                context.Wait(SendWelcomeMessageAsync);
            }

        }

        private async Task DialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string message = await result;

                await this.MessageReceivedAsync(context, result);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("오류가 생겼습니다. 죄송합니다.");
            }
        }


    }
}