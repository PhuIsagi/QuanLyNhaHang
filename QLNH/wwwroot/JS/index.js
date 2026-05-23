document.addEventListener("DOMContentLoaded", function () {
    localStorage.removeItem('userInfo');

    const loginForm = document.getElementById('loginForm');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const user = document.getElementById('username').value;
            const pass = document.getElementById('password').value;
            const btnLogin = document.getElementById('btnLogin');
            const alertBox = document.getElementById('alert-container');

            btnLogin.innerHTML = '<span class="spinner-border spinner-border-sm"></span> ĐANG XỬ LÝ...';
            btnLogin.disabled = true;

            fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: user, password: pass })
            })
                .then(response => response.json())
                .then(data => {
                    console.log("Dữ liệu Server gửi về:", data);
                    if (data.success) {
                        const maNhanVien = data.maNV || data.maNv || data.manv || data.MaNV || data.MaNv;

                        localStorage.setItem('userInfo', JSON.stringify({
                            fullname: data.fullName || data.fullname || "User",
                            role: data.role || data.Role || "PhucVu",
                            avatar: data.avatar || "",
                            maNV: maNhanVien
                        }));

                        console.log("Đã lưu vào bộ nhớ:", {
                            fullname: data.fullName,
                            maNV: maNhanVien
                        });

                        window.location.href = data.redirectUrl;
                    } else {
                        alertBox.innerHTML = `
                            <div class="alert alert-danger alert-dismissible fade show" role="alert" style="opacity: 0.9;">
                                ${data.message}
                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                            </div>`;
                        btnLogin.innerHTML = 'ĐĂNG NHẬP';
                        btnLogin.disabled = false;
                    }
                })
                .catch(error => {
                    alertBox.innerHTML = `
                        <div class="alert alert-warning alert-dismissible fade show" role="alert" style="opacity: 0.9;">
                            Lỗi kết nối đến máy chủ!
                            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                        </div>`;
                    btnLogin.innerHTML = 'ĐĂNG NHẬP';
                    btnLogin.disabled = false;
                });
        });
    }
});