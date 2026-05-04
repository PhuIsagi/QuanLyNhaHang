const API_REPORT_URL = '/api/manager/revenue-report';
const API_DISH_URL = '/api/manager/dishes';

let categoryBarChart = null;
let dishPieChart = null;
let dishModal = null;

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

function formatDate(date) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
}

$(document).ready(function () {
    // 1. Load Sidebar
    fetch('/sidebar.html')
        .then(response => response.text())
        .then(data => {
            document.getElementById('sidebar-placeholder').innerHTML = data;

            const userInfoRaw = localStorage.getItem('userInfo');
            if (!userInfoRaw) { window.location.href = '/index.html'; return; }
            const userInfo = JSON.parse(userInfoRaw);

            // Gán tên
            document.getElementById('sidebar_fullname').innerText = userInfo.fullname || 'Quản lý';

            // Xử lý Avatar
            const avatarImg = document.getElementById('sidebar_avatar');
            const defaultIcon = document.getElementById('sidebar_default_icon');

            if (avatarImg) {
                if (userInfo.avatar && userInfo.avatar.trim() !== '') {
                    avatarImg.src = userInfo.avatar;
                    avatarImg.style.display = 'inline-block';
                    if (defaultIcon) defaultIcon.style.display = 'none';

                    avatarImg.onerror = function () {
                        this.src = 'https://cdn-icons-png.flaticon.com/512/3135/3135715.png';
                    };
                } else {
                    avatarImg.src = 'https://cdn-icons-png.flaticon.com/512/3135/3135715.png';
                    avatarImg.style.display = 'inline-block';
                    if (defaultIcon) defaultIcon.style.display = 'none';
                }
            }

            // Phân quyền
            const userRole = (userInfo.role || "").trim();
            document.querySelectorAll('.role-item').forEach(item => {
                const rolesAllowed = item.getAttribute('data-role').split(',').map(r => r.trim());
                if (rolesAllowed.includes(userRole)) item.classList.remove('d-none');
            });

            document.querySelectorAll('.sidebar .nav-link').forEach(link => {
                if (link.getAttribute('href').includes("manager.html")) link.classList.add('active');
            });
        });

    dishModal = new bootstrap.Modal(document.getElementById('dishModal'));

    setTodayFilter();
    loadDishes();
});

window.logoutUser = function () {
    localStorage.removeItem('userInfo');
    window.location.href = '/index.html';
};

function drawCategoryBarChart(data) {
    const ctx = document.getElementById('categoryBarChart').getContext('2d');
    if (categoryBarChart) categoryBarChart.destroy();

    if (data.length === 0) data = [{ category: 'Chưa có data', revenue: 0 }];

    const labels = data.map(d => d.category);
    const revenues = data.map(d => d.revenue);

    categoryBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu (VNĐ)',
                data: revenues,
                backgroundColor: '#3498db',
                borderRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
}

function drawDishPieChart(data) {
    const ctx = document.getElementById('dishPieChart').getContext('2d');
    if (dishPieChart) dishPieChart.destroy();

    if (data.length === 0) data = [{ dish_name: 'Chưa có data', quantity: 1 }];

    const labels = data.map(d => d.dish_name);
    const quantities = data.map(d => d.quantity);
    const bgColors = ['#FF4B2B', '#f39c12', '#27ae60', '#3498db', '#9b59b6', '#34495e', '#e67e22', '#1abc9c', '#e74c3c', '#95a5a6'];

    dishPieChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: quantities,
                backgroundColor: bgColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { position: 'right' } }
        }
    });
}

function displayTopDishes(data) {
    const tbody = $('#top_dishes_list');
    tbody.empty();
    if (data.length === 0) {
        tbody.append('<tr><td colspan="3" class="text-center text-muted py-4">Chưa có dữ liệu</td></tr>');
        return;
    }
    data.forEach((dish, index) => {
        let rankClass = index === 0 ? 'text-danger' : (index === 1 ? 'text-warning' : 'text-primary');
        tbody.append(`
            <tr>
                <td class="fw-bold ${rankClass}">${index + 1}</td>
                <td class="fw-bold text-dark">${dish.dish_name}</td>
                <td class="text-end fw-bold text-success">${dish.quantity}</td>
            </tr>
        `);
    });
}

function setTodayFilter() {
    const today = formatDate(new Date());
    $('#start_date').val(today);
    $('#end_date').val(today);
    fetchReport();
}

function fetchReport() {
    $.get(API_REPORT_URL, { start_date: $('#start_date').val(), end_date: $('#end_date').val() }, function (response) {
        if (response.success) {
            $('#kpi_revenue').text(formatCurrency(response.summary.total_revenue));
            $('#kpi_invoices').text(response.summary.total_invoices);
            $('#kpi_avg_revenue').text(formatCurrency(response.summary.avg_revenue_per_invoice));
            $('#report_period').text(response.summary.report_period);

            drawCategoryBarChart(response.category_report);
            drawDishPieChart(response.top_dishes);
            displayTopDishes(response.top_dishes);
        } else {
            alert('Lỗi: ' + response.msg);
        }
    });
}

function previewImage(input) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $('#imagePreview').attr('src', e.target.result).show();
            $('#dishImageBase64').val(e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function loadDishes() {
    $.get(API_DISH_URL, function (data) {
        let html = '';
        if (!data || data.length === 0) {
            $('#dishes_table_body').html('<tr><td colspan="5" class="text-center py-4 text-muted">Chưa có món ăn nào trong hệ thống.</td></tr>');
            return;
        }

        data.forEach(d => {
            let id = d.MaMon !== undefined ? d.MaMon : d.maMon;
            let name = d.TenMon !== undefined ? d.TenMon : d.tenMon;
            let catId = d.MaNhom !== undefined ? d.MaNhom : d.maNhom;
            let price = d.GiaTien !== undefined ? d.GiaTien : d.giaTien;
            let img = d.HinhAnh !== undefined ? d.HinhAnh : d.hinhAnh;

            let safeImg = img ? img.replace(/'/g, "\\'") : '';
            let imgDisplay = img ? img : 'https://via.placeholder.com/50';

            html += `
            <tr>
                <td><img src="${imgDisplay}" onerror="this.src='https://via.placeholder.com/50'" style="width: 50px; height: 50px; object-fit: cover; border-radius: 5px; border: 1px solid #eee;"></td>
                <td class="fw-bold text-dark">${name}</td>
                <td>Nhóm ${catId}</td>
                <td class="text-danger fw-bold">${formatCurrency(price)}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-primary me-1" onclick="editDish(${id}, '${name}', ${catId}, ${price}, '${safeImg}')"><i class="fas fa-edit"></i> Sửa</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteDish(${id}, '${name}')"><i class="fas fa-trash"></i> Xóa</button>
                </td>
            </tr>`;
        });
        $('#dishes_table_body').html(html);
    }).fail(function () {
        $('#dishes_table_body').html('<tr><td colspan="5" class="text-center py-4 text-danger">Lỗi kết nối API lấy danh sách món ăn.</td></tr>');
    });
}

function showDishModal() {
    $('#dishForm')[0].reset();
    $('#dishId').val('0');
    $('#imagePreview').hide().attr('src', '');
    $('#dishImageBase64').val('');
    $('#dishModalTitle').text('Thêm Món Mới');
    dishModal.show();
}

function editDish(id, name, catId, price, imgBase64) {
    $('#dishId').val(id);
    $('#dishName').val(name);
    $('#dishCategory').val(catId);
    $('#dishPrice').val(price);
    $('#dishImageBase64').val(imgBase64);

    if (imgBase64 && imgBase64.trim() !== '' && imgBase64 !== 'null') {
        $('#imagePreview').attr('src', imgBase64).show();
    } else {
        $('#imagePreview').hide();
    }

    $('#dishModalTitle').text('Cập nhật Món Ăn');
    dishModal.show();
}

function saveDish() {
    const dish = {
        MaMon: parseInt($('#dishId').val()),
        TenMon: $('#dishName').val(),
        MaNhom: parseInt($('#dishCategory').val()),
        GiaTien: parseFloat($('#dishPrice').val()),
        HinhAnh: $('#dishImageBase64').val()
    };

    if (!dish.TenMon || isNaN(dish.MaNhom) || isNaN(dish.GiaTien)) {
        alert("Vui lòng nhập đầy đủ thông tin Tên món, Mã nhóm và Giá tiền!");
        return;
    }

    const method = dish.MaMon === 0 ? 'POST' : 'PUT';
    const url = dish.MaMon === 0 ? API_DISH_URL : `${API_DISH_URL}/${dish.MaMon}`;

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json',
        data: JSON.stringify(dish),
        success: function () {
            dishModal.hide();
            loadDishes();
        },
        error: function (err) { alert("Lỗi xử lý! Vui lòng kiểm tra lại API."); }
    });
}

function deleteDish(id, name) {
    if (confirm(`Bạn có chắc chắn muốn xóa món "${name}" không?`)) {
        $.ajax({
            url: `${API_DISH_URL}/${id}`,
            type: 'DELETE',
            success: function () { loadDishes(); },
            error: function () { alert("Không thể xóa món ăn này vì nó đang nằm trong hóa đơn!"); }
        });
    }
}