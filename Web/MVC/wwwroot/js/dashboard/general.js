var token = $("meta[name='_token']").attr('content'), key = false, coin = "eth", timeFrames = ["m5", "m10", "m15", "m30", "h1"/*, "daily"*/];
const connection = new signalR.HubConnectionBuilder().withUrl("/crypto").build();
const FULL_DASH = 280, totalTime = 30; let timerInterval = null, timeLeft = totalTime;

function startCountdown() {
    clearInterval(timerInterval); // تایمر قبلی رو پاک کن
    timeLeft = totalTime;

    const $progress = $('.circle-progress');

    function updateTimer() {
        let minutes = Math.floor(timeLeft / 60);
        let seconds = timeLeft % 60;
        $('#timer').text(`${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`);

        let offset = FULL_DASH * (1 - timeLeft / totalTime);
        $progress.css('stroke-dashoffset', offset);

        //if (timeLeft <= 60) {
        //    $progress.css('stroke', 'red');
        //} else {
        //    $progress.css('stroke', 'green');
        //}

        timeLeft--;

        if (timeLeft < 0) {
            clearInterval(timerInterval);
        }
    }

    updateTimer(); // اجرای اولیه
    timerInterval = setInterval(updateTimer, 1000);
}

// مقدار اولیه برای رسم صحیح نوار
$(document).ready(() => {
    $('.circle-progress').attr({
        'stroke-dasharray': FULL_DASH,
        'stroke-dashoffset': 0
    });
});

async function Start() {
    try {
        await connection.start();
        console.log("Online");
        connection.invoke("SetConnection");
    } catch (e) {
        console.log("DisConnect");
        setTimeout(Start, 4000);
    }
}
connection.onclose(Start);
Start();

function SetCurrentCoin(currentCoin) {
    try {
        connection.invoke("SetCoin", currentCoin);
    } catch (e) {
        console.warn("Connection state issue");
    }
}

connection.on("testresponse", function (val) {
    console.log("date: ", val);
})

connection.on("RecieveError", (data) => {
    console.log("Error:", data);
});

connection.on("LivePrice", function (coin, price) {
    console.log(`${coin}: ${price}`);
});

// لغو درخواست آپدیت
function stopUpdates() {
    connection.invoke("UnsubscribeFromUpdates");
}

function UpdateFunc(data) {
    console.log("Updated data:", data);
}

$(document).on('click', '.delete-request', function (e) {
    DeleteItem($(this).data('id'), $(this).data('action'))
});

function DeleteItem(id, action) {
    if (id == null) return;
    Swal.mixin({
        customClass: {
            confirmButton: "btn btn-success",
            cancelButton: "btn btn-danger",
        },
        buttonStyling: false
    }).fire({
        //icon: 'info',
        title: 'آیا از حذف آیتم انتخاب شده مطمئن هستید؟',
        position: 'bottom-end',
        showClass: {
            popup: `
                        animate__animated
                        animate__fadeInUp
                        animate__faster
                    `
        },
        hideClass: {
            popup: `
                        animate__animated
                        animate__fadeOutDown
                        animate__faster
                    `
        },
        showConfirmButton: true,
        showCancelButton: true,
        cancelButtonText: 'انصراف',
        confirmButtonText: 'بله، حذف کن',
        width: 460
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'post',
                url: `${action}/delete`,
                data: { id: parseInt(id) },
                contextType: 'application/json; charset=utf-8',
                dataType: 'json',
                headers: {
                    "X-XSRF-TOKEN": token
                },
                beforeSend: function () {
                },
                success: function (data) {
                    if (data.result) {
                        ConfirmationMessage("حذف با موفقیت انجام شد", "success");
                        $(`.row-item-${id}`).remove();
                    } else {
                        ConfirmationMessage("حذف با خطا مواجه شد", "error");
                    }
                },
                complete: function () {
                },
                error: function (e) {
                    console.log(e);
                    ConfirmationMessage("خطایی رخ داد", "error");
                }
            });
        }
    });

}

function GetLastSectionUrl() {
    var url = $(location).attr('href');
    const urlObj = new URL(url);
    urlObj.search = '';
    urlObj.hash = '';
    const result = urlObj.toString();
    parts = result.split("/");
    if (parts[parts.length - 1].length < 1 || parts[parts.length - 1] == '')
        return parts[parts.length - 3];
    else
        return parts[parts.length - 2];
}

$(document).on('change', '.select-currency', function () {
    var selectedCoin = $(this).val();
    //var indicator = $(this).data('indicator');
    var indicator = GetLastSectionUrl();
    if (selectedCoin) {
        window.location.href = `/dashboard/${indicator}/${selectedCoin}`;
    }
});

var KTLayoutSearch = function () {
    var e, t, n, i, r, o, a, l, s, u, d, c, m = function (e) {
        var query = e.getQuery().toLowerCase(), len = $(n).find('.currency').length;
        t.classList.add("d-none");
        //setTimeout(function () {
        //}, 1000);
        $(n).find('.currency').each(function () {
            var name = $(this).find('.currency-name').text().toLowerCase(), symbol = $(this).find('.currency-symbol').text().toLowerCase();
            if (name.includes(query) || symbol.includes(query)) {
                if ($(this).hasClass('d-none')) $(this).removeClass('d-none');
            } else {
                $(this).addClass('d-none');
            }
        });
        $(n).find('.currency.d-none').length == length ? (n.classList.add("d-none"), r.classList.remove("d-none")) : (n.classList.remove("d-none"), r.classList.add("d-none"));

        $(n).find('.fiat').each(function () {
            var name = $(this).find('.fiat-name').text().toLowerCase(), symbol = $(this).find('.fiat-symbol').text().toLowerCase();
            if (name.includes(query) || symbol.includes(query)) {
                if ($(this).hasClass('d-none')) $(this).removeClass('d-none');
            } else {
                $(this).addClass('d-none');
            }
        });
        e.complete();
    }, f = function (e) {
        t.classList.remove("d-none"),
            n.classList.add("d-none"),
            r.classList.add("d-none"),
            $(n).find('.currency.d-none').each(function () {
                $(this).removeClass('d-none');
            }), $(n).find('.fiat.d-none').each(function () {
                $(this).removeClass('d-none');
            });
    };
    return {
        init: function () {
            (e = document.querySelector("#kt_header_search")) && (i = e.querySelector('[data-kt-search-element="wrapper"]'),
                e.querySelector('[data-kt-search-element="form"]'),
                t = e.querySelector('[data-kt-search-element="main"]'),
                n = e.querySelector('[data-kt-search-element="results"]'),
                r = e.querySelector('[data-kt-search-element="empty"]'),
                (c = new KTSearch(e)).on("kt.search.process", m),
                c.on("kt.search.clear", f))
        }
    }
}();
KTUtil.onDOMContentLoaded((function () {
    KTLayoutSearch.init()
}
));