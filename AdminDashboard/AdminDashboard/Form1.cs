using AdminDashboard.Handler;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public partial class adminFrm : Form
    {
        private Panel sidebar;
        private Label dashboardTitle;
        public TextBox emailTextBox;
        public TextBox passwordTextBox;
        private Button loginButton;

        private Panel contentPanel;
        private Panel adminPanel;
        private Panel adminInfoPanel;
        private Label adminName;
        private Label profileIcon;
        private Label contentTitle;

        private string Token;
        public adminFrm()
        {
            InitializeComponent();
            InitializeDashboard();
        }
        private void InitializeDashboard()
        {
            // Sidebar panel
            sidebar = new Panel();
            sidebar.BackColor = Color.FromArgb(51, 51, 76);
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 200;
            this.Controls.Add(sidebar);

            // Dashboard title in sidebar
            dashboardTitle = new Label();
            dashboardTitle.Text = "Admin Dashboard";
            dashboardTitle.ForeColor = Color.White;
            dashboardTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            dashboardTitle.Location = new Point(20, 20);
            dashboardTitle.AutoSize = true;
            sidebar.Controls.Add(dashboardTitle);

            // Email TextBox
            emailTextBox = new TextBox();
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Location = new Point(20, 95);
            emailTextBox.Size = new Size(160, 25);
            emailTextBox.Font = new Font("Segoe UI", 10);
            emailTextBox.AddPlaceholder("Enter your email");
            sidebar.Controls.Add(emailTextBox);

            // Password TextBox
            passwordTextBox = new TextBox();
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Location = new Point(20, 155);
            passwordTextBox.Size = new Size(160, 25);
            passwordTextBox.Font = new Font("Segoe UI", 10);
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.AddPlaceholder("Enter your password");
            sidebar.Controls.Add(passwordTextBox);

            // Main content panel
            contentPanel = new Panel
            {
                Visible = false,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(33, 37, 41),
                ForeColor = Color.White
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();

            // Create the admin panel container
            adminPanel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.DarkBlue
            };
            contentPanel.Controls.Add(adminPanel);

            // Dashboard title in top-left (inside adminPanel)
            contentTitle = new Label
            {
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Text = "Dashboard",
                Location = new Point(30, 15),
                AutoSize = true
            };
            adminPanel.Controls.Add(contentTitle);

            // Admin info panel int top right 
            adminInfoPanel = new Panel
            {
                Size = new Size(200, 40),
                Location = new Point(adminPanel.Width - 220, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };
            adminPanel.Controls.Add(adminInfoPanel);

            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            adminInfoPanel.Controls.Add(flowPanel);

            // Profile icon (left side)
            profileIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 14),
                Size = new Size(40, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 5, 0)  // Space between icon and name
            };
            flowPanel.Controls.Add(profileIcon);

            // Admin name (right side)
            adminName = new Label
            {
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 10, 0, 0),
                Margin = new Padding(0)
            };
            flowPanel.Controls.Add(adminName);

            // Login Button
            loginButton = new Button();
            loginButton.Text = "Login";
            loginButton.BackColor = Color.FromArgb(0, 120, 215);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            loginButton.Location = new Point(20, 200);
            loginButton.Size = new Size(160, 35);
            loginButton.Click += async (_, e) =>
            {
                contentPanel.Visible = false;
                string email = emailTextBox.Text;
                string password = passwordTextBox.Text;
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both email and password");
                    return;
                }

                loginButton.Enabled = false;
                loginButton.Text = "Authenticating...";
                try
                {
                    var userInfo = await new Login().AuthenticateUserAsync(email, password);
                    if (userInfo == null)
                    {
                        MessageBox.Show("Invalid credentials", "Login Failed");
                        return;
                    }
                    if (userInfo.User.Role == "user")
                    {
                        MessageBox.Show("Unauthorize", "Login Failed");
                        return;
                    }
                    Token = userInfo.Token;
                    adminName.Text = userInfo?.User.Name;
                    contentPanel.Visible = true;

                    /* Chat
                    if(userInfo.User.Role == "user")
                    {
                        // Display instruction label
                        var titleLabel = new Label
                        {
                            Text = "Choose a category to contact with a live agent",
                            Location = new System.Drawing.Point(20, 20),
                            AutoSize = true
                        };
                        contentPanel.Controls.Add(titleLabel);

                        // Create a group of radio buttons for single selection
                        List<RadioButton> categoryRadioButtons = new List<RadioButton>();

                        var categories = new (string Text, int Y)[]
                        {
                            ("Customer Service", 60),
                            ("Call Center", 90),
                            ("Billing Issue", 120),
                            ("Other", 150)
                        };

                        foreach (var (text, y) in categories)
                        {
                            var radioButton = new RadioButton
                            {
                                Text = text,
                                Location = new System.Drawing.Point(40, y),
                                AutoSize = true
                            };
                            contentPanel.Controls.Add(radioButton);
                            categoryRadioButtons.Add(radioButton);
                        }

                        // Start Chat button
                        var startChatBtn = new Button
                        {
                            Text = "Start Chat",
                            Location = new System.Drawing.Point(150, 200)
                        };
                        startChatBtn.Click += async (se, ev) =>
                        {
                            // Find the selected radio button
                            var selectedCategory = categoryRadioButtons
                                .FirstOrDefault(rb => rb.Checked)?.Text;

                            if (string.IsNullOrEmpty(selectedCategory))
                            {
                                MessageBox.Show("Please select a category.", "No Category Selected");
                                return;
                            }

                            var res = await new Chat(Token).CreateAsync(selectedCategory);
                            if(res == null)
                            {
                                MessageBox.Show("Something Error");
                                return;
                            }

                            // Open chat form with the selected category
                            var chatForm = new ChatForm(Token, res.chatId, selectedCategory);
                            chatForm.Show();
                        };
                        contentPanel.Controls.Add(startChatBtn);
                        return;
                    }
                    
                    var chatButton = new Button
                    {
                        Text = "💬",
                        Size = new Size(40, 40),
                        BackColor = Color.Yellow,
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = Color.Black,
                        Dock = DockStyle.Right
                    };
                    chatButton.Click += async (s, ev) =>
                    {
                        var res = await new Chat(Token).CreateAsync();
                        var chatForm = new ChatForm(Token, res.chatId, res.category); // or pass userId, etc.
                        if (chatForm == null)
                        {
                            MessageBox.Show("There is no Pending Chats");
                            return;
                        }
                        chatForm.Show(); // Use ShowDialog() if you want it modal
                    };
                    // Add button to the same panel
                    adminInfoPanel.Controls.Add(chatButton);
                    */

                    await LoadDashboardMetricsAsync(Token);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Login Error");
                }
                finally
                {
                    // Restore button state
                    loginButton.Enabled = true;
                    loginButton.Text = "Login";
                }
            };
            sidebar.Controls.Add(loginButton);
        }
        private async Task LoadDashboardMetricsAsync(string Token)
        {
            var metrics = new[]
                    {
                        new { Title="Manage Users",      Key="users",     Subtitle="Users Counts",      X=30,  Y=80,  Color=Color.Gray },
                        new { Title="Manage Coupons",    Key="coupons",   Subtitle="Coupons Counts",    X=280, Y=80,  Color=Color.Red },
                        new { Title="Manage Orders",     Key="orders",    Subtitle="Orders Counts",     X=530, Y=80,  Color=Color.Green },
                        new { Title="Manage Products",   Key="products",  Subtitle="Products Counts",   X=30,  Y=280, Color=Color.Blue },
                        new { Title="Manage Brands",     Key="brands",    Subtitle="Brands Counts",     X=280, Y=280, Color=Color.BlueViolet },
                        new { Title="Manage Categories", Key="categories",Subtitle="Categories Counts", X=530, Y=280, Color=Color.DarkBlue }
                    };

            contentPanel.SuspendLayout();
            for (int i = 0; i < metrics.Length; i++)
            {
                var key = metrics[i].Key;
                var card = new MetricCard
                {
                    Title = metrics[i].Title,
                    Tag = key,
                    Count = 0,
                    Subtitle = metrics[i].Subtitle,
                    CardColor = metrics[i].Color,
                    Location = new Point(metrics[i].X, metrics[i].Y)
                };
                card.CardClicked += (s, ev) =>
                {
                    var clickedCard = (MetricCard)s;
                    var currentValue = clickedCard.Tag.ToString();

                    clickedCard.Enabled = false;
                    clickedCard.Cursor = Cursors.WaitCursor;

                    Form res = (currentValue == "users") ? res = new UsersManagementForm(Token) :
                        (currentValue == "coupons") ? res = new CouponsManagementForm(Token) :
                        (currentValue == "orders") ? res = new OrdersManagementForm(Token) :
                        (currentValue == "products") ? res = new ProductsManagementForm(Token) :
                        (currentValue == "brands") ? res = new BrandsManagementForm(Token) :
                        (currentValue == "categories") ? res = new CategoriesManagementForm(Token) :
                        throw new InvalidOperationException($"Unknown key: {key}");

                    res.StartPosition = FormStartPosition.CenterParent;
                    res.FormClosed += async (se, args) =>
                    {
                        int newCnt = 0;
                        if (currentValue == "products")
                            newCnt = await new Product(Token).GetTotalCountAsync();

                        else if (currentValue == "brands")
                            newCnt = await new Brand(Token).GetTotalCountAsync();

                        else if (currentValue == "categories")
                            newCnt = await new Category(Token).GetTotalCountAsync();

                        else if (currentValue == "users")
                            newCnt = await new Profile(Token).GetTotalCountAsync();

                        else if (currentValue == "coupons")
                            newCnt = await new Coupon(Token).GetTotalCountAsync();

                        else if (currentValue == "orders")
                            newCnt = await new Order(Token).GetTotalCountAsync();

                        clickedCard.RefreshCount(newCnt);
                        clickedCard.Enabled = true;
                        clickedCard.Cursor = Cursors.Hand;
                    };
                    res.Show(this);
                };
                contentPanel.Controls.Add(card);

                ControlHandler.SetDoubleBuffered(card);
                try
                {
                    int count = await FetchCount(key, Token);
                    if (!card.IsDisposed)
                    {
                        card.Invoke((MethodInvoker)(() => card.RefreshCount(count)));
                    }
                }
                catch
                {
                    if (!card.IsDisposed)
                    {
                        card.Invoke((MethodInvoker)(() => card.RefreshCount(-1)));
                    }
                }
            }
            contentPanel.ResumeLayout(true);
        }
        private async Task<int> FetchCount(string key, string token)
        {
            switch (key)
            {
                case "users": return await new Profile(token).GetTotalCountAsync();
                case "coupons": return await new Coupon(token).GetTotalCountAsync();
                case "orders": return await new Order(token).GetTotalCountAsync();
                case "products": return await new Product(token).GetTotalCountAsync();
                case "brands": return await new Brand(token).GetTotalCountAsync();
                case "categories": return await new Category(token).GetTotalCountAsync();
                default: return 0;
            };
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Admin Dashboard";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            DoubleBuffered = true;

            ControlHandler.SetDoubleBuffered(contentPanel);
            ControlHandler.SetResizeRedraw(contentPanel);
            ControlHandler.SetDoubleBuffered(adminPanel);
            ControlHandler.SetResizeRedraw(adminInfoPanel);

            foreach (var c in contentPanel.Controls)
                if (c is MetricCard card)
                    ControlHandler.SetDoubleBuffered(card);
        }
    }
}
