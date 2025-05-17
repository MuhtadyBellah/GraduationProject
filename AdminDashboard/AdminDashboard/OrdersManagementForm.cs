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
    public partial class OrdersManagementForm : Form
    {
        private DataGridView ordersGridView;
        private readonly string _token;

        public OrdersManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadOrders();
        }
        
        private void InitializeForm()
        {
            // Form settings
            this.Text = "Manage Orders";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // DataGridView setup
            ordersGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            // Add columns
            ordersGridView.Columns.Add("IdColumn", "ID");
            ordersGridView.Columns.Add("UserColumn", "USER");
            ordersGridView.Columns.Add("TotalColumn", "TOTAL");
            ordersGridView.Columns.Add("StatusColumn", "STATUS");
            ordersGridView.Columns.Add("PaymentColumn", "PAYMENT");
            ordersGridView.Columns.Add("CreatedAtColumn", "CREATED AT");

            // Add action buttons
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Name = "Delete",
                FlatStyle = FlatStyle.Flat,
            };

            ordersGridView.Columns.Add(deleteButtonColumn);

            // Format columns
            ordersGridView.Columns["TotalColumn"].DefaultCellStyle.Format = "C2";
            ordersGridView.Columns["TotalColumn"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            ordersGridView.Columns["CreatedAtColumn"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            // Style status column
            ordersGridView.CellFormatting += OrdersGridView_CellFormatting;

            // Handle button clicks
            ordersGridView.CellClick += OrdersGridView_CellClick;

            // Add to form
            this.Controls.Add(ordersGridView);
        }

        private async void LoadOrders()
        {
            var orderService = new Order(_token);
            var orders = await orderService.GetAllAsync();

            ordersGridView.Rows.Clear();

            foreach (var order in orders.orders)
            {
                ordersGridView.Rows.Add(
                    order.Id,
                    order.name,
                    order.total,
                    order.status,
                    order.payment_method ?? "N/A",
                    order.created_at
                );
            }
        }

        private void OrdersGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (ordersGridView.Columns[e.ColumnIndex].Name == "Delete")
            {
                var cell = ordersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                if (cell != null)
                {
                    // Set flat button look with color  
                    cell.Style.BackColor = Color.FromArgb(220, 53, 69); // Red for Delete  
                    cell.Style.ForeColor = Color.White;
                    cell.Style.SelectionBackColor = cell.Style.BackColor;
                    cell.Style.SelectionForeColor = Color.White;
                }
            }
            if (ordersGridView.Columns[e.ColumnIndex].Name == "StatusColumn")
            {
                var cell = ordersGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value.ToString().ToLower() == "pending")
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.Font = new Font(ordersGridView.Font, FontStyle.Bold);
                }
                else if (cell.Value.ToString().ToLower() == "complete")
                {
                    cell.Style.ForeColor = Color.Green;
                }
            }
        }

        private async void OrdersGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (ordersGridView.Columns[e.ColumnIndex].Name == "Delete")
            {
                var couponId = (int)ordersGridView.Rows[e.RowIndex].Cells["IdColumn"].Value;
                if (MessageBox.Show("Delete this order?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var success = await new Order(_token).DeleteAsync(couponId);
                    if (success) LoadOrders();
                }
            }
        }
        
        private void OrdersManagementForm_Load(object sender, EventArgs e)
        {

        }
    }
}
