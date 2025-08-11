$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%">' +
                        '<option value="">ALL</option>' +
                        '<option value="ACTIVE">ACTIVE</option>' +
                        '<option value="REJECTED">REJECTED</option>' +
                        '<option value="EDIT - PENDING">EDIT - PENDING</option>' +
                    '</select>')
    });
    var table = $('#myTable').DataTable({
        "autoWidth": false,
        "info": false,
        "sAjaxSource": ajaxHandler,
        "bLengthChange": false,
        "oLanguage": {
            "sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
        },
        "aoColumnDefs": [{
            'bSortable': false, 'aTargets': [3] //Disable sort for last column
        }],
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "AccNo", "sWidth": "15%" },
            { "sName": "Name", "sWidth": "40%" },
            { "sName": "Status", "sWidth": "20%" },
            { "sWidth": "15%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            var id = $('td:eq(0)', nRow).text();
            var status = $('td:eq(2)', nRow).text();
            var controller = '<div><a href="' + baseUrl + 'OtherAccount/Edit/' + id + '/" title="Edit"><span class="btn btn-info btn-sm fa fa-edit"></span><a>';
            
            if (status.indexOf("PENDING") == -1)
            {
                $('td:eq(3)', nRow).html(controller);
            }

            if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
                $('.dataTables_paginate').css("display", "block");
            } else {
                $('.dataTables_paginate').css("display", "none");
            }
        }
    });
    $("#GvHeader").after($("#GvFilter"));
    $("#BtnFilter").bind("click", function () {
        $("#filterDDL option[value='']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#filterDDL option:selected').val()).draw();
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $("#filterDDL option[value='']").attr("selected", "selected");
        $('#filterDDL option:selected').val("");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
});