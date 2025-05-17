using AdminDashboard.Request;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace AdminDashboard
{
    public partial class ChatForm : Form
    {
        private readonly SupabaseRealtimeClient _connection;
        private readonly int _chatId;
        private readonly string _token;
        private readonly string _category;
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;
        public ChatForm(string token, int chatId, string category)
        {
            _category = category;
            _token = token;
            _chatId = chatId;
            InitializeComponent();
            InitializeChat();
            _connection = new SupabaseRealtimeClient(chatBox, token);
        }

        private async void ChatForm_Load(object sender, EventArgs e)
        {
            await _connection.ConnectAsync();
            Text = $"Chat – {_chatId}";
        }

        private void InitializeChat()
        {
            this.Size = new System.Drawing.Size(600, 500);
            this.Text = "Realtime Chat";
            this.BackColor = Color.FromArgb(18, 18, 18);  // Dark background

            var backButton = new Button
            {
                Text = "←",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(40, 30),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatAppearance = { BorderSize = 0 }
            };
            backButton.Click += async (sender, e) =>
            {

                if (string.IsNullOrWhiteSpace(inputBox.Text)) return;


                var userInfo = await new Profile(_token).GetUserAsync();
                var role = userInfo.User.Role;
                var name = userInfo.User.Name;
                var frm = ((Control)sender).FindForm();
                if (role == "user"){
                    chatBox.SelectionAlignment = HorizontalAlignment.Left;
                    chatBox.SelectionColor = Color.White;
                    chatBox.SelectionBackColor = Color.FromArgb(60, 60, 60);
                    chatBox.AppendText("This session has reached a conclusion,\nIf you need any further assistance, feel free to start a new chat and we will be glad to help,\nKind regards");
                    frm?.Close();
                }

                if (role == "admin") // For admin messages
                {
                    string desc = null;
                    do
                    {
                        desc = Interaction.InputBox(
                            "Enter a description about this chat:",
                            "Chat Description",
                            ""
                        );
                        MessageBox.Show("Description is required to create a ticket.", "Validation Error");
                    } while (string.IsNullOrWhiteSpace(desc));

                    var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
                    var ticketNumber = $"{datePart}-{_chatId}-{_category}-{name}";
                    var ticket = new ChatTicketRequest
                    {
                        ChatId = _chatId,
                        Description = desc,
                        TicketNumber = ticketNumber,
                        Topic = _category
                    };

                    if (await new ChatTicket(_token).CreateAsync(ticket))
                    {
                        MessageBox.Show($"Ticket {ticketNumber} created successfully.", "Success");
                        if(await new Chat(_token).UpdateAsync(_chatId))
                        {
                            MessageBox.Show($"Chat {_chatId} is Closed.", "Success");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show($"Failed to close chat {_chatId}.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to create ticket.", "Error");
                    }
                }
            };
            this.Controls.Add(backButton);

            chatBox = new RichTextBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(540, 270),
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            this.Controls.Add(chatBox);

            inputBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 340),
                Size = new System.Drawing.Size(400, 30),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(inputBox);

            sendButton = new Button
            {
                Text = "Send",
                Location = new System.Drawing.Point(440, 340),
                Size = new System.Drawing.Size(120, 30),
                BackColor = Color.FromArgb(0, 122, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            sendButton.FlatAppearance.BorderSize = 0;
            sendButton.Click += async (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(inputBox.Text)) return;

                var role = await new Profile(_token).GetRole();
                var message = new MessageRequest
                {
                    ChatId = _chatId,
                    Content = inputBox.Text,
                    UserDisplay = role,
                };

                var response = await new Message(_token).CreateAsync(message);
                if (!response)
                {
                    MessageBox.Show("Failed to send message.");
                    return;
                }

                if (role == "user")
                {
                    chatBox.SelectionAlignment = HorizontalAlignment.Right;
                    chatBox.SelectionColor = Color.White;
                    chatBox.SelectionBackColor = Color.FromArgb(0, 122, 255);
                }
                else if (role == "admin") // For admin messages
                {
                    chatBox.SelectionAlignment = HorizontalAlignment.Left;
                    chatBox.SelectionColor = Color.White;
                    chatBox.SelectionBackColor = Color.FromArgb(60, 60, 60);
                }

                // Append the message with the sender's name
                chatBox.AppendText($"{message}\n");
                inputBox.Clear();
                chatBox.ScrollToCaret();
            };
            this.Controls.Add(sendButton);
        }
    }
}
