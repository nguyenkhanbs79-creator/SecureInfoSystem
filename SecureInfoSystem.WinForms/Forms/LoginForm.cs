using System;
using System.Windows.Forms;
using SecureInfoSystem.BLL.Services;
using System.Windows.Forms;

namespace SecureInfoSystem.WinForms.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;

        public LoginForm()
        {
            InitializeComponent();
            string connectionString = "Data Source=.;Initial Catalog=SecureDb;Integrated Security=True;";
            _authService = new AuthService(connectionString);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Password", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = _authService.Login(username, password);
            if (success)
            {
                MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }
    }
}
