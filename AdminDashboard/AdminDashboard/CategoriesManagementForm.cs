using AdminDashboard.Handler;
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
    public partial class CategoriesManagementForm : Form
    {
        private DataGridView categoriesGridView;
        private readonly string _token;
        private Button addcategoryButton;
        private DataGridViewButtonColumn editButtonColumn;
        private DataGridViewButtonColumn deleteButtonColumn;

        public CategoriesManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadCategories();
        }

        private void InitializeForm()
        {
            // Form settings
            this.Text = "Manage Categories";
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
            addcategoryButton = new Button
            {
                Text = "+ Add Category",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(20, 10)
            };
            addcategoryButton.Click += (s, e) => ShowCategoryInputPanel();
            headerPanel.Controls.Add(addcategoryButton);

            // DataGridView setup
            categoriesGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };
            mainPanel.Controls.Add(categoriesGridView, 0, 1);

            // Add columns
            categoriesGridView.Columns.Add("IdColumn", "Id");
            categoriesGridView.Columns.Add("NameColumn", "NAME");
            categoriesGridView.Columns.Add("DescriptionColumn", "DESCRIPTION");
            
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

            categoriesGridView.Columns.Add(editButtonColumn);
            categoriesGridView.Columns.Add(deleteButtonColumn);

            // Format columns
            categoriesGridView.Columns["IdColumn"].Width = 80;
            categoriesGridView.Columns["NameColumn"].Width = 100;
            categoriesGridView.Columns["DescriptionColumn"].Width = 500;

            // Handle events
            categoriesGridView.CellClick += async (s, e) =>
            {
                if (e.RowIndex < 0) return;

                var categoryId = (int)categoriesGridView.Rows[e.RowIndex].Cells["IdColumn"].Value;
                if (categoriesGridView.Columns[e.ColumnIndex].Name == "Edit")
                {
                    var exist = new CategoriesResponse
                    {
                        Id = categoryId,
                        Name = categoriesGridView.Rows[e.RowIndex].Cells["NameColumn"].Value.ToString(),
                        Description = categoriesGridView.Rows[e.RowIndex].Cells["DescriptionColumn"].Value.ToString()
                    };
                    ShowCategoryInputPanel(true, exist);
                }
                else if (categoriesGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    if (MessageBox.Show("Delete this category?", "Confirm",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (await new Category(_token).DeleteAsync(categoryId))
                            LoadCategories(); // Refresh

                        else
                            MessageBox.Show("Failed to delete category.");
                    }
                }
            };

            categoriesGridView.CellFormatting += (s, e) =>
            {
                if (categoriesGridView.Columns[e.ColumnIndex].Name == "Edit" ||
                    categoriesGridView.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var cell = categoriesGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                    if (cell != null)
                    {
                        // Set flat button look with color
                        cell.Style.BackColor = categoriesGridView.Columns[e.ColumnIndex].Name == "Edit"
                            ? Color.FromArgb(0, 123, 255) // Blue for Edit
                            : Color.FromArgb(220, 53, 69); // Red for Delete

                        cell.Style.ForeColor = Color.White;
                        cell.Style.SelectionBackColor = cell.Style.BackColor;
                        cell.Style.SelectionForeColor = Color.White;
                    }
                }
            };
        }

        private async void LoadCategories()
        {
            var categoryService = new Category(_token);
            var categories = await categoryService.GetAllAsync();

            categoriesGridView.Rows.Clear();
            foreach (var category in categories)
            {
                var rowIndex = categoriesGridView.Rows.Add(
                    category.Id,
                    category.Name,
                    category.Description
                );
            }
        }

        private Panel overlayPanel;
        private TextBox txtCategoryName, txtCategoryDescription;
        private Button btnSubmit, btnCancel;

        private void ShowCategoryInputPanel(bool isEdit = false, CategoriesResponse data = null)
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
                Text = isEdit ? "Edit Category" : "Add Category",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtCategoryName = new TextBox { Location = new Point(20, 60), Width = 350 };
            txtCategoryName.AddPlaceholder("Category Name");

            txtCategoryDescription = new TextBox { Location = new Point(20, 100), Width = 350 };
            txtCategoryDescription.AddPlaceholder("Category Description");

            if (isEdit && data != null)
            {
                txtCategoryName.Text = data.Name;
                txtCategoryDescription.Text = data.Description;
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
            btnSubmit.Click += async (s, e) => await SubmitCategoryForm(isEdit, data?.Id ?? 0);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(140, 200),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.Controls.Remove(overlayPanel);

            overlayPanel.Controls.AddRange(new Control[] {
                lblTitle, txtCategoryName, txtCategoryDescription,
                btnSubmit, btnCancel
            });

            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }
        private async Task SubmitCategoryForm(bool isEdit, int categoryId)
        {
            var category = new CategoriesResponse
            {
                Name = txtCategoryName.Text,
                Description = txtCategoryDescription.Text
            };

            var service = new Category(_token);
            bool success;

            if (isEdit)
                success = await service.UpdateAsync(categoryId, category);
            else
                success = await service.CreateAsync(category);

            if (success)
            {
                this.Controls.Remove(overlayPanel);
                LoadCategories();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private void CategoriesManagementForm_Load(object sender, EventArgs e)
        {

        }
    }
}
