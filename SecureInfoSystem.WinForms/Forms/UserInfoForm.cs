using System;
using System.Windows.Forms;
using SecureInfoSystem.BLL.Services;
using SecureInfoSystem.Entities;

namespace SecureInfoSystem.WinForms.Forms
{
    public partial class UserInfoForm : Form
    {
        private readonly UserService _userService;
        private readonly int _userId;
        private User _currentUser;

        public UserInfoForm(int userId, string connectionString)
        {
            InitializeComponent();
            _userId = userId;
            _userService = new UserService(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            try
            {
                var user = _userService.GetDecryptedUserInfo(_userId);
                if (user == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin người dùng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                _currentUser = user;
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.PhoneEncrypted;
                txtAddress.Text = user.AddressEncrypted;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải thông tin người dùng. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Không có dữ liệu người dùng để cập nhật", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentUser.FullName = txtFullName.Text.Trim();
            _currentUser.Email = txtEmail.Text.Trim();
            _currentUser.PhoneEncrypted = txtPhone.Text.Trim();
            _currentUser.AddressEncrypted = txtAddress.Text.Trim();

            try
            {
                bool updated = _userService.UpdateUserInfo(_currentUser);
                if (updated)
                {
                    MessageBox.Show("Cập nhật thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
