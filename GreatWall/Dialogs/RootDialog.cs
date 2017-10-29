using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace GreatWall.Dialogs
{
    
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string welcomeMessage = "안녕하세요 만리장성 봇 입니다. 1.주문, 2. FAQ 중에 선택하세요";

        //초기화 , context 에는 chatbot에 관한 주요 기능이 다 있다. 
        public Task StartAsync(IDialogContext context)
        {
            //사용자 입력을 기다리고 있다. 
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        //사용자의 입력이 들어 왔다. 
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
           
            await context.PostAsync(welcomeMessage);

            var message = context.MakeMessage();
            //CardAction === button 
            var actions = new List<CardAction>();

            //버튼을 누르는 순간, CardAction 
            //button에 대한 action이 지정 
            actions.Add(new CardAction() { Title ="1. 주문", Value ="1"}); 
            actions.Add(new CardAction() { Title ="2. FAQ", Value ="2"});


            //messinger에 상관 없이 똑같이 보인다. 
            //값을 주면,값을 준것만 동작한다. 
            message.Attachments.Add(
                new HeroCard
                {
                    Title = "원하는 기능을 선택하세요",
                    Buttons = actions
                }.ToAttachment()
            );

            //사용자에게 응답을 해준다. 
            await context.PostAsync(message);
            context.Wait(SendWelcomeMessageAsync);
        }

        //사용자 입력에 대한 처리를 위해서 , 3단계로 구성을 해야 인사 기능이 구현이 된다. 
        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //erase space text
            string selected = activity.Text.Trim();
            
            if(selected == "1")
            {
                await context.PostAsync("음식 주문 메뉴 입니다. 원하시는 음식을 입력해 주십시오.");
                //다른 다이얼로그를 추가해서 이동 하라. 
                //다른 다이얼로그로 갔다가 다시 왔을때 어떠한 기능을 할것인지 메소드를 지정해준다. 
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