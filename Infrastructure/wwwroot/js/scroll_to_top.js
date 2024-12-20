document.addEventListener('DOMContentLoaded', function () {
    // Hiển thị nút khi cuộn xuống
    window.onscroll = function () {
        var backToTopButton = document.querySelector('.back-to-top');
        if (document.body.scrollTop > 100 || document.documentElement.scrollTop > 100) {
            backToTopButton.style.display = 'flex';
        } else {
            backToTopButton.style.display = 'none';
        }
    };

    // Xử lý sự kiện khi click vào nút
    var backToTopButton = document.querySelector('.back-to-top');
    if (backToTopButton) {
        backToTopButton.addEventListener('click', function (e) {
            e.preventDefault(); // Ngăn chặn hành vi mặc định

            // Sử dụng scroll bằng 'smooth' để cuộn mượt mà
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
});
