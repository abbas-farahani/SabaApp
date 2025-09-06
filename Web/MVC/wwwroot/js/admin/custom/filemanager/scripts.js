var dropzone = document.querySelector("#media-uploader");//, previewNode = dropzone.querySelector(".dropzone-item"), previewTemplate = previewNode.parentNode.innerHtml;
//previewNode.id = "";
//previewNode.parentNode.removeChild(previewNode);

var uploader = new Dropzone("#media-uploader", { // #kt_ecommerce_add_product_media
    url: "/media/uploader",
    autoProcessQueue: true,
    parallelUploads: true,
    addRemoveLinks: true,
    maxFiles: 10,
    maxFilesize: 512,
    chunking: true,
    forceChunking: true,
    chunkSize: 8388608,
    retryChunks: true,
    retryChunksLimit: 3,
    //previewTemplate: previewTemplate,
    previewContainer: dropzone.querySelector(".dropzone-items"),
    acceptedFiles: ".pdf, .docx, .doc, .xls, .xlsx, .zip, .rar, .csv, .jpg, .jpeg, .png, .svg, .mp3, .mp4, .mkv, .wmv, .webp, .webm, .avi",
    init: function () {
        var dropzone = this;
        $("#media-uploader-btn").click(function () {
            dropzone.hiddenFileInput.setAttribute("accept", ".pdf, .docx, .doc, .xls, .xlsx, .zip, .rar, .csv, .jpg, .jpeg, .png, .svg, .mp3, .mp4, .mkv, .wmv, .webp, .webm, .avi");
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



//const setupDropzone = (form) => {
//    const dropzoneElement = form.querySelector('[data-kt-inbox-form="dropzone"]');
//    const uploadButton = form.querySelector('[data-kt-inbox-form="dropzone_upload"]');
//    let previewTemplate = dropzoneElement.querySelector(".dropzone-item").outerHTML;
//    dropzoneElement.querySelector(".dropzone-item").remove();

//    new Dropzone('[data-kt-inbox-form="dropzone"]', {
//        url: "https://preview.keenthemes.com/api/dropzone/void.php",
//        parallelUploads: 20,
//        maxFilesize: 1,
//        previewTemplate: previewTemplate,
//        previewsContainer: '[data-kt-inbox-form="dropzone"] .dropzone-items',
//        clickable: uploadButton
//    });
//};