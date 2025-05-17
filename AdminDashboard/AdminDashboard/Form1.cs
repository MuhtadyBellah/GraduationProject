using AdminDashboard.Handler;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private Panel card;
        private Label titleLabel;
        private Button valueLabel;
        private Label subtitleLabel;

        private string Token;
        public adminFrm()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            // Form settings
            this.Text = "Admin Dashboard";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

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
            loginButton.Click += async (sender, e) =>
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
                    var login = new Login(email, password);
                    var userInfo = await login.AuthenticateUserAsync();
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

                    /*
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
                    await CreateMetricCard(contentPanel, "Manage Users", "users", "Users Counts", 30, 80, Color.Gray);
                    await CreateMetricCard(contentPanel, "Manage Coupons", "coupons", "Revenue", 280, 80, Color.Red);
                    await CreateMetricCard(contentPanel, "Manage Orders", "orders", "Orders Counts", 530, 80, Color.Green);
                    await CreateMetricCard(contentPanel, "Manage Products", "products", "Products Counts", 30, 280, Color.Blue);
                    await CreateMetricCard(contentPanel, "Manage Brands", "brands", "Brands Counts", 280, 280, Color.BlueViolet);
                    await CreateMetricCard(contentPanel, "Manage Categories", "categories", "Categories Counts", 530, 280, Color.DarkBlue);
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

            // Admin name label
            adminName = new Label
            {
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Padding = new Padding(0, 10, 10, 0)
            };
            adminInfoPanel.Controls.Add(adminName);

            // Profile icon
            profileIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 14),
                Size = new Size(40, 40),
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            adminInfoPanel.Controls.Add(profileIcon);
        }

        private async Task CreateMetricCard(Panel parent, string title, string value, string subtitle, int x, int y, Color color, int width = 220)
        {
            card = new Panel();
            card.BackColor = color;
            card.Size = new Size(width, 150);
            card.Location = new Point(x, y);
            card.Padding = new Padding(15);
            parent.Controls.Add(card);

            titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            titleLabel.Location = new Point(15, 15);
            titleLabel.AutoSize = true;
            card.Controls.Add(titleLabel);

            int? cnt = null;
            if (value == "products")
                cnt = await new Product(Token).GetTotalCountAsync();

            else if (value == "brands")
                cnt = await new Brand(Token).GetTotalCountAsync();

            else if (value == "categories")
                cnt = await new Category(Token).GetTotalCountAsync();
                
            else if (value == "users")
                cnt = await new Profile(Token).GetTotalCountAsync();

            else if (value == "coupons")
                cnt = await new Coupon(Token).GetTotalCountAsync();

            else if (value == "orders")
                cnt = await new Order(Token).GetTotalCountAsync();

            valueLabel = new Button();
            valueLabel.Text = cnt?.ToString() ?? "";
            valueLabel.ForeColor = Color.White;
            valueLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            valueLabel.Location = new Point(15, 45);
            valueLabel.AutoSize = true;
            valueLabel.Click += (sender, e) =>
            {
                var clickedLabel = (Button)sender;
                string currentValue = value;

                clickedLabel.Enabled = false;
                clickedLabel.Cursor = Cursors.WaitCursor;

                Form res;
                if (currentValue == "users")
                    res = new UsersManagementForm(Token);

                else if (currentValue == "coupons")
                    res = new CouponsManagementForm(Token);

                else if (currentValue == "orders")
                    res = new OrdersManagementForm(Token);

                else if (currentValue == "products")
                    res = new ProductsManagementForm(Token);

                else if (currentValue == "brands")
                    res = new BrandsManagementForm(Token);

                else if (currentValue == "categories")
                    res = new CategoriesManagementForm(Token);

                else
                    throw new NotImplementedException();

                res.StartPosition = FormStartPosition.CenterParent;
                res.FormClosed += async (s, args) =>
                {
                    int? newCnt = null;
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

                    clickedLabel.Text = newCnt?.ToString() ?? "";

                    clickedLabel.Enabled = true;
                    clickedLabel.Cursor = Cursors.Hand;
                };
                res.Show(this);
            };
            card.Controls.Add(valueLabel);

            if (!string.IsNullOrEmpty(subtitle))
            {
                subtitleLabel = new Label();
                subtitleLabel.Text = subtitle;
                subtitleLabel.ForeColor = Color.White;
                subtitleLabel.Font = new Font("Segoe UI", 10);
                subtitleLabel.Location = new Point(15, 95);
                subtitleLabel.AutoSize = true;
                card.Controls.Add(subtitleLabel);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
