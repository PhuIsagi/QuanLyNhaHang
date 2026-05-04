let selectedTableId = null;

window.confirmSelectTable = function (tableId, status) {
    if (status === 'DatTruoc')
        return alert("Bàn này ĐÃ ĐẶT TRƯỚC!");
    if (status === 'CoKhach')
        return alert("Bàn này ĐANG CÓ KHÁCH!");

    selectedTableId = tableId;

    const modalNum = document.getElementById('modalTableNum');
    if (modalNum) modalNum.innerText = tableId;

    const btn = document.getElementById('btnGoToMenu');
    if (btn) {
        btn.onclick = function () {
            window.location.href = "/menu?table_id=" + selectedTableId;
        };
    }

    const modalEl = document.getElementById('confirmModal');
    if (modalEl) {
        new bootstrap.Modal(modalEl).show();
    }
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

            const currentPage = window.location.pathname.split("/").pop() || "table_list.html";
            document.querySelectorAll('.sidebar .nav-link').forEach(link => {
                if (link.getAttribute('href') === currentPage) {
                    link.classList.add('active');
                }
            });
        })
        .catch(err => console.error("Lỗi tải Sidebar:", err));

    fetch('/api/tables')
        .then(res => res.json())
        .then(data => {
            let floor1Html = '';
            let floor2Html = '';

            data.forEach(ban => {
                let colClass = [9, 10, 19, 20].includes(ban.soBan) ? 'col-xl-6 col-lg-6 col-md-12' : 'col-xl-3 col-lg-3 col-md-6';

                let tableCard = `
                    <div class="${colClass}">
                        <div class="table-box ${ban.cssClass}" onclick="confirmSelectTable('${ban.soBan}', '${ban.trangThai}')">
                            <div class="table-capacity"><i class="fas fa-user-friends"></i> ${ban.soGhe}</div>
                            <div class="table-title">Bàn ${ban.soBan}</div>
                            <div class="table-status-text">${ban.statusText}</div>
                            <i class="fas ${ban.iconClass} bg-icon"></i>
                        </div>
                    </div>`;

                if (ban.tang === 1 || ban.tang === null) {
                    floor1Html += tableCard;
                } else if (ban.tang === 2) {
                    floor2Html += tableCard;
                }
            });

            document.getElementById('floor1_container').innerHTML = floor1Html;
            document.getElementById('floor2_container').innerHTML = floor2Html;
        })
        .catch(err => {
            console.error("Lỗi tải API Bàn:", err);
            document.getElementById('floor1_container').innerHTML = '<div class="text-danger w-100 text-center mt-5">Lỗi kết nối máy chủ!</div>';
            document.getElementById('floor2_container').innerHTML = '<div class="text-danger w-100 text-center mt-5">Lỗi kết nối máy chủ!</div>';
        });

});