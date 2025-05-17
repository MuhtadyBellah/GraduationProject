using AdminDashboard.Handler;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public partial class BrandsManagementForm : Form
    {
        private DataGridView brandsGridView;
        private readonly string _token;
        private Button addBrandButton;
        private DataGridViewButtonColumn editButtonColumn;
        private DataGridViewButtonColumn deleteButtonColumn;

        public BrandsManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadBrands(); // Changed to call the async method without await  
        }

        private void InitializeForm()
        {
            // Form settings  
            this.Text = "Manage Brands";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main panel with table layout  
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                RowStyles =
                   {
                       new RowStyle(SizeType.Absolute, 60), // Header  
                       new RowStyle(SizeType.Percent, 100)  // Data  
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

            // Add Brand button  
            addBrandButton = new Button
            {
                Text = "+ Add Brand",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(20, 10)
            };
            addBrandButton.Click += (s, e) => ShowBrandInputPanel();
            headerPanel.Controls.Add(addBrandButton);

            // DataGridView setup  
            brandsGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            mainPanel.Controls.Add(brandsGridView, 0, 1);

            // Add columns  
            brandsGridView.Columns.Add("IdColumn", "Id");
            brandsGridView.Columns.Add("NameColumn", "NAME");
            brandsGridView.Columns.Add("DescriptionColumn", "DESCRIPTION");

            // Action buttons  
            editButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Edit",
                Name = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };

            deleteButtonColumn = new DataGridViewButtonColumn
            {
                Text = "Delete",
                Name = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat
            };

            brandsGridView.Columns.Add(editButtonColumn);
            brandsGridView.Columns.Add(deleteButtonColumn);

            // Format columns  
            brandsGridView.Columns["IdColumn"].Width = 80;
            brandsGridView.Columns["NameColumn"].Width = 100;
            brandsGridView.Columns["DescriptionColumn"].Width = 500;

            // Handle events  
            brandsGridView.CellClick += async (s, e) =>
            {
                if (e.RowIndex < 0) return;

                var brandId = (int)brandsGridView.Rows[e.RowIndex].Cells["IdColumn"].Value;
                if (brandsGridView.Columns[e.ColumnIndex].Name == "Edit")
                {

                    var exist = new BrandResponse
                    {
                        Id = brandId,
                        Name = brandsGridView.Rows[e.RowIndex].Cells["NameColumn"].Value.ToString(),
                        Description = brandsGridView.Rows[e.RowIndex].Cells["DescriptionColumn"].Value.ToString()
                    };
                    ShowBrandInputPanel(true, exist);
                }
                else if (brandsGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    if (MessageBox.Show("Delete this brand?", "Confirm",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (await new Brand(_token).DeleteAsync(brandId))
                            LoadBrands(); // Refresh  

                        else
                            MessageBox.Show("Failed to delete brand.");
                    }
                }
            };

            brandsGridView.CellFormatting += (s, e) =>
            {
                if (brandsGridView.Columns[e.ColumnIndex].Name == "Edit" ||
                    brandsGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var cell = brandsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                    if (cell != null)
                    {
                        // Set flat button look with color  
                        cell.Style.BackColor = brandsGridView.Columns[e.ColumnIndex].Name == "Edit"
                            ? Color.FromArgb(0, 123, 255) // Blue for Edit  
                            : Color.FromArgb(220, 53, 69); // Red for Delete  

                        cell.Style.ForeColor = Color.White;
                        cell.Style.SelectionBackColor = cell.Style.BackColor;
                        cell.Style.SelectionForeColor = Color.White;
                    }
                }
            };
        }

        private async void LoadBrands()
        {
            var brandService = new Brand(_token);
            var brands = await brandService.GetAllAsync();

            brandsGridView.Rows.Clear();
            foreach (var brand in brands)
            {
                var rowIndex = brandsGridView.Rows.Add(
                    brand.Id,
                    brand.Name,
                    brand.Description
                 );
            }
        }

        private Panel overlayPanel;
        private TextBox txtBrandName, txtBrandDescription;
        private Button btnSubmit, btnCancel;

        private void ShowBrandInputPanel(bool isEdit = false, BrandResponse data = null)
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

            txtBrandName = new TextBox { Location = new Point(20, 60), Width = 350 };
            txtBrandName.AddPlaceholder("Brand Name");

            txtBrandDescription = new TextBox { Location = new Point(20, 100), Width = 350 };
            txtBrandDescription.AddPlaceholder("Brand Description");

            if (isEdit && data != null)
            {
                txtBrandName.Text = data.Name;
                txtBrandDescription.Text = data.Description;
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
            btnSubmit.Click += async (s, e) => await SubmitBrandForm(isEdit, data?.Id ?? 0);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(140, 200),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.Controls.Remove(overlayPanel);

            overlayPanel.Controls.AddRange(new Control[] {
                lblTitle, txtBrandName, txtBrandDescription,
                btnSubmit, btnCancel
            });

            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }
        private async Task SubmitBrandForm(bool isEdit, int brandId)
        {
            var brand = new BrandResponse
            {
                Name = txtBrandName.Text,
                Description = txtBrandDescription.Text
            };

            var service = new Brand(_token);
            bool success;

            if (isEdit)
                success = await service.UpdateAsync(brandId, brand);
            else
                success = await service.CreateAsync(brand);

            if (success)
            {
                this.Controls.Remove(overlayPanel);
                LoadBrands();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }


        private void BrandsManagementForm_Load(object sender, EventArgs e)
        {

        }
    }
}
