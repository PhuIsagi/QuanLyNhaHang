let currentMode = 'order';

$(document).ready(function () {
    const userInfoRaw = localStorage.getItem('userInfo');
    if (!userInfoRaw) {
        window.location.href = '/index.html';
        return;
    }
    const userInfo = JSON.parse(userInfoRaw);
    $('#user_fullname').text(userInfo.fullname || 'Đầu Bếp');
    if (userInfo.avatar) $('#user_avatar').attr('src', userInfo.avatar);

    loadKitchenData();
    setInterval(loadKitchenData, 10000);
});

window.logout = function () {
    localStorage.removeItem('userInfo');
    window.location.href = "/index.html";
};

window.switchMode = function (mode) {
    currentMode = mode;
    $('.nav-tab-item').removeClass('active');
    $('#btn_mode_' + mode).addClass('active');
    loadKitchenData();
};

window.loadKitchenData = function () {
    let noCacheUrl = '/api/kitchen/tasks?mode=' + currentMode + '&_t=' + Date.now();

    $.ajax({
        url: noCacheUrl,
        type: 'GET',
        cache: false,
        success: function (data) {
            $('#waiting_count').text(data.waiting_list.length);
            $('#cooking_count').text(data.cooking_list.length);

            let waitHtml = '';
            if (data.waiting_list.length === 0) {
                waitHtml = '<div class="text-center mt-5 text-muted"><i class="fas fa-check-circle fa-2x mb-3"></i><p>Chưa có món nào chờ nấu!</p></div>';
            } else {
                data.waiting_list.forEach(item => waitHtml += createCardHtml(item, 'wait'));
            }
            $('#waiting_list_container').html(waitHtml);

            let cookHtml = '';
            if (data.cooking_list.length === 0) {
                cookHtml = '<div class="text-center mt-5 text-muted"><p>Đang rảnh tay...</p></div>';
            } else {
                data.cooking_list.forEach(item => cookHtml += createCardHtml(item, 'cook'));
            }
            $('#cooking_list_container').html(cookHtml);
        },
        error: function () {
            console.error("Lỗi khi tải dữ liệu Bếp!");
        }
    });
};

function createCardHtml(item, colType) {
    let id = item.maChiTiet || item.MaChiTiet;
    let ten = item.tenMon || item.TenMon;
    let sl = item.soLuong || item.SoLuong;
    let ban = item.soBan || item.SoBan;
    let gc = item.ghiChu || item.GhiChu;
    let ids = item.ids || item.Ids;
    let gcList = item.ghiChuList || item.GhiChuList;

    let html = `<div class="p-3 mb-3 bg-white rounded shadow-sm border" style="border-left: 5px solid ${colType === 'wait' ? '#ffc107' : '#198754'} !important;">`;

    html += `<div class="d-flex justify-content-between align-items-center mb-2">
                <span class="fw-bold fs-5 text-dark">${ten}</span>
                <span class="badge bg-danger fs-6 px-3 py-2">x${sl}</span>
             </div>`;

    if (currentMode === 'order') {
        html += `<div class="text-muted fw-bold mb-1"><i class="fas fa-map-marker-alt text-primary me-1"></i> Bàn ${ban}</div>`;
        if (gc) {
            html += `<div class="fst-italic text-secondary" style="font-size: 0.85rem;">📝 Ghi chú: ${gc}</div>`;
        }
    } else if (currentMode === 'dish') {
        if (gcList && gcList.length > 0) {
            html += `<div class="fst-italic text-secondary mb-2" style="font-size: 0.85rem; max-height: 60px; overflow-y: auto;">`;
            gcList.forEach(note => html += `<div class="mb-1">📝 ${note}</div>`);
            html += `</div>`;
        }
    }

    html += `<div class="mt-3 text-end">`;
    if (colType === 'wait') {
        if (currentMode === 'order') {
            html += `<button class="btn btn-warning fw-bold text-dark w-100 shadow-sm" onclick="updateStatusSingle(${id}, 'DangCheBien', this)"><i class="fas fa-fire me-1"></i> BẮT ĐẦU NẤU</button>`;
        } else {
            html += `<button class="btn btn-warning fw-bold text-dark w-100 shadow-sm" onclick='updateStatusGroup(${JSON.stringify(ids)}, "DangCheBien", this)'><i class="fas fa-fire me-1"></i> NẤU TẤT CẢ</button>`;
        }
    } else if (colType === 'cook') {
        if (currentMode === 'order') {
            html += `<button class="btn btn-success fw-bold w-100 shadow-sm" onclick="updateStatusSingle(${id}, 'HoanTat', this)"><i class="fas fa-check-circle me-1"></i> XONG</button>`;
        } else {
            html += `<button class="btn btn-success fw-bold w-100 shadow-sm" onclick='updateStatusGroup(${JSON.stringify(ids)}, "HoanTat", this)'><i class="fas fa-check-double me-1"></i> XONG TẤT CẢ</button>`;
        }
    }
    html += `</div></div>`;

    return html;
}

window.updateStatusSingle = function (id, status, btnElement) {
    if (btnElement) {
        btnElement.innerHTML = '<i class="fas fa-circle-notch fa-spin"></i> Đang xử lý...';
        btnElement.disabled = true;
    }

    fetch('/api/cap-nhat-mon', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ type: 'bep-single', id: id, status: status })
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                
                if (btnElement) {
                    $(btnElement).closest('.bg-white').slideUp(300, function () {
                        loadKitchenData();
                    });
                } else {
                    loadKitchenData();
                }
            } else {
                alert("Lỗi: " + data.msg);
                if (btnElement) { btnElement.disabled = false; btnElement.innerText = "Thử lại"; }
            }
        })
        .catch(err => {
            alert("Lỗi kết nối server!");
            if (btnElement) { btnElement.disabled = false; btnElement.innerText = "Thử lại"; }
        });
};

window.updateStatusGroup = function (ids, status, btnElement) {
    if (btnElement) {
        btnElement.innerHTML = '<i class="fas fa-circle-notch fa-spin"></i> Đang xử lý...';
        btnElement.disabled = true;
    }

    fetch('/api/cap-nhat-mon', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        
        body: JSON.stringify({ type: 'bep-group', ids: ids, status: status })
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                if (btnElement) {
                    $(btnElement).closest('.bg-white').slideUp(300, function () {
                        loadKitchenData();
                    });
                } else {
                    loadKitchenData();
                }
            } else {
                alert("Lỗi: " + data.msg);
                if (btnElement) { btnElement.disabled = false; btnElement.innerText = "Thử lại"; }
            }
        })
        .catch(err => {
            alert("Lỗi kết nối server!");
            if (btnElement) { btnElement.disabled = false; btnElement.innerText = "Thử lại"; }
        });
};