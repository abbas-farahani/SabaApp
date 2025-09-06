$(document).ready(function () {
    CallCKEditor('.page-classic-editor');
    const statusBullet = document.getElementById("new-page-status-bullet"), statusClasses = ["bg-success", "bg-warning", "bg-danger", "bg-primary"], datePicker = document.getElementById("new-page-datepicker"), statusSelect = document.getElementById("Status");
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
        }ca
    });

    $("#new-page-datepicker").flatpickr({
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

});