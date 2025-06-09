using AdminDashboard.Handler;
using AdminDashboard.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public partial class ProductsManagementForm : Form
    {
        private FlowLayoutPanel productsPanel;
        private Panel headerPanel;
        private readonly string _token;
        private Button addProductButton;
        private int currentPage = 1;
        private int pageSize = 2;
        private int totalItems = 0;
        private Panel paginationPanel;
        private FlowLayoutPanel flowPanel;
        private static readonly MemoryCache _imageCache = new MemoryCache(new MemoryCacheOptions());
        private const int MaxCacheSizeMB = 100;

        public ProductsManagementForm(string token)
        {
            _token = token;
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Main panel with scroll
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            this.Controls.Add(mainPanel);

            // Header panel
            headerPanel = new Panel
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
        private async Task LoadProductsAsync()
        { 
            var loadingLabel = new Label
            {
                Text = "Loading products...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12)
            };
            productsPanel.Controls.Clear();
            productsPanel.Controls.Add(loadingLabel);
            productsPanel.Refresh();

            try
            {
                var pagedResponse = await new Product(_token).GetAllPagedAsync(pageSize, currentPage);
                var cards = pagedResponse.data.Select(p => (Panel)CreateProductCard(p)).ToArray();

                productsPanel.SuspendLayout();
                productsPanel.Controls.Clear();
                productsPanel.Controls.AddRange(cards);
                productsPanel.ResumeLayout();

                totalItems = pagedResponse.count;
                RenderPaginationControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }
        private void RenderPaginationControls()
        {
            // Remove old panel
            if (paginationPanel != null)
                this.Controls.Remove(paginationPanel);

            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // FlowLayoutPanel to hold buttons
            flowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Fill
            };
            paginationPanel.Controls.Add(flowPanel);

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            void AddButton(string text, int? goToPage = null, bool enabled = true, bool isCurrent = false)
            {
                var btn = new Button
                {
                    Text = text,
                    Enabled = enabled,
                    Width = 40,
                    Height = 30,
                    Margin = new Padding(3)
                };

                if (isCurrent)
                {
                    btn.Font = new Font(btn.Font, FontStyle.Bold);
                    btn.BackColor = Color.LightGray;
                }

                btn.Click += async (s, e) =>
                {
                    if (goToPage.HasValue)
                    {
                        currentPage = goToPage.Value;
                        await LoadProductsAsync();
                    }
                };
                flowPanel.Controls.Add(btn);
            }

            // Prev button
            AddButton("<", currentPage - 1, currentPage > 1);
            bool dotPrinted = false;
            for (int i = 1; i <= totalPages; i++)
            {
                if (i == 1 || 
                    i == totalPages || 
                    Math.Abs(i - currentPage) <= 1) 
                    
                    AddButton(i.ToString(), i, i != currentPage, i == currentPage);

                else if(i == (1 + currentPage) / 2 || 
                    i == (totalPages + currentPage) / 2)
                {
                    AddButton(i.ToString(), i);
                    dotPrinted = false;
                }
                else
                {
                    // Print "..." only once between ranges
                    if (!dotPrinted)
                    {
                        AddButton("...", null, false);
                        dotPrinted = true;
                    }
                }
            }
            // Next button
            AddButton(">", currentPage + 1, currentPage < totalPages);
            this.Controls.Add(paginationPanel);
            paginationPanel.BringToFront();
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
            {
                if (_imageCache.TryGetValue(product.PictureUrl, out Image img))
                {
                    pictureBox.Image = img;
                }
                else
                {
                    // Using a simple built-in placeholder if Resources are not set up
                    pictureBox.Image = SystemIcons.Information.ToBitmap();

                    _ = LoadImageAsync(product.PictureUrl, pictureBox);
                }
            }
            else
            {
                // Using a simple built-in placeholder if Resources are not set up
                pictureBox.Image = SystemIcons.Information.ToBitmap();

            }
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
        private async Task LoadImageAsync(string url, PictureBox pictureBox)
        {
            try
            {
                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(url).ConfigureAwait(false))
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var image = Image.FromStream(stream);
                    // Add to cache if under size limit
                    var totalCacheSize = _imageCache.GetCurrentStatistics()?.CurrentEstimatedSize ?? 0;
                    if (totalCacheSize + stream.Length < MaxCacheSizeMB * 1024 * 1024)
                    {
                        _imageCache.Set(url, image, new MemoryCacheEntryOptions
                        {
                            Size = stream.Length,
                            SlidingExpiration = TimeSpan.FromMinutes(30)
                        });
                    }

                    // Update UI if still needed
                    if (!pictureBox.IsDisposed)
                    {
                        pictureBox.Invoke((Action)(() =>
                        {
                            if (!pictureBox.IsDisposed)
                                pictureBox.Image = image;
                        }));
                    }
                }
            }
            catch
            {
                pictureBox.Image = null;
            }
        }
        private async void DeleteProduct(int productId)
        {
            if (MessageBox.Show("Delete this product?", "Confirm",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (await new Product(_token).DeleteAsync(productId))
                    await LoadProductsAsync();
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
                await LoadProductsAsync();
            }
            else
            {
                MessageBox.Show("Failed");
            }
        }

        private async void ProductsManagementForm_Load(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }
    }
}
