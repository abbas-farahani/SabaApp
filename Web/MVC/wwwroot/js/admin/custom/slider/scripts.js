$(document).ready(function () {
    var imageInputElement = document.querySelector("[data-kt-image-input]");
    var imageInput = KTImageInput.getInstance(imageInputElement);
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
                if (response.status == "success") {
                    $('input[name="BackgroundValue"]').val(response.url);
                    toastr.success('فایل با موفقیت آپلود شد.', "بارگذاری تصویر اسلاید");
                } else {
                    $('.image-input-wrapper').css('background-image', '');
                    $('input[name="BackgroundValue"]').val('');
                    toastr.error(response.message, "خطای بارگذاری");
                }
            },
            error: function (error) {
                console.log(error);
                alert('خطا در آپلود فایل.');
            }
        });

    });

    var dropzone = document.querySelector("#media-uploader"),
        uploader = new Dropzone("#media-uploader", { 
        url: "/media/uploader",
        autoProcessQueue: true,
        parallelUploads: true,
        addRemoveLinks: true,
        maxFiles: 1,
        maxFilesize: 512,
        chunking: true,
        forceChunking: true,
        chunkSize: 8388608,
        retryChunks: true,
        retryChunksLimit: 3,
        previewContainer: dropzone.querySelector(".dropzone-items"),
        acceptedFiles: ".mp4, .m4a, .ogg",
        init: function () {
            var dropzone = this;
            $("#media-uploader-btn").click(function () {
                dropzone.hiddenFileInput.setAttribute("accept", ".mp4, .m4a, .ogg");
                dropzone.hiddenFileInput.click();
            });
            this.on("addedfile", function (file) {
                try {
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });
            this.on("uploadprogress", function (file, progress) {
                try {
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });

            this.on("sending", function (file) {
                try {
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });

            this.on("complete", function (file) {
                try {
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });

            this.on("success", function (file, response) {
                try {
                    var element = $('.media-uploader .upload-results');
                    if (element.length) {
                        if (response.status == "success") {
                            element.append(`<input type="hidden" name="gallery" value="${response.url}" />`);
                            element.append(`
                        <div class="symbol symbol-75px" data-uid="${file.upload.uuid}">
                            <div class="symbol-label" style="background-image:url('${response.url}')"></div>
                        </div>`);
                        }
                    }
                    console.log("success: ", response);
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });

            this.on("queuecomplete", function () {
                try {
                    toastr.success("بارگذاری تکمیل شد", "بارگذاری گالری");
                } catch (e) {
                    console.log("Errorn in 'sddd' event: ", e);
                }
            });
        }
    });

    $('[name=BackgroundType]').on('change', function () {
        var val = $(this).val();
        if (val == "video") {
            $('.image-type').addClass('d-none');
            $('.video-type').removeClass('d-none');
            $('.color-type').addClass('d-none');

        } else if (val == "color") {
            $('.image-type').addClass('d-none');
            $('.video-type').addClass('d-none');
            $('.color-type').removeClass('d-none');

        } else {
            $('.image-type').removeClass('d-none');
            $('.video-type').addClass('d-none');
            $('.color-type').addClass('d-none');
        }
    });

    $('[name=color-value]').on('input', function () {
        $('[name=hexInput]').val($(this).val());
        $('[name=BackgroundValue]').val($(this).val());
    });
});