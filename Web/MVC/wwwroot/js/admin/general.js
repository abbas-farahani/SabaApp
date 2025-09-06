var token = $("meta[name='_token']").attr('content'), key = false;

$(document).ready(function () {
    if ($('#LanguagesList').length > 0) {
        var languagesListFormat = function (item) {
            if (!item.id) {
                return item.text;
            }
            var span = document.createElement('span');
            var imgUrl = item.element.getAttribute('data-kt-select2-country');
            var template = '';

            template += '<img src="' + imgUrl + '" class="rounded-circle h-20px me-2" alt="image"/>';
            template += item.text;

            span.innerHTML = template;

            return $(span);
        }
        $('#LanguagesList').select2({
            templateSelection: languagesListFormat,
            templateResult: languagesListFormat
        });
    }
});
$(document).on('blur', '.sluggable', function () {
    if (!key && $(this).data('generator') == "on" && $(this).val().length > 3) {
        $('.slug-container').removeClass('d-none');
        var data = {
            title: $('input.sluggable').val(),
            type: $(this).data('type')
        };
        SlugGenerator(data);
        key = true;
    }
});

$(document).on('click', '.change-slug', function () {
    $('.custom-slug-form').removeClass('d-none');
});

$(document).on('click', '.custom-slug-submit', function (e) {
    e.preventDefault();
    if ($('input.slug-input').val() == $('input.slug-input').data('value')) return;

    $('.custom-slug-form').addClass('d-none');
    var data = {
        title: $('input.sluggable').val(),
        type: $(this).data('type'),
        slug: $('input.slug-input').val()
    };
    SlugGenerator(data);
    $('.slug-anchor').text($('input.slug-input').val());
});

$(document).on('click', '.custom-slug-cancel', function (e) {
    e.preventDefault();
    $('.custom-slug-form').addClass('d-none');
    $('input.slug-input').val($('.slug-anchor').text());
});

function SlugGenerator(data) {
    var target = document.querySelector('.slug-preview');
    var blockUI = (KTBlockUI.getInstance(target) != null) ? KTBlockUI.getInstance(target) : new KTBlockUI(target);
    $.ajax({
        type: 'post',
        url: '/generateslug',
        data: data,
        contextType: 'application/json; charset=utf-8',
        dataType: 'json',
        headers: {
            "X-XSRF-TOKEN": token
        },
        beforeSend: function () {
            $('form.sluggable input[type="submit"]').attr('disabled', true);
            blockUI.block();
        },
        success: function (data) {
            $('input.slug-input').val(data.slug);
            $('.slug-anchor').text(data.slug);
        },
        complete: function () {
            $('form.sluggable input[type="submit"]').attr('disabled', false);
            if (blockUI.isBlocked())
                blockUI.release();
        }
    });
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
                data: { id: id },
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