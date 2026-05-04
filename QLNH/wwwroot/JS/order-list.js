document.addEventListener("DOMContentLoaded", function () {
    initSidebar();
    loadOrders();

    setInterval(loadOrders, 5000);
});

function initSidebar() {
    fetch('/sidebar.html')
        .then(response => {
            if (!response.ok) throw new Error("Không tìm thấy sidebar.html");
            return response.text();
        })
        .then(data => {
            document.getElementById('sidebar-placeholder').innerHTML = data;

            const userInfoRaw = localStorage.getItem('userInfo');
            if (!userInfoRaw) {
                window.location.href = '/index.html';
                return;
            }

            const userInfo = JSON.parse(userInfoRaw);
            document.getElementById('sidebar_fullname').innerText = userInfo.fullname || 'User';

            if (userInfo.avatar) {
                document.getElementById('sidebar_avatar').src = userInfo.avatar;
                document.getElementById('sidebar_avatar').style.display = 'inline-block';
                document.getElementById('sidebar_default_icon').style.display = 'none';
            }

            const userRole = (userInfo.role || "").trim();
            document.querySelectorAll('.role-item').forEach(item => {
                const rolesAllowed = item.getAttribute('data-role').split(',').map(r => r.trim());
                if (rolesAllowed.includes(userRole)) {
                    item.classList.remove('d-none');
                }
            });

            const currentPage = window.location.pathname.split("/").pop() || "order_list.html";
            document.querySelectorAll('.sidebar .nav-link').forEach(link => {
                if (link.getAttribute('href').includes(currentPage)) {
                    link.classList.add('active');
                }
            });
        })
        .catch(err => console.error("Lỗi tải Sidebar:", err));
}

function logoutUser() {
    localStorage.removeItem('userInfo');
    window.location.href = '/index.html';
}

function loadOrders() {
    fetch('/api/orders/active')
        .then(res => res.json())
        .then(data => {
            const grid = document.getElementById('orderGrid');

            if (!data || data.length === 0) {
                grid.innerHTML = `
                    <div class="text-center w-100 pt-5" style="color: rgba(0,0,0,0.5);">
                        <i class="fas fa-mug-hot fa-3x mb-3"></i>
                        <p class="mt-2 fs-5">Hiện chưa có đơn hàng nào đang chờ.</p>
                    </div>`;
                return;
            }

            let html = '';
            data.forEach(order => {
                let waited = order.waitedMin || 0;
                let isCompletedHtml = order.isCompleted
                    ? '<span class="done-status text-success fw-bold"><i class="fas fa-check-circle"></i> Đã xong</span>'
                    : `<span class="wait-time text-danger fw-bold"><i class="far fa-clock"></i> Chờ: ${waited} phút</span>`;

                let itemsHtml = '';
                order.chiTiet.forEach(item => {
                    let statusClass = (item.trangThaiMon === 'Served' || item.trangThaiMon === 'DaPhucVu') ? 'served' : '';
                    let noteHtml = item.ghiChu ? `<small class="text-muted fst-italic d-block" style="font-size: 0.7rem;">${item.ghiChu}</small>` : '';

                    let statusText = '';
                    let disableCheckbox = true;
                    let opacityStyle = 'opacity: 0.3;';
                    let isChecked = '';

                    if (item.trangThaiMon === 'ChoCheBien') statusText = '<span class="text-warning">Chờ nấu</span>';
                    else if (item.trangThaiMon === 'DangCheBien') statusText = '<span class="text-info">Đang nấu</span>';
                    else if (item.trangThaiMon === 'HoanTat') {
                        statusText = '<span class="text-success">Bếp đã xong</span>';
                        disableCheckbox = false;
                        opacityStyle = '';
                    }
                    else if (item.trangThaiMon === 'DaPhucVu' || item.trangThaiMon === 'Served') {
                        statusText = '<span class="st-served text-muted text-decoration-line-through">Đã phục vụ</span>';
                        isChecked = 'checked disabled';
                        opacityStyle = '';
                    }

                    itemsHtml += `
                        <div class="item-row ${statusClass} d-flex justify-content-between align-items-center mb-2 pb-2 border-bottom">
                            <div class="d-flex flex-column" style="width: 60%;">
                                <span class="item-name fw-bold text-dark">${item.tenMon}</span>
                                ${noteHtml}
                                <small style="font-size: 0.75rem;">${statusText}</small>
                            </div>
                            <div class="text-end d-flex align-items-center gap-3">
                                <span class="badge bg-secondary">x${item.soLuong}</span>
                                <div class="form-check m-0">
                                    <input class="form-check-input serve-check" type="checkbox" style="transform: scale(1.2); cursor: pointer; ${opacityStyle}"
                                           onchange="serveDish(this, ${item.maChiTiet}, '${item.tenMon}', ${order.soBan})"
                                           ${isChecked} ${disableCheckbox ? 'disabled title="Món chưa nấu xong"' : ''}>
                                </div>
                            </div>
                        </div>`;
                });

                html += `
                    <div class="order-card p-3 mb-3 bg-white rounded shadow-sm border" data-table="${order.soBan}">
                        <div class="card-header-custom d-flex justify-content-between align-items-start border-bottom pb-2 mb-2">
                            <div>
                                <div class="order-no fw-bold text-primary fs-5">MÃ ĐƠN: #${String(order.maHoaDon).padStart(4, '0')}</div>
                                ${isCompletedHtml}
                            </div>
                            <div class="table-badge badge bg-info text-dark fs-6 rounded-pill px-3 py-2">Bàn ${order.soBan}</div>
                        </div>
                        <div class="order-items overflow-auto" style="max-height: 200px;">
                            ${itemsHtml}
                        </div>
                        <div class="card-footer-custom d-flex justify-content-between align-items-center border-top pt-2 mt-2">
                            <span class="small text-muted fw-bold">Tạm tính:</span>
                            <span class="total-price fw-bold text-danger fs-5">${order.tongThanhToan.toLocaleString('vi-VN')} đ</span>
                        </div>
                    </div>`;
            });
            grid.innerHTML = html;
            searchOrder();
        })
        .catch(err => {
            console.error(err);
            document.getElementById('orderGrid').innerHTML = '<div class="text-danger w-100 text-center mt-5 fw-bold">Lỗi kết nối máy chủ!</div>';
        });
}

function searchOrder() {
    const text = document.getElementById('searchInput').value.toLowerCase();
    document.querySelectorAll('.order-card').forEach(card => {
        const tableNum = card.dataset.table;
        if (tableNum.includes(text)) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

function serveDish(checkbox, id, dishName, tableNum) {
    if (checkbox.checked) {
        if (!confirm(`Xác nhận đã phục vụ món "${dishName}" cho Bàn ${tableNum}?`)) {
            checkbox.checked = false;
            return;
        }

        fetch('/api/cap-nhat-mon', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                type: 'mon-an',
                id: id,
                status: 'DaPhucVu'
            })
        })
            .then(r => r.json())
            .then(data => {
                if (data.success) {
                    let row = checkbox.closest('.item-row');
                    if (row) {
                        row.classList.add('served');
                        row.style.opacity = '0.5';
                        let statusBadge = row.querySelector('small:last-child');
                        if (statusBadge) {
                            statusBadge.innerHTML = '<span class="text-muted text-decoration-line-through">Đã phục vụ</span>';
                        }
                    }
                    checkbox.disabled = true;

                    loadOrders();
                } else {
                    alert("❌ Lỗi: " + data.msg);
                    checkbox.checked = false;
                }
            })
            .catch(err => {
                console.error(err);
                alert("Lỗi kết nối server!");
                checkbox.checked = false;
            });
    }
}