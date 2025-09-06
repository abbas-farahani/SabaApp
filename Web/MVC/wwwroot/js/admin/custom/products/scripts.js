$(document).ready(function () {
    CallCKEditor('.product-classic-editor');
    CallCKEditor('.product-desc-classic-editor');
    const statusBullet = document.getElementById("new-product-status-bullet"), statusClasses = ["bg-success", "bg-warning", "bg-danger", "bg-primary"], datePicker = document.getElementById("new-product-datepicker"), statusSelect = document.getElementById("Status"), imageInputElement = document.querySelector("[data-kt-image-input]"), imageInput = KTImageInput.getInstance(imageInputElement); var gallery = [];
    $(statusSelect).on("change", function (event) {
        statusBullet.classList.remove(...statusClasses);
        switch (event.target.value) {
            case "published":
                statusBullet.classList.add("bg-success");
                hideDatePicker();
                break;
            case "scheduled":
                statusBullet.classList.add("bg-warning");
                showDatePicker();
                break;
            case "inactive":
                statusBullet.classList.add("bg-danger");
                hideDatePicker();
                break;
            case "draft":
                statusBullet.classList.add("bg-primary");
                hideDatePicker();
                break;
        }
    });

    $("#new-product-datepicker").flatpickr({
        enableTime: true,
        locale: "fa",
        dateFormat: "Y-m-d H:i:s"
    });

    function showDatePicker() {
        datePicker.parentNode.classList.remove("d-none");
    }
    function hideDatePicker() {
        datePicker.parentNode.classList.add("d-none");
    }

    imageInput.on("kt.imageinput.changed", function () {
        var formData = new FormData();
        var fileInput = $('[name="avatar"')[0].files[0];
        formData.append('file', fileInput);

        $.ajax({
            url: '/media/attach',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log(response);
                if (response.status == "success") {
                    gallery.push(response.url);
                    $('input[name="product.FeaturedImage"]').val(response.url);
                    $('input[name="ThumbName"]').val(response.fileName);
                    toastr.success('فایل با موفقیت آپلود شد.', "بارگذاری تصویر شاخص");
                } else {
                    $('.image-input-wrapper').css('background-image', '');
                    $('input[name="product.FeaturedImage"]').val('');
                    $('input[name="ThumbName"]').val('');
                    toastr.error(response.message, "خطای بارگذاری");
                }
            },
            error: function (error) {
                console.log(error);
                alert('خطا در آپلود فایل.');
            }
        });

    });

    $('#LanguagesList').on('change', function () {
        if ($(this).val() != $(this).data('default')) {
            $('.translation-card .card-footer').html(`<span class="text-danger">حتما محصول اصلی را انتخاب کنید</span>`);
            $('#OriginalProduct .select2').removeClass('d-none');
            if ($('#OriginalProduct').val() == null || $('#OriginalProduct').val() == '')
                $('#add_product_submit').addClass('d-none');
        }
        else {
            $('.translation-card .card-footer').html('');
            $('#add_product_submit').removeClass('d-none');
            $('#OriginalProduct .select2').addClass('d-none');
        }
    });

    $('#OriginalProduct').on('change', function () {
        if ($(this).val() != null)
            $('#add_product_submit').removeClass('d-none');
    });
});

//$(document).on('submit', 'form.new-product', function (e) {
//    e.preventDefault();
//    var value = $('#LanguagesList').val(), default = $('#LanguagesList').data('default');
//
//});
