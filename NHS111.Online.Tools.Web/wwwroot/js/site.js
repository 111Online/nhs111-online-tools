$('#infoModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var data = button.data('json') // Extract info from data-* attributes
    data["User ID"] = button.data('userid')

    var modal = $(this)
    var str = ""

    if (typeof data == "string") {
        str = "<p>This is an old style feedback, without extra data. The page url was:</p><p>" + data + "</p>"
    }
    else {
        for (var i = 0; i < Object.keys(data).length; i++) {
            var key = Object.keys(data)[i]
            str += "<p><b>" + key + ":</b> " + data[key] + "</p>"
        }
    }

    modal.find('.modal-body').html(str)
})