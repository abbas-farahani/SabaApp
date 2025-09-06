function ConfirmationMessage(value, type = "info") {
    Swal.fire({
        icon: type,
        title: value,
        position: 'bottom-end',
        timer: 4000,
        timerProgressBar: true,
        showConfirmation: false,
        width: 400
    });
}

$(document).ready(function(){
    const date = new Date;
    $('.datetime-now').text(date.toLocaleString("fa-IR"));
});

$(document).on('click', '.logout-btn', function () {
    $('form.logout-form').submit();
});

toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toastr-bottom-left",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};