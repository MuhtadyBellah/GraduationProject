using AdminDashboard.Handler;
using AdminDashboard.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public partial class ProductsManagementForm : Form
    {
        private FlowLayoutPanel productsPanel;
        private readonly string _token;
        private Button addProductButton;

        public ProductsManagementForm(string token)
        {
            InitializeComponent();
            _token = token;
            InitializeForm();
            LoadProducts();
        }
        
        private void InitializeForm()
        {
            // Form settings
            this.Text = "Manage Products";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main panel with scroll
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            this.Controls.Add(mainPanel);

            // Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };
            mainPanel.Controls.Add(headerPanel);

            // Add Product button
            addProductButton = new Button
            {
                Text = "+ Add Product",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 123, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 40),
                Location = new Point(20, 10)
            };
            addProductButton.Click += (s, e) => ShowProductInputPanel();
            headerPanel.Controls.Add(addProductButton);

            // Products container
            productsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.White,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown
            };
            mainPanel.Controls.Add(productsPanel);
        }

        private async void LoadProducts()
        {
            var productService = new Product(_token);
            var products = await productService.GetAllPagedAsync();

            productsPanel.Controls.Clear();

            foreach (var product in products.data)
            {
                var productCard = CreateProductCard(product);
                productsPanel.Controls.Add(productCard);
            }
        }

        private Panel CreateProductCard(ProductResponse product)
        {
            var card = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Size = new Size(900, 180),
                Margin = new Padding(0, 0, 0, 20)
            };

            // Product image (left side)
            var pictureBox = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(20, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.WhiteSmoke
            };

            if (!string.IsNullOrEmpty(product.PictureUrl))
                pictureBox.Load(product.PictureUrl);

            card.Controls.Add(pictureBox);

            // Product details (right side)
            var detailsPanel = new Panel
            {
                Location = new Point(190, 15),
                Size = new Size(550, 150)
            };
            card.Controls.Add(detailsPanel);

            // Product name
            var nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(0, 0),
                AutoSize = true
            };
            detailsPanel.Controls.Add(nameLabel);

            // Product description
            var descLabel = new Label
            {
                Text = product.Description,
                Font = new Font("Segoe UI", 9),
                Location = new Point(0, 30),
                MaximumSize = new Size(550, 40),
                AutoSize = true
            };
            detailsPanel.Controls.Add(descLabel);

            // Price and quantity
            var priceLabel = new Label
            {
                Text = $"{product.Price:C2}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Location = new Point(0, 80),
                AutoSize = true
            };
            detailsPanel.Controls.Add(priceLabel);

            var quantityLabel = new Label
            {
                Text = $"Quantity: {product.Quantity}",
                Font = new Font("Segoe UI", 10),
                Location = new Point(120, 82),
                AutoSize = true
            };
            detailsPanel.Controls.Add(quantityLabel);

            // Action buttons
            var editButton = new Button
            {
                Text = "Edit",
                Tag = product.Id,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Size = new Size(80, 30),
                Location = new Point(750, 20)
            };
            editButton.Click += (s, e) => ShowProductInputPanel(true, product);
            card.Controls.Add(editButton);

            var deleteButton = new Button
            {
                Text = "Delete",
                Tag = product.Id,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Size = new Size(80, 30),
                Location = new Point(750, 60)
            };
            deleteButton.Click += (s, e) => DeleteProduct((int)deleteButton.Tag);
            card.Controls.Add(deleteButton);

            return card;
        }

        private async void DeleteProduct(int productId)
        {
            if (MessageBox.Show("Delete this product?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(await new Product(_token).DeleteAsync(productId))
                    LoadProducts();
            }
        }

        private Panel overlayPanel;
        private TextBox txtName, txtDescription, txtPrice, txtQuantity, txtBrandId, txtTypeId;
        private Button btnSubmit, btnCancel;
        private string picturePath1 = null;
        private string picturePath2 = null;
        private Label lblPic1, lblPic2;

        private void ShowProductInputPanel(bool isEdit = false, ProductResponse data = null)
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
                Text = isEdit ? "Edit Product" : "Add Product",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            txtName = new TextBox { Location = new Point(20, 60), Width = 350 };
            txtName.AddPlaceholder("Name");

            txtDescription = new TextBox { Location = new Point(20, 100), Width = 350 };
            txtDescription.AddPlaceholder("Description");
            
            txtPrice = new TextBox { Location = new Point(20, 140), Width = 350 };
            txtPrice.AddPlaceholder("Price");

            txtQuantity = new TextBox { Location = new Point(20, 180), Width = 350 };
            txtQuantity.AddPlaceholder("Quantity");

            txtBrandId = new TextBox { Location = new Point(20, 220), Width = 400 };
            txtBrandId.AddPlaceholder("Brand ID");
            
            txtTypeId = new TextBox { Location = new Point(20, 260), Width = 400 };
            txtTypeId.AddPlaceholder("Category ID");

            var btnPic1 = new Button { Text = "Upload Image 1", Location = new Point(20, 300), Width = 150 };
            lblPic1 = new Label { Text = "No file selected", AutoSize = true, Location = new Point(180, 305), Width = 220 };

            var btnPic2 = new Button { Text = "Upload Image 2", Location = new Point(20, 330), Width = 150 };
            lblPic2 = new Label { Text = "No file selected", AutoSize = true, Location = new Point(180, 335), Width = 220 };

            btnPic1.Click += (s, e) =>
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    picturePath1 = dialog.FileName;
                    lblPic1.Text = Path.GetFileName(picturePath1);
                }
            };

            btnPic2.Click += (s, e) =>
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    picturePath2 = dialog.FileName;
                    lblPic2.Text = Path.GetFileName(picturePath2);
                }
            };

            if (isEdit && data != null)
            {
                txtName.Text = data.Name;
                txtDescription.Text = data.Description;
                txtPrice.Text = data.Price.ToString();
                txtQuantity.Text = data.Quantity.ToString();
                txtBrandId.Text = data.ProductBrandId.ToString();
                txtTypeId.Text = data.ProductTypeId.ToString();
            }

            btnSubmit = new Button
            {
                Text = isEdit ? "Update" : "Add",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 500),
                Width = 100
            };
            btnSubmit.Click += async (s, e) => await SubmitProductForm(isEdit, data?.Id ?? 0);

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(140, 500),
                Width = 100
            };
            btnCancel.Click += (s, e) => this.Controls.Remove(overlayPanel);

            overlayPanel.Controls.AddRange(new Control[] {
                lblTitle, txtName, txtDescription, txtPrice, txtQuantity,
                txtBrandId, txtTypeId,
                btnPic1, lblPic1, btnPic2, lblPic2,
                btnSubmit, btnCancel
            });

            this.Controls.Add(overlayPanel);
            overlayPanel.BringToFront();
        }

        private IFormFile ConvertToIFormFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FormFile(stream, 0, stream.Length, null, Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" // or detect dynamically
            };
        }

        private async Task SubmitProductForm(bool isEdit, int productId)
        {
            var product = new ProductRequest
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                Price = decimal.TryParse(txtPrice.Text, out var price) ? price : 0,
                quantity = int.TryParse(txtQuantity.Text, out var quantity) ? quantity : 0,
                productBrandId = int.TryParse(txtBrandId.Text, out var brandId) ? brandId : 1,
                productTypeId = int.TryParse(txtTypeId.Text, out var typeId) ? typeId : 1,
                PictureFile = ConvertToIFormFile(picturePath1),
                PictureFileGlB = ConvertToIFormFile(picturePath2)
            };

            var service = new Product(_token);
            bool success;

            if (isEdit)
                success = await service.UpdateAsync(productId, product);
            else
                success = await service.CreateAsync(product);

            if (success)
            {
                this.Controls.Remove(overlayPanel);
                LoadProducts();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private void ProductsManagementForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
