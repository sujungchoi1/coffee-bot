using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using CoffeeBot.Models;

namespace CoffeeBot.Dialogs
{
    public class CoffeeDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserData> _userDataAccessor;
        // private readonly IStatePropertyAccessor<CoffeeOrder> _coffeeOrderAccessor;
        public CoffeeDialog(UserState userState )
            : base(nameof(CoffeeDialog))
        {
            _userDataAccessor = userState.CreateProperty<UserData>("UserData");

            // Waterfall steps to be executed
            // var greetingStep = new WaterfallStep[]
            // {
            //     CustomerNameAsync,
            // };
            var waterfallSteps = new WaterfallStep[]
            {
                CustomerNameAsync,
                CoffeeMenuAsync,
                MilkOptionAsync,
                CoffeeSizeAsync,
                CoffeeTempAsync,
                ExtraOrderAsync,
                // ExtraOrderDetailAsync,
                ConfirmAsync,
                FinalStepAsync,
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            // AddDialog(new WaterfallDialog(nameof(WaterfallDialog), greetingStep));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }
        private static async Task<DialogTurnResult> CustomerNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("May I have your name please?") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> CoffeeMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["customerName"] = (string)stepContext.Result;
            var name = stepContext.Values["customerName"];

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Hi {name}, what would you like to order?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Americano", "Latte", "Mocha", "Green Tea Latte" }),
                }, cancellationToken);
        }
        private static async Task<DialogTurnResult> MilkOptionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["coffeeName"] = ((FoundChoice)stepContext.Result).Value;

            var coffeeName = stepContext.Values["coffeeName"];

            if ((string)coffeeName == "Latte" || (string)coffeeName == "Green Tea Latte")
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What type of milk would you like?"),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "Oat Milk", "Almond Milk", "Whole Milk" }),
                    }, cancellationToken);
            }
            // else if ((string)coffeeName == "Americano" || (string)coffeeName == "Mocha")
            // {
            //     return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("excellent choice!") }, cancellationToken);
            // }
            // await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("excellent choice!") }, cancellationToken);
            // return await stepContext.NextAsync("No Milk", cancellationToken);
            else
            {
                // return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Excellent choice! Please type anything to move to the next step!") }, cancellationToken);
                return await stepContext.NextAsync(new List<CoffeeOrder>(), cancellationToken);
            }

        }

        private static async Task<DialogTurnResult> CoffeeSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // stepContext.Values["noMilkOption"] = (string)stepContext.Result;
            // stepContext.Values[ "milkOption"] = (string)stepContext.Result;
            // stepContext.Values["milkOption"] = ((FoundChoice)stepContext.Result).Value;

            // ((string)stepContext.Result);

            if ((string)stepContext.Values["coffeeName"] == "Latte" || (string)stepContext.Values["coffeeName"] == "Green Tea Latte")
            {
                stepContext.Values["milkOption"] = ((FoundChoice)stepContext.Result).Value;
            }
            if ((string)stepContext.Values["coffeeName"] == "Mocha" || ((string)stepContext.Values["coffeeName"] == "Americano"))
            {
                stepContext.Values["milkOption"] = null;
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What size would you like for your coffee?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Small - 8 oz", "Medium - 10 oz", "Large - 12 oz" }),
                }, cancellationToken);
        }
        private static async Task<DialogTurnResult> CoffeeTempAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["coffeeSize"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Would you like it hot or iced?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Hot", "Iced" }),
                }, cancellationToken);

        }
        private static async Task<DialogTurnResult> ExtraOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["coffeeTemp"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Would you like to add anything else to your order?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Extra Shot", "Extra Syrup", "Whip Cream", "No, I'm done!" }),
                }, cancellationToken);
        }
        // private static async Task<DialogTurnResult> ExtraOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        // {
        //     stepContext.Values["coffeeTemp"] = ((FoundChoice)stepContext.Result).Value;

        //     return await stepContext.PromptAsync(nameof(ChoicePrompt),
        //         new PromptOptions
        //         {
        //             Prompt = MessageFactory.Text("Would you like to add anything else to your order?"),
        //             Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" }),
        //             // Choices = ChoiceFactory.ToChoices(new List<string> { "Extra Shot", "Extra Syrup", "Whip Cream", "No" }),
        //         }, cancellationToken);
        //     // return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to add anything else to your order?") }, cancellationToken);
        // }

        // *** If no, next async function is not initiated (probably cause stepcontext value is null?) ***
        // private static async Task<DialogTurnResult> ExtraOrderDetailAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        // {
        //     stepContext.Values["extra"] = ((FoundChoice)stepContext.Result).Value;
        //     var extra = stepContext.Values["extra"];
        //     // if user said 'yes'
        //     if ((string)extra == "Yes")
        //     {
        //         return await stepContext.PromptAsync(nameof(ChoicePrompt),
        //             new PromptOptions
        //             {
        //                 Prompt = MessageFactory.Text("What would you like to add?"),
        //                 Choices = ChoiceFactory.ToChoices(new List<string> { "Extra Shot", "Extra Syrup", "Whip Cream" }),
        //             }, cancellationToken);
        //     }
        //     else
        //     {
        //         await stepContext.NextAsync("No", cancellationToken);
        //         return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Great! Let us confirm our order!") }, cancellationToken);
        //     }
        // }

        private async Task<DialogTurnResult> ConfirmAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["extraOrder"] = ((FoundChoice)stepContext.Result).Value;

            // var extra = (string)stepContext.Values["extraOrder"] == "No" 
            //     ? await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please wait."), cancellationToken)
            //     : await stepContext.Context.SendActivityAsync(MessageFactory.Text("Great, let us confirm your order"), cancellationToken);

            var customerName = stepContext.Values["customerName"];
            var coffeeName = stepContext.Values["coffeeName"];
            var coffeeSize = stepContext.Values["coffeeSize"];
            var coffeeTemp = stepContext.Values["coffeeTemp"];
            // var extraOrder = stepContext.Values["extraOrder"];
            var extraOrder = (string)stepContext.Values["extraOrder"] == "No, I'm done!" ? null : "and " + stepContext.Values["extraOrder"];
            var milkOption = stepContext.Values["milkOption"] == null ? null : "with " + stepContext.Values["milkOption"];

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Let us confirm the order"), cancellationToken);
            // await Task.Delay(1000); can't put delays in between await functions
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Order for {customerName}:\n {coffeeSize} {coffeeTemp} {coffeeName} {milkOption} {extraOrder}"), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Everything looks correct?") }, cancellationToken);
        }

        // place the order (need order class)

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If Yes, a new order is added to the UserData
            if ((bool)stepContext.Result)
            {
                // var newOrder = (UserData)stepContext.Options;

                var newOrder = new UserData() { Name = (string)stepContext.Values["customerName"]};
                newOrder.Orders.Add(new CoffeeOrder()
                {
                    CustomerName = (string)stepContext.Values["customerName"],
                    CoffeeMenu = (string)stepContext.Values["coffeeName"],
                    CoffeeSize = (string)stepContext.Values["coffeeSize"],
                    CoffeeTemp = (string)stepContext.Values["coffeeTemp"],
                    ExtraOrder = (string)stepContext.Values["extraOrder"],
                    MilkOption = (string)stepContext.Values["milkOption"],
                });

                await _userDataAccessor.SetAsync(stepContext.Context, newOrder, cancellationToken);

                await stepContext.Context.SendActivityAsync("Thank you for your order! It will be ready shortly");

                Console.WriteLine(newOrder.Orders.FirstOrDefault().CoffeeMenu);

                return await stepContext.EndDialogAsync(newOrder, cancellationToken);
            }
            // If No, start over (asks for the name again; need to separate the first dialogue into a separate waterfall)
                // Provide an option to say what was wrong??
            await stepContext.Context.SendActivityAsync("We're sorry, we'll take your order again.");

            return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);

        }
    }
}