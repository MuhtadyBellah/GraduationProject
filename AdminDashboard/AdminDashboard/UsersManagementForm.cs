using AdminDashboard.Handler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace AdminDashboard
{
    public partial class UsersManagementForm : Form
    {
        private DataGridView usersGridView;
        private readonly string _token;

        public UsersManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadUsers();
        }

        
        private void InitializeForm()
        {
            // Form settings
            this.Text = "Manage Users";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView setup
            usersGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                Height = 50
            };

            usersGridView.RowTemplate.Height = 100;
            // Add columns
            usersGridView.Columns.Add("IdColumn", "Id");

            var imgColumn = new DataGridViewImageColumn
            {
                Name = "ImageColumn",
                HeaderText = "Profile Picture",
                ImageLayout = DataGridViewImageCellLayout.Zoom
            };
            usersGridView.Columns.Add(imgColumn);
            usersGridView.Columns.Add("NameColumn", "Name");
            usersGridView.Columns.Add("EmailColumn", "Email");
            usersGridView.Columns.Add("PhoneColumn", "Phone");
            usersGridView.Columns.Add("GenderColumn", "Gender");
            usersGridView.Columns.Add("VerifiedColumn", "Verified");
            usersGridView.Columns.Add("RoleColumn", "Role");
         
            // Add buttons to Edit and Delete columns
            DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Name = "Edit",
                FlatStyle = FlatStyle.Flat
            };

            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Name = "Delete",
                FlatStyle = FlatStyle.Flat
            };

            usersGridView.Columns.Add(editButtonColumn);
            usersGridView.Columns.Add(deleteButtonColumn);

            usersGridView.CellFormatting += (s, e) =>
            {
                if (usersGridView.Columns[e.ColumnIndex].Name == "Edit" ||
                    usersGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var cell = usersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                    if (cell != null)
                    {
                        // Set flat button look with color  
                        cell.Style.BackColor = usersGridView.Columns[e.ColumnIndex].Name == "Edit"
                            ? Color.FromArgb(0, 123, 255) // Blue for Edit  
                            : Color.FromArgb(220, 53, 69); // Red for Delete  

                        cell.Style.ForeColor = Color.White;
                        cell.Style.SelectionBackColor = cell.Style.BackColor;
                        cell.Style.SelectionForeColor = Color.White;
                    }
                }
                if(usersGridView.Columns[e.ColumnIndex].Name == "VerifiedColumn")
                {
                    if (e.Value != null && e.Value.ToString() == "No verified yet")
                    {
                        e.CellStyle.BackColor = Color.FromArgb(220, 53, 69); // Red for not verified
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.FromArgb(40, 167, 69); // Green for verified
                    }
                    e.CellStyle.ForeColor = Color.White;
                }
            };
            usersGridView.CellClick += async (s, e) =>
            {
                if (e.RowIndex < 0) return;

                var userId = (int)usersGridView.Rows[e.RowIndex].Cells["IdColumn"].Value;
                if (usersGridView.Columns[e.ColumnIndex].Name == "Edit")
                {
                    var role = usersGridView.Rows[e.RowIndex].Cells["RoleColumn"].Value.ToString();
                    ShowUserInputPanel(true, userId, role);
                }
                else if (usersGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    if (MessageBox.Show("Delete this User?", "Confirm",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (await new Profile(_token).DeleteAsync(userId))
                            LoadUsers(); // Refresh  

                        else
                            MessageBox.Show("Failed to delete user, Is Verified.");
                    }
                }
            };
            // Add to form
            this.Controls.Add(usersGridView);
        }


        private Panel overlayPanel;
        private TextBox txtRole;
        private Button btnSubmit, btnCancel;

        private void ShowUserInputPanel(bool isEdit, int userId, string role = null)
        {
            overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle,
                Parent = this
            };

            var lblTitle = new Label
            {
                Text = isEdit ? "Edit Brand" : "Add Brand",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtRole = new TextBox { Location = new Point(20, 60), Width = 350 };
            txtRole.AddPlaceholder("Role");

            if (isEdit && !string.IsNullOrEmpty(role))
            {
                txtRole.Text = role;
            }

            btnSubmit = new Button
            {
                Text = isEdit ? "Update" : "Add",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 200),
                Width = 100
            };
            btnSubmit.Click += async (s, e) => await SubmitBrandForm(isEdit, userId);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(140, 200),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.Controls.Remove(overlayPanel);

            overlayPanel.Controls.AddRange(new Control[] {
                lblTitle, txtRole,
                btnSubmit, btnCancel
            });

            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }
        private async Task SubmitBrandForm(bool isEdit, int userId)
        {
            var service = new Profile(_token);

            bool success = false;

            if (isEdit)
                success = await service.UpdateAsync(userId, txtRole.Text);
            
            if (success)
            {
                this.Controls.Remove(overlayPanel);
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private async void LoadUsers()
        {
            var userService = new Profile(_token);
            var users = await userService.GetAllUsersAsync();

            usersGridView.Rows.Clear();

            foreach (var user in users.users)
            {
                user.image = $"https://concise-ant-sound.ngrok-free.app/{user.image}";
                Image userImage = null;
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(user.image);
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            userImage = Image.FromStream(stream);
                        }
                    }
                }
                catch
                {
                    userImage = null; // fallback if image can't be loaded
                }

                usersGridView.Rows.Add(
                    user.Id,
                    userImage,
                    user.Name ?? "",
                    user.Email ?? "",
                    user.Phone ?? "No phone provided",
                    user.Gender ?? "No gender specified",
                    user.email_verified_at ?? "No verified yet",
                    user.Role ?? "User"
                );
            }
        }

        private void UsersManagementForm_Load(object sender, EventArgs e)
        {
        }
    }
}
