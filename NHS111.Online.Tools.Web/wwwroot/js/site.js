$('#infoModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget) // Button that triggered the modal
    var data = button.data('json') // Extract info from data-* attributes

    var modal = $(this)
    var str = ""
    
    for (var i = 0; i < Object.keys(data).length; i++) {
        var key = Object.keys(data)[i]
        str += "<p>" + key + ": " + data[key] + "</p>"
    }

    modal.find('.modal-body').html(str)
})