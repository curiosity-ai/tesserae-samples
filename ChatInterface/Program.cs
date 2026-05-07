using System;
using System.Linq;
using H5;
using H5.Core;
using static H5.Core.es5;
using static H5.Core.dom;
using Tesserae;
using static Tesserae.UI;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChatInterface
{
    class MockMessage
    {
        public string text { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Theme.Dark();
            document.body.style.background = Theme.Default.Background;
            document.body.style.color = Theme.Default.Foreground;

            var chatArea = ChatArea();

            string[] predefinedAnswers;
            try
            {
                var client = new HttpClient();
                var json = await client.GetStringAsync("mock_data.json");
                var messages = JsonConvert.DeserializeObject<List<MockMessage>>(json);
                predefinedAnswers = messages.Select(m => m.text).ToArray();
            }
            catch (Exception ex)
            {
                console.error("Failed to load mock data:", ex);
                predefinedAnswers = new[] { "Fallback answer: Error loading data." };
            }

            var random = new Random();

            Tesserae.OmniBox input = null;
            int typingAnimationId = 0;

            void AddAIAnswer()
            {
                typingAnimationId++;
                int currentAnimationId = typingAnimationId;

                var answer = predefinedAnswers[random.Next(predefinedAnswers.Length)];
                var words = answer.Split(' ');

                var copyButton = Button(UIcons.Copy).NoBorder().NoBackground().Tooltip("Copy");
                copyButton.Render().classList.remove("tss-btn-default");
                copyButton.Render().classList.add("tss-btn-icon-only");

                var msgComponent = ChatMessage(TextBlock(""), Avatar(null, "AI"), copyButton).MaxWidth();
                chatArea.Add(msgComponent);

                int index = 0;
                string currentText = "";

                void TypeNextWord()
                {
                    if (typingAnimationId != currentAnimationId)
                    {
                        // Stoped or new one started
                        return;
                    }

                    if (index >= words.Length)
                    {
                        input.IsGenerating = false;
                        return;
                    }

                    currentText += (index > 0 ? " " : "") + words[index];
                    msgComponent.ReplaceContent(TextBlock(currentText));
                    msgComponent.KeepVisible();

                    index++;
                    window.setTimeout(_ => TypeNextWord(), 150);
                }

                window.setTimeout(_ => TypeNextWord(), 500);
            }

            // Preload some messages
            chatArea.Add(ChatMessage(TextBlock("Hello there!"), Avatar(null, "U")).RightAligned().MaxWidth());
            chatArea.Add(ChatMessage(TextBlock("Hi! How can I help you today?"), Avatar(null, "AI")).MaxWidth());

            input = UI.OmniBox(new Tesserae.OmniBox.Config(Tesserae.OmniBox.Mode.Chat)
            {
                PlaceholderChat = "Ask anything...",
                IconStop = UIcons.Stop,
                IconChat = UIcons.ArrowRight
            })
            .OnChat((sender, msg) =>
            {
                var text = msg.Text;
                if (string.IsNullOrWhiteSpace(text)) return;

                chatArea.Add(ChatMessage(TextBlock(text), Avatar(null, "U")).RightAligned().MaxWidth());

                sender.IsGenerating = true;
                AddAIAnswer();
            })
            .OnStop((sender) =>
            {
                typingAnimationId++; // cancel
                sender.IsGenerating = false;
            });

            var chatContainer = VStack().WS().HS().Grow().Children(
                chatArea.WS().H(10).Grow(),
                input.WS().H(150)
            );

            document.body.appendChild(chatContainer.Render());
        }
    }
}
