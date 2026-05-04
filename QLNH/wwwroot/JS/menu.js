let cart = {};

window.addToCart = function (id, name, price, change) {
    if (!cart[id]) cart[id] = { name: name, price: price, qty: 0, note: "" };
    cart[id].qty += change;
    const qtyElement = document.getElementById(`qty-${id}`);

    if (cart[id].qty <= 0) {
        delete cart[id];
        if (qtyElement) qtyElement.innerText = "0";
    } else {
        if (qtyElement) qtyElement.innerText = cart[id].qty;
    }
    renderCart();
};

window.updateNote = function (id, value) {
    if (cart[id]) cart[id].note = value;
};

function renderCart() {
    const container = document.getElementById('cart-items');
    if (!container) return;

    container.innerHTML = "";
    let total = 0;

    if (Object.keys(cart).length === 0) {
        container.innerHTML = `
            <div class="text-center mt-5">
                <i class="fas fa-utensils fa-3x mb-3 text-black-50"></i>
                <p class="text-muted">Chưa có món nào được chọn</p>
            </div>`;
    }

    for (const [id, item] of Object.entries(cart)) {
        const itemTotal = item.qty * item.price;
        total += itemTotal;

        const html = `
            <div class="cart-item">
                <div class="d-flex justify-content-between align-items-start mb-2">
                    <div>
                        <div class="cart-item-name">${item.name}</div>
                        <small class="cart-item-price">${item.price.toLocaleString()} x ${item.qty}</small>
                    </div>
                    <div class="cart-item-total">${itemTotal.toLocaleString()}</div>
                </div>
                <input type="text" class="form-control form-control-sm cart-note"
                       placeholder="Ghi chú (ít cay, không hành...)"
                       value="${item.note}"
                       oninput="updateNote(${id}, this.value)">
            </div>`;
        container.insertAdjacentHTML('beforeend', html);
    }

    const totalElement = document.getElementById('cart-total');
    if (totalElement) totalElement.innerText = total.toLocaleString();
}

window.filterCategory = function (catId, el) {
    document.querySelectorAll('.cat-chip').forEach(e => e.classList.remove('active'));
    el.classList.add('active');

    document.querySelectorAll('.dish-card').forEach(card => {
        card.style.display = (catId === 'all' || card.dataset.category == catId) ? 'block' : 'none';
    });
};

window.searchDish = function () {
    const text = document.getElementById('searchInput').value.toLowerCase();
    document.querySelectorAll('.dish-card').forEach(card => {
        card.style.display = card.dataset.name.includes(text) ? 'block' : 'none';
    });
};

window.submitOrder = function () {
    const tableId = document.getElementById('selectedTable').value;

    if (!tableId) return alert("Vui lòng chọn bàn trước!");
    if (Object.keys(cart).length === 0) return alert("Đơn hàng trống, vui lòng chọn món!");

    const items = Object.entries(cart).map(([id, item]) => ({
        id: id,
        quantity: item.qty,
        price: item.price,
        note: item.note
    }));

    fetch('/api/luu-don-hang', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ so_ban: tableId, items: items })
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                alert("" + data.msg);
                window.location.href = "/table-list";
            } else {
                alert("Lỗi: " + data.msg);
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert("Lỗi kết nối server!");
        });
};

window.logoutUser = function () {
    localStorage.removeItem('userInfo');
    window.location.href = '/index.html';
};


document.addEventListener("DOMContentLoaded", function () {

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

            const currentPage = window.location.pathname.split("/").pop() || "menu.html";
            document.querySelectorAll('.sidebar .nav-link').forEach(link => {
                if (link.getAttribute('href') === currentPage) {
                    link.classList.add('active');
                }
            });
        })
        .catch(err => console.error("Lỗi tải Sidebar:", err));

    fetch('/api/categories')
        .then(res => res.json())
        .then(data => {
            let catHtml = `<div class="cat-chip active" onclick="filterCategory('all', this)">All</div>`;
            data.forEach(cat => {
                catHtml += `<div class="cat-chip" onclick="filterCategory('${cat.maNhom}', this)">${cat.tenNhom}</div>`;
            });
            document.getElementById('category_container').innerHTML = catHtml;
        })
        .catch(err => console.error("Lỗi tải danh mục:", err));

    fetch('/api/menu')
        .then(res => res.json())
        .then(data => {
            let menuHtml = '';
            data.forEach(mon => {
                let hinhAnh = mon.hinhAnh ? mon.hinhAnh : 'https://via.placeholder.com/200';
                menuHtml += `
                    <div class="dish-card" data-category="${mon.maNhom}" data-name="${mon.tenMon.toLowerCase()}">
                        <img src="${hinhAnh}" class="dish-img">
                        <div class="p-3 text-center">
                            <div class="fw-bold text-truncate mb-1">${mon.tenMon}</div>
                            <div class="text-danger fw-bold mb-2" style="color: #ff6b6b !important;">${mon.giaTien.toLocaleString('vi-VN')}</div>
                            <div class="d-flex justify-content-center gap-3 align-items-center">
                                <div class="qty-btn" onclick="addToCart(${mon.maMon}, '${mon.tenMon}', ${mon.giaTien}, -1)">-</div>
                                <span id="qty-${mon.maMon}" class="fw-bold">0</span>
                                <div class="qty-btn" onclick="addToCart(${mon.maMon}, '${mon.tenMon}', ${mon.giaTien}, 1)">+</div>
                            </div>
                        </div>
                    </div>`;
            });
            document.getElementById('menu_container').innerHTML = menuHtml;
        })
        .catch(err => console.error("Lỗi tải thực đơn:", err));

    fetch('/api/tables')
        .then(res => res.json())
        .then(data => {
            let tableHtml = '<option value="" class="text-dark">-- Chọn Bàn --</option>';
            data.forEach(ban => {
                tableHtml += `<option value="${ban.soBan}" class="text-dark">Bàn ${ban.soBan} (${ban.statusText})</option>`;
            });
            document.getElementById('selectedTable').innerHTML = tableHtml;

            const urlParams = new URLSearchParams(window.location.search);
            const tableId = urlParams.get('table_id');
            if (tableId) {
                document.getElementById('selectedTable').value = tableId;
            }
        })
        .catch(err => console.error("Lỗi tải danh sách bàn:", err));
});