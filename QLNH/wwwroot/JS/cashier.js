let currentBillData = null;
let currentMaHoaDon = null;

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}

function parseCurrency(value) {
    if (!value) return 0;
    return parseFloat(value.toString().replace(/\D/g, '')) || 0;
}

$(document).ready(function () {
    // Xử lý ô nhập tiền mặt
    $('#customer_cash').on('input', function () { calculateChange(); });
    $('#customer_cash').on('blur', function () {
        const rawValue = parseCurrency($(this).val());
        if (rawValue > 0) $(this).val(rawValue.toLocaleString('vi-VN'));
    });
    $('#customer_cash').on('focus', function () {
        const rawValue = parseCurrency($(this).val());
        if (rawValue > 0) $(this).val(rawValue);
        else $(this).val('');
    });

    // Lấy danh sách bàn
    $.get('/api/tables', function (data) {
        let options = '<option value="">-- Chọn bàn --</option>';
        data.forEach(ban => {
            if (ban.trangThai === 'CoKhach') {
                options += `<option value="${ban.soBan}">Bàn ${ban.soBan} (${ban.statusText})</option>`;
            }
        });
        $('#select_table').html(options);
    }).fail(function () {
        console.error("Lỗi khi tải danh sách bàn.");
    });
});

function calculateChange() {
    const cashInputFormatted = $('#customer_cash').val();
    const cashInput = parseCurrency(cashInputFormatted);
    const totalAmount = parseFloat($('#summary_total').data('total')) || 0;
    let changeExact = cashInput - totalAmount;

    if (changeExact < 0) {
        $('#change_amount_display').text('Thiếu ' + formatCurrency(Math.abs(changeExact))).addClass('text-danger').removeClass('text-success');
        $('#btn_checkout_cash').prop('disabled', true);
    } else {
        $('#change_amount_display').text(formatCurrency(changeExact)).addClass('text-success').removeClass('text-danger');
        $('#change_amount').val(changeExact);
        if (totalAmount > 0) $('#btn_checkout_cash').prop('disabled', false);
    }
}

function fetchBill() {
    const soBan = $('#select_table').val();

    $('#bill_items').html('<tr><td colspan="4" class="text-center text-muted py-5"><i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu...</td></tr>');
    $('#summary_total').data('total', 0).text('0 VNĐ');
    $('#customer_cash').val('');
    $('#change_amount_display').text('0 VNĐ');

    // Reset nút thanh toán
    $('#btn_checkout_cash').prop('disabled', true);

    currentBillData = null;
    currentMaHoaDon = null;

    if (!soBan) {
        $('#bill_items').html('<tr><td colspan="4" class="text-center text-muted py-5"><i class="fas fa-utensils"></i><br>Vui lòng chọn bàn</td></tr>');
        return;
    }

    $.get(`/api/get-bill/${soBan}`, function (response) {
        if (response.success) {
            currentBillData = response;
            currentMaHoaDon = response.ma_hoa_don;

            $('#bill_id').text('#' + response.ma_hoa_don);
            $('#time_in').text(response.thoi_gian_vao);

            let itemsHtml = '';
            response.items.forEach(item => {
                itemsHtml += `
                    <tr>
                        <td class="ps-4 fw-bold text-dark">${item.ten_mon}
                            ${item.ghi_chu ? '<br><small class="text-muted fst-italic fw-normal"><i class="fas fa-pen-alt me-1"></i>' + item.ghi_chu + '</small>' : ''}
                        </td>
                        <td class="text-center align-middle"><span class="badge bg-light text-dark border px-2">x${item.so_luong}</span></td>
                        <td class="text-end align-middle">${formatCurrency(item.don_gia)}</td>
                        <td class="text-end align-middle pe-4 fw-bold" style="color: #333;">${formatCurrency(item.thanh_tien)}</td>
                    </tr>
                `;
            });
            $('#bill_items').html(itemsHtml);

            const calcs = response.calculations;
            $('#summary_quantity').text(calcs.tong_so_luong);
            $('#summary_subtotal').text(formatCurrency(calcs.tam_tinh));
            $('#summary_vat').text(formatCurrency(calcs.vat));
            $('#summary_discount').text(formatCurrency(calcs.giam_gia));
            $('#summary_total').text(formatCurrency(calcs.tong_thanh_toan)).data('total', calcs.tong_thanh_toan);

            calculateChange();

        } else {
            $('#select_table').val('');
            $('#bill_items').html('<tr><td colspan="4" class="text-center text-muted py-5">' + response.msg + '</td></tr>');
            alert(response.msg);
        }
    });
}

function checkout() {
    const totalAmount = parseFloat($('#summary_total').data('total'));
    if (!currentMaHoaDon || totalAmount <= 0) {
        alert('Vui lòng chọn bàn có hóa đơn hợp lệ.');
        return;
    }

    const soBan = $('#select_table').val();

    if (!confirm(`Xác nhận thanh toán HĐ #${currentMaHoaDon} cho Bàn ${soBan}?`)) {
        return;
    }

    $('#btn_checkout_cash').prop('disabled', true);

    const tienKhachDua = parseCurrency($('#customer_cash').val());

    const postData = {
        ma_hoa_don: currentMaHoaDon,
        tong_thanh_toan: totalAmount,
        tien_khach_dua: tienKhachDua,
        giam_gia: currentBillData.calculations.giam_gia,
        phuong_thuc: 'Tiền mặt'
    };

    $.ajax({
        url: '/api/checkout',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(postData),
        success: function (response) {
            if (response.success) {
                alert(`Thanh toán thành công! Bàn ${soBan} đã trống.`);
                window.location.reload();
            } else {
                alert('Lỗi thanh toán: ' + response.msg);
                $('#btn_checkout_cash').prop('disabled', false);
            }
        },
        error: function () {
            alert('Lỗi kết nối máy chủ.');
            $('#btn_checkout_cash').prop('disabled', false);
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    fetch('/sidebar.html')
        .then(response => {
            if (!response.ok) throw new Error("Không tìm thấy sidebar.html");
            return response.text();
        })
        .then(data => {
            document.getElementById('sidebar-placeholder').innerHTML = data;

            const userInfoRaw = localStorage.getItem('userInfo');
            if (!userInfoRaw) { window.location.href = '/index.html'; return; }

            const userInfo = JSON.parse(userInfoRaw);
            document.getElementById('sidebar_fullname').innerText = userInfo.fullname || 'Thu Ngân';

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

            const userRole = (userInfo.role || "").trim();
            document.querySelectorAll('.role-item').forEach(item => {
                const rolesAllowed = item.getAttribute('data-role').split(',').map(r => r.trim());
                if (rolesAllowed.includes(userRole)) item.classList.remove('d-none');
            });

            const currentPage = window.location.pathname.split("/").pop() || "cashier.html";
            document.querySelectorAll('.sidebar .nav-link').forEach(link => {
                if (link.getAttribute('href').includes(currentPage)) link.classList.add('active');
            });
        })
        .catch(err => console.error("Lỗi tải Sidebar:", err));
});

window.logoutUser = function () {
    localStorage.removeItem('userInfo');
    window.location.href = '/index.html';
};