using AdminDashboard.Handler;
using AdminDashboard.Request;
using AdminDashboard.Request.AdminDashboard.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public partial class CouponsManagementForm : Form
    {

        private DataGridView couponsGridView;
        private readonly string _token;
        private Button addCouponButton;

        public CouponsManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadCoupons();
        }

        private void InitializeForm()
        {
            // Form settings
            this.Text = "Manage Coupons";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main table layout
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                RowStyles =
           {
               new RowStyle(SizeType.Absolute, 50), // Header row
               new RowStyle(SizeType.Percent, 100)  // DataGridView row
           }
            };
            this.Controls.Add(mainPanel);

            // Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            mainPanel.Controls.Add(headerPanel, 0, 0);

            // Add Coupon button
            addCouponButton = new Button
            {
                Text = "+ Add Coupon",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35),
                Location = new Point(20, 7)
            };
            addCouponButton.Click += (s, e) => ShowCouponInputPanel();
            headerPanel.Controls.Add(addCouponButton);

            // DataGridView setup
            couponsGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            mainPanel.Controls.Add(couponsGridView, 0, 1);

            // Add columns
            couponsGridView.Columns.Add("IdColumn", "ID");
            couponsGridView.Columns.Add("NameColumn", "NAME");
            couponsGridView.Columns.Add("CodeColumn", "CODE");

            var discountColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "DISCOUNT",
                Name = "DiscountColumn"
            };
            couponsGridView.Columns.Add(discountColumn);

            couponsGridView.Columns.Add("DurationColumn", "DURATION");
            couponsGridView.Columns.Add("StatusColumn", "USED");

            // Add action buttons
            var editButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Name = "Edit",
                FlatStyle = FlatStyle.Flat
            };

            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Name = "Delete",
                FlatStyle = FlatStyle.Flat
            };

            couponsGridView.Columns.Add(editButtonColumn);
            couponsGridView.Columns.Add(deleteButtonColumn);

            // Format columns
            couponsGridView.Columns["IdColumn"].Width = 50;
            couponsGridView.Columns["CodeColumn"].Width = 100;
            couponsGridView.Columns["DiscountColumn"].DefaultCellStyle.Format = "0.00%";
            couponsGridView.Columns["Edit"].Width = 60;
            couponsGridView.Columns["Delete"].Width = 70;

            // Style status column
            couponsGridView.CellFormatting += CouponsGridView_CellFormatting;

            // Handle button clicks
            couponsGridView.CellClick += CouponsGridView_CellClick;
        }

        private async void LoadCoupons()
        {
            var couponService = new Coupon(_token);
            var coupons = await couponService.GetAllAsync();

            couponsGridView.Rows.Clear();

            foreach (var coupon in coupons.coupons)
            {
                couponsGridView.Rows.Add(
                    coupon.Id,
                    coupon.Name,
                    coupon.coupon_id,
                    decimal.Parse(coupon.Discount) / 100m, // Convert to decimal (10.00 -> 0.10)
                    coupon.Duration,
                    coupon.is_used ? "Used" : "Available"
                );
            }
        }

        private void CouponsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (couponsGridView.Columns[e.ColumnIndex].Name == "Edit" ||
                    couponsGridView.Columns[e.ColumnIndex].Name == "Delete")
            {
                var cell = couponsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                if (cell != null)
                {
                    // Set flat button look with color
                    cell.Style.BackColor = couponsGridView.Columns[e.ColumnIndex].Name == "Edit"
                        ? Color.FromArgb(0, 123, 255) // Blue for Edit
                        : Color.FromArgb(220, 53, 69); // Red for Delete

                    cell.Style.ForeColor = Color.White;
                    cell.Style.SelectionBackColor = cell.Style.BackColor;
                    cell.Style.SelectionForeColor = Color.White;
                }
            }
            if (couponsGridView.Columns[e.ColumnIndex].Name == "StatusColumn" && e.Value != null)
            {
                var status = e.Value.ToString();
                if (status == "Used")
                {
                    e.CellStyle.ForeColor = Color.Gray;
                }
                else if (status == "Available")
                {
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(couponsGridView.Font, FontStyle.Bold);
                }
            }
        }

        private async void CouponsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var couponId = (int)couponsGridView.Rows[e.RowIndex].Cells["IdColumn"].Value;

            if (couponsGridView.Columns[e.ColumnIndex].Name == "Edit")
            {
                var couponData = new CouponData
                {
                    Id = couponId,
                    Name = couponsGridView.Rows[e.RowIndex].Cells["NameColumn"].Value.ToString(),
                    coupon_id = couponsGridView.Rows[e.RowIndex].Cells["CodeColumn"].Value.ToString(),
                    Discount = couponsGridView.Rows[e.RowIndex].Cells["DiscountColumn"].Value.ToString(),
                    Duration = couponsGridView.Rows[e.RowIndex].Cells["DurationColumn"].Value.ToString()
                };
                ShowCouponInputPanel(true, couponData); // Show edit form
                
            }
            else if (couponsGridView.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this coupon?", "Confirm",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var success = await new Coupon(_token).DeleteAsync(couponId);
                    if (success) LoadCoupons();
                }
            }
        }

        private Panel overlayPanel;
        private TextBox txtCouponName, txtDuration, txtPercentOff, txtCouponNumber;
        private Button btnSubmit, btnCancel;

        private void ShowCouponInputPanel(bool isEdit = false, CouponData data = null)
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
                Text = isEdit ? "Edit Coupon" : "Add Coupon",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCouponName = new TextBox { Location = new Point(20, 60), Width = 350 };
            txtCouponName.AddPlaceholder("Coupon Name");

            txtDuration = new TextBox { Location = new Point(20, 100), Width = 350 };
            txtDuration.AddPlaceholder("Duration");

            txtPercentOff = new TextBox { Location = new Point(20, 140), Width = 350 };
            txtPercentOff.AddPlaceholder("Percent Off");

            txtCouponNumber = new TextBox { Location = new Point(20, 180), Width = 350 };
            txtCouponNumber.AddPlaceholder("Coupon Number");

            if (isEdit && data != null)
            {
                txtCouponName.Text = data.Name;
                txtDuration.Text = data.Duration;
                txtPercentOff.Text = data.Discount;
                txtCouponNumber.Text = data.coupon_id;
            }

            btnSubmit = new Button
            {
                Text = isEdit ? "Update" : "Add",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 230),
                Width = 100
            };
            btnSubmit.Click += async (s, e) => await SubmitCouponForm(isEdit, data?.Id ?? 0);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(140, 230),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.Controls.Remove(overlayPanel);

            overlayPanel.Controls.AddRange(new Control[] {
                lblTitle, txtCouponName, txtDuration, txtPercentOff, txtCouponNumber,
                btnSubmit, btnCancel
            });

            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }

        private async Task SubmitCouponForm(bool isEdit, int couponId)
        {
            var coupon = new CouponRequest
            {
                name = txtCouponName.Text,
                duration = txtDuration.Text,
                percent_off = txtPercentOff.Text,
                coupon_number = txtCouponNumber.Text
            };

            var service = new Coupon(_token); // Ensure this class exists and matches your API

            bool success;

            if (isEdit)
                success = await service.UpdateAsync(couponId, coupon);
            else
                success = await service.CreateAsync(coupon);

            if (success)
            {
                this.Controls.Remove(overlayPanel);
                LoadCoupons(); // Make sure this method exists and reloads the grid/list
            }
            else
            {
                MessageBox.Show("Failed to save coupon.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CouponsManagementForm_Load(object sender, EventArgs e)
        {

        }
    }
}
