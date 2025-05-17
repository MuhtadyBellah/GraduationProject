using System.Drawing;
using System.Windows.Forms;

namespace AdminDashboard.Handler
{
    public static class txtbxExtensions
    {
        public static TextBox AddPlaceholder(this TextBox textBox, string placeholder)
        {
            var pass = textBox.UseSystemPasswordChar;
            textBox.UseSystemPasswordChar = false; 
            textBox.ForeColor = Color.Gray;
            textBox.Text = placeholder;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;

                    if (pass)
                    {
                        textBox.PasswordChar = '*';
                    }
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;

                    if (pass)
                    {
                        textBox.PasswordChar = '\0';
                    }
                }
            };
            return textBox;
        }

    }
}
